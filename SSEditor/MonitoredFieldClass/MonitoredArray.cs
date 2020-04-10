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
    class MonitoredArray<T> : MonitoredField<T>  where T:SSJson
    {
        public ObservableCollection<JsonToken> ContentArray { get; } = new ObservableCollection<JsonToken>();

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var fileArrayPair = from f in Files
                                   where f.ReadToken(FieldPath) != null
                                   select new { value = f.ReadToken(FieldPath), file = f };
                ContentArray.Clear();
                foreach (var pair in fileArrayPair)
                {
                    if (pair.value is JsonArray jArray)
                    {
                        foreach (JsonToken data in jArray.Values)
                        {
                            ContentArray.Add(data);
                        }
                    }
                    else
                        throw new ArgumentException("Path leads to non array token");
                }
                
            }
        }
        public override JsonToken GetJsonEquivalent()
        {
            JsonArray jArray = new JsonArray();
            foreach (JsonToken data in ContentArray)
            {
                jArray.Values.Add(data);
            }
            return jArray; 
        }

        protected override void ResolveAdd(T file)
        {
            Resolve();
        }

        protected override void ResolveRemove(T file)
        {
            Resolve();
        }
        public override string ToString()
        {
            return base.FieldPath + " Array: (" + this.ContentArray.Count.ToString() + ")value, first one: " + (this.ContentArray.FirstOrDefault()?.ToString() ?? "none") ;
        }
    }
}
