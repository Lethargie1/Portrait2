using EditorInterface.ViewModel;
using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class Bootstrapper : Bootstrapper<ShellViewModel>
    {
        protected override void ConfigureIoC(StyletIoC.IStyletIoCBuilder builder)
        {
            base.ConfigureIoC(builder);
            builder.Bind<SSDirectory>().ToInstance(new SSDirectory());
        }
    }
}
