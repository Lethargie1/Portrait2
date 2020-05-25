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

                //tabs for hulls
                //ActivateItem(new FactionGroupKnownHullViewModel(FactionGroup?.KnownShipsTag, FactionGroup?.KnownShipsHulls, ShipHullRessourcesViewModelFactory.getVM()) { DisplayName = "Known Hull", LongDisplayName = "" });

            }
        }
        public BPPackageRessourcesViewModel BPPackageRessourcesViewModel{get; set;}

        private ShipHullRessourcesViewModel _ShipHullRessourcesViewModel;
        public ShipHullRessourcesViewModel ShipHullRessourcesViewModel
        {
            get => _ShipHullRessourcesViewModel;
            set
            {
                _ShipHullRessourcesViewModel = value;
                var KnownHullVM = new FactionGroupKnownHullViewModel(FactionGroup?.KnownShipsTag, FactionGroup?.KnownShipsHulls, ShipHullRessourcesViewModel,BPPackageRessourcesViewModel);
                KnownHullVM.DisplayName = "Known Hull";
                KnownHullVM.LongDisplayName = "";
                ActivateItem(KnownHullVM);               
            }

        }

        private PortraitsRessourcesViewModel _PortraitsRessourcesViewModel;
        public PortraitsRessourcesViewModel PortraitsRessourcesViewModel
        {
            get => _PortraitsRessourcesViewModel;
            set
            {
                _PortraitsRessourcesViewModel = value;

                string DisplayNameArticled = FactionGroup?.DisplayNameWithArticle?.Content?.ToString();

                string FemaleDisplayPortrait = DisplayNameArticled != null ? "Female Portraits from " + DisplayNameArticled : "Female portraits";
                var PortraitsFVM = new FactionGroupPortraitViewModel(FactionGroup?.FemalePortraits, PortraitsRessourcesViewModel);
                PortraitsFVM.DisplayName = "Portraits (f)";
                PortraitsFVM.LongDisplayName = FemaleDisplayPortrait;
                ActivateItem( PortraitsFVM );

                string maleDisplayPortrait = DisplayNameArticled != null ? "Male Portraits from " + DisplayNameArticled : "Male portraits";
                var PortraitsMVM = new FactionGroupPortraitViewModel(FactionGroup?.MalePortraits, PortraitsRessourcesViewModel);
                PortraitsMVM.DisplayName = "Portraits (m)";
                PortraitsMVM.LongDisplayName = maleDisplayPortrait;
                ActivateItem(PortraitsMVM);

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
