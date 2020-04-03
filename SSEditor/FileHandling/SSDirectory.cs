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
            Mods.Clear();
            modFactory = new SSModFactory(InstallationUrl);
            modFactory.Type = SSMod.ModType.Mod;
            SSBaseUrl ModFolderPath = InstallationUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();

            SSMod currentMod = modFactory.CreateMod(new SSLinkUrl("starsector-core"));
            Mods.Add(currentMod);
            foreach (DirectoryInfo ModDirectory in ModsEnumerable)
            {
                SSLinkUrl modLink = new SSLinkUrl(Path.Combine("mods", ModDirectory.Name));
                SSMod exist = Mods.FirstOrDefault(M => M.ModUrl.Link.Equals(modLink.Link));
                if (exist != null)
                    throw new ArgumentException("Cannot add existing mod to directory");
                currentMod = modFactory.CreateMod(modLink);
                currentMod.TypeChanged += ModTypeChangedHandler;
                Mods.Add(currentMod);
            }
        }

        public void PopulateMergedCollections()
        {
            GroupedFiles.Clear();
            foreach (SSMod currentMod in Mods)
            {
                if (currentMod.CurrentType == SSMod.ModType.skip || currentMod.CurrentType == SSMod.ModType.Ressource)
                    continue;
                foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
                {
                    if (modFile is SSNoMergeFile)
                    { continue; }

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
                            case SSFileCsv fileCsv:
                                SSFileCsvGroup fcg = matchingGroup as SSFileCsvGroup;
                                fcg.Add(fileCsv);
                                break;
                            case SSGenericFile file:
                                if (matchingGroup is SSFileGroup<SSGenericFile> genericGroup)
                                {
                                    genericGroup.Add(file);
                                }
                                break;
                            default:
                                throw new NotImplementedException("Could not add a file in the directory to a known group");
                                

                        }

                    }

                }
            }
        }

        public void MergeDirectory(SSLinkUrl newModLink)
        {
            //foreach (SSMod currentMod in Mods)
            //{
            //    if (currentMod.CurrentType == SSMod.ModType.skip || currentMod.CurrentType == SSMod.ModType.Ressource || currentMod.CurrentType == SSMod.ModType.Core)
            //        continue;
            //    foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
            //    {
            //        if (modFile is SSNoMergeFile)
            //        {
            //            modFile.CopyTo(InstallationUrl + newModLink);
            //        }
            //    }
            //}
            IEnumerable<SSFileCsvGroup> csvGroups = from ISSFileGroup fg in GroupedFiles
                    where fg is SSFileCsvGroup
                    select fg as SSFileCsvGroup;
            //foreach (SSFileCsvGroup csvGroup in csvGroups)
            //{
            //    csvGroup.WriteMergeTo(InstallationUrl + newModLink);
            //}
            IEnumerable<SSFactionGroup> fGroups = from ISSFileGroup fg in GroupedFiles
                                                    where fg is SSFactionGroup
                                                    select fg as SSFactionGroup;
            foreach (SSFactionGroup fg in fGroups)
            {
                fg.MustOverwrite = true;
                fg.WriteMergeTo(InstallationUrl + newModLink);
            }
        }

        private void ModTypeChangedHandler(Object sender, ModTypeChangeEventArgs e)
        {
            //empty for now
            if (e.OldType == SSMod.ModType.Mod)
            {
                if (e.NewType == SSMod.ModType.Ressource || e.NewType == SSMod.ModType.skip)
                {
                    //we went from mod to skip, remove files from the merged list
                }
            }
            else if (e.NewType == SSMod.ModType.Mod)
            {
                //we went from skip to mod, perhaps add file to list
            }
        }
    }
}
