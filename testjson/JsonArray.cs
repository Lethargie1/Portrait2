using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FVJson
{
    public class JsonArray : JsonToken
    {
        public List<JsonToken> Values { get; } = new List<JsonToken>();

        public JsonArray() { }
        public JsonArray(Queue<TextToken> textQueue)
        {
            Type = TokenType.Array;
            TextToken current = textQueue.Dequeue();

            if (current.type != TextToken.TextTokenType.BeginArray)
                throw new FormatException("Texttoken is not an Array beginning");

            TextToken peek = textQueue.Peek();
            while (textQueue.Count != 0)
            {
                peek = textQueue.Peek();
                switch (peek.type)
                {
                    case TextToken.TextTokenType.Double:
                    case TextToken.TextTokenType.False:
                    case TextToken.TextTokenType.Integer:
                    case TextToken.TextTokenType.Reference:
                    case TextToken.TextTokenType.True:
                    case TextToken.TextTokenType.String:
                    case TextToken.TextTokenType.BeginArray:
                    case TextToken.TextTokenType.BeginObject:
                        JsonToken value = JsonReader.ReadNext(textQueue);
                        //if (value.Type == JsonToken.TokenType.Array)
                        //    throw new FormatException("Cant have array in array)");
                        Values.Add(value);
                        break;
                    case TextToken.TextTokenType.ValueSeparator:
                        textQueue.Dequeue();
                        break;                   
                    case TextToken.TextTokenType.EndArray:
                        textQueue.Dequeue();
                        return;
                    default:
                        throw new FormatException("Found something improper in an array");
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
            if (Values.Count == 0)
                return "[]";
            if (Values[0].Type == TokenType.Integer)
                result = "[";
            else
                result = "[\n\r\t" + AddTab;
            for (int i = 0; i < Values.Count - 1; i++)
            {
                if (Values[i].Type == TokenType.Integer)
                    result = result + Values[i].ToJsonString(tab+1) + ",";
                else
                    result = result + Values[i].ToJsonString(tab+1) + ",\n\r\t" +AddTab;
            }
            if (Values.Count > 0)
            {
                result = result + Values.Last().ToJsonString(tab+1);
                if (Values.Last().Type == TokenType.Integer)
                    ;
                else
                    result = result + ",\n\r" +AddTab;
            }
            result = result + "]";
            return result;
        }

        public override JsonToken SelectFromQueuePath(Queue<string> path)
        {
            if (path.Count == 0)
                return this;
            string part = path.Dequeue();
            int index = Convert.ToInt32(part);
            if (index>=0 && index<Values.Count)
                return Values[index].SelectFromQueuePath(path);
            else
                throw new ArgumentOutOfRangeException();
        }

        public override Dictionary<string, JsonToken> GetPathedChildrens()
        {
            Dictionary<string, JsonToken> result = new Dictionary<String, JsonToken>() { { "", this } };
            for (int valueCounter = 0; valueCounter < Values.Count(); valueCounter++)
            {
                Dictionary<string, JsonToken> subResult = Values[valueCounter].GetPathedChildrens();
                foreach (KeyValuePair<string, JsonToken> subkv in subResult)
                {
                    result.Add("[" + valueCounter + "]" +subkv.Key, subkv.Value);
                }
            }
            return result;
        }
    }
}
