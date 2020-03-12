using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    class MonitoredValue<Token,T>: MonitoredField<T> where T:SSFile where Token:ITokenValue, new()
    {
        public Token Content { get; } = new Token();


        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.ReadValue(FieldPath) != null
                                   select new { modName = f.SourceMod , value = f.ReadValue(FieldPath), file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                string ValueResult = Ordered.FirstOrDefault()?.value;
                T FileResult = Ordered.FirstOrDefault()?.file;
                
                Content.SetContent(ValueResult, FileResult);
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
