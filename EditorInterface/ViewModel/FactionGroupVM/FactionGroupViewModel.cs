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
        public SSFactionGroup FactionGroup{get;set;}
        //public PortraitsRessources PortraitsRessource { get; private set; }
        public PortraitsRessourcesViewModelFactory PortraitsRessourcesVMFactory { get; private set; }
        public FactionGroupViewModel(SSFactionGroup factionGroup, PortraitsRessourcesViewModelFactory portraitsRessourcesVMFactory, string priorFactionSelectedTabName)
        {
            FactionGroup = factionGroup;
            //PortraitsRessource = portraitsRessource;
            PortraitsRessourcesVMFactory = portraitsRessourcesVMFactory;
            string DisplayNameArticled = FactionGroup?.DisplayNameWithArticle?.Content?.ToString();

            ActivateItem(new FactionGroupValueViewModel(FactionGroup) { DisplayName = "Values" });
            string FemaleDisplayPortrait = DisplayNameArticled != null ? "Female Portraits from " + DisplayNameArticled : "Female portraits";
            ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.FemalePortraits, PortraitsRessourcesVMFactory.getVM()) { DisplayName = "Portraits (f)" , LongDisplayName = FemaleDisplayPortrait });

            string maleDisplayPortrait = DisplayNameArticled != null ? "Male Portraits from " + DisplayNameArticled : "Male portraits";
            ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.MalePortraits, PortraitsRessourcesVMFactory.getVM()) { DisplayName = "Portraits (m)", LongDisplayName = maleDisplayPortrait });



            Screen tabMatching = this.Items.FirstOrDefault(x => x.DisplayName == priorFactionSelectedTabName);
            if (tabMatching != null)
                this.ActivateItem(tabMatching);

        }
    }
}
