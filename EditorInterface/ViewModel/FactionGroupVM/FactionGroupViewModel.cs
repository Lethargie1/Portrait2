using SSEditor.FileHandling;
using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupViewModel : Conductor<Screen>.Collection.OneActive
    {

        //public PortraitsRessources PortraitsRessource { get; private set; }
        public FactionGroupViewModel()
        {
        }
        public ShipHullRessourcesViewModel ShipHullRessourcesViewModel {get;set;}
        public PortraitsRessourcesViewModel PortraitsRessourcesViewModel { get; set; }

        private SSFactionGroup _FactionGroup;
        public SSFactionGroup FactionGroup
        {
            get => _FactionGroup;
            set
            {
                _FactionGroup = value;

                string DisplayNameArticled = FactionGroup?.DisplayNameWithArticle?.Content?.ToString();
                ActivateItem(new FactionGroupValueViewModel(FactionGroup) { DisplayName = "Values" });

                //tabs for portraits
                string FemaleDisplayPortrait = DisplayNameArticled != null ? "Female Portraits from " + DisplayNameArticled : "Female portraits";
                ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.FemalePortraits, PortraitsRessourcesViewModel) { DisplayName = "Portraits (f)", LongDisplayName = FemaleDisplayPortrait });
                string maleDisplayPortrait = DisplayNameArticled != null ? "Male Portraits from " + DisplayNameArticled : "Male portraits";
                ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.MalePortraits, PortraitsRessourcesViewModel) { DisplayName = "Portraits (m)", LongDisplayName = maleDisplayPortrait });

                //tabs for hulls
                //ActivateItem(new FactionGroupKnownHullViewModel(FactionGroup?.KnownShipsTag, FactionGroup?.KnownShipsHulls, ShipHullRessourcesViewModelFactory.getVM()) { DisplayName = "Known Hull", LongDisplayName = "" });
                ActivateItem(new FactionGroupKnownHullViewModel(FactionGroup?.KnownShipsTag, FactionGroup?.KnownShipsHulls, ShipHullRessourcesViewModel) { DisplayName = "Known Hull", LongDisplayName = "" });

            }
        }

        protected override void ChangeActiveItem(Screen newItem, bool closePrevious)
        {
            base.ChangeActiveItem(newItem, true);
        }
        public string SelectedTabName
        {
            get => this.ActiveItem.DisplayName;
            set
            {
                Screen tabMatching = this.Items.FirstOrDefault(x => x.DisplayName == value);
                if (tabMatching != null)
                    this.ActivateItem(tabMatching);
            }
        }
    }
}
