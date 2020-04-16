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
            //this is where we can modify the mods we wanna use
            test.PopulateMergedCollections();
            List<ISSGroup> factions = test.GetMergedFaction();
            //this is where we can do some stuff in the faction themselves
            foreach (ISSGroup f in factions)
            {
                if (f is SSFactionGroup g)
                {
                    g.MalePortraits?.ContentArray.Clear();
                    g.MalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai1.png"));
                    g.FemalePortraits?.ContentArray.Clear();
                    g.FemalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai2.png"));
                }
            }
            test.CopyFactions(new SSLinkUrl("mods\\hyes"));
  
        }
    }
}