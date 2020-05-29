
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
        //Func<Func<FactionGroupViewModel>> FactionGroupVMFactoryFactory { get; set; }
        private Func<FactionGroupViewModel> FactionGroupVMFactory { get; set; }
        private FactionEditorFactory FactionEditorFactory { get; set; }
        public FactionEditor FactionEditor { get; private set; }
        public List<SSFactionGroup> Factions { get => FactionEditor.Factions; }
        public IWindowManager WindowManager { get; set; }

        public ShipHullRessourcesViewModel ShipHullRessourcesViewModel { get; set; }
        public Func<ShipHullRessourcesViewModel> ShipHullRessourcesViewModelFactory { get; set; }

        public PortraitsRessourcesViewModel PortraitsRessourcesViewModel { get; set; }
        public Func<PortraitsRessourcesViewModel> PortraitsRessourcesViewModelFactory { get; set; }

        public FactionEditorViewModel(
            FactionEditorFactory factionEditorFactory, 
            Func<FactionGroupViewModel> factionGroupViewModelFactory, 
            IWindowManager windowManager, 
            Func<ShipHullRessourcesViewModel> shipHullRessourcesViewModelFactory, 
            Func<PortraitsRessourcesViewModel> portraitsRessourcesViewModelFactory)
        {
            FactionEditorFactory = factionEditorFactory;
            FactionGroupVMFactory = factionGroupViewModelFactory;
            WindowManager = windowManager;
            ShipHullRessourcesViewModelFactory = shipHullRessourcesViewModelFactory;
            PortraitsRessourcesViewModelFactory = portraitsRessourcesViewModelFactory;
        }

        protected override void OnActivate() 
        {
            FactionEditor = FactionEditorFactory.CreateFactionEditor();
            //FactionGroupVMFactory = FactionGroupVMFactoryFactory();
            ShipHullRessourcesViewModel = ShipHullRessourcesViewModelFactory();
            PortraitsRessourcesViewModel = PortraitsRessourcesViewModelFactory();
        }

        private FactionGroupViewModel _SelectedFactionViewModel = null;
        public FactionGroupViewModel SelectedFactionViewModel
        {
            get
            {
                if (_SelectedFactionViewModel == null)
                {
                    _SelectedFactionViewModel = FactionGroupVMFactory();
                    _SelectedFactionViewModel.FactionGroup = SelectedFaction;
                    _SelectedFactionViewModel.ShipHullRessourcesViewModel = ShipHullRessourcesViewModel;
                    _SelectedFactionViewModel.PortraitsRessourcesViewModel = PortraitsRessourcesViewModel;
                    _SelectedFactionViewModel.BuildEditableTab();
                    _SelectedFactionViewModel.SelectedTabName = PriorFactionSelectedTabName;
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
                var screened = SelectedFactionViewModel as IScreenState;
                screened.Close();
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
            dlg.DefaultExt = ".lepgfaction"; // Default file extension
            dlg.Filter = "LEPG faction modification (.lepgfaction)|*.lepgfaction"; // Filter files by extension
            // Show save file dialog box
            try
            {
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
            catch (Exception e)
            {
                WindowManager.ShowMessageBox($"Something failed while saving. I got error: {e.Message}. Does that help you?");
            }
        }

        public void ReadGroupModification()
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".lepgfaction"; // Default file extension
            dlg.Filter = "LEPG faction modification (.lepgfaction)|*.lepgfaction"; // Filter files by extension
            // Show open file dialog box
            try
            {
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
            catch (Exception e)
            {
                WindowManager.ShowMessageBox($"Something failed while Loading. I got error: {e.Message}. Does that help you?");
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
