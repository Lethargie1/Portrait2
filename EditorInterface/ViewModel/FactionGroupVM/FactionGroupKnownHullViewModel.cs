using FVJson;
using SSEditor.MonitoringField;
using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupKnownHullViewModel : Conductor<ShipHullRessourcesViewModel>
    {
        public string LongDisplayName { get; set; }
        public ShipHullRessourcesViewModel ShipHullRessourcesVM { get; private set; }
        public MonitoredArray TagMonitor { get; private set; }
        public MonitoredArray HullMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TagMonitor?.ContentArray; }

        public List<IShipHull> KnownShipList
        {
            get
            {
                var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                var hullIds = HullMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                return ShipHullRessourcesVM.ShipHullRessources.MakeShipHullListFromTagAndId(tags, hullIds);
            }
        }

        public JsonToken SelectedPortraitArray { get; set; }
        public FactionGroupKnownHullViewModel(MonitoredArray tagMonitor, MonitoredArray hullMonitor, ShipHullRessourcesViewModel shipHullRessourcesVM)
        {
            TagMonitor = tagMonitor;
            HullMonitor = hullMonitor;
            ShipHullRessourcesVM = shipHullRessourcesVM;
            ActivateItem(ShipHullRessourcesVM);
        }
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            ActivateItem(ShipHullRessourcesVM);
        }
    }

    public class DesignFactionGroupKnownHullViewModel
    {
        public List<IShipHull> KnownShipList { get; set; }
        public string LongDisplayName { get; set; }
        public string DisplayName { get; set; }

    }
}
