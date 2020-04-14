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
            
            test.ReadMods("hyes");
            test.PopulateMergedCollections();
            test.MergeDirectory(new SSLinkUrl("mods\\hyes"));
            SSModFactory factory = new SSModFactory(SSUrl);
            factory.Type = SSMod.ModType.Mod;
            SSMod CoreMod = factory.CreateMod(CoreUrl);
            SSMod TahlanMod = factory.CreateMod(TahlanUrl);

            SSFaction CoreHeg = new SSFaction(CoreMod, SSUrl + CoreUrl + HegemonyUrl);
            SSFaction TahlanHeg = new SSFaction(TahlanMod, SSUrl + TahlanUrl + HegemonyUrl);
            SSFaction SWPHeg = new SSFaction(CoreMod, SSUrl + SWPUrl + HegemonyUrl);

            List<string> TokenPath = new List<string> { "knownShips", "hulls" };
            List<string> TokenPath2 = new List<string> { "music", "theme" };
            List<string> TokenPath3 = new List<string> { "color" };
            List<string> TokenPath4 = new List<string> { "priorityShips", "hulls" };

            SSFactionGroup HegemonyFaction = new SSFactionGroup();
            HegemonyFaction.Add(TahlanHeg);

            foreach (JsonValue t in HegemonyFaction?.KnownHull?.ContentArray)
                Console.WriteLine(t.ToString());

            HegemonyFaction.Add(CoreHeg);
            Console.WriteLine("====Heg====");

            foreach (JsonValue t in HegemonyFaction?.KnownHull?.ContentArray ?? new ObservableCollection<JsonToken>())
                Console.WriteLine(t.ToString());

            HegemonyFaction.Remove(TahlanHeg);
            Console.WriteLine("====Heg====");

           
        }
    }
}