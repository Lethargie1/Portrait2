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
        public event EventHandler Repopulated;
        public Dictionary<string, ISSGroup> GroupedFiles { get; private set; } = new Dictionary<string, ISSGroup>();
        public ObservableCollection<SSMod> Mods { get; private set; } = new ObservableCollection<SSMod>();
        public SSBaseUrl InstallationUrl { get; set; } = null;
        public List<ISSGroup> factions { get; set; } = null;
        public List<JsonToken> Portraits { get; set; }

        SSModFactory modFactory;
        private SSFileGroupFactory groupFactory = new SSFileGroupFactory();

        public SSDirectory() { }
        public SSDirectory(SSBaseUrl url)
        {
            InstallationUrl = url;
        }

        public JsonArray ReadUsedMod()
        {
            SSBaseLinkUrl ModFolderPath = InstallationUrl + new SSLinkUrl("mods");
            SSMod Dummy = new SSMod(ModFolderPath);
            SSRelativeUrl rela = new SSRelativeUrl("enabled_mods.json");

            SSJson file = new SSJson(Dummy, rela);
            file.ExtractFile();
            JsonToken trying;
            file.Fields.TryGetValue(".enabledMods", out trying);
            if (trying == null)
                return new JsonArray();
            return trying as JsonArray;
        }

        public void SetUrl(string directoryUrl)
        {
            InstallationUrl = new SSBaseUrl(directoryUrl);
        }
        public void ReadMods(string targetFolder = null)
        {
            Mods.Clear();
            modFactory = new SSModFactory(InstallationUrl);
            SSBaseUrl ModFolderPath = InstallationUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();


            SSMod currentMod = modFactory.CreateMod(new SSLinkUrl("starsector-core"));
            //currentMod.FindFiles();
            Mods.Add(currentMod);
            foreach (DirectoryInfo ModDirectory in ModsEnumerable)
            {
                SSLinkUrl modLink = new SSLinkUrl(Path.Combine("mods", ModDirectory.Name));
                SSMod exist = Mods.FirstOrDefault(M => M.ModUrl.Link.Equals(modLink.Link));
                if (exist != null)
                    throw new ArgumentException("Cannot add existing mod to directory");
                try
                {
                    currentMod = modFactory.CreateMod(modLink);
                    //if (ModDirectory.Name == targetFolder)
                        //currentMod.ChangeType(ModType.Skip);
                    //else
                        //currentMod.FindFiles();

                    Mods.Add(currentMod);
                }
                catch (FileNotFoundException)
                {
                    //incomplete mod, lets just not add it
                }
            }
        }

        public void PopulateMergedCollections()
        {
            GroupedFiles.Clear();
            foreach (SSMod currentMod in Mods)
            {
                if (currentMod.CurrentType == ModType.Skip || currentMod.CurrentType == ModType.Ressource || currentMod.CurrentType == ModType.Patch || currentMod.CurrentType == ModType.Other)
                    continue;
                foreach (ISSGenericFile modFile in currentMod.FilesReadOnly)
                {
                    if (modFile is SSNoMerge)
                        continue;
                    if (!GroupedFiles.ContainsKey(modFile.RelativeUrl.ToString()))
                    {
                        ISSGroup matchingGroup = groupFactory.CreateGroupFromFile(modFile);
                        GroupedFiles.Add(matchingGroup.RelativeUrl.ToString(), matchingGroup);
                    }
                    else
                    {
                        ISSGroup matchingGroup = GroupedFiles[modFile.RelativeUrl.ToString()];
                        matchingGroup.Add(modFile);

                    }

                }
            }
            OnRaiseRepopulated();
        }


        protected void OnRaiseRepopulated()
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler handler = Repopulated;
            handler?.Invoke(this, null);
        }

        /// <summary>Read same typed json from the directory and extract their content</summary>
        /// <param name="Refresh">Force call to ExtractMonitoredContent (this clear modification)</param>
        public IEnumerable<T> GetAndReadJsonGroupsByType<T> (bool refresh = false) where T:ISSJsonGroup
        {
            var Extracted = (from KeyValuePair<string, ISSGroup> kv in this.GroupedFiles
                                 where kv.Value is T
                                 select kv.Value).Select(g =>
                                 {
                                     T f = (T)g;
                                     if (f.MonitoredContent == null || refresh)
                                         f.ExtractMonitoredContent();
                                     return f;
                                 });
            return Extracted;
        }
    }


}
