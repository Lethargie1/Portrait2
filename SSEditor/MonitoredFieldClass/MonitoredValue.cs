
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredValue<T>: MonitoredField<T> where T:SSJson 
    {
        public JsonValue Content { get; private set; }

        public MonitoredValue(JsonValue content)
        {
            Content = content;
        }

        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.Fields.ContainsKey(FieldPath) == true
                                   select new { modName = f.SourceMod.ModName , value = f.Fields[FieldPath], file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                JsonToken TokenResult = Ordered.FirstOrDefault()?.value;
                if (TokenResult is JsonValue value)
                    Content.SetContent(value.Content);
                else if (TokenResult == null)
                    Content.SetContent(null);
                else
                    throw new ArgumentException("Path leads to wrong type of token");
                T FileResult = Ordered.FirstOrDefault()?.file;
            }
        }
        public override JsonToken GetJsonEquivalent()
        {
            return Content;
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
            return base.FieldPath+": "+ Content.ToString();
        }

        public override Dictionary<string, MonitoredField<T>> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField<T>>() { { "", this } };
        }
    }
}
