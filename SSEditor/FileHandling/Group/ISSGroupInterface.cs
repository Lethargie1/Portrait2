using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public interface ISSGroup
    {
        void WriteMergeTo(SSBaseLinkUrl newPath);
        SSRelativeUrl CommonRelativeUrl { get; }
        bool MustOverwrite { get; set; }
    }
    public interface ISSJsonGroup : ISSGroup
    {

    }
}
