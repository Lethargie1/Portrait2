using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    class MonitoredArrayValue<Token> : MonitoredField where Token : ITokenAsArray, new()
    {
        public Token Content { get; } = new Token();
        
        public List<string> ValueArray { get => Content.ValueArray; }

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
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
