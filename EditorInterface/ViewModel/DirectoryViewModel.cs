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

        public DirectoryViewModel()
        {
            StarsectorUrl.ConductWith(this);
            StarsectorUrl.ValidityChecker = StarsectorValidityChecker.CheckSSFolderValidity;
            StarsectorUrl.Bind(x => x.UrlState, StarsectorUrl_StateChanged);
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
