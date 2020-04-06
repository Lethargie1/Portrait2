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
    class MonitoredValue<Token,T>: MonitoredField<T> where T:SSJson where Token:ITokenValue, new()
    {
        public Token Content { get; } = new Token();
        public JTokenType ValueType { get; set; } = JTokenType.String;

        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.ReadValue(FieldPath) != null
                                   select new { modName = f.SourceMod.ModName , value = f.ReadValue(FieldPath), file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                string ValueResult = Ordered.FirstOrDefault()?.value;
                T FileResult = Ordered.FirstOrDefault()?.file;
                
                Content.SetContent(ValueResult, FileResult);
            }
        }
        public override JToken GetJsonEquivalent()
        {
            JValue result1;
            switch (ValueType)
            {
                case JTokenType.String:
                    result1 = new JValue(Content.Value);
                    break;
                case JTokenType.Integer:
                    result1 = new JValue(Convert.ToInt32(Content.Value));
                    break;
                default:
                    throw new NotImplementedException();
            }
            return result1;
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
    }
}
