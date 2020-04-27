using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class DirectoryViewModel : Screen
    {
        public EditableUrlViewModel StarsectorUrl { get; set; } = new EditableUrlViewModel("Starsector folder");
        public EditableUrlViewModel TargetModFolderUrl { get; set; } = new EditableUrlViewModel("Folder where patch will be written");

        private SSMod _SelectedMod;
        public SSMod SelectedMod 
        {
            get => this._SelectedMod;
            set => this._SelectedMod = value;
                }
        public SSDirectory Directory { get; private set; }

        public DirectoryViewModel(SSDirectory directory)
        {
            Directory = directory;
            StarsectorUrl.ValidityChecker = StarsectorValidityChecker.CheckSSFolderValidity;
            
            StarsectorUrl.Bind(x => x.UrlState, StarsectorUrl_StateChanged);
            StarsectorUrl.Url = Properties.Settings.Default.StarsectorUrl;
        }

        public bool CanExploreDirectory()
        {
            if (StarsectorUrl.UrlState == URLstate.Acceptable && TargetModFolderUrl.UrlState == URLstate.Acceptable)
                return true;
            else
                return false;
        }
        public void ExploreDirectory()
        {
            Properties.Settings.Default.StarsectorUrl = StarsectorUrl.Url;
            Properties.Settings.Default.Save();
            Directory.SetUrl(StarsectorUrl.Url);
            Directory.ReadMods();
        }

        private void StarsectorUrl_StateChanged(object sender, Stylet.PropertyChangedExtendedEventArgs<URLstate> e)
        {
                if (e.NewValue == URLstate.Acceptable)
                {
                    TargetModFolderUrl.ValidityChecker = StarsectorValidityChecker.GetCheckModFolderValidity(StarsectorUrl.Url);
                    TargetModFolderUrl.Url = StarsectorUrl.Url + Properties.Settings.Default.PatchTarget;
                }

        }


    }
}
