using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    
    public class SSMod
    {
        public enum ModType {Core, Mod, Ressource, skip};

        public event EventHandler<ModTypeChangeEventArgs> TypeChanged;


        public SSBaseLinkUrl ModUrl { get; private set; }
        public SSFile ModInfo { get; private set; }
        public ModType CurrentType { get; private set; }

        public ReadOnlyObservableCollection<SSFile> FilesReadOnly { get; private set; }
        protected ObservableCollection<SSFile> Files { get; } = new ObservableCollection<SSFile>();

        public SSMod(SSBaseLinkUrl fullModUrl, ModType type)
        {
            ModUrl = fullModUrl ?? throw new ArgumentNullException("fullModUrl", "Mod Url cannot be null");
            CurrentType = type;
            FilesReadOnly = new ReadOnlyObservableCollection<SSFile>(Files);
        }

        public void ChangeType(ModType newType)
        {
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
            handler.Invoke(this, e);            
        }

        public void FindFiles()
        {
            if (CurrentType == ModType.skip || CurrentType == ModType.Ressource)
                return;
            SSFullUrl factionFolderUrl = ModUrl + new SSRelativeUrl("data\\world\\factions");

            DirectoryInfo FactionDirectory = new DirectoryInfo(factionFolderUrl.ToString());
            if (!FactionDirectory.Exists)
                return;

            IEnumerable<FileInfo> FileInfoList = FactionDirectory.EnumerateFiles();
            var Potential = from file in FileInfoList
                            where file.Extension == ".faction"
                            select file;

            foreach (FileInfo file in Potential)
            {
                Files.Add(new SSFile(factionFolderUrl + file.Name));
            }
        }
    }

    public class ModTypeChangeEventArgs : EventArgs
    {
        public ModTypeChangeEventArgs(SSMod.ModType oldType, SSMod.ModType newType)
        {
            OldType = oldType;
            NewType = newType;
        }
        public SSMod.ModType OldType { get; private set; }
        public SSMod.ModType NewType { get; private set; }
    }

    public class SSModFactory
    {
        public static SSLinkUrl CoreLink = new SSLinkUrl("starsector-core");
        public SSBaseUrl InstallationUrl { get; set; }
        public SSMod.ModType Type { get; set; } = SSMod.ModType.skip;

        public SSModFactory (SSBaseUrl installation)
        {
            InstallationUrl = installation;
        }

        public SSMod CreateMod(SSLinkUrl link)
        {
            SSMod newMod;
            if (link.Equals(CoreLink))
                newMod = new SSMod(InstallationUrl + link, SSMod.ModType.Core);
            else
                newMod = new SSMod(InstallationUrl + link, Type);
            newMod.FindFiles();
            return newMod;
        }
    }
}
