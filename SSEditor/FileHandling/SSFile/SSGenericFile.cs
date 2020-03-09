using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSGenericFile : ISSGenericFile
    {
        public SSLinkRelativeUrl LinkRelativeUrl { get; private set; }
        public string FileName { get; private set; }
        public SSMod SourceMod { get; private set; }

        public SSGenericFile(SSMod mod, SSFullUrl url)
        {
            SourceMod = mod;
            LinkRelativeUrl = url.LinkRelative;
        }
    }
    interface ISSGenericFile
    {
        SSLinkRelativeUrl LinkRelativeUrl { get; }
        string FileName { get; }
        SSMod SourceMod { get; }
    }
}
