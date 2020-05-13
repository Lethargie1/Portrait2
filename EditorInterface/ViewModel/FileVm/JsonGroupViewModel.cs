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
        public ISSJsonGroup JsonGroup { get; set; }
        public JsonGroupViewModel(ISSJsonGroup jsonGroup)
        {
            JsonGroup = jsonGroup;
        }

        public string Filepath { get => JsonGroup?.RelativeUrl.ToString(); }

        public bool MustOverwrite { get => JsonGroup?.MustOverwrite ?? false; }

        public List<string> Modification
        {
            get 
            {
                return JsonGroup.GetModifications().Select(x => x.ToString()).ToList();
            }
        }

    }

}
