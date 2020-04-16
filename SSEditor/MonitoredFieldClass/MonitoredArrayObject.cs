
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredArrayObject<T> : MonitoredField<T> where T : SSJson
    {
        public List<MonitoredField<T>> JObjectArray { get; private set; } = new List<MonitoredField<T>>();
        public override JsonToken GetJsonEquivalent()
        {
            JsonArray NewContent = new JsonArray();
            foreach (MonitoredField<T> mf in JObjectArray)
            {
                JsonToken a = mf.GetJsonEquivalent();
                NewContent.Values.Add(a);
            }
            return NewContent;
        }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var parents = from f in Files
                                where f.Fields.ContainsKey(this.FieldPath) == true
                                select f.Fields[this.FieldPath];
                var ChildrenExample = parents.Cast<JsonArray>().SelectMany(c => c.Values);
                JObjectArray.Clear();
                int counter = 0;
                foreach (var child in ChildrenExample)
                {
                    MonitoredField<T> tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(child, this.FieldPath + "[" + counter + "]");
                    counter++;
                    tempchildfield.ReplaceFiles(base.Files);
                    JObjectArray.Add(tempchildfield);
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
            for (int valueCounter = 0; valueCounter < JObjectArray.Count(); valueCounter++)
            {
                Dictionary<string, MonitoredField<T>> subResult = JObjectArray[valueCounter].GetPathedChildrens();
                foreach (KeyValuePair<string, MonitoredField<T>> subkv in subResult)
                {
                    result.Add("[" + valueCounter + "]" + subkv.Key, subkv.Value);
                }
            }
            return result;
        }
    }
}
