using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSDirectory
    {

        public ObservableCollection<SSFileGroup> GroupedFiles { get; private set; } = new ObservableCollection<SSFileGroup>();
        public ObservableCollection<SSMod> Mods { get; private set; } = new ObservableCollection<SSMod>();
        public SSBaseUrl InstallationUrl { get; set; }

        SSModFactory modFactory;

        public SSDirectory(SSBaseUrl url)
        {
            InstallationUrl = url;  
        }

        public void ReadMods()
        {

            modFactory = new SSModFactory(InstallationUrl);
            modFactory.Type = SSMod.ModType.Mod;
            SSBaseUrl ModFolderPath = InstallationUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();
            ReadMod(new SSLinkUrl("starsector-core"));
            foreach (DirectoryInfo ModDirectory in ModsEnumerable)
            {
                ReadMod(new SSLinkUrl(Path.Combine("mods", ModDirectory.Name)));               
            }
        }

        private void ReadMod(SSLinkUrl modLink)
        {
            SSMod exist = Mods.FirstOrDefault(M => M.ModUrl.Link.Equals(modLink.Link));
            if (exist != null)
                throw new ArgumentException("Cannot add existing mod to directory");
            SSMod currentMod = modFactory.CreateMod(modLink);
            Mods.Add(currentMod);
            foreach (SSFile modFile in currentMod.FilesReadOnly)
            {
                SSFileGroup matchingGroup = GroupedFiles.SingleOrDefault(T => T.CommonRelativeUrl.Equals(modFile.RelativePath.GetRelative()));
                if (matchingGroup == null)
                {
                    matchingGroup = new SSFileGroup();
                    GroupedFiles.Add(matchingGroup);
                }
                matchingGroup.Add(modFile);
            }
        }
    }
}
