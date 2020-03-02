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
            Console.Write(FileUrl);

            Console.ReadKey();
        }
    }
}