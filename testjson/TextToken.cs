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
        public enum TokenMarkers
        {
            ValueSeparator1 = ',',
            ValueSeparator2 = ';',
            NameSeparator = ':',
            BeginArray = '[',
            EndArray = ']',
            BeginObject = '{',
            EndObject = '}',
            StringStart = '"',
            EscapeChar = '\\'

        };
        public enum WhiteSpace
        {
            Type0 = ' ',
            Type1 = '\t',
            Type2 = '\n',
            Type3 = '\r'
        };
        public string content { get; set; }
        public TextTokenType type { get; set; }
        public override string ToString()
        {
            return content;
        }
        public static TextToken GetNextTextToken(StringReader reader)
        {
            TextToken result = new TextToken() { content = "" };
            int peekResult = reader.Peek();
            while (peekResult != -1)
            {
                if (!Enum.IsDefined(typeof(WhiteSpace),peekResult))
                {
                    int nextPeekResult;
                    result.content = ((char)reader.Read()).ToString();
                    switch (peekResult)
                    {
                        case (int)TokenMarkers.BeginArray:
                            result.type = TextTokenType.BeginArray;
                            break;
                        case (int)TokenMarkers.BeginObject:
                            result.type = TextTokenType.BeginObject;
                            break;
                        case (int)TokenMarkers.EndArray:
                            result.type = TextTokenType.EndArray;
                            break;
                        case (int)TokenMarkers.EndObject:
                            result.type = TextTokenType.EndObject;
                            break;
                        case (int)TokenMarkers.NameSeparator:
                            result.type = TextTokenType.NameSeparator;
                            break;
                        case (int)TokenMarkers.ValueSeparator2:
                        case (int)TokenMarkers.ValueSeparator1:
                            result.type = TextTokenType.ValueSeparator;
                            break;
                        case (int)TokenMarkers.StringStart:
                            result.type = TextTokenType.String;
                            nextPeekResult = reader.Peek();
                            while (true)
                            {
                                switch (nextPeekResult)
                                {
                                    case (int)TokenMarkers.EscapeChar:
                                        //next character was escaped
                                        result.content = result.content + ((char)reader.Read()).ToString();
                                        result.content = result.content + ((char)reader.Read()).ToString();
                                        break;
                                    case -1:
                                    case (int)TokenMarkers.StringStart:
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
                                if (Enum.IsDefined(typeof(WhiteSpace), nextPeekResult))
                                {
                                    reader.Read();
                                }
                                else
                                {
                                    switch (nextPeekResult)
                                    {
                                        case -1:
                                        case (int)TokenMarkers.StringStart:
                                            throw new FormatException();
                                        case (int)TokenMarkers.EndObject:
                                        case (int)TokenMarkers.ValueSeparator1:
                                        case (int)TokenMarkers.ValueSeparator2:
                                        case (int)TokenMarkers.NameSeparator:
                                        case (int)TokenMarkers.EndArray:
                                            result.type = TextTokenType.Reference;
                                            if (Regex.IsMatch(result.content, @"^\-?[0-9]+$"))
                                                //int and double are not strongly typed in Json, lets not do the same
                                                result.type = TextTokenType.Double;
                                            else
                                            if (Regex.IsMatch(result.content, @"^\-?[0-9\.]+$"))
                                                result.type = TextTokenType.Double;
                                            else
                                            if (Regex.IsMatch(result.content, @"^\-?[0-9\.]+f$"))
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
