using EditorInterface.ViewModel;
using FluentValidation;
using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using SSEditor.Ressources;
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
            builder.Bind<SSModWritable>().ToInstance(new SSModWritable());
            builder.Bind<ShipHullRessources>().ToSelf();
            builder.Bind<VariantsRessources>().ToSelf();
            builder.Bind<FactionEditorFactory>().ToSelf();
            builder.Bind<PortraitsRessources>().ToSelf();
            builder.Bind<PortraitsRessourcesViewModel>().ToSelf();
            builder.Bind<ShipHullRessourcesViewModel>().ToSelf();
            builder.Bind<PortraitsRessourcesViewModelFactory>().ToSelf();
            builder.Bind(typeof(IModelValidator<>)).To(typeof(FluentModelValidator<>));
            builder.Bind(typeof(IValidator<>)).ToAllImplementations();
            builder.Bind(typeof(IMessageBoxViewModel)).To(typeof(SSMessageBoxViewModel));
        }
    }
}
