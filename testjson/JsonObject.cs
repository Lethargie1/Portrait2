using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FVJson
{
    public class JsonObject : JsonToken
    {
        public Dictionary<JsonValue, JsonToken> Values = new Dictionary<JsonValue, JsonToken>();

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
                string propertyName;
                JsonToken value;
                switch (current.type)
                {
                    case TextToken.TextTokenType.Reference:
                    case TextToken.TextTokenType.String:
                        propertyName = current.content;
                        TextToken.TextTokenType CurType = current.type;
                        current = textQueue.Dequeue();
                        if (current.type != TextToken.TextTokenType.NameSeparator)
                            throw new FormatException("No name separator");
                        value = JsonReader.ReadNext(textQueue);
                        JsonValue keyAsStr = new JsonValue(propertyName, TokenType.String);
                        JsonValue keyAsRef = new JsonValue(propertyName, TokenType.Reference);
                        if (Values.ContainsKey(keyAsRef) || Values.ContainsKey(keyAsStr))
                            throw new ArgumentException("Cant add existing key");
                        if (CurType == TextToken.TextTokenType.Reference)
                            Values.Add(keyAsRef, value);
                        else if (CurType == TextToken.TextTokenType.String)
                            Values.Add(keyAsStr, value);
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
            foreach (KeyValuePair<JsonValue, JsonToken> kp in Values)
            {
                result = result + "\t" +AddTab + kp.Key.ToJsonString() + ":" + kp.Value.ToJsonString(tab+1) + ",\r";
            }
            result = result + AddTab+ "}";
            return result;
        }
        public override JsonToken SelectFromQueuePath(Queue<string> path)
        {
            if (path.Count == 0)
                return this;
            string part = path.Dequeue();
            JsonValue asRef = new JsonValue(part, TokenType.Reference);
            JsonValue asStr = new JsonValue(part, TokenType.String);
            if (Values.ContainsKey(asStr))
                return Values[asStr].SelectFromQueuePath(path);
            else if(Values.ContainsKey(asRef))
                return Values[asRef].SelectFromQueuePath(path);
            else
                throw new ArgumentOutOfRangeException();
        }

    }
}
