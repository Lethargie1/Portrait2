using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Ookii.Dialogs.Wpf;

using SSEditor.FileHandling;

namespace PortraitCrusher
{
    public enum URLstate { Acceptable, Rejected, Unset }
    public class URLViewModel : ViewModelBase
    {
        #region Field
        string _Url;
        #endregion

        #region Properties
        public string Url
        {
            get
            {
                return _Url;
            }
            set
            {
                _Url = value;
                base.NotifyPropertyChanged();
                base.NotifyPropertyChanged("DisplayUrl");
                base.NotifyPropertyChanged("UrlState");
            }
        }
        public string DisplayName { get; set; }
        public Predicate<string> ValidityChecker { get; set; } = null;
        public URLstate UrlState
        {
            get
            {
                if (_Url == null)
                    return URLstate.Unset;
                if (ValidityChecker == null)
                {
                    if (ExistFileValidityChecker(Url)== true)
                        return URLstate.Acceptable;
                    else
                        return URLstate.Rejected;
                }
                else
                {
                    if (ValidityChecker(this.Url) == true)
                        return URLstate.Acceptable;
                    else
                        return URLstate.Rejected;
                }

            }
        }
        #endregion

        #region Constructors
        public URLViewModel()
        {
            Url = null;
            DisplayName = null;
        }
        public URLViewModel(string displayName, string pointed = null)
        {
            Url = pointed;
            DisplayName = displayName;
        }
        #endregion

        public static bool ExistDirectoryValidityChecker(string url)
        {
            if (url == null)
                return false;
            DirectoryInfo Directory = new DirectoryInfo(url);
            if (!Directory.Exists)
            {
                return false;
            }
            else
                return true;
        }
        public static bool ExistFileValidityChecker(string url)
        {
            if (url == null)
                return false;
            FileInfo fileInfo = new FileInfo(url);
            if (!fileInfo.Exists)
            {
                return false;
            }
            else
                return true;
        }
    }

    public class EditableURLViewModel : URLViewModel
    {
        #region Command properties
        RelayCommand<object> _SelectNewUrlCommand;
        public ICommand SelectNewUrlCommand
        {
            get
            {
                if (_SelectNewUrlCommand == null)
                {
                    _SelectNewUrlCommand = new RelayCommand<object>(param => this.SelectNewUrl_Execute());
                }
                return _SelectNewUrlCommand;
            }
        }
        #endregion

        #region Properties
        public string ButtonName { get; set; } = "Edit Url";
        #endregion

        #region Constructors
        public EditableURLViewModel()
            : base()
        { }
        public EditableURLViewModel(string displayName, string buttonName)
            : base(displayName)
        {
            ButtonName = buttonName;
        }
        #endregion

        #region helper method
        private void SelectNewUrl_Execute()
        {
            VistaFolderBrowserDialog OpenRootFolder = new VistaFolderBrowserDialog();
            if (OpenRootFolder.ShowDialog() == true)
            {
                base.Url = OpenRootFolder.SelectedPath;

            }
            return;
        }
        #endregion

    }


    public static class StarsectorValidityChecker
    {
        public static bool CheckSSFolderValidity(string url)
        {
            if (url == null)
                return false;
            try
            {


                DirectoryInfo CoreFactionDirectory = new DirectoryInfo(url);
                if (!CoreFactionDirectory.Exists)
                {
                    return false;
                }
                IEnumerable<DirectoryInfo> DirList = CoreFactionDirectory.EnumerateDirectories();
                List<DirectoryInfo> SSCoreFolder = (from dir in DirList
                                                    where dir.Name == "starsector-core"
                                                    select dir).ToList();
                IEnumerable<FileInfo> FileList = CoreFactionDirectory.EnumerateFiles();
                List<FileInfo> SSExecutable = (from file in FileList
                                               where file.Name == "starsector.exe"
                                               select file).ToList();
                if (SSCoreFolder.Count == 1 && SSExecutable.Count == 1)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Predicate<string> GetCheckModFolderValidity(string starsectorUrl)
        {
            Predicate<string> CheckModFolderValidity = delegate (string url)
             {
                 return Regex.IsMatch(url, starsectorUrl.Replace("\\","\\\\") + @"\\mods\\[^\\]+");
             };
            return CheckModFolderValidity;
        }
    }
}
