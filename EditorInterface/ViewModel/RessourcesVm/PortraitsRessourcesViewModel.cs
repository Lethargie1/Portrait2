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
    public class PortraitsRessourcesViewModel : Screen
    {
        public PortraitsRessourcesViewModelFactory SourceFactory { get; set; }
        public PortraitsRessources PortraitsRessources { get; private set; }
        public PortraitsRessourcesViewModel(PortraitsRessources portraitsRessources)
        {
            PortraitsRessources = portraitsRessources;
        }

        public PortraitsRessourcesViewModel(PortraitsRessources portraitsRessources, PortraitsRessourcesViewModelFactory sourceFactory) : this(portraitsRessources)
        {
            SourceFactory = sourceFactory;
        }
        protected override void OnViewLoaded()
        {
            if (SourceFactory != null)
                this.SelectedIndex = SourceFactory.SharedIndex;
            else
                this.SelectedIndex = 0;
            base.OnViewLoaded();
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

    public class PortraitsRessourcesViewModelFactory
    {
        public PortraitsRessources PortraitRessources { get; set; }
        public PortraitsRessourcesViewModelFactory(PortraitsRessources portraitRessources)
        {
            PortraitRessources = portraitRessources;
        }

        public PortraitsRessourcesViewModel getVM()
        {
            return new PortraitsRessourcesViewModel(PortraitRessources, this);
        }
        public int SharedIndex { get; set; } = 0;
    }
}
