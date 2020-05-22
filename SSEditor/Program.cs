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
using SSEditor.Ressources;

namespace SSEditor
{
    class Program
    {
        static void Main()
        {
            
            SSBaseUrl SSUrl = new SSBaseUrl("E:\\SS\\Starsector");
            SSLinkUrl CoreUrl = new SSLinkUrl("starsector-core");
            //SSLinkUrl TahlanUrl = new SSLinkUrl("mods\\tahlan");
            SSLinkUrl SWPUrl = new SSLinkUrl("mods\\Ship and Weapon Pack");
            SSRelativeUrl shipdataUrl = new SSRelativeUrl("data\\hulls\\ship_data.csv");

            SSFullUrl dataurl = SSUrl + SWPUrl + shipdataUrl;
            CSVContent content;
            using (StreamReader sr = File.OpenText(dataurl.ToString()))
            {
                content = CSVContent.ExtractFromText(sr);
            }

            dataurl = SSUrl + CoreUrl + shipdataUrl;
            CSVContent content2;
            using (StreamReader sr = File.OpenText(dataurl.ToString()))
            {
                content2 = CSVContent.ExtractFromText(sr);
            }

            CSVContent content3 = CSVContent.Merge(new[]{content,content2});
            var b = content3.GetLineByColumnValue("id", "swp_archon")["tags"];
            //SSBaseUrl ModFolderPath = SSUrl + "mods";
            //DirectoryInfo ModsDirectory = new DirectoryInfo(ModFolderPath.ToString());
            //IEnumerable<DirectoryInfo> ModsEnumerable = ModsDirectory.EnumerateDirectories();

            SSDirectory directory = new SSDirectory();
            directory.InstallationUrl = SSUrl;
            directory.ReadMods();
            directory.PopulateMergedCollections();
            VariantsRessources variant = new VariantsRessources(directory);
            ShipHullRessources ship = new ShipHullRessources(directory,variant);
            BPPackageRessources BPRessource = new BPPackageRessources(directory, ship);
            //SSDirectory test = new SSDirectory(SSUrl);
            //SSModWritable target = new SSModWritable();
            //target.ModUrl = SSUrl + new SSLinkUrl("mods\\lepg");
            //test.ReadMods("lepg");
            //test.PopulateMergedCollections();

            //FactionEditor factionEditor = new FactionEditor(test);
            //List<SSFactionGroup> factions = factionEditor.GetFaction();
            //
        }
    }
}