using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupPortraitViewModel : Screen
    {
        private PortraitsRessources PortraitsRessources { get; set; }
        public List<Portraits> AvailablePortraits
        {
            get
            {
                return PortraitsRessources.RessourceCorrespondance.Select(kv => kv.Value).ToList();
            }
        }
        public FactionGroupPortraitViewModel(PortraitsRessources portraitsRessources)
        {
            PortraitsRessources = portraitsRessources;

        }
    }
}
