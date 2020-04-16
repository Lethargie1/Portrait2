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
        public JsonObject(IEnumerable<JsonToken> enumerable, string fieldRootName)
        {
            int counter = 0;
            foreach (JsonToken token in enumerable)
            {
                Values.Add(new JsonValue(fieldRootName + counter), token);
                counter++;
            }
        }

        public void AddSubField(string path, JsonToken content)
        {
            if (path == "")
                throw new FormatException("path cannot be empty");
            string[] parts = path.Split('.');
            if (parts[0] != "")
                throw new FormatException("path must start with .");
            int count = parts.Count();
            if (count < 2)
                throw new FormatException("path muts contains .");
            JsonValue partKey = new JsonValue(parts[1]);
            if (count == 2)
            {
                if (Values.ContainsKey(partKey))
                    throw new FormatException("Cant add existing field");
                else
                {
                    Values.Add(partKey, content);
                    return;
                }
            }
            else
            {
                string[] remain = new string[count-2];
                Array.Copy(parts, 2, remain,0,count-2);
                string newPath = "." + string.Join(".", remain);
                if (Values.ContainsKey(partKey))
                {
                    if (Values[partKey] is JsonObject jObject)
                    {
                        jObject.AddSubField(newPath, content);
                    }
                    else
                    {
                        throw new FormatException("Cannot add subfield to non object field");
                    }
                }
                else
                {
                    JsonObject newJobject = new JsonObject();
                    newJobject.AddSubField(newPath, content);
                    Values.Add(partKey, newJobject);
                    return;
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

        public override Dictionary<string, JsonToken> GetPathedChildrens()
        {
            Dictionary<string,JsonToken> result = new Dictionary<String, JsonToken>() {{"",this}};
            foreach (KeyValuePair<JsonValue,JsonToken > kv in Values)
            {
                Dictionary<string, JsonToken> subResult = kv.Value.GetPathedChildrens();
                foreach(KeyValuePair<string, JsonToken> subkv in subResult)
                {
                    result.Add("." + kv.Key.ToString() + subkv.Key, subkv.Value);
                }
            }
            return result;
        }
    }
}
