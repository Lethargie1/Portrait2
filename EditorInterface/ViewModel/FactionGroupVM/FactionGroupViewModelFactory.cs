using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupViewModelFactory
    {
        PortraitsRessourcesViewModelFactory PortraitsRessourcesViewModelFactory { get; set; }

        public FactionGroupViewModelFactory(PortraitsRessourcesViewModelFactory portraitsRessourcesViewModelFactory)
        {
            PortraitsRessourcesViewModelFactory = portraitsRessourcesViewModelFactory;
        }

        /// <summary>Produce a FactionGroupViewmOdel, but Ressources Viewmodel factory are shared, allowing permanence of selected index and stuff</summary>
        /// <param name="SelectedFaction">Factiongroup wrapped by the created viewModel</param>
        /// <param name="PriorFactionSelectedTabName">Name of the tab opened in the prior created viewmodel, should probably be handled internaly</param>
        public FactionGroupViewModel GetFactionGroupViewModel(SSFactionGroup SelectedFaction, string PriorFactionSelectedTabName = "")
        {
            return new FactionGroupViewModel(SelectedFaction, PortraitsRessourcesViewModelFactory, PriorFactionSelectedTabName);
        }

    }
}
