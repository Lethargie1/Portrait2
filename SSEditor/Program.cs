using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FVJson;
using SSEditor.FileHandling.Editors;

namespace SSEditor
{
    class Program
    {
        static void Main()
        {
            
            SSBaseUrl SSUrl = new SSBaseUrl("E:\\SS\\Starsector");
            SSLinkUrl CoreUrl = new SSLinkUrl("starsector-core");
            SSLinkUrl TahlanUrl = new SSLinkUrl("mods\\tahlan");
            SSLinkUrl SWPUrl = new SSLinkUrl("mods\\Ship and Weapon Pack");
            SSRelativeUrl HegemonyUrl = new SSRelativeUrl("data\\world\\factions\\hegemony.faction");

           

            SSBaseUrl ModFolderPath = SSUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();

            
            SSDirectory test = new SSDirectory(SSUrl);
            SSModWritable target = new SSModWritable(SSUrl + new SSLinkUrl("mods\\lepg"));
            test.ReadMods("lepg");
            test.PopulateMergedCollections();

            FactionEditor factionEditor = new FactionEditor(test, target);
            List<SSFactionGroup> factions = factionEditor.GetFaction();
            foreach (SSFactionGroup f in factions)
            {
                    f.MalePortraits?.ContentArray.Clear();
                    f.MalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai1.png"));
                    f.FemalePortraits?.ContentArray.Clear();
                    f.FemalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai2.png"));
                    f.MustOverwrite = true;
            }
            factionEditor.ReplaceFactionToWrite();
            target.WriteMod();
        }
    }
}