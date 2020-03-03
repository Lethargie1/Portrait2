using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoredTokenClass
{
    class MonitoredArrayValue<Token> : MonitoredField, ITokenAsArray where Token : ITokenAsArray, new()
    {
        private Token Content = new Token();

        public string Value { get => Content.Value; }
        public SSFile Source { get => Content.Source; }
        public List<string> ValueArray { get => Content.ValueArray; }

        public override void Resolve(List<SSFile> fileList)
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in fileList
                                   where f.ReadArray(FieldPath) != null
                                   select new { modName = f.ModName, value = f.ReadArray(FieldPath), file = f };
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file };
                List<string> ValueResult = Ordered.FirstOrDefault()?.value;
                SSFile FileResult = Ordered.FirstOrDefault()?.file;

                Content.SetContent(ValueResult, FileResult);
            }
        }

        public void SetContent(List<string> valueList, SSFile source)
        {
            Content.SetContent(valueList, source);
        }
    }
}
