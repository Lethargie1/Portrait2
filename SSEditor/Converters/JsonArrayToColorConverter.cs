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

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
