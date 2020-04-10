
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public static class MonitoredFieldFactory<T> where T:SSJson
    {
        public static MonitoredField<T> CreateFieldFromExampleToken(JsonToken token, string fieldpath)
        {
            MonitoredField<T> result = null;

            bool isValueArray = Regex.Match(fieldpath.Split('.').Last(), @"color|button|^music_").Success; ;
            switch (token)
            {
                case JsonArray jArray:
                    if (isValueArray)
                        return new MonitoredArrayValue<T>() {FieldPath = fieldpath };
                    JsonToken TestChild = jArray.Values.FirstOrDefault();
                    switch (TestChild)
                    {
                        case JsonObject jObject:
                            return new MonitoredArrayObject<T>() { FieldPath = fieldpath };
                        default:
                            return new MonitoredArray<T>() { FieldPath = fieldpath };
                    }
                case JsonObject jObject:
                    return new MonitoredObject<T>() { FieldPath = fieldpath };
                case JsonValue jValue:
                    return new MonitoredValue<T>(jValue) { FieldPath = fieldpath };
                default:
                    throw new ArgumentException();
            }
        }
    }
}
