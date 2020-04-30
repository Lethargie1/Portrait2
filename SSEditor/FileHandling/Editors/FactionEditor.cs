using FVJson;
using SSEditor.Ressources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling.Editors
{
    public class FactionEditor
    {
        public SSDirectory Directory { get; set; }

        public PortraitsRessources PortraitsRessource { get; set; }
        public List<SSFactionGroup> Factions { get; set; } = null;
        public List<JsonValue> Portraits { get; set; }

        public FactionEditor() { }
        /// <summary>Constructor for an editor modifying faction files of the directory</summary>
        /// <param name="directory">list of all groups made by currents mods</param>
        /// <param name="receiver">Mod target where modification are stored</param>
        public FactionEditor(SSDirectory directory)
        {
            this.Directory = directory;
            this.GetFaction();
            this.PortraitsRessource = new PortraitsRessources(Directory);
        }

        /// <summary>Extract faction group from the directory</summary>
        public List<SSFactionGroup> GetFaction()
        {
            Factions = (from KeyValuePair<string, ISSGroup> kv in Directory.GroupedFiles
                        where kv.Value is SSFactionGroup
                        select kv.Value).Select(g =>
                        {
                            SSFactionGroup f = (SSFactionGroup)g;
                            if (f.MonitoredContent == null)
                                f.ExtractMonitoredContent();
                            return g as SSFactionGroup;
                        }).ToList();
            
            return Factions;
        }

        /// <summary>Replace whatever faction are in the receiver by those of this editor</summary>
        public void ReplaceFactionToWrite(SSModWritable receiver)
        {
            if (Factions == null)
                throw new InvalidOperationException("no factions merged");
            
            IEnumerable<SSFactionGroup> OldFactions = from ISSWritable w in receiver.FileList
                                                      where w is SSFactionGroup
                                                      select w as SSFactionGroup;
            foreach (SSFactionGroup f in OldFactions)
                receiver.FileList.Remove(f);
            foreach (SSFactionGroup f in Factions)
                receiver.FileList.Add(f);

            SSRelativeUrl settingUrl = new SSRelativeUrl("data\\config\\settings.json");
            SSJson SettingFile = (from ISSWritable w in receiver.FileList
                                where w.RelativeUrl == settingUrl
                                select w as SSJson).SingleOrDefault();
            if (SettingFile == null)
            {
                SettingFile = new SSJson(receiver,  settingUrl);
                SettingFile.JsonContent = new JsonObject();
                receiver.FileList.Add(SettingFile);
            }


            Portraits = new List<JsonValue>(Ressources.PortraitsRessources.GetOriginalPortraits(Factions));
            JsonObject finalPortraits = new JsonObject(Portraits, "portraits");
            SettingFile.JsonContent.AddSubField(".graphics.portraits", finalPortraits);


            IEnumerable<SSFactionGroup> writed = from SSFactionGroup f in Factions
                                                 where f.WillCreateFile == true
                                                 select f;
            IEnumerable<string> UsedMod = writed.Select(f => f.MonitoredContent).SelectMany(m => m.Files).Select(f => f.SourceMod).Distinct().Select(mod => mod.ModName);
            JsonValue OldDesc = receiver.ModInfo.Fields[".description"] as JsonValue;
            string old = OldDesc.ToString();
            OldDesc.SetContent(old + " Faction were modified using mods: " + string.Join(", ", UsedMod));
        }
    }

    public class FactionEditorFactory
    {
        private SSDirectory Directory { get; set; }
        public FactionEditorFactory(SSDirectory directory)
        {
            Directory = directory;
        }

        public FactionEditor CreateFactionEditor()
        {
            return new FactionEditor(Directory);
        }
    }
}
