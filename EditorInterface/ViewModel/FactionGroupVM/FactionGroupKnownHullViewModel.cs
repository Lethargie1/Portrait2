using FVJson;
using SSEditor.MonitoringField;
using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            
            if (HullMonitor!= null)
            {
                CollectionChangedEventManager.AddHandler(HullMonitor.ContentArray, Hull_CollectionChanged);
            }
            if(TagMonitor!= null)
            {
                CollectionChangedEventManager.AddHandler(TagMonitor.ContentArray, Tag_CollectionChanged);
            }
        }

        private void Hull_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(nameof(DisplayShipList));
        }

        private void Tag_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshBPPackageViewModel();
            NotifyOfPropertyChange(nameof(BPPackageListViewModel));
            NotifyOfPropertyChange(nameof(DisplayShipList));
        }


        public string LongDisplayName { get; set; }
        public ShipHullRessourcesViewModel ShipHullRessourcesVM { get; private set; }
        public BPPackageRessourcesViewModel BPPackageRessourcesViewModel { get; private set; }
        public MonitoredArray TagMonitor { get; private set; }
        public MonitoredArray HullMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TagMonitor?.ContentArray; }

        public void RefreshBPPackageViewModel()
        {
            var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString()) ?? new List<string>();
            var PackageList = tags.Select(x => BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x)).Where(x => x != null).ToList();
            var result = new BPPackageListViewModel();
            result.Packages = new ObservableCollection<BPPackage>(PackageList);
            BPPackageListViewModel = result;
        }
        private BPPackageListViewModel _BPPackageListViewModel;
        public BPPackageListViewModel BPPackageListViewModel
        {
            get
            {
                if (_BPPackageListViewModel == null)
                    RefreshBPPackageViewModel();
                return _BPPackageListViewModel;
            }
            private set => _BPPackageListViewModel = value;
        }


        protected List<IEventBinding> binding = new List<IEventBinding>();

        private bool _ShowBluePrintSeparate = true;
        public bool ShowBluePrintSeparate
        {
            get => _ShowBluePrintSeparate;
            set => SetAndNotify(ref _ShowBluePrintSeparate, value, nameof(DisplayShipList));
        }
        public IEnumerable<IShipHull> ShipHullfromTag
        {
            get
            {
                var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
                var ShipFromPackage = tags.Select(x => this.BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x))
                                          .Where(BpPack => BpPack != null)
                                          .SelectMany(BpPack => BpPack.BluePrints);
                return ShipFromPackage;
            }
        }

        public IEnumerable<IShipHull> TotalShipHulls
        {
            get
            {
                return Enumerable.Concat<IShipHull>(ShipHullfromTag, IndividualShipHulls).Distinct();
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

        public IShipHull SelectedShip { get; set; }

        public void AddShip()
        {
            var Selected = ShipHullRessourcesVM.SelectedShipHullRessource;
            if (TotalShipHulls.Select(x => x.Id).Contains(Selected.Id))
                return;
            HullMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(Selected.Id), typeof(ShipHullRessources)));
        }

        public void AddPackage()
        {
            var Selected = ShipHullRessourcesVM.SelectedPackage;
            var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
            if (tags.Contains(Selected.BluePrintTag))
                return;
            TagMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(Selected.BluePrintTag), typeof(BPPackageRessources)));
        }

        public void Reset()
        {
            HullMonitor.ResetModification();
            TagMonitor.ResetModification();
        }
        
        public void RemovePackage()
        {
            var Selected = BPPackageListViewModel.SelectedPackage;
            TagMonitor?.Modify(MonitoredArrayModification.GetRemoveModification(new JsonValue(Selected.BluePrintTag)));
        }

        public void RemoveShip()
        {
            var Selected = SelectedShip;

            bool IsIndividual = IndividualShipHulls.Select(x => x.Id).Contains(Selected.Id);

            if (IsIndividual)
                HullMonitor.Modify(MonitoredArrayModification.GetRemoveModification(new JsonValue(Selected.Id)));
        }
    }


}
