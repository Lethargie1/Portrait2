using FVJson;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{

    public class SSMod :  PropertyChangedBase, ISSMod
    {

        public static List<ModType> Switchable = new List<ModType> { ModType.Mod, ModType.Ressource, ModType.Skip };
        public static List<ModType> Unactivated = new List<ModType> { ModType.Skip, ModType.Ressource };
        public static List<ModType> AlwaysFalse = new List<ModType> { ModType.Other, ModType.Patch };
        public static List<ModType> AlwaysTrue = new List<ModType> { ModType.Core};
        public event EventHandler<ModTypeChangeEventArgs> TypeChanged;

        public string DisplayName
        {
            get
            {
                return CurrentType.ToString() + ": " + ((ModInfo?.Fields[".name"] as JsonValue)?.ToString() ?? ("Unnamed " + ModUrl.Link));
            }
        }
        public SSBaseLinkUrl ModUrl { get; private set; }
        public SSJson ModInfo { get; private set; }
        public string ModId { get; private set; }
        private ModType _CurrentType;
        public ModType CurrentType { get => _CurrentType; private set { SetAndNotify(ref _CurrentType, value); NotifyOfPropertyChange(nameof(DisplayName)); } }
        public string ModName { get; private set; }

        public override string ToString()
        {
            return  CurrentType.ToString() + ": " + ((ModInfo?.Fields[".name"] as JsonValue)?.ToString() ?? ("Unnamed " + ModUrl.Link));
        }

        private ReadOnlyObservableCollection<ISSGenericFile> _FilesReadOnly;
        public ReadOnlyObservableCollection<ISSGenericFile> FilesReadOnly 
        {
            get 
            {
                if  (_FilesReadOnly == null)
                    _FilesReadOnly = new ReadOnlyObservableCollection<ISSGenericFile>(Files);
                return _FilesReadOnly;
            } 
        }

        private ObservableCollection<ISSGenericFile> _Files;
        protected ObservableCollection<ISSGenericFile> Files
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new ObservableCollection<ISSGenericFile>();
                    this.FindFiles();
                }
                return _Files;
            }
        }

        public SSMod(SSBaseLinkUrl fullModUrl)
        {
            ModUrl = fullModUrl ?? throw new ArgumentNullException("fullModUrl", "Mod Url cannot be null");
            DirectoryInfo FactionDirectory = new DirectoryInfo(ModUrl.ToString());
            ModName = FactionDirectory.Name;
            //CurrentType = type;
            

            FindModInfo();
            //ModInfo = new SSJson(this, fullModUrl + new SSRelativeUrl("mod_info.json"));
        }
        private void FindModInfo()
        {
            SSRelativeUrl ModInfoRela = new SSRelativeUrl("mod_info.json");
            SSFullUrl TargetUrl = ModUrl + ModInfoRela;
            FileInfo test = new FileInfo(TargetUrl.ToString());
            if (test.Exists)
            {
                this.CurrentType = ModType.Mod;
                ModInfo = new SSJson(this, ModInfoRela);
                ModName = ((JsonValue)ModInfo.Fields[".name"]).ToString();
                ModId = ((JsonValue)ModInfo.Fields[".id"]).ToString();
            }
            else
            {
                this.CurrentType = ModType.Other;
                ModName = TargetUrl.Link;
            }
        }

        public void MakeItCore()
        {
            if (ModInfo != null)
                throw new InvalidOperationException("cannot make Core, Mod_info exist");
            JsonObject root = new JsonObject();
            root.Values.Add(new JsonValue("id"), new JsonValue("starsector-core"));
            root.Values.Add(new JsonValue("name"), new JsonValue("Vanilla starsector"));
            root.Values.Add(new JsonValue("version"), new JsonValue("9.1a"));
            root.Values.Add(new JsonValue("description"), new JsonValue("It's the best"));
            SSRelativeUrl TargetUrl = new SSRelativeUrl("mod_info.json");
            ModInfo = new SSJson(this, TargetUrl);
            ModInfo.JsonType = SSJson.JsonFileType.NotExtrated;
            ModInfo.JsonContent = root;
            this.CurrentType = ModType.Core;
        }
        private void GenerateModInfo()
        {
            SSRelativeUrl TargetUrl = new SSRelativeUrl("mod_info.json");
            if (CurrentType== ModType.Core)
            {
                JsonObject root = new JsonObject();
                root.Values.Add(new JsonValue("id"), new JsonValue("starsector-core"));
                root.Values.Add(new JsonValue("name"), new JsonValue("Vanilla starsector"));
                
                ModInfo = new SSJson(this, TargetUrl);
                ModInfo.JsonType = SSJson.JsonFileType.NotExtrated;
                ModInfo.JsonContent = root;
            }
            else
            {
                SSFullUrl TestUrl = ModUrl + new SSRelativeUrl("mod_info.json");
                FileInfo test = new FileInfo(TestUrl.ToString());
                if (test.Exists)
                    ModInfo = new SSJson(this, TargetUrl);
                else
                    throw new FileNotFoundException("Mod_info was not found");
            }
        }
        public void ChangeType(ModType newType)
        {
            if (CurrentType == ModType.Core)
            {
                throw new InvalidOperationException("Cannot change type of the core startector");
            }
            ModType OldType = CurrentType;
            CurrentType = newType;
            OnRaiseTypeChanged(new ModTypeChangeEventArgs(OldType, CurrentType));
        }
        protected void OnRaiseTypeChanged(ModTypeChangeEventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of
            // a race condition if the last subscriber unsubscribes
            // immediately after the null check and before the event is raised.
            EventHandler<ModTypeChangeEventArgs> handler = TypeChanged;
            handler?.Invoke(this, e);            
        }

        public void FindFiles()
        {
            Files.Clear();
            if (CurrentType == ModType.Skip || CurrentType == ModType.Ressource)
                return;
            DirectoryInfo root = new DirectoryInfo(ModUrl.ToString());
            IEnumerable<FileInfo> AllFiles = root.EnumerateFiles(".", SearchOption.AllDirectories);

            DirectoryInfo upDirectory;
            string relativePath;
            foreach (FileInfo f in AllFiles)
            {
                upDirectory = f.Directory;
                relativePath = "";
                if (upDirectory.FullName == root.FullName)
                {
                    if (f.Extension == ".txt")
                    {
                        continue;
                    }
                }
                while (upDirectory.FullName != root.FullName)
                {
                    relativePath = upDirectory.Name + "\\" + relativePath;
                    upDirectory = upDirectory.Parent;
                }
                SSFullUrl fileUrl = ModUrl + new SSRelativeUrl(relativePath) + f.Name;
                Files.Add(SSGenericFileFactory.BuildFile(this, fileUrl.GetRelative()));
            }

            
        }
    }

    public class ModTypeChangeEventArgs : EventArgs
    {
        public ModTypeChangeEventArgs(ModType oldType, ModType newType)
        {
            OldType = oldType;
            NewType = newType;
        }
        public ModType OldType { get; private set; }
        public ModType NewType { get; private set; }
    }

    public class SSModFactory
    {
        public const string CoreLink = "starsector-core";
        public SSBaseUrl InstallationUrl { get; set; }

        public SSModFactory (SSBaseUrl installation)
        {
            InstallationUrl = installation;
        }

        public SSMod CreateMod(SSLinkUrl link)
        {
            SSMod newMod;
            switch (link.Link)
            {
                case CoreLink:
                    newMod = new SSMod(InstallationUrl + link);
                    newMod.MakeItCore();
                    break;
                default:
                    newMod = new SSMod(InstallationUrl + link);
                    if (newMod.CurrentType != ModType.Other)
                    {
                        string modId = ((JsonValue)newMod.ModInfo.Fields[".id"]).ToString();
                        if (modId == SSModWritable.ID)
                            newMod.ChangeType(ModType.Patch);
                    }
                    break;
            }

                
            
            return newMod;
        }
    }
}
