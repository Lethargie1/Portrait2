using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoredTokenClass
{
    class MonitoredValue<Token>: MonitoredField where Token:ITokenValue,new()
    {
        public Token Content { get; } = new Token();


        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.ReadValue(FieldPath) != null
                                   select new { modName = f.ModName , value = f.ReadValue(FieldPath), file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                string ValueResult = Ordered.FirstOrDefault()?.value;
                SSFile FileResult = Ordered.FirstOrDefault()?.file;
                
                Content.SetContent(ValueResult, FileResult);
            }
        }

        protected override void ResolveAdd(SSFile file)
        {
            Resolve();
        }

        protected override void ResolveRemove(SSFile file)
        {
            Resolve();
        }
    }
}
