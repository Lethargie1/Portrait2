using FVJson;
using SSEditor.MonitoringField;
using SSEditor.Ressources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonToken = FVJson.JsonToken;

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
            foreach (SSFactionGroup f in OldFactions.ToList())
                receiver.FileList.Remove(f);
            foreach (SSFactionGroup f in Factions)
                receiver.FileList.Add(f);

            SSRelativeUrl settingUrl = new SSRelativeUrl("data\\config\\settings.json");
            SSJson SettingFile = (from ISSWritable w in receiver.FileList
                                where w.RelativeUrl.Equals(settingUrl)
                                select w as SSJson).SingleOrDefault();
            Portraits = new List<JsonValue>(Ressources.PortraitsRessources.GetOriginalPortraits(Factions));
            var UsedPortraits = new List<JsonValue>(Ressources.PortraitsRessources.GetCurrentPortraits(Factions));
            var UnusedPortrait = Portraits.Except(UsedPortraits);
            JsonObject finalPortraits = new JsonObject(UnusedPortrait, "portraits");
            if (SettingFile == null)
            {
                SettingFile = new SSJson(receiver,  settingUrl);
                SettingFile.JsonContent = new JsonObject();
                receiver.FileList.Add(SettingFile);
                SettingFile.JsonContent.AddSubField(".graphics.portraits", finalPortraits);
                SettingFile.RefreshFields();
            }
            else 
            {
                JsonToken setted;
                SettingFile.Fields.TryGetValue(".graphics.portraits", out setted);
                if (setted == null)
                    SettingFile.JsonContent.AddSubField(".graphics.portraits", finalPortraits);
                else
                {
                    JsonObject set = (JsonObject)setted;
                    set.Values.Clear();
                    foreach (KeyValuePair<JsonValue, JsonToken> kv in finalPortraits.Values)
                        set.Values.Add(kv.Key,kv.Value);
                    SettingFile.RefreshFields();
                }
            }


            
            


            IEnumerable<SSFactionGroup> OverWritten = from SSFactionGroup f in Factions
                                                 where f.MustOverwrite == true
                                                 select f;
            IEnumerable<string> ModOverWritten = OverWritten.Select(f => f.MonitoredContent).SelectMany(m => m.Files).Select(f => f.SourceMod).Distinct().Select(mod => mod.ModName);

            IEnumerable<JsonValue> AddedPortrait = Factions.FindAll(f => f.MonitoredContent.IsModified()).SelectMany(f =>
            {
                List<MonitoredArrayModification> result = new List<MonitoredArrayModification>();
                result.AddRange(f.MalePortraits.GetAddedMod());
                result.AddRange(f.FemalePortraits.GetAddedMod());
                return result;
                }).Select(m => ((JsonValue)m.Content)).Distinct();

            JsonRelativeToPortraits converter = new JsonRelativeToPortraits();
            IEnumerable<string> addMod = AddedPortrait.Select(j => converter.Convert(new object[] { j, PortraitsRessource }, null, null, null)).Select(p => ((Portraits)p).SourceModName).Distinct();

            var together = (ModOverWritten ?? Enumerable.Empty<string>()).Concat(addMod ?? Enumerable.Empty<string>()).Distinct() ;
            JsonValue OldDesc = receiver.ModInfo.Fields[".description"] as JsonValue;
            string old = OldDesc.ToString();
            OldDesc.SetContent(old + " Faction were modified using mods: " + string.Join(", ", together));
        }



        private JsonSerializerSettings  setting = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
        public string GetModificationsAsJson()
        {
            List<GroupModification> ModificationList = Factions.SelectMany(fg => fg.GetModifications()).ToList();
            
            return JsonConvert.SerializeObject(ModificationList, setting);
        }

        public void ApplyModificationFromJson(string text)
        {
            List<GroupModification> ModificationList = JsonConvert.DeserializeObject<List<GroupModification>>(text, setting);
            foreach (GroupModification gm in ModificationList)
            {
                

                SSFactionGroup match = Factions.Find(f => f.RelativeUrl.Equals(gm.GroupUrl));
                if (match!= null)
                {
                    if (gm.Modification.RessourceType == typeof(PortraitsRessources))
                    {
                        if (PortraitsRessource.FindBinaryFromDirectory(gm.Modification.GetContentAsValue().ToString()) == null)
                            continue;
                    }
                    else if (gm.Modification.RessourceType != null)
                    {
                        throw new ArgumentException("faction modification refer to unknown ressource");
                    }
                    MonitoredField pointed;
                    match.PathedContent.TryGetValue(gm.FieldPath, out pointed);
                    if (pointed == null)
                        continue;
                    switch (gm.Modification)
                    {
                        case MonitoredArrayModification mam:
                            ((MonitoredArray)pointed).Modify(mam);
                            break;
                        case MonitoredArrayValueModification mavm:
                            ((MonitoredArrayValue)pointed).Modify(mavm);
                            break;
                        case MonitoredValueModification mvm:
                            ((MonitoredValue)pointed).Modify(mvm);
                            break;
                    }
                    

                }
            }
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
