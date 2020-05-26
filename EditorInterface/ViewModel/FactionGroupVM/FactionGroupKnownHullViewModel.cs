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


        protected List<IEventBinding> binding = new List<IEventBinding>();

        private bool _ShowBluePrintSeparate = true;
        public bool ShowBluePrintSeparate
        {
            get => _ShowBluePrintSeparate;
            set => SetAndNotify(ref _ShowBluePrintSeparate, value, nameof(DisplayShipList));
        }

        public IEnumerable<IShipHull> TotalShipHulls
        {
            get
            {
                var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                var ShipFromPackage = tags.Select(x => this.BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x))
                                          .Where(BpPack => BpPack != null)
                                          .SelectMany(BpPack => BpPack.BluePrints);


                var hullIds = HullMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                var IndividualShip = hullIds.Select(x => this.ShipHullRessourcesVM.ShipHullRessources.IdToRessource(x));

                return Enumerable.Concat<IShipHull>(ShipFromPackage, IndividualShip).Distinct();
            }
        }

        public IEnumerable<IShipHull> IndividualShipHulls
        {
            get
            {
                var hullIds = HullMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                var IndividualShip = hullIds.Select(x => this.ShipHullRessourcesVM.ShipHullRessources.IdToRessource(x));

                return IndividualShip.Distinct();
            }
        }

        public List<IShipHull> DisplayShipList
        {
            get
            {

                List<IShipHull> allShips;
                if (ShowBluePrintSeparate)
                    allShips =IndividualShipHulls.ToList();
                else
                    allShips =TotalShipHulls.ToList();


                return allShips;
            }
        }



        public void AddShip()
        {
            var Selected = ShipHullRessourcesVM.SelectedShipHullRessource;
            if (TotalShipHulls.Select(x => x.Id).Contains(Selected.Id))
                return;
            HullMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(Selected.Id), typeof(ShipHullRessources)));
            NotifyOfPropertyChange(nameof(DisplayShipList));
        }

        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
        }
    }


}
