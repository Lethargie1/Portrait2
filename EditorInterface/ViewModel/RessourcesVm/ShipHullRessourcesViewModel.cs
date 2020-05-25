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

        public BPPackageListViewModel BPPackageListViewModel
        {
            get
            {
                return new BPPackageListViewModel() { Packages = new ObservableCollection<BPPackage>(BPPackageRessources.AvailableBPPackages.Select(x => x.Value).ToList()) };
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

    }





}


