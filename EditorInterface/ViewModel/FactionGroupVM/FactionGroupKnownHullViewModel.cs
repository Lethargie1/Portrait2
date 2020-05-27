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
using System.Windows.Data;

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
            Hull_CollectionChanged(null, null);
        }

        private void Hull_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IEnumerable<IShipHull> allShips;
            if (ShowBluePrintSeparate)
                allShips = IndividualShipHulls;
            else
                allShips = TotalShipHulls;
            DisplayShipList.Clear();
            DisplayShipList.AddRange(allShips);
        }

        private void Tag_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshBPPackageViewModel();
            NotifyOfPropertyChange(nameof(BPPackageListViewModel));
            Hull_CollectionChanged(null, null);
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

        private bool _ShowBluePrintSeparate = false;
        public bool ShowBluePrintSeparate
        {
            get => _ShowBluePrintSeparate;
            set
            {
                _ShowBluePrintSeparate = value;
                Hull_CollectionChanged(null, null);
            }
        }
        public IEnumerable<IShipHull> ShipHullfromTag
        {
            get
            {
                var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString()) ?? new List<string>();
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
                var hullIds = HullMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString()) ?? new List<string>();
                var IndividualShip = hullIds.Select(x => this.ShipHullRessourcesVM.ShipHullRessources.IdToRessource(x));

                return IndividualShip.Distinct();
            }
        }


        public BindableCollection<IShipHull> DisplayShipList { get; private set; } = new BindableCollection<IShipHull>();

        CollectionView _DisplayShipView;
        public CollectionView DisplayShipView
        {
            get
            {
                if (_DisplayShipView == null)
                {

                    _DisplayShipView = (CollectionView)CollectionViewSource.GetDefaultView(DisplayShipList);
                    //_FilesToWriteView = new CollectionView(FilesToWrite);
                    //_FilesToWriteView.Filter = x => ((ISSWritable)x).WillCreateFile;
                    //PropertyGroupDescription groupDescription = new PropertyGroupDescription("SourceMod", new PortraitModToGroupConverter());

                }
                return _DisplayShipView;
            }
        }



        public IShipHull SelectedShip { get; set; }

        public void AddShip(IShipHull input = null)
        {
            IShipHull Selected;
            if (input == null)
                Selected = ShipHullRessourcesVM.SelectedShipHullRessource;
            else
                Selected = input;
            if (TotalShipHulls.Select(x => x.Id).Contains(Selected.Id))
                return;
            var priorRemoval = HullMonitor.GetModification().Select(groupMod => groupMod.Modification).Where(mod =>
             {
                 var typed = (MonitoredArrayModification)mod;
                 return typed.ModType == MonitoredArrayModification.ModificationType.Remove && typed.GetContentAsValue().ToString() == Selected.Id;
             }).SingleOrDefault();

            if (priorRemoval == null)
            {
                var AddedId = HullMonitor.GetModification().Select(groupMod => groupMod.Modification).Where(mod =>
                {
                    var typed = (MonitoredArrayModification)mod;
                    return typed.ModType == MonitoredArrayModification.ModificationType.Add;
                })
                .Select( mod => mod.GetContentAsValue().ToString()).ToList();
                AddedId.Add(Selected.Id);
                var usingPackages = Selected.Tags.Select(x => this.BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x))
                                          .Where(BpPack => BpPack != null);
                //any package that is fully defined by the added Id and that contains the newly added ship is added instead of the ship
                var RestoredPackage = usingPackages.Where(package =>
               {
                   var PackageIds = package.BluePrints.Select(x => x.Id);
                   return PackageIds.Contains(Selected.Id) && !PackageIds.Except(AddedId).Any();
               }).FirstOrDefault();
                if (RestoredPackage != null)
                {
                    this.AddPackage(RestoredPackage);
                    var PackageIds = RestoredPackage.BluePrints.Select(x => x.Id).Where(x=> x != Selected.Id);
                    foreach (string shipId in PackageIds)
                        HullMonitor?.Modify(MonitoredArrayModification.GetRemoveModification(new JsonValue(shipId)));
                }
                else
                    HullMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(Selected.Id), typeof(ShipHullRessources)));

            }
            else
                HullMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(Selected.Id), typeof(ShipHullRessources)));
        }

        public void AddPackage(BPPackage input = null)
        {
            BPPackage Selected;
            if (input == null)
                Selected = ShipHullRessourcesVM.SelectedPackage;
            else
                Selected = input;
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
        
        public void RemovePackage(BPPackage input = null)
        {
            BPPackage Selected;
            if (input == null)
                Selected = BPPackageListViewModel.SelectedPackage;
            else
                Selected = input;
            TagMonitor?.Modify(MonitoredArrayModification.GetRemoveModification(new JsonValue(Selected.BluePrintTag)));
        }

        public void RemoveShip()
        {
            var Selected = SelectedShip;
            if (Selected == null)
                return;
            bool IsIndividual = IndividualShipHulls.Select(x => x.Id).Contains(Selected.Id);
            if (IsIndividual)
                HullMonitor.Modify(MonitoredArrayModification.GetRemoveModification(new JsonValue(Selected.Id)));

            var tags = TagMonitor?.ContentArray.Select(x => ((JsonValue)x).Content.ToString());
            var UsedPackage = tags.Select(x => this.BPPackageRessourcesViewModel.BPPackageRessources.TagToRessource(x))
                                          .Where(BpPack => BpPack != null);
            var ContainingPackage = UsedPackage.Where(Pack => Pack.BluePrints.Select(x => x.Id).Contains(Selected.Id)).ToList();
            ContainingPackage.SelectMany(pack =>
                              {
                                  this.RemovePackage(pack);
                                  return pack.BluePrints;
                              })
                             .Select(ship => 
                             {
                                 if (ship.Id != Selected.Id)
                                    this.AddShip(ship);
                                 return true;
                             }).ToList();

            
        }


    }


}
