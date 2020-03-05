using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    class MonitoredArray<Token> : MonitoredField where Token : ITokenValue, new()
    {
        public ObservableCollection<Token> ContentArray { get; } = new ObservableCollection<Token>();

        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var fileArrayPair = from f in Files
                                   where f.ReadArray(FieldPath) != null
                                   select new { value = f.ReadArray(FieldPath), file = f };
                ContentArray.Clear();
                foreach (var pair in fileArrayPair)
                {
                    foreach (string data in pair.value)
                    {
                        Token temp = new Token();
                        temp.SetContent(data, pair.file);
                        ContentArray.Add(temp);
                    }
                }
                
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
