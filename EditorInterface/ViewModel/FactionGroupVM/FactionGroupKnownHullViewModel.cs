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
        public FactionGroupKnownHullViewModel(MonitoredArray tagMonitor, MonitoredArray hullMonitor, ShipHullRessourcesViewModel shipHullRessourcesVM, BPPackageRessourcesViewModel bPPackageRessourcesViewModel)
        {
            TagMonitor = tagMonitor;
            HullMonitor = hullMonitor;
            ShipHullRessourcesVM = shipHullRessourcesVM;
            BPPackageRessourcesViewModel = bPPackageRessourcesViewModel;
            ActivateItem(ShipHullRessourcesVM);
        }


        public string LongDisplayName { get; set; }
        public ShipHullRessourcesViewModel ShipHullRessourcesVM { get; private set; }
        public BPPackageRessourcesViewModel BPPackageRessourcesViewModel { get; private set; }
        public MonitoredArray TagMonitor { get; private set; }
        public MonitoredArray HullMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TagMonitor?.ContentArray; }

        public BPPackageListViewModel BPPackageListViewModel
        {
            get
            {
                
                var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString()) ?? new List<string>();
                var PackageList = tags.Select(x => BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x)).Where(x => x!= null).ToList();
                var result = new BPPackageListViewModel();
                result.Packages = new ObservableCollection<BPPackage>(PackageList);
                return result;
            }
        }

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
        
        
    }

    public class DesignFactionGroupKnownHullViewModel
    {
        public List<IShipHull> KnownShipList { get; set; }
        public string LongDisplayName { get; set; }
        public string DisplayName { get; set; }

    }
}
