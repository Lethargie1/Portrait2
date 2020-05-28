using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;

namespace EditorInterface.ViewModel
{
    public class ShipHullRessourcesViewModel : Screen
    {
        //public ShipHullRessourcesViewModelFactory SourceFactory { get; set; }
        public ShipHullRessources ShipHullRessources { get; private set; }
        public BPPackageRessources BPPackageRessources { get; set; }

        public ShipHullRessourcesViewModel(ShipHullRessources shipHullRessources, BPPackageRessources bPPackageRessources)
        {
            ShipHullRessources = shipHullRessources;
            BPPackageRessources = bPPackageRessources;
        }

        private List<IShipHull> _AvailableShips;
        public List<IShipHull> AvailableShips
        {
            get
            {
                if (_AvailableShips == null)
                    _AvailableShips = ShipHullRessources.UsableShipHull.Select(kv => kv.Value).ToList();
                return _AvailableShips;
            }
        }

        CollectionView _AvailableShipsView;
        public CollectionView AvailableShipsView
        {
            get
            {
                if (_AvailableShipsView == null)
                {

                    _AvailableShipsView = (CollectionView)CollectionViewSource.GetDefaultView(AvailableShips);
                    SortDescription AlphabeticalSorting = new SortDescription("HullName", ListSortDirection.Ascending);
                    _AvailableShipsView.SortDescriptions.Add(AlphabeticalSorting);
                }
                
                return _AvailableShipsView;
            }
        }
        public void ChangeSort(string columnName)
        {
            SortDescription newDescription;
            var oldSort = AvailableShipsView.SortDescriptions.FirstOrDefault();
            string oldColumn = oldSort.PropertyName;
            ListSortDirection oldDirection = oldSort.Direction;
            if (oldColumn != columnName)
                newDescription = new SortDescription(columnName, ListSortDirection.Ascending);
            else if (oldDirection == ListSortDirection.Ascending)
                newDescription = new SortDescription(columnName, ListSortDirection.Descending);
            else
                newDescription = new SortDescription(columnName, ListSortDirection.Ascending);
            AvailableShipsView.SortDescriptions.Clear();
            AvailableShipsView.SortDescriptions.Add(newDescription);
        }


        private BPPackageListViewModel _BPPackageListViewModel;
        public BPPackageListViewModel BPPackageListViewModel
        {
            get
            {
                if (_BPPackageListViewModel == null)
                {
                    if (BPPackageRessources?.AvailableBPPackages == null)
                        _BPPackageListViewModel = null;
                    else
                        _BPPackageListViewModel = new BPPackageListViewModel() { Packages = new ObservableCollection<BPPackage>(BPPackageRessources.AvailableBPPackages.Select(x => x.Value).Where(x => x.BluePrints.Count > 0).ToList()) };
                }
                    
                return _BPPackageListViewModel;
            }
        }

        private IShipHull _SelectedShipHullRessource;
        public IShipHull SelectedShipHullRessource
        {
            get => _SelectedShipHullRessource;
            set => SetAndNotify(ref _SelectedShipHullRessource, value);
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                SetAndNotify(ref _SelectedIndex, value);
            }
        }

        public BPPackage SelectedPackage
        {
            get => BPPackageListViewModel.SelectedPackage;
        }

        public event EventHandler ItemShiftClicked;

        protected virtual void OnItemShiftClicked()
        {
            EventHandler handler = ItemShiftClicked;
            handler?.Invoke(this, null);
        }

        public void HandleListViewClick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
                OnItemShiftClicked();
        }
    }





}


