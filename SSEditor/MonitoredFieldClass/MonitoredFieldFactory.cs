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
                    JToken TestChild = token.Values().FirstOrDefault();
                    switch (TestChild?.Type ?? JTokenType.String)
                    {
                        case JTokenType.Property:
                            result = new MonitoredObjectArray<T>() { FieldPath = token.Path };
                            break;
                        case JTokenType.Integer:
                            if (token.Values().Count() == 4)
                                result = new MonitoredArrayValue<Color, T>() { FieldPath = token.Path };
                            else
                                result = new MonitoredArray<Text, T>() { FieldPath = token.Path , ValueType = JTokenType.Integer};
                            break;
                        default:
                            result = new MonitoredArray<Text, T>() { FieldPath = token.Path };
                            break;
                    }
                    break;
                case JTokenType.Object:
                    result = new MonitoredPropertyArray<T>() { FieldPath = token.Path };
                    break;
                case JTokenType.Integer:
                    result = new MonitoredValue<Text, T>() { FieldPath = token.Path, ValueType= JTokenType.Integer};
                    
                    break;
                default:
                    result = new MonitoredValue<Text, T>() { FieldPath = token.Path };
                    break;
            }
            return result;
                
            
            
        }
    }
}
