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
            SSDisplayName = new MonitoredValueViewModel(FactionGroup?.DisplayName);
            DisplayNameWithArticle = new MonitoredValueViewModel(FactionGroup?.DisplayNameWithArticle);
            ShipNamePrefix = new MonitoredValueViewModel(FactionGroup?.ShipNamePrefix);
            Color = new MonitoredColorViewModel(FactionGroup?.Color);
            BaseUIColor = new MonitoredColorViewModel(FactionGroup?.BaseUIColor);
            DarkUIColor = new MonitoredColorViewModel(FactionGroup?.DarkUIColor);
            SecondaryUIColor = new MonitoredColorViewModel(FactionGroup?.SecondaryUIColor);


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

        public MonitoredValueViewModel SSDisplayName { get; }
        public MonitoredValueViewModel DisplayNameWithArticle { get; }

        public MonitoredValueViewModel ShipNamePrefix { get; }

        public MonitoredColorViewModel Color { get; }
        public MonitoredColorViewModel BaseUIColor { get; }
        public MonitoredColorViewModel DarkUIColor { get; }
        public MonitoredColorViewModel SecondaryUIColor { get; }



    }
}
