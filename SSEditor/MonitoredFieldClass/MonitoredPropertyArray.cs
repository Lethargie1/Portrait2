using Newtonsoft.Json.Linq;
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
    public class MonitoredPropertyArray<T> : MonitoredField<T> where T : SSFile
    {
        public ObservableCollection<MonitoredField<T>> MonitoredProperties { get; private set; } = new ObservableCollection<MonitoredField<T>>();
        public override JObject GetJsonEquivalent()
        {
            JObject NewContent = new JObject();
            foreach (MonitoredField<T> mf in MonitoredProperties)
            {
                JObject a = mf.GetJsonEquivalent();
                NewContent.ConcatRecursive(a);
            }
            return NewContent;
        }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var childrens = from f in Files
                                 where f.JsonContent.SelectToken(this.FieldPath) != null
                                 select f.JsonContent.SelectToken(this.FieldPath).Children();
                var GroupedChildrenExample = childrens.SelectMany(c => c).GroupBy(c => c.Path).SelectMany(c=>c.First());
                MonitoredProperties.Clear();
                foreach (var child in GroupedChildrenExample)
                {
                    MonitoredField<T> tempchildfield = MonitoredFieldFactory<T>.CreateFieldFromExampleToken(child);
                    tempchildfield.ReplaceFiles(base.Files);
                    MonitoredProperties.Add(tempchildfield);
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
