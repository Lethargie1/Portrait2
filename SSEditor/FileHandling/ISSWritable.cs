using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public interface ISSWritable
    {
        bool MustOverwrite { get; }
        SSRelativeUrl RelativeUrl { get; }
        void WriteTo(SSBaseLinkUrl baseLinkUrl);
    }
}
