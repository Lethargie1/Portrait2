using FVJson;
using SSEditor.Converters;
using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class FactionGroupTokenViewModel : FactionGroupValueViewModel
    {
        private SSFactionGroup Group { get; set; }

        public FactionGroupTokenViewModel(SSFactionGroup group) : base (group)
        {
            Group = group;
            binding.Add(Group.Bind(x => x.IsModified, (sender, arg) => NotifyOfPropertyChange(nameof(StatusLetter))));
            binding.Add(Group.Bind(x => x.MustOverwrite, (sender, arg) => NotifyOfPropertyChange(nameof(StatusLetter))));
        }


        public string StatusLetter
        {
            get
            {
                NotifyOfPropertyChange(nameof(StatusColor));
                if (Group.MustOverwrite == true)
                    return "R";
                if (Group?.MonitoredContent?.IsModified() == true)
                    return "M";
                else
                    return "";
            }
        }

        public string StatusColor
        {
            get
            {
                if (Group.MustOverwrite == true)
                    return "#FFFF0000";
                if (Group?.MonitoredContent?.IsModified() == true)
                    return "#fffcf403";
                else
                    return "Transparent";
            }
        }

        public bool ForceOverwrite
        {
            get
            {
                return Group.ForceOverwrite;
            }
            set
            {
                Group.ForceOverwrite = value;
                NotifyOfPropertyChange(nameof(StatusLetter));
            }
        }
    }
}
