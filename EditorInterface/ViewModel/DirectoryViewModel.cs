using EditorInterface.Properties;
using EditorInterface.Validation;
using FluentValidation;
using Ookii.Dialogs.Wpf;
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
        private string _FolderUrl;
        public string FolderUrl 
        {
            get 
            {
                if (_FolderUrl == null)
                    _FolderUrl = Settings.Default.StarsectorUrl;
                return _FolderUrl; 
            } 
            set { SetAndNotify(ref _FolderUrl, value); NotifyOfPropertyChange(nameof(HasNoFolderError)); }
        }

        public bool HasNoFolderError
        {
            get
            {
                var test = this.GetErrors(nameof(FolderUrl));
                if (test == null || test.Cast<Object>().Count() == 0)
                    return true;
                else
                    return false;
            }
        }

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

        public DirectoryViewModel(SSDirectory directory, IModelValidator<DirectoryViewModel> validator) : base(validator)
        {
            Directory = directory;
            this.Validate();
        }


        public void ExploreDirectory()
        {
            Properties.Settings.Default.StarsectorUrl = FolderUrl;
            Properties.Settings.Default.Save();
            Directory.SetUrl(FolderUrl);
            Directory.ReadMods();
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

        public void SelectNewUrl()
        {
            VistaFolderBrowserDialog OpenRootFolder = new VistaFolderBrowserDialog();
            if (OpenRootFolder.ShowDialog() == true)
            {
                this.FolderUrl = OpenRootFolder.SelectedPath;

            }
            return;
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

    public class DirectoryViewModelValidator : AbstractValidator<DirectoryViewModel>
    {
        public DirectoryViewModelValidator()
        {
            RuleFor(x => x.FolderUrl).Must(x =>
            {
                return StarsectorValidityChecker.CheckSSFolderValidity(x);
            }).WithMessage("Path must lead to Starsector installation");
        }
    }
}
