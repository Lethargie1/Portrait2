using FVJson;
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
    public class SSModWritable: ISSMod
    {
        public const string ID = "lepg";

        private SSBaseLinkUrl _ModUrl;
        public SSBaseLinkUrl ModUrl 
        { 
            get => _ModUrl;
            set 
            {
                _ModUrl = value;
                this.MakeModInfoBase();
            } 
        }
        public SSJson ModInfo { get; private set; }
        public string ModName { get; private set; }
        public ModType CurrentType { get; } = ModType.Mod;

        public ObservableCollection<ISSWritable> FileList { get; } = new ObservableCollection<ISSWritable>();
        public ObservableCollection<ISSMod> ModRequired { get; } = new ObservableCollection<ISSMod>();
        
        public SSModWritable()
        {
        }
        public void MakeModInfoBase()
        {
            if (ModUrl == null)
                throw new InvalidOperationException();
            JsonObject root = new JsonObject();
            root.Values.Add(new JsonValue("id"), new JsonValue("lepg"));
            root.Values.Add(new JsonValue("name"), new JsonValue("Lethargie's editable patch generator"));
            root.Values.Add(new JsonValue("author"), new JsonValue("Lethargie"));
            root.Values.Add(new JsonValue("version"), new JsonValue("1.0"));
            root.Values.Add(new JsonValue("gameVersion"), new JsonValue("0.9.1a"));
            root.Values.Add(new JsonValue("description"), new JsonValue("Lethargie editable patch is a mod created by Lethargie's editable patcher. It is able to take in account your personal modlist if you so desire "));
            root.Values.Add(new JsonValue("replace"), new JsonArray());
            SSFullUrl TargetUrl = ModUrl + new SSRelativeUrl("mod_info.json");
            if (ModInfo != null)
                FileList.Remove(ModInfo);
            ModInfo = new SSJson(this, TargetUrl.GetRelative());
            ModInfo.JsonType = SSJson.JsonFileType.NotExtrated;
            ModInfo.JsonContent = root;
            FileList.Add(ModInfo);
        }
        public void WriteMod()
        {
            try
            {
                var targetdir = new DirectoryInfo(ModUrl.ToString());
                if (targetdir.Exists)
                    targetdir.Delete(true);

                JsonArray replaceList = ModInfo.Fields[".replace"] as JsonArray;
                IEnumerable<ISSWritable> replaceFiles = from ISSWritable f in FileList
                                                        where f.MustOverwrite == true && f.WillCreateFile == true
                                                        select f;
                foreach (ISSWritable f in replaceFiles)
                {
                    string cleaned = Regex.Replace(f.RelativeUrl.ToString(), @"\\", @"\\");
                    replaceList.Values.Add(new JsonValue(cleaned));
                }

                foreach (ISSWritable f in FileList)
                {
                    f.WriteTo(ModUrl);
                }
            }catch (Exception e)
            {
                throw new Exception("failed");
            }
        }



        public override string ToString()
        {
            return "Mod: " + (ModInfo?.ReadToken("name")?.ToString() ?? ("Unnamed " + ModUrl.Link));
        }
    }
}
