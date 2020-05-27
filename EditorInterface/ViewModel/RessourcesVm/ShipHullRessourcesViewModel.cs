using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        
        public List<IShipHull> AvailableShips
        {
            get
            {
                return ShipHullRessources.UsableShipHull.Select(kv => kv.Value).ToList();
            }
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


