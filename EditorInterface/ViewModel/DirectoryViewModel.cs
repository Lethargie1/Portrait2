using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

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
            set
            {
                this._SelectedMod = value;
                NotifyOfPropertyChange(nameof(SelectedModViewModel));
            }
        }

        public ModDetailedViewModel SelectedModViewModel
        {
            get => new ModDetailedViewModel(SelectedMod);
        }
        public SSDirectory Directory { get; private set; }

        public DirectoryViewModel(SSDirectory directory)
        {
            Directory = directory;
            StarsectorUrl.ValidityChecker = StarsectorValidityChecker.CheckSSFolderValidity;
            
            StarsectorUrl.Bind(x => x.UrlState, StarsectorUrl_StateChanged);
            StarsectorUrl.Url = Properties.Settings.Default.StarsectorUrl;
        }

        //public bool CanExploreDirectory
        //{
        //    get
        //    {
        //        if (StarsectorUrl.UrlState == URLstate.Acceptable && TargetModFolderUrl.UrlState == URLstate.Acceptable)
        //            return true;
        //        else
        //            return false;
        //    }
        //}
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

        public void HandleModChecking(SSMod sender) 
        {
            

            if (SSMod.Switchable.Contains(sender.CurrentType))
            {
                if (SSMod.Unactivated.Contains(sender.CurrentType))
                    sender.ChangeType(ModType.Mod);
                else
                    sender.ChangeType(ModType.Skip);
            }
        }
        public void DirectoryExploreComplete()
        {
            Directory.PopulateMergedCollections();
            this.RequestClose();
        }


        
    }

    [ValueConversion(typeof(ModType), typeof(bool))]
    public class ModTypeToActivatedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModType source = (ModType)value;
            if (SSMod.Switchable.Contains(source))
            {
                if (SSMod.Unactivated.Contains(source))
                    return false;
                else
                    return true;
            }
            else
            {
                if (SSMod.AlwaysFalse.Contains(source))
                    return false;
                if (SSMod.AlwaysTrue.Contains(source))
                    return true;
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(ModType), typeof(bool))]
    public class ModTypeToEnabledConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ModType source = (ModType)value;
            if (SSMod.Switchable.Contains(source))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
