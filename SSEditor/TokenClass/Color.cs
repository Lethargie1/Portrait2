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

        public List<string> ValueArray => throw new NotImplementedException();

        public void SetContent(List<string> valueList, SSFile source)
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
