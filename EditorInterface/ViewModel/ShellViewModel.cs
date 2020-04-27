using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class ShellViewModel : Conductor<IScreen>.StackNavigation
    {
        public MainMenuViewModel Menu {get; private set;}

        public ShellViewModel(MainMenuViewModel menu)
        {
            Menu = menu;
            this.ActivateItem(Menu);
        }

        public void SwitchToScreen(IScreen screen)
        {
            this.ActivateItem(screen);
        }

    }
}
