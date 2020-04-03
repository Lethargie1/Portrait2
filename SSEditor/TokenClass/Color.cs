using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSEditor.FileHandling;

namespace SSEditor.TokenClass
{
    class Color : SourcedToken, ITokenAsArray
    {
        public override string DefaultValue
        {
            get
            {
                return "#FFFFFFFF";
            }
        }

        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public List<int> ValueArray
        {
            get
            {
                string a = base.Value.Substring(1, 2);
                string b = base.Value.Substring(3, 2);
                string c = base.Value.Substring(5, 2);
                string d = base.Value.Substring(7, 2);
                List<int> result = new List<int>();
                result.Add(Convert.ToInt32(a,16));
                result.Add(Convert.ToInt32(b, 16));
                result.Add(Convert.ToInt32(c, 16));
                result.Add(Convert.ToInt32(d, 16));
                return result;
            }
        }

        public void SetContent(List<string> valueList, ISSFile source)
        {
            if (valueList==null)
            {
                base.Value = null;
                base.Source = null;
                return;
            }
            List<int> ColorCode = valueList
                             .Select(s => Int32.TryParse(s, out int n) ? n : (int?)null)
                                .Where(n => n.HasValue)
                                .Select(n => n.Value)
                                .ToList();
            if (ColorCode.Count == 4)
            {
                List<string> ColorArray = (from color in ColorCode
                                           select color.ToString("X2")).ToList<string>();
                base.Value = "#" + ColorArray[3] + ColorArray[0] + ColorArray[1] + ColorArray[2];
                base.Source = source;
            }
        }
    }
}
