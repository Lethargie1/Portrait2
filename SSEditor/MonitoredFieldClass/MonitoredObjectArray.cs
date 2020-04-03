using Newtonsoft.Json.Linq;
using SSEditor.FileHandling;
using SSEditor.JsonHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredObjectArray<T> : MonitoredField<T> where T : SSFile
    {
        public List<MonitoredField<T>> JObjectArray { get; private set; } = new List<MonitoredField<T>>();
        public override JToken GetJsonEquivalent()
        {
            JObject NewContent = new JObject();
            //foreach (MonitoredField<T> mf in JObjectArray)
            //{
            //    JObject a = mf.GetJsonEquivalent();
            //    NewContent.Merge(a);
            //}
            return new JValue(0);
        }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var childrens = from f in Files
                                where f.JsonContent.SelectToken(this.FieldPath) != null
                                select f.JsonContent.SelectToken(this.FieldPath).Children();
                var ChildrenExample = childrens.SelectMany(c => c);
                JObjectArray.Clear();
                foreach (var child in ChildrenExample)
                {
                    MonitoredField<T> tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(child);
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
