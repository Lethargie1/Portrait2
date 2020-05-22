using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class ShipHullRessourcesViewModel : Screen
    {
        //public ShipHullRessourcesViewModelFactory SourceFactory { get; set; }
        public ShipHullRessources ShipHullRessources { get; private set; }
        public ShipHullRessourcesViewModel(ShipHullRessources shipHullRessources)
        {
            ShipHullRessources = shipHullRessources;
        }

        
        public List<IShipHull> AvailableShips
        {
            get
            {
                return ShipHullRessources.UsableShipHull.Select(kv => kv.Value).ToList();
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


