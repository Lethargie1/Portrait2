using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class MainMenuViewModel: Screen
    {
        public DirectoryViewModel Directory { get; private set; }

        public MainMenuViewModel( DirectoryViewModel directoryvm)
        {
            Directory = directoryvm;
        }
        public void Activate(IScreen toActivate)
        {
            var a = this.Parent as IConductor<IScreen>;
            a.ActivateItem(toActivate);
        }
    }
}
