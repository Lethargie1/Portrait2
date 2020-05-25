using SSEditor.Ressources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class BPPackageRessourcesViewModel
    {
        public BPPackageRessources BPPackageRessources { get; private set; }

        public BPPackageRessourcesViewModel(BPPackageRessources bPPackageRessources)
        {
            BPPackageRessources = bPPackageRessources;
        }

    }
}
