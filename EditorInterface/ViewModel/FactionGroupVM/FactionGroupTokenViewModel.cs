﻿using FVJson;
using SSEditor.Converters;
using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class FactionGroupTokenViewModel :Screen 
    {
        private SSFactionGroup Group { get; set; }

        private List<IEventBinding> binding = new List<IEventBinding>();
        public FactionGroupTokenViewModel(SSFactionGroup group)
        {
            Group = group;
            binding.Add(Group.Bind(x => x.IsModified, (sender, arg) => NotifyOfPropertyChange(nameof(StatusLetter))));
            binding.Add(Group.Bind(x => x.MustOverwrite, (sender, arg) => NotifyOfPropertyChange(nameof(StatusLetter))));
        }

        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
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
    }
}
