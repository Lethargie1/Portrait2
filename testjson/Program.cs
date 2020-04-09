using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace testjson
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "E:\\SS\\Starsector\\starsector-core\\data\\campaign\\pings.json";
            string ReadResult = File.ReadAllText(url);
            var result = Regex.Replace(ReadResult, "#.*", "");
            using (var reader = new StringReader(result))
            {
                JsonReader jreader = new JsonReader(reader);
                JsonToken a = jreader.UnJson();
                string path = "distress_call.sounds[2";
                bool c = a.ExistPath(path);
                string b = a.ToJsonString();
            }
            
        }

    }
}
