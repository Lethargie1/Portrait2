using FVJson;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSDirectory
    {

        public Dictionary<string,ISSGroup> GroupedFiles { get; private set; } = new Dictionary<string,ISSGroup>();
        public ObservableCollection<SSMod> Mods { get; private set; } = new ObservableCollection<SSMod>();
        public SSBaseUrl InstallationUrl { get; set; }
        public List<ISSGroup> factions { get; set; } = null;
        public List<JsonToken> Portraits { get; set; }

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
            modFactory.Type = ModType.Mod;
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
                    currentMod.ChangeType(ModType.skip);
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
                if (currentMod.CurrentType == ModType.skip || currentMod.CurrentType == ModType.Ressource)
                    continue;
                foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
                {
                    if (!(modFile is ISSMergable MergableModFile))
                    { continue; }

                    if (!GroupedFiles.ContainsKey(MergableModFile.RelativeUrl.ToString()))
                    {
                        ISSGroup matchingGroup = groupFactory.CreateGroupFromFile(MergableModFile);
                        GroupedFiles.Add(matchingGroup.RelativeUrl.ToString(), matchingGroup);
                    }
                    else
                    {
                        ISSGroup matchingGroup = GroupedFiles[MergableModFile.RelativeUrl.ToString()];
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
                if (currentMod.CurrentType == ModType.skip || currentMod.CurrentType == ModType.Ressource || currentMod.CurrentType == ModType.Core)
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
        public List<ISSGroup> GetMergedFaction()
        {
            factions = (from KeyValuePair<string, ISSGroup> kv in GroupedFiles
                                     where kv.Value is SSFactionGroup
                                     select kv.Value).Select(g => {
                                         g.MustOverwrite = true;
                                         (g as SSFactionGroup).ExtractMonitoredContent();
                                         return g;
                                     }).ToList();
            Portraits = factions.Cast<SSFactionGroup>().SelectMany(g =>
            {
                List<JsonToken> result = new List<JsonToken>();
                if (g.MalePortraits != null)
                    result.AddRange(g.MalePortraits.ContentArray);
                if (g.FemalePortraits != null)
                    result.AddRange(g.FemalePortraits?.ContentArray);
                return result;
            }).Distinct().ToList();
            return factions;
        }

        public void CopyFactions(SSLinkUrl newModLink)
        {
            if (factions == null)
                throw new InvalidOperationException("no factions merged");
            
            JsonObject finalPortraits = new JsonObject(Portraits, "portraits");
            JsonObject settingContent = new JsonObject();
            settingContent.AddSubField(".graphics.portraits", finalPortraits);

            foreach (ISSGroup f in factions)
            {
                if (f is SSFactionGroup g)
                {
                    g.WriteTo(InstallationUrl + newModLink);
                }
            }
            SSJsonGroup.WriteJsonTo(InstallationUrl + newModLink + new SSRelativeUrl("data\\config\\settings.json"), settingContent);
            GenerateModInfo(newModLink, factions);


        }


        public void GenerateModInfo(SSLinkUrl newModLink, List<ISSGroup> mergedGroup = null)
        {
            JsonObject root = new JsonObject();
            root.Values.Add(new JsonValue("id"), new JsonValue("testlfe"));
            root.Values.Add(new JsonValue("name"), new JsonValue("Lethargie metafaction editor"));
            root.Values.Add(new JsonValue("author"), new JsonValue("Lethargie"));
            root.Values.Add(new JsonValue("version"), new JsonValue("1.0"));
            root.Values.Add(new JsonValue("gameVersion"), new JsonValue("0.9.1a"));
            if (mergedGroup == null || mergedGroup.Count == 0)
                root.Values.Add(new JsonValue("description"), new JsonValue("merged  patch automagicaly generated from no source mod"));
            else
            {
                IEnumerable<ISSJsonGroup> fGroups = from ISSGroup fg in mergedGroup
                                                     where fg is ISSJsonGroup
                                                     select fg as ISSJsonGroup;
                var a = fGroups.SelectMany(fg => fg.GetJSonFiles()).Select(f => f.SourceMod.ModName).Distinct();
                string modlist = string.Join(", ", a);
                root.Values.Add(new JsonValue("description"), new JsonValue("merged  patch automagicaly generated from: " + modlist));
                var b = fGroups.Select(f => f.RelativeUrl.ToString());
                JsonArray replaceList = new JsonArray();
                foreach (string replace in b)
                {
                    string cleaned = Regex.Replace(replace, @"\\", @"\\");
                    replaceList.Values.Add(new JsonValue(cleaned));
                }
                root.Values.Add(new JsonValue("replace"), replaceList);
            }


            SSFullUrl TargetUrl = InstallationUrl + newModLink + new SSRelativeUrl("mod_info.json");

            //we need to make sure the directory exist
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
            {
                string result = root.ToJsonString();
                sw.Write(result);
            }


        }

        //public void MergeDirectory(SSLinkUrl newModLink)
        //{
        //    CopyUnmergable(newModLink);

        //    IEnumerable<ISSJsonGroup> fGroups = from ISSGroup fg in GroupedFiles
        //                                            where fg is ISSJsonGroup
        //                                            select fg as ISSJsonGroup;
        //    var a = fGroups.SelectMany(fg => fg.GetJSonFiles());
        //    IEnumerable<ISSJson> failedExtractedFile = from ISSJson f in a
        //                                               where f.ExtractedProperly == false
        //                                               select f;
        //    CopyMergable(newModLink);
        //}

        private void ModTypeChangedHandler(Object sender, ModTypeChangeEventArgs e)
        {
            //empty for now
            if (e.OldType == ModType.Mod)
            {
                if (e.NewType == ModType.Ressource || e.NewType == ModType.skip)
                {
                    //we went from mod to skip, remove files from the merged list
                }
            }
            else if (e.NewType == ModType.Mod)
            {
                //we went from skip to mod, perhaps add file to list
            }
        }
    }
}
