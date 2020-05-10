using FVJson;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SSEditor.Converters
{
    public class JsonArrayToColorConverter
    {
        public object Convert(JsonArray array)
        {
            if (array == null)
                return null;
            List<int> ColorCode = array.Values.Select(j =>
            {
                JsonValue jval = (JsonValue)j;
                int number = System.Convert.ToInt32(jval.Content);
                //double number = (double)(jval.Content);
                return number;
            }).ToList();
            if (ColorCode.Count == 4)
            {
                List<string> ColorArray = (from color in ColorCode
                                           select color.ToString("X2")).ToList<string>();
                return "#" + ColorArray[3] + ColorArray[0] + ColorArray[1] + ColorArray[2];
            }
            else
                throw new ArgumentException("Not a 4 value array");
        }

        public object ConvertBack(string value)
        {
            string first = value.Substring(1, 2);
            string second = value.Substring(3, 2);
            string third = value.Substring(5, 2);
            string fourth = value.Substring(7, 2);

            JsonArray result = new JsonArray();
            result.Values.Add(new JsonValue(System.Convert.ToInt32(second,16)));
            result.Values.Add(new JsonValue(System.Convert.ToInt32(third,16)));
            result.Values.Add(new JsonValue(System.Convert.ToInt32(fourth,16)));
            result.Values.Add(new JsonValue(System.Convert.ToInt32(first,16)));
            return result;
        }
    }
}
