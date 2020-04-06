using Newtonsoft.Json.Linq;
using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    class MonitoredArrayValue<Token, T> : MonitoredField<T> where Token : ITokenAsArray, new() where T:SSJson
    {
        public Token Content { get; } = new Token();
        
        public List<int> ValueArray { get => Content.ValueArray; }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.ReadArray(FieldPath) != null
                                   select new { modName = f.SourceMod.ModName, value = f.ReadArray(FieldPath), file = f };
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file };
                List<string> ValueResult = Ordered.FirstOrDefault()?.value;
                ISSJson FileResult = Ordered.FirstOrDefault()?.file;

                Content.SetContent(ValueResult, FileResult);
            }
        }
        public override JToken GetJsonEquivalent()
        {
            return new JArray(Content.ValueArray.ToArray());
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
            return base.FieldPath + ": " + Content.ToString();
        }
    }
}
