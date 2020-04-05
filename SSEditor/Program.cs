﻿using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor
{
    class Program
    {
        static void Main()
        {
            SSBaseUrl SSUrl = new SSBaseUrl("C:\\Program Files (x86)\\Fractal Softworks\\Starsector");
            SSLinkUrl CoreUrl = new SSLinkUrl("starsector-core");
            SSLinkUrl TahlanUrl = new SSLinkUrl("mods\\tahlan");
            SSLinkUrl SWPUrl = new SSLinkUrl("mods\\Ship and Weapon Pack");
            SSRelativeUrl HegemonyUrl = new SSRelativeUrl("data\\world\\factions\\hegemony.faction");

            SSBaseUrl ModFolderPath = SSUrl + "mods";
            DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();


            SSDirectory test = new SSDirectory(SSUrl);
            
            test.ReadMods();
            test.Mods[1].ChangeType(SSMod.ModType.skip);
            test.Mods[2].ChangeType(SSMod.ModType.skip);
            test.Mods[3].ChangeType(SSMod.ModType.skip);
            test.Mods[5].ChangeType(SSMod.ModType.skip);
            test.PopulateMergedCollections();
            test.MergeDirectory(new SSLinkUrl("mods\\hyes"));
            SSModFactory factory = new SSModFactory(SSUrl);
            factory.Type = SSMod.ModType.Mod;
            SSMod CoreMod = factory.CreateMod(CoreUrl);
            SSMod TahlanMod = factory.CreateMod(TahlanUrl);

            SSFactionFile CoreHeg = new SSFactionFile(CoreMod, SSUrl + CoreUrl + HegemonyUrl);
            SSFactionFile TahlanHeg = new SSFactionFile(TahlanMod, SSUrl + TahlanUrl + HegemonyUrl);
            SSFactionFile SWPHeg = new SSFactionFile(CoreMod, SSUrl + SWPUrl + HegemonyUrl);

            List<string> TokenPath = new List<string> { "knownShips", "hulls" };
            List<string> TokenPath2 = new List<string> { "music", "theme" };
            List<string> TokenPath3 = new List<string> { "color" };
            List<string> TokenPath4 = new List<string> { "priorityShips", "hulls" };

            SSFactionGroup HegemonyFaction = new SSFactionGroup();
            HegemonyFaction.Add(TahlanHeg);

            foreach (Text t in HegemonyFaction?.KnownHull?.ContentArray ?? new ObservableCollection<Text>())
                Console.WriteLine(t.Value + " " + t.Source);

            HegemonyFaction.Add(CoreHeg);
            Console.WriteLine("====Heg====");

            foreach (Text t in HegemonyFaction?.KnownHull?.ContentArray ?? new ObservableCollection<Text>())
                Console.WriteLine(t.Value + " " + t.Source);

            HegemonyFaction.Remove(TahlanHeg);
            Console.WriteLine("====Heg====");

            foreach (Text t in HegemonyFaction?.KnownHull?.ContentArray ?? new ObservableCollection<Text>())
                Console.WriteLine(t.Value + " " + t.Source);

            Console.ReadKey();
        }
    }
}