
using FVJson;
using SSEditor.FileHandling;
using SSEditor.JsonHandling;
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
                                where f.ReadToken(this.FieldPath) != null
                                select f.JsonContent.SelectToken(this.FieldPath);
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
    }
}
