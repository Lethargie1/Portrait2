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
        public ShipHullRessourcesViewModelFactory SourceFactory { get; set; }
        public ShipHullRessources ShipHullRessources { get; private set; }
        public ShipHullRessourcesViewModel(ShipHullRessources shipHullRessources)
        {
            ShipHullRessources = shipHullRessources;
        }

        public ShipHullRessourcesViewModel(ShipHullRessources shipHullRessources, ShipHullRessourcesViewModelFactory sourceFactory) : this(shipHullRessources)
        {
            SourceFactory = sourceFactory;
        }

        public List<IShipHull> AvailableShips
        {
            get
            {
                return ShipHullRessources.UsableShipHull.Select(kv => kv.Value).ToList();
            }
        }


        protected override void OnViewLoaded()
        {
            if (SourceFactory != null)
                this.SelectedIndex = SourceFactory.SharedIndex;
            else
                this.SelectedIndex = 0;
            base.OnViewLoaded();
        }

        private int _SelectedIndex;
        public int SelectedIndex
        {
            get => _SelectedIndex;
            set
            {
                SetAndNotify(ref _SelectedIndex, value);
                if (SourceFactory != null)
                    SourceFactory.SharedIndex = value;
            }
        }
    }

    public class ShipHullRessourcesViewModelFactory
    {
        public ShipHullRessources ShipHullRessources { get; set; }
        public ShipHullRessourcesViewModelFactory(ShipHullRessources shipHullRessources)
        {
            ShipHullRessources = shipHullRessources;
        }

        public ShipHullRessourcesViewModel getVM()
        {
            return new ShipHullRessourcesViewModel(ShipHullRessources, this);
        }
        public int SharedIndex { get; set; } = 0;
    }





}


