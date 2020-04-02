using Newtonsoft.Json.Linq;
using SSEditor.FileHandling;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public static class MonitoredFieldFactory<T> where T:SSFile
    {
        public static MonitoredField<T> CreateFieldFromExampleToken(JToken token)
        {
            if (token.Type == JTokenType.Array)
            {
                //either a value as array, or an array
                if (token.Values().Count() == 4 && token.Values().FirstOrDefault().Type == JTokenType.Integer)
                {
                    MonitoredArrayValue<Color, T> TempArrayValue = new MonitoredArrayValue<Color, T>() { FieldPath = token.Path };
                    return TempArrayValue;
                }
                MonitoredArray<Text,T> TempArray = new MonitoredArray<Text, T>() { FieldPath = token.Path };
                return TempArray;
            }
            else
            {
                MonitoredValue<Text, T> TempValue = new MonitoredValue<Text, T>() { FieldPath = token.Path };
                return TempValue;
            }
            
            
        }
    }
}
