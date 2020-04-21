using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public enum ModType { Core, Mod, Ressource, Skip, Patch };

    public interface ISSMod
    {

        SSBaseLinkUrl ModUrl { get; }
        ModType CurrentType { get;}
        string ModName { get;  }
    }
}
