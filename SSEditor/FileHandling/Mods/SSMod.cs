﻿using FVJson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{

    public class SSMod : ISSMod
    {
       

        public event EventHandler<ModTypeChangeEventArgs> TypeChanged;


        public SSBaseLinkUrl ModUrl { get; private set; }
        public SSJson ModInfo { get; private set; }
        public ModType CurrentType { get; private set; }
        public string ModName { get; private set; }

        public override string ToString()
        {
            return "Mod: " + ((ModInfo?.Fields[".name"] as JsonValue)?.ToString() ?? ("Unnamed " + ModUrl.Link));
        }

        public ReadOnlyObservableCollection<ISSGenericFile> FilesReadOnly { get; private set; }
        protected ObservableCollection<ISSGenericFile> Files { get; } = new ObservableCollection<ISSGenericFile>();

        public SSMod(SSBaseLinkUrl fullModUrl, ModType type)
        {
            ModUrl = fullModUrl ?? throw new ArgumentNullException("fullModUrl", "Mod Url cannot be null");
            DirectoryInfo FactionDirectory = new DirectoryInfo(ModUrl.ToString());
            ModName = FactionDirectory.Name;
            CurrentType = type;
            FilesReadOnly = new ReadOnlyObservableCollection<ISSGenericFile>(Files);

            GenerateModInfo();
            //ModInfo = new SSJson(this, fullModUrl + new SSRelativeUrl("mod_info.json"));
        }
        private void GenerateModInfo()
        {
            if (CurrentType== ModType.Core)
            {
                JsonObject root = new JsonObject();
                root.Values.Add(new JsonValue("id"), new JsonValue("starsector-core"));
                root.Values.Add(new JsonValue("name"), new JsonValue("Vanilla starsector"));
                SSFullUrl TargetUrl = ModUrl + new SSRelativeUrl("mod_info.json");
                ModInfo = new SSJson(this, TargetUrl);
                ModInfo.JsonType = SSJson.JsonFileType.NotExtrated;
                ModInfo.JsonContent = root;
            }
            else
            {
                SSFullUrl TargetUrl = ModUrl + new SSRelativeUrl("mod_info.json");
                FileInfo test = new FileInfo(TargetUrl.ToString());
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
            if (CurrentType == ModType.skip || CurrentType == ModType.Ressource)
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
                Files.Add(SSGenericFileFactory.BuildFile(this, fileUrl));
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
        public static SSLinkUrl CoreLink = new SSLinkUrl("starsector-core");
        public SSBaseUrl InstallationUrl { get; set; }
        public ModType Type { get; set; } = ModType.skip;

        public SSModFactory (SSBaseUrl installation)
        {
            InstallationUrl = installation;
        }

        public SSMod CreateMod(SSLinkUrl link)
        {
            SSMod newMod;
            if (link.Equals(CoreLink))
                newMod = new SSMod(InstallationUrl + link, ModType.Core);
            else
                newMod = new SSMod(InstallationUrl + link, Type);
            
            return newMod;
        }
    }
}
