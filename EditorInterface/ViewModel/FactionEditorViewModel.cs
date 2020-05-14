
using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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

        private FactionGroupViewModel _SelectedFactionViewModel = null;
        public FactionGroupViewModel SelectedFactionViewModel
        {
            get
            {
                if (_SelectedFactionViewModel == null)
                {
                    _SelectedFactionViewModel = new FactionGroupViewModel(SelectedFaction, PortraitsRessourcesVMFactory, PriorFactionSelectedTabName);
                    return _SelectedFactionViewModel;
                }
                else
                    return _SelectedFactionViewModel;
            }

            private set
            {
                _SelectedFactionViewModel = value;
            }
        }
        SSFactionGroup _SelectedFaction;
        public SSFactionGroup SelectedFaction
        {
            get => this._SelectedFaction;
            set
            {
                PriorFactionSelectedTabName = SelectedFactionViewModel?.ActiveItem.DisplayName;
                SelectedFactionViewModel = null;
                this._SelectedFaction = value;
                NotifyOfPropertyChange(nameof(SelectedFactionViewModel));
            }
        }
        private string PriorFactionSelectedTabName = null; 
        public override void RequestClose(bool? dialogResult = null)
        {
            base.RequestClose(dialogResult);
        }


        public void SaveGroupModification()
        {
            // Configure save file dialog box
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string filename = dlg.FileName;
                using (StreamWriter sw = File.CreateText(filename))
                {
                    string toSave = FactionEditor?.GetModificationsAsJson();
                    if (toSave != null)
                        sw.Write(toSave);
                }
            }
            
        }

        public void ReadGroupModification()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                string ReadResult = File.ReadAllText(filename);
                FactionEditor?.ApplyModificationFromJson(ReadResult);
            }
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
