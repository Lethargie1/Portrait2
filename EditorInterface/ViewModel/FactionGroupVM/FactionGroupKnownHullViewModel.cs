using FVJson;
using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel.FactionGroupVM
{
    class FactionGroupKnownHullViewModel : Conductor<ShipHullRessourcesViewModel>
    {
        public string LongDisplayName { get; set; }
        public ShipHullRessourcesViewModel ShipHullRessourcesVM { get; private set; }
        public MonitoredArray TargetMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TargetMonitor?.ContentArray; }

        public JsonToken SelectedPortraitArray { get; set; }
        public FactionGroupKnownHullViewModel(MonitoredArray targetMonitor, ShipHullRessourcesViewModel shipHullRessourcesVM)
        {
            TargetMonitor = targetMonitor;
            ShipHullRessourcesVM = shipHullRessourcesVM;
            ActivateItem(ShipHullRessourcesVM);
        }
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            ActivateItem(ShipHullRessourcesVM);
        }
    }
}
