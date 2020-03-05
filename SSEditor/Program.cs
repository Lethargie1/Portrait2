using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

           
            SSFile CoreHeg = new SSFile(SSUrl + CoreUrl + HegemonyUrl);
            SSFile TahlanHeg = new SSFile(SSUrl + TahlanUrl + HegemonyUrl);
            SSFile SWPHeg = new SSFile(SSUrl + SWPUrl + HegemonyUrl);

            List<string> TokenPath = new List<string> { "knownShips", "hulls" };
            List<string> TokenPath2 = new List<string> { "music", "theme" };
            List<string> TokenPath3 = new List<string> { "color" };
            List<string> TokenPath4 = new List<string> { "priorityShips", "hulls" };

            SSFactionGroup HegemonyFaction = new SSFactionGroup();
            HegemonyFaction.CommonFiles.Add(TahlanHeg);

            foreach (Text t in HegemonyFaction.KnownHull.ContentArray)
                Console.WriteLine(t.Value + " " + t.Source);

            HegemonyFaction.CommonFiles.Add(CoreHeg);
            Console.WriteLine("====Heg====");

            foreach (Text t in HegemonyFaction.KnownHull.ContentArray)
                Console.WriteLine(t.Value + " " + t.Source);

            HegemonyFaction.CommonFiles.Remove(TahlanHeg);
            Console.WriteLine("====Heg====");

            foreach (Text t in HegemonyFaction.KnownHull.ContentArray)
                Console.WriteLine(t.Value + " " + t.Source);

            Console.ReadKey();
        }
    }
}