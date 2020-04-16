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
                                   where f.Fields.ContainsKey(FieldPath) == true
                                   select new { value = f.Fields[FieldPath], file = f };
                ContentArray.Clear();
                foreach (var pair in fileArrayPair)
                {
                    switch (pair.value)
                    {
                        case JsonArray jArray:
                            foreach (JsonToken data in jArray.Values)
                            {
                                ContentArray.Add(data);
                            }
                            break;
                        case JsonValue jValue:
                            ContentArray.Add(jValue);
                            break;
                        default:
                            throw new ArgumentException("Path leads to non array token");
                    }
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

        public override Dictionary<string, MonitoredField<T>> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField<T>>() { { "", this } };
        }
    }
}
