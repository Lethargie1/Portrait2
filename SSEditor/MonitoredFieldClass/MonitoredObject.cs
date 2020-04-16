
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredObject<T> : MonitoredField<T> where T : SSJson
    {
        public Dictionary<JsonValue,MonitoredField<T>> MonitoredProperties { get; private set; } = new Dictionary<JsonValue, MonitoredField<T>>();

        public void Add(string field, MonitoredField<T> fieldMonitor)
        {
            MonitoredProperties.Add(new JsonValue(field), fieldMonitor);
        }


        public override JsonToken GetJsonEquivalent()
        {
            JsonObject NewContent = new JsonObject();
            foreach (KeyValuePair<JsonValue,MonitoredField<T>> kv in MonitoredProperties)
            {
                JsonToken a = kv.Value.GetJsonEquivalent();
                NewContent.Values.Add(kv.Key, a);
            }
            return NewContent;
        }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var parents = from f in Files
                                 where f.Fields.ContainsKey(this.FieldPath)== true
                                 select f.Fields[this.FieldPath];
                var OneKeyValuePerKey = parents.Cast<JsonObject>().SelectMany(c => c.Values).GroupBy(c => c.Key).Select(d => d.First());
                MonitoredProperties.Clear();
                foreach (var KeyValue in OneKeyValuePerKey)
                {
                    JsonValue KeyAsStr = new JsonValue(KeyValue.Key.ToString(),JsonToken.TokenType.String);
                    JsonValue KeyAsRef = new JsonValue(KeyValue.Key.ToString(), JsonToken.TokenType.Reference);
                    //this is in case someone suround a field name with "" and someone else dosn't
                    if (MonitoredProperties.ContainsKey(KeyAsStr) || MonitoredProperties.ContainsKey(KeyAsRef))
                        continue;
                    MonitoredField<T> tempchildfield;
                    if (FieldPath=="")
                        tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(KeyValue.Value,  "." + KeyValue.Key.ToString());
                    else
                        tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(KeyValue.Value,this.FieldPath + "." + KeyValue.Key.ToString());

                    tempchildfield.ReplaceFiles(base.Files);
                    MonitoredProperties.Add(KeyValue.Key,tempchildfield);
                }

            }
        }

        protected override void ResolveAdd(T file)
        {
            Resolve();
        }

        protected override void ResolveRemove(T file)
        {
            Resolve();
        }

        public override Dictionary<string, MonitoredField<T>> GetPathedChildrens()
        {
            Dictionary<string, MonitoredField<T>> result = new Dictionary<String, MonitoredField<T>>() { { "", this } };
            foreach (KeyValuePair<JsonValue, MonitoredField<T>> kv in MonitoredProperties)
            {
                Dictionary<string, MonitoredField<T>> subResult = kv.Value.GetPathedChildrens();
                foreach (KeyValuePair<string, MonitoredField<T>> subkv in subResult)
                {
                    result.Add("." + kv.Key.ToString() + subkv.Key, subkv.Value);
                }
            }
            return result;
        }
    }
}
