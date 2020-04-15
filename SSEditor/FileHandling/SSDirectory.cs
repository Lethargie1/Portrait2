using SSEditor.MonitoringField;
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

        public Dictionary<string,ISSGroup> GroupedFiles { get; private set; } = new Dictionary<string,ISSGroup>();
        public ObservableCollection<SSMod> Mods { get; private set; } = new ObservableCollection<SSMod>();
        public SSBaseUrl InstallationUrl { get; set; }

        SSModFactory modFactory;
        private SSFileGroupFactory groupFactory = new SSFileGroupFactory();

        public SSDirectory(SSBaseUrl url)
        {
            InstallationUrl = url;  
        }

        public void ReadMods(string targetFolder)
        {
            Mods.Clear();
            modFactory = new SSModFactory(InstallationUrl);
            modFactory.Type = SSMod.ModType.Mod;
            SSBaseUrl ModFolderPath = InstallationUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();

            SSMod currentMod = modFactory.CreateMod(new SSLinkUrl("starsector-core"));
            currentMod.FindFiles();
            Mods.Add(currentMod);
            foreach (DirectoryInfo ModDirectory in ModsEnumerable)
            {
                SSLinkUrl modLink = new SSLinkUrl(Path.Combine("mods", ModDirectory.Name));
                SSMod exist = Mods.FirstOrDefault(M => M.ModUrl.Link.Equals(modLink.Link));
                if (exist != null)
                    throw new ArgumentException("Cannot add existing mod to directory");
                currentMod = modFactory.CreateMod(modLink);
                if (ModDirectory.Name == targetFolder)
                    currentMod.ChangeType(SSMod.ModType.skip);
                else
                    currentMod.FindFiles();
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
                    if (!(modFile is ISSMergable MergableModFile))
                    { continue; }

                    if (!GroupedFiles.ContainsKey(MergableModFile.LinkRelativeUrl.GetRelative().ToString()))
                    {
                        ISSGroup matchingGroup = groupFactory.CreateGroupFromFile(MergableModFile);
                        GroupedFiles.Add(matchingGroup.CommonRelativeUrl.ToString(), matchingGroup);
                    }
                    else
                    {
                        ISSGroup matchingGroup = GroupedFiles[MergableModFile.LinkRelativeUrl.GetRelative().ToString()];
                        switch (MergableModFile)
                        {
                            case SSFaction factionfile:
                                if (matchingGroup is SSFactionGroup factionGroup)
                                {
                                    factionGroup.Add(factionfile);
                                }
                                break;
                            case SSJson file:
                                if (matchingGroup is SSGroup<SSJson> fileGroup)
                                {
                                    fileGroup.Add(file);
                                }
                                break;
                            case SSCsv fileCsv:
                                SSCsvGroup fcg = matchingGroup as SSCsvGroup;
                                fcg.Add(fileCsv);
                                break;
                            default:
                                throw new NotImplementedException("Could not add a file in the directory to a known group");
                                

                        }

                    }

                }
            }
        }

        public void CopyUnmergable(SSLinkUrl newModLink)
        {
            foreach (SSMod currentMod in Mods)
            {
                if (currentMod.CurrentType == SSMod.ModType.skip || currentMod.CurrentType == SSMod.ModType.Ressource || currentMod.CurrentType == SSMod.ModType.Core)
                    continue;
                foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
                {
                    if (modFile is SSNoMerge)
                    {
                        modFile.CopyTo(InstallationUrl + newModLink);
                    }
                }
            }
        }

        public void CopyMergable(SSLinkUrl newModLink)
        {
            SSJsonGroup mod = GroupedFiles["mod_info.json"] as SSJsonGroup;
            mod.ExtractMonitoredContent();
            MonitoredArray<SSJson> modplugin = new MonitoredArray<SSJson>() { FieldPath = ".modPlugin" };
            mod.CopyFilesToMonitored(modplugin);
            mod.MonitoredContent.MonitoredProperties[new FVJson.JsonValue("modPlugin")] = modplugin;

            foreach (KeyValuePair<string, ISSGroup> kvG in GroupedFiles)
            {
                kvG.Value.MustOverwrite = false;
                kvG.Value.WriteMergeTo(InstallationUrl + newModLink);
            }
            
            //IEnumerable<SSCsvGroup> CGroups = from ISSGroup fg in GroupedFiles
            //                                    where fg is SSCsvGroup
            //                                    select fg as SSCsvGroup;
            //foreach (SSCsvGroup fg in CGroups)
            //{
            //    fg.MustOverwrite = false;
            //    fg.WriteMergeTo(InstallationUrl + newModLink);
            //}
        }
        public void MergeDirectory(SSLinkUrl newModLink)
        {
            CopyUnmergable(newModLink);

            IEnumerable<ISSJsonGroup> fGroups = from ISSGroup fg in GroupedFiles
                                                    where fg is ISSJsonGroup
                                                    select fg as ISSJsonGroup;
            var a = fGroups.SelectMany(fg => fg.GetJSonFiles());
            IEnumerable<ISSJson> failedExtractedFile = from ISSJson f in a
                                                       where f.ExtractedProperly == false
                                                       select f;
            CopyMergable(newModLink);
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
