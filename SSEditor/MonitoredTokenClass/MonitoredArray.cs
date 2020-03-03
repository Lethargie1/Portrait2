using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoredTokenClass
{
    class MonitoredArray<Token> : MonitoredField where Token : ITokenValue, new()
    {
        public ObservableCollection<Token> Array { get; }

        public override void Resolve(List<SSFile> fileList)
        {
            if (FieldPath != null)
            {
                var fileArrayPair = from f in fileList
                                   where f.ReadArray(FieldPath) != null
                                   select new { value = f.ReadArray(FieldPath), file = f };

                var Projected = fileArrayPair.SelectMany()
                var Expended = from fA in fileArrayPair
                              select new { p.value, p.file };
                List<string> ValueResult = Ordered.FirstOrDefault()?.value;
                SSFile FileResult = Ordered.FirstOrDefault()?.file;

                Content.SetContent(ValueResult, FileResult);
            }
        }
    }
}
