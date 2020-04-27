using Ookii.Dialogs.Wpf;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public enum URLstate { Acceptable, Rejected, Unset }
    public class EditableUrlViewModel : Screen
    {
        

        string _Url;
        public string Url
        {
            get{ return this._Url; }
            set{ SetAndNotify(ref _Url, value); this.NotifyOfPropertyChange(nameof(this.UrlState)); }
        }
        public string ControlName { get; private set; }
        public Predicate<string> ValidityChecker { get; set; } = null;
        public URLstate UrlState
        {
            get
            {
                if (_Url == null)
                    return URLstate.Unset;
                if (ValidityChecker == null)
                {
                    if (ExistFileValidityChecker(Url) == true)
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
        public string ButtonName { get; set; } = "Select path";

        #region Constructors
        public EditableUrlViewModel()
        {
            Url = null;
            ControlName = null;
        }
        public EditableUrlViewModel(string displayName, string pointed = null)
        {
            Url = pointed;
            ControlName = displayName;
        }
        #endregion

        public void SelectNewUrl()
        {
            VistaFolderBrowserDialog OpenRootFolder = new VistaFolderBrowserDialog();
            if (OpenRootFolder.ShowDialog() == true)
            {
                this.Url = OpenRootFolder.SelectedPath;

            }
            return;
        }



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
                return Regex.IsMatch(url, starsectorUrl.Replace("\\", "\\\\") + @"\\mods\\[^\\]+");
            };
            return CheckModFolderValidity;
        }
    }
}
