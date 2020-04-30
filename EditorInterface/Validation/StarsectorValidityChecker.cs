using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EditorInterface.Validation
{
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

        public static bool CheckModFolderEmpty(string starsectorUrl)
        {
            if (starsectorUrl == null)
                throw new ArgumentException("Cannot check empty path");
                //return Regex.IsMatch(url, starsectorUrl.Replace("\\", "\\\\") + @"\\mods\\[^\\]+");
                DirectoryInfo root = new DirectoryInfo(starsectorUrl);
            if (root.Exists == false)
                return true;
                IEnumerable<FileInfo> AllFiles = root.EnumerateFiles(".", SearchOption.AllDirectories);
                if (AllFiles.Count() > 0)
                {
                    return false;
                }
                else
                    return true;

        }
    }
}
