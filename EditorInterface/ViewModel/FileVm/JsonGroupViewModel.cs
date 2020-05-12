using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class JsonGroupViewModel
    {
        public SSJsonGroup JsonGroup { get; set; }
        public JsonGroupViewModel(SSJsonGroup jsonGroup)
        {
            JsonGroup = jsonGroup;
        }

        public string filepath { get => JsonGroup?.RelativeUrl.ToString(); }

        public bool MustOverwrite { get => JsonGroup?.MustOverwrite ?? false; }

    }

}
