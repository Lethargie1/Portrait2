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

        public FactionGroupViewModel(SSFactionGroup factionGroup, PortraitsRessources portraitsRessource)
        {
            FactionGroup = factionGroup;
            PortraitsRessource = portraitsRessource;
            Items.Add(new FactionGroupPortraitViewModel(PortraitsRessource) { DisplayName = "Portraits" });
            ActivateItem(new FactionGroupValueViewModel(FactionGroup) {DisplayName = "Values" });
        }
    }
}
