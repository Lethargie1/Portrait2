using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace testjson
{
    public class JsonObject : JsonToken
    {
        public Dictionary<string, JsonToken> Values = new Dictionary<string, JsonToken>();

        public JsonObject() { }
        public JsonObject(Queue<TextToken> textQueue)
        {
            Type = TokenType.Object;
            TextToken current = textQueue.Dequeue();
            if (current.type != TextToken.TextTokenType.BeginObject)
                throw new FormatException("Texttoken is not an object beginning");

            while (textQueue.Count != 0)
            {
                current = textQueue.Dequeue();
                switch (current.type)
                {
                    case TextToken.TextTokenType.String:
                        string propertyName = current.content;
                        current = textQueue.Dequeue();
                        if (current.type != TextToken.TextTokenType.NameSeparator)
                            throw new FormatException("No name separator");
                        JsonToken value = JsonReader.ReadNext(textQueue);
                        Values.Add(propertyName, value);
                        break;
                    case TextToken.TextTokenType.ValueSeparator:
                        break;
                    case TextToken.TextTokenType.EndObject:
                        return;
                    default:
                        throw new FormatException("Found something improper in an object");
                }
            }
        }

        public override string ToJsonString()
        {
            return this.ToJsonString(0);
        }

        public override string ToJsonString(int tab)
        {
            string AddTab = "";
            for (int i = 0; i < tab; i++)
                AddTab = AddTab + "\t";
            string result;
            result = "{\r";
            foreach (KeyValuePair<string, JsonToken> kp in Values)
            {
                result = result + "\t" +AddTab + "\""+ kp.Key+ "\"" + ":" + kp.Value.ToJsonString(tab+1) + ",\r";
            }
            result = result + AddTab+ "}";
            return result;
        }
        public override JsonToken SelectFromQueuePath(Queue<string> path)
        {
            if (path.Count == 0)
                return this;
            string part = path.Dequeue();
            if (Values.ContainsKey(part))
                return Values[part].SelectFromQueuePath(path);
            else
                throw new ArgumentOutOfRangeException();
        }

    }
}
