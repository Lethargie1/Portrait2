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
            MonitoredField<T> result = null;
            switch(token.Type)
            {
                case JTokenType.Array:
                    //either a value as array, or an array
                    if (token.Values().Count() == 4 && token.Values().FirstOrDefault().Type == JTokenType.Integer)
                        result = new MonitoredArrayValue<Color, T>() { FieldPath = token.Path };
                    else
                        result = new MonitoredArray<Text, T>() { FieldPath = token.Path };
                    break;
                case JTokenType.Object:
                    result = new MonitoredPropertyArray<T>() { FieldPath = token.Path };
                    break;
                default:
                    result = new MonitoredValue<Text, T>() { FieldPath = token.Path };
                    break;
            }
            return result;
                
            
            
        }
    }
}
