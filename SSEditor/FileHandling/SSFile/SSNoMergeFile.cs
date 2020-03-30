using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSNoMergeFile : SSGenericFile, ISSFileGroup
    {

        public SSRelativeUrl CommonRelativeUrl => LinkRelativeUrl?.GetRelative();


        public SSNoMergeFile(SSMod mod, SSFullUrl url) : base (mod,url)
        {}
    }
}
