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
            URLRelative ModUrl = new URLRelative("E:\\SS\\Starsector", "starsector-core", "");
            URLRelative ModUrl1 = new URLRelative("E:\\SS\\Starsector", "mods\\tahlan", "");
            URLRelative ModUrl2 = new URLRelative("E:\\SS\\Starsector", "mods\\Ship and Weapon Pack", "");
            URLRelative FileUrl = ModUrl.CreateFromCommon("data\\world\\factions\\hegemony.faction");
            URLRelative FileUrl1 = ModUrl1.CreateFromCommon("data\\world\\factions\\hegemony.faction");
            URLRelative FileUrl2 = ModUrl2.CreateFromCommon("data\\world\\factions\\hegemony.faction");
            SSFile filetest = new SSFile(FileUrl);
            List<SSFile> ListTest = new List<SSFile> { new SSFile(FileUrl), new SSFile(FileUrl1), new SSFile(FileUrl2) };

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