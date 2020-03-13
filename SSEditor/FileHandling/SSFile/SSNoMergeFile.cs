using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSNoMergeFile : ISSGenericFile, ISSFileGroup
    {
        public SSLinkRelativeUrl LinkRelativeUrl { get; private set; }
        public string FileName { get; private set; }
        public SSMod SourceMod { get; private set; }

        public SSRelativeUrl CommonRelativeUrl => LinkRelativeUrl?.GetRelative();


        public SSNoMergeFile(SSMod mod, SSFullUrl url)
        {
            SourceMod = mod;
            LinkRelativeUrl = url.LinkRelative;
        }
    }
}
