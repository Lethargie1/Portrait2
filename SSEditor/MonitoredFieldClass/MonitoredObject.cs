
using FVJson;
using SSEditor.FileHandling;
using SSEditor.JsonHandling;
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
                                 where f.JsonContent?.ExistPath(this.FieldPath)== true
                                 select f.JsonContent.SelectToken(this.FieldPath);
                var OneKeyValuePerKey = parents.Cast<JsonObject>().SelectMany(c => c.Values).GroupBy(c => c.Key).Select(d => d.First());
                MonitoredProperties.Clear();
                foreach (var KeyValue in OneKeyValuePerKey)
                {
                    MonitoredField<T> tempchildfield;
                    if (FieldPath=="")
                        tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(KeyValue.Value,  KeyValue.Key.ToString());
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
    }
}
