using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoredTokenClass
{
    class MonitoredValue<Token>: MonitoredField, ITokenValue where Token:ITokenValue,new()
    {
        private Token Content = new Token();


        public string Value { get => Content.Value; }
        public SSFile Source { get => Content.Source; }

        override public void Resolve(List<SSFile> fileList)
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in fileList
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

        public void SetContent(string value, SSFile source)
        {
            Content.SetContent(value, source);
        }
    }
}
