using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FVJson
{
    public class TextToken
    {
        public enum TextTokenType { BeginArray, BeginObject, EndArray, EndObject, NameSeparator, ValueSeparator, String, False, Null, True, Integer, Double, Reference };
        public string content { get; set; }
        public TextTokenType type { get; set; }
        public override string ToString()
        {
            return content;
        }
        public static TextToken GetNextTextToken(StringReader reader)
        {
            TextToken result = new TextToken() { content = "" };
            int[] allowedSpace = new int[] { ' ', '\t', '\n', '\r' };
            int peekResult = reader.Peek();
            while (peekResult != -1)
            {
                if (!allowedSpace.Contains(peekResult))
                {
                    int nextPeekResult;
                    result.content = ((char)reader.Read()).ToString();
                    switch (peekResult)
                    {
                        case '[':
                            result.type = TextTokenType.BeginArray;
                            break;
                        case '{':
                            result.type = TextTokenType.BeginObject;
                            break;
                        case ']':
                            result.type = TextTokenType.EndArray;
                            break;
                        case '}':
                            result.type = TextTokenType.EndObject;
                            break;
                        case ':':
                            result.type = TextTokenType.NameSeparator;
                            break;
                        case ',':
                            result.type = TextTokenType.ValueSeparator;
                            break;
                        case '"':
                            result.type = TextTokenType.String;
                            nextPeekResult = reader.Peek();
                            while (true)
                            {
                                switch (nextPeekResult)
                                {
                                    case -1:
                                    case ',':
                                        throw new FormatException();
                                    case '\"':
                                        result.content = result.content + ((char)reader.Read()).ToString();
                                        if (result.content.Length == 2)
                                            result.content = "";
                                        else
                                            result.content = result.content.Substring(1,result.content.Length-2);
                                        return result;
                                    default:
                                        result.content = result.content + ((char)reader.Read()).ToString(); 
                                        break;
                                }
                                nextPeekResult = reader.Peek();
                            }
                        default:
                            nextPeekResult = reader.Peek();
                            while (true)
                            {
                                switch (nextPeekResult)
                                {
                                    case -1:
                                    case '"':
                                        throw new FormatException();
                                    case ']':
                                    case ',':
                                    case ':':
                                    case '}':
                                        result.type = TextTokenType.Reference;
                                        if (Regex.IsMatch(result.content, @"^[0-9]+$"))
                                            result.type = TextTokenType.Integer;
                                        else
                                        if (Regex.IsMatch(result.content, @"^[0-9\.]+$"))
                                            result.type = TextTokenType.Double;
                                        else
                                        if (Regex.IsMatch(result.content, @"^[0-9\.]+f$"))
                                        {
                                            result.type = TextTokenType.Double;
                                            result.content = result.content.Substring(0, result.content.Length - 1);
                                        }
                                        else                                        
                                        if (Regex.IsMatch(result.content, @"false"))
                                            result.type = TextTokenType.False;
                                        else
                                        if (Regex.IsMatch(result.content, @"true"))
                                            result.type = TextTokenType.True;
                                        return result;
                                    default:
                                        result.content = result.content + ((char)reader.Read()).ToString(); ;
                                        break;
                                }
                                nextPeekResult = reader.Peek();
                            }
                    }
                    return result;
                }
                reader.Read();
                peekResult = reader.Peek();
            }
            return null;
        }


    }
}
