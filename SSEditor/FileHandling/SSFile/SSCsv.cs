using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSCsv : SSGeneric, ISSMergable
    {
        public SSCsv(SSMod ssmod, SSFullUrl url) : base(ssmod, url) { }
        
    }
}
