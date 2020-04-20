using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSModWritable
    {
        public SSBaseLinkUrl ModUrl { get; private set; }
        public SSJson ModInfo { get; private set; }
        public string ModName { get; private set; }


        public ObservableCollection<ISSWritable> FileList { get; set; } = new ObservableCollection<ISSWritable>();
        
        public SSModWritable()
        { }


        public override string ToString()
        {
            return "Mod: " + (ModInfo?.ReadToken("name")?.ToString() ?? ("Unnamed " + ModUrl.Link));
        }
    }
}
