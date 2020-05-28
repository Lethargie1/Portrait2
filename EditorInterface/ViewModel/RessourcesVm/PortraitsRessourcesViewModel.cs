using SSEditor.FileHandling;
using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class PortraitsRessourcesViewModel : Screen
    {
        //public PortraitsRessourcesViewModelFactory SourceFactory { get; set; }
        public PortraitsRessources PortraitsRessources { get; private set; }
        public PortraitsRessourcesViewModel(PortraitsRessources portraitsRessources)
        {
            PortraitsRessources = portraitsRessources;
        }


        private List<Portraits> _AvailablePortrais;
        public List<Portraits> AvailablePortraits
        {
            get
            {
                if (_AvailablePortrais==null)
                    _AvailablePortrais = PortraitsRessources.RessourceCorrespondance.Select(kv => kv.Value).ToList();
                return _AvailablePortrais;
            }
        }
        CollectionView _AvailablePortraitsView;
        public CollectionView AvailablePortraitsView
        {
            get
            {
                if (_AvailablePortraitsView == null)
                {

                    _AvailablePortraitsView = (CollectionView)CollectionViewSource.GetDefaultView(AvailablePortraits);
                    //_FilesToWriteView = new CollectionView(FilesToWrite);
                    //_FilesToWriteView.Filter = x => ((ISSWritable)x).WillCreateFile;
                    //PropertyGroupDescription groupDescription = new PropertyGroupDescription("SourceMod", new PortraitModToGroupConverter());
                    _AvailablePortraitsView.GroupDescriptions.Clear();
                    _AvailablePortraitsView.GroupDescriptions.Add(PortraitsRessources.GroupDescription);
                }
                return _AvailablePortraitsView;
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
            }
        }

    }

}
