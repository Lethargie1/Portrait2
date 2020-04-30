using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSCsv : SSGeneric, ISSMergable
    {
        public SSCsv(SSMod ssmod, SSRelativeUrl url) : base(ssmod, url) { }
        
    }
}
