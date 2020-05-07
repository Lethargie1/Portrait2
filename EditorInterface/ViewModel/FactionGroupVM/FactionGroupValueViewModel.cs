using SSEditor.FileHandling;
using SSEditor.MonitoringField;
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

        private List<IEventBinding> binding = new List<IEventBinding>();
        public FactionGroupValueViewModel(SSFactionGroup factionGroup)
        {
            FactionGroup = factionGroup;
        }
        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
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
                FactionGroup.DisplayName.ApplyModification(new FVJson.JsonValue(value));
                
                NotifyOfPropertyChange(nameof(SSDisplayNameWarning));
            }
        }
        public void ResetDisplayName()
        {
            FactionGroup?.DisplayName?.Reset();
            NotifyOfPropertyChange(nameof(SSDisplayName));
        }
        public string SSDisplayNameWarning { get => FactionGroup?.DisplayName.HasMultipleSourceFile ?? false ? "Has multiple source" : null; }

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
