using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.JsonHandling
{
    public static class JsonExtension
    {
        public static JObject FlattenToJson(this IEnumerable<IJsonConvertable> collection)
        {
            JObject Result = new JObject();
            foreach (IJsonConvertable obj in collection)
            {
                Result.ConcatRecursive(obj.JsonEquivalent());
            }
            return Result;
        }
        public static void ConcatRecursive(this JObject Modified, JObject Concatened)
        {
            foreach (KeyValuePair<string, JToken> x in Concatened)
            {
                if (Modified.ContainsKey(x.Key))
                {
                    JToken ModifiedToken = Modified.Value<JToken>(x.Key);


                    if (ModifiedToken.Type == JTokenType.Object && x.Value.Type == JTokenType.Object)
                    {
                        (ModifiedToken as JObject).ConcatRecursive(x.Value as JObject);
                    }
                    else
                    {
                        JArray United = new JArray();
                        JArray SourceArray;
                        if (ModifiedToken.Type != JTokenType.Array) { SourceArray = new JArray(ModifiedToken); }
                        else { SourceArray = ModifiedToken as JArray; }
                        JArray AddedArray;
                        if (x.Value.Type != JTokenType.Array) { AddedArray = new JArray(x.Value); }
                        else { AddedArray = x.Value as JArray; }

                        foreach (JToken SourcePart in SourceArray)
                            United.Add(SourcePart);
                        foreach (JToken AddedPart in AddedArray)
                            United.Add(AddedPart);

                        JProperty NewOuter = new JProperty(x.Key, United);
                        Modified.Property(x.Key).Replace(NewOuter);
                    }


                }
                else
                { Modified.Add(x.Key, x.Value); }
            }
        }


    }

    public interface IJsonConvertable
    {
        JObject JsonEquivalent();
    }
}
