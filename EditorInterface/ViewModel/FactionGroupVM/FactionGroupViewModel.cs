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


        public FactionGroupViewModel()
        {
        }

        public void BuildEditableTab()
        {
            this.ActivateGenericTab();
            this.ActivateHullBasedTab();
            this.ActivatePortraitBasedTab();
        }
        public SSFactionGroup FactionGroup { get; set; }
        public ShipHullRessourcesViewModel ShipHullRessourcesViewModel { get; set; }
        public PortraitsRessourcesViewModel PortraitsRessourcesViewModel { get; set; }

        private void ActivateGenericTab()
        {
            string DisplayNameArticled = FactionGroup?.DisplayNameWithArticle?.Content?.ToString();
            ActivateItem(new FactionGroupValueViewModel(FactionGroup) { DisplayName = "Values" });
        }

        private void ActivateHullBasedTab()
        {
            var KnownHullVM = new FactionGroupKnownHullViewModel(FactionGroup?.KnownShipsTag, FactionGroup?.KnownShipsHulls, ShipHullRessourcesViewModel);
            KnownHullVM.DisplayName = "Known Ships";
            KnownHullVM.LongDisplayName = "";
            ActivateItem(KnownHullVM);

            var PriorityHullVM = new FactionGroupKnownHullViewModel(FactionGroup?.PriorityShipsTag, FactionGroup?.PriorityShipsHulls, ShipHullRessourcesViewModel);
            PriorityHullVM.DisplayName = "Priority Ships";
            PriorityHullVM.LongDisplayName = "";
            ActivateItem(PriorityHullVM);

            var ShipsWhenImportingHullVM = new FactionGroupKnownHullViewModel(FactionGroup?.ShipsWhenImportingTag, FactionGroup?.ShipsWhenImportingHulls, ShipHullRessourcesViewModel);
            ShipsWhenImportingHullVM.DisplayName = "Importing Ships";
            ShipsWhenImportingHullVM.LongDisplayName = "";
            ActivateItem(ShipsWhenImportingHullVM);
        }

        private void ActivatePortraitBasedTab()
        {
            string DisplayNameArticled = FactionGroup?.DisplayNameWithArticle?.Content?.ToString();

            string FemaleDisplayPortrait = DisplayNameArticled != null ? "Female Portraits from " + DisplayNameArticled : "Female portraits";
            var PortraitsFVM = new FactionGroupPortraitViewModel(FactionGroup?.FemalePortraits, PortraitsRessourcesViewModel);
            PortraitsFVM.DisplayName = "Portraits (f)";
            PortraitsFVM.LongDisplayName = FemaleDisplayPortrait;
            ActivateItem(PortraitsFVM);

            string maleDisplayPortrait = DisplayNameArticled != null ? "Male Portraits from " + DisplayNameArticled : "Male portraits";
            var PortraitsMVM = new FactionGroupPortraitViewModel(FactionGroup?.MalePortraits, PortraitsRessourcesViewModel);
            PortraitsMVM.DisplayName = "Portraits (m)";
            PortraitsMVM.LongDisplayName = maleDisplayPortrait;
            ActivateItem(PortraitsMVM);
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



        protected override void ChangeActiveItem(Screen newItem, bool closePrevious)
        {
            base.ChangeActiveItem(newItem, true);
        }


    }
}
