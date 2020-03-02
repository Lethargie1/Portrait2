using SSEditor.FileHandling;
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
            URLRelative FileUrl = ModUrl.CreateFromCommon("data\\world\\factions\\hegemony.faction");
            SSFile filetest = new SSFile(FileUrl);
            string value = filetest.ReadValue(new List<string> { "music","theme" });
            List<string> values = filetest.ReadArray(new List<string> { "priorityShips","hulls" });
            List<string> values2 = filetest.ReadArray(new List<string> { "music", "theme" });
            Console.WriteLine(value);
            Console.WriteLine(values.First());
            Console.WriteLine(values2.FirstOrDefault());
            Console.ReadKey();
        }
    }
}