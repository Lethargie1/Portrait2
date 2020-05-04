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
        void WriteTo(SSBaseLinkUrl newPath);
        SSRelativeUrl RelativeUrl { get; }
        bool MustOverwrite { get; }
        bool ForceOverwrite { get; set; }
    }
    public interface ISSJsonGroup : ISSGroup
    {
        ReadOnlyObservableCollection<SSJson> GetJSonFiles();
    }
}
