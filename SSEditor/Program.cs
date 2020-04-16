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
            test.CopyMergable(new SSLinkUrl("mods\\hyes"));
            //test.CopyUnmergable(new SSLinkUrl("mods\\hyes"));
            //test.MergeDirectory(new SSLinkUrl("mods\\hyes"));
            SSModFactory factory = new SSModFactory(SSUrl);


           
        }
    }
}