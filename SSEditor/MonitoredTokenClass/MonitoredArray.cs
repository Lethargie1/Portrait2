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
        public ObservableCollection<Token> Array { get; } = new ObservableCollection<Token>();

        public override void Resolve(List<SSFile> fileList)
        {
            if (FieldPath != null)
            {
                var fileArrayPair = from f in fileList
                                   where f.ReadArray(FieldPath) != null
                                   select new { value = f.ReadArray(FieldPath), file = f };
                Array.Clear();
                foreach (var pair in fileArrayPair)
                {
                    foreach (string data in pair.value)
                    {
                        Token temp = new Token();
                        temp.SetContent(data, pair.file);
                        Array.Add(temp);
                    }
                }
                
            }
        }
    }
}
