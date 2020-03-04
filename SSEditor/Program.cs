using SSEditor.FileHandling;
using SSEditor.MonitoredTokenClass;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
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

           
            SSFile filetest = new SSFile(SSUrl + CoreUrl + HegemonyUrl);
            List<SSFile> ListTest = new List<SSFile> {
                new SSFile(SSUrl + CoreUrl + HegemonyUrl),
                new SSFile(SSUrl + TahlanUrl + HegemonyUrl),
                new SSFile(SSUrl + SWPUrl + HegemonyUrl) };

            List<string> TokenPath = new List<string> { "knownShips", "hulls" };
            List<string> TokenPath2 = new List<string> { "music" };
            List<string> TokenPath3 = new List<string> { "color" };
            string value = filetest.ReadValue(TokenPath);
            MonitoredArray<Text> Monitor = new MonitoredArray<Text>();
            Monitor.FieldPath = TokenPath;
            Monitor.Resolve(ListTest);

            List<string> values = filetest.ReadArray(new List<string> { "priorityShips","hulls" });

            Console.WriteLine(value);
            Console.WriteLine(values.FirstOrDefault());


            Console.ReadKey();
        }
    }
}