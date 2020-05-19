using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSCsv : SSGeneric, ISSMergable
    {
        private CSVContent _Content;
        public CSVContent Content
        {
            get
            {
                if (_Content == null)
                {
                    var url = this.SourceMod.ModUrl + this.RelativeUrl;
                    using (StreamReader sr = File.OpenText(url.ToString()))
                    {
                        _Content = CSVContent.ExtractFromText(sr);
                    }
                }
                return _Content;
            }
        }

        public SSCsv(SSMod ssmod, SSRelativeUrl url) : base(ssmod, url) { }

    }
}
