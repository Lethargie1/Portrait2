using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupTokenViewModel :Screen 
    {
        private SSFactionGroup Group { get; set; }
        public FactionGroupTokenViewModel(SSFactionGroup group)
        {
            Group = group;
        }

        public string Name { get => Group.DisplayName?.Content?.ToString() ?? Group.ToString(); }
    }
}
