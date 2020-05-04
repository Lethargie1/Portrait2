﻿using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace EditorInterface.ViewModel
{
    public class FactionEditorViewModel : Screen
    {
        Func<PortraitsRessourcesViewModelFactory> PortraitsRessourcesVMFactoryFactory { get; set; }
        private PortraitsRessourcesViewModelFactory PortraitsRessourcesVMFactory { get; set; }
        private FactionEditorFactory FactionEditorFactory { get; set; }
        public FactionEditor FactionEditor { get; private set; }
        public List<SSFactionGroup> Factions { get => FactionEditor.Factions; }


        public FactionEditorViewModel(FactionEditorFactory factionEditorFactory, Func<PortraitsRessourcesViewModelFactory> portraitsRessourcesViewModelFactoryFactory)
        {
            FactionEditorFactory = factionEditorFactory;
            PortraitsRessourcesVMFactoryFactory = portraitsRessourcesViewModelFactoryFactory;
        }

        protected override void OnActivate() 
        {
            FactionEditor = FactionEditorFactory.CreateFactionEditor();
            PortraitsRessourcesVMFactory = PortraitsRessourcesVMFactoryFactory();
        }

        public FactionGroupViewModel SelectedFactionViewModel { get => new FactionGroupViewModel(SelectedFaction, PortraitsRessourcesVMFactory); }
        SSFactionGroup _SelectedFaction;
        public SSFactionGroup SelectedFaction
        {
            get => this._SelectedFaction;
            set
            {
                this._SelectedFaction = value;
                NotifyOfPropertyChange(nameof(SelectedFactionViewModel));
            }
        }
        public override void RequestClose(bool? dialogResult = null)
        {
            base.RequestClose(dialogResult);
        }


    }
    [ValueConversion(typeof(SSFactionGroup), typeof(FactionGroupTokenViewModel))]
    public class FactionToFactionVMConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new FactionGroupTokenViewModel((SSFactionGroup)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
