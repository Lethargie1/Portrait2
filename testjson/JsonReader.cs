using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace testjson
{
    public class JsonReader
    {
        private StringReader Reader { get; set; }
        public JsonReader(StringReader reader)
        {
            Reader = reader;
        }

        public JsonToken UnJson()
        {
            Queue<TextToken> fileContent = new Queue<TextToken>();
            TextToken temp = TextToken.GetNextTextToken(Reader);

            while (temp != null)
            {
                fileContent.Enqueue(temp);
                temp = TextToken.GetNextTextToken(Reader);
            };
            JsonObject a = new JsonObject(fileContent);
            return a;
        }

        public static JsonToken ReadNext(Queue<TextToken> textQueue)
        {
            TextToken peek = textQueue.Peek();
            switch (peek.type)
            {
                case TextToken.TextTokenType.BeginObject:
                    return new JsonObject(textQueue);
                case TextToken.TextTokenType.BeginArray:
                    return new JsonArray(textQueue);
                case TextToken.TextTokenType.Double:
                case TextToken.TextTokenType.False:
                case TextToken.TextTokenType.Integer:
                case TextToken.TextTokenType.Reference:
                case TextToken.TextTokenType.String:
                case TextToken.TextTokenType.True:
                    return new JsonValue(textQueue);
                default:
                    throw new FormatException("Queue is not proper for token extraction");
            }
        }

    }
}
