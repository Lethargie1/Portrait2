using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupValueViewModel:Screen
    {
        public SSFactionGroup FactionGroup { get; set; }

        public FactionGroupValueViewModel(SSFactionGroup factionGroup)
        {
            FactionGroup = factionGroup;
        }
        public string Id
        {
            get { return FactionGroup?.Id?.Content.ToString(); }
        }

        public string SSDisplayName
        {
            get { return FactionGroup?.DisplayName?.Content.ToString(); }
            set 
            { 
                FactionGroup.DisplayName.Modification = new FVJson.JsonValue(value);
                
                NotifyOfPropertyChange(nameof(SSDisplayNameWarning));
            }
        }
        public string SSDisplayNameWarning { get => FactionGroup.DisplayName.HasMultipleSourceFile ? "Has multiple source" : null; }

        public string SSDisplayNameWithArticle
        {
            get { return FactionGroup?.DisplayNameWithArticle?.Content.ToString(); }
        }

        public string ShipNamePrefix
        {
            get { return FactionGroup?.ShipNamePrefix?.Content.ToString(); }
        }
    }
}
