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
        public PortraitsRessources PortraitsRessource { get; private set; }

        public FactionGroupViewModel(SSFactionGroup factionGroup, PortraitsRessourcesViewModel portraitsRessourceVM)
        {
            FactionGroup = factionGroup;
            //PortraitsRessource = portraitsRessource;
            ActivateItem(new FactionGroupValueViewModel(FactionGroup) { DisplayName = "Values" });
            ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.FemalePortraits, portraitsRessourceVM) { DisplayName = "Female portraits" });
            ActivateItem(new FactionGroupPortraitViewModel(FactionGroup?.MalePortraits, portraitsRessourceVM) { DisplayName = "Male portraits" });
            
        }
    }
}
