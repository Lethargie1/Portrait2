using FVJson;
using SSEditor.Converters;
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

        public string Color
        {
            get
            {
                JsonArrayToColorConverter converter = new JsonArrayToColorConverter();
                JsonArray source = Group.FactionColor?.ContentArray;
                if (source == null || source.Values.Count != 4)
                    return "#FFFFFFFF";
                string color = (string)converter.Convert(source);
                return color;
            }
        }
    }
}
