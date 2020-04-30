using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class PortraitsRessourcesViewModel : Screen
    {
        public PortraitsRessources PortraitsRessources { get; private set; }
        public PortraitsRessourcesViewModel(PortraitsRessources portraitsRessources)
        {
            PortraitsRessources = portraitsRessources;
        }

        public List<Portraits> AvailablePortraits
        {
            get
            {
                return PortraitsRessources.RessourceCorrespondance.Select(kv => kv.Value).ToList();
            }
        }

        private Portraits _SelectedPortraitRessource;
        public Portraits SelectedPortraitRessource
        {
            get => _SelectedPortraitRessource;
            set => SetAndNotify(ref _SelectedPortraitRessource, value); 
        }
    }
}
