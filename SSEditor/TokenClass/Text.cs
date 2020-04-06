using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSEditor.FileHandling;

namespace SSEditor.TokenClass
{
    class Text : SourcedToken, ITokenValue
    {
        public override string DefaultValue
        {
            get
            {
                return "no value";
            }
        }
        public override bool IsValid
        {
            get
            {
                return true;
            }
        }

        public void SetContent(string value, ISSJson source)
        {
            base.Value = value;
            base.Source = source;

        }
    }
}
