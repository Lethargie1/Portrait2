using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FVJson
{
    public abstract class JsonToken
    {
        public enum TokenType { String, Integer, Double, Reference, Boolean, Array, Object };

        public TokenType Type { get; set; }


        public JsonToken SelectToken(string path)
        {
            if (path == "")
                return this;
            string[] parts = path.Split('.');
            Queue<string> pathQueue = new Queue<string>();
            foreach (string part in parts)
            {
                Match Number = Regex.Match(part, @"\[([0-9]*)\]$");
                if (Number.Success)
                {
                    string newpart = Regex.Replace(part, @"\[[0-9]*\]$", "");
                    string numb = Number.Groups[1].ToString();
                    pathQueue.Enqueue(newpart);
                    pathQueue.Enqueue(numb);
                }
                else
                    pathQueue.Enqueue(part);
            }
            return SelectFromQueuePath(pathQueue);

        }
        public bool ExistPath(string path)
        {
            try
            {this.SelectToken(path);}
            catch (ArgumentOutOfRangeException e)
            { return false; }
            return true;
        }

        public abstract string ToJsonString();
        public abstract string ToJsonString(int tab);
        public abstract JsonToken SelectFromQueuePath(Queue<string> path);


        


    }
}
