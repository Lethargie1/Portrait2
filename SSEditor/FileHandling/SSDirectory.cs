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

        public ObservableCollection<ISSFileGroup> GroupedFiles { get; private set; } = new ObservableCollection<ISSFileGroup>();
        public ObservableCollection<SSMod> Mods { get; private set; } = new ObservableCollection<SSMod>();
        public SSBaseUrl InstallationUrl { get; set; }

        SSModFactory modFactory;
        private SSFileGroupFactory groupFactory = new SSFileGroupFactory();

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
            foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
            {
                ISSFileGroup matchingGroup = GroupedFiles.SingleOrDefault(T =>
                {
                    return T.CommonRelativeUrl.Equals(modFile.LinkRelativeUrl.GetRelative());
                });
                if (matchingGroup == null)
                {
                    matchingGroup = groupFactory.CreateGroupFromFile(modFile);
                    GroupedFiles.Add(matchingGroup);
                }
                else
                {
                    switch (modFile)
                    {
                        case SSFactionFile factionfile:
                            if (matchingGroup is SSFactionGroup factionGroup)
                            {
                                factionGroup.Add(factionfile);
                            }
                            break;
                        case SSFile file:
                            if (matchingGroup is SSFileGroup<SSFile> fileGroup)
                            {
                                fileGroup.Add(file);
                            }
                            break;
                        case SSGenericFile file:
                            if (matchingGroup is SSFileGroup<SSGenericFile> genericGroup)
                            {
                                genericGroup.Add(file);
                            }
                            break;
                        default:
                            throw new NotImplementedException("Could not add a file in the directory to a known group");
                            break;   

                    }

                }
                
            }
        }
    }
}
