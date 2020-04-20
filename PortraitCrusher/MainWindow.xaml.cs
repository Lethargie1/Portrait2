using FVJson;
using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PortraitCrusher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            //this.VerifyPropertyName(propertyName);
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion

        EditableURLViewModel _StarsectorFolderUrl;
        public EditableURLViewModel StarsectorFolderUrl
        {
            get
            {
                if (_StarsectorFolderUrl == null)
                {
                    _StarsectorFolderUrl = new EditableURLViewModel("Starsector folder", "Select path");
                    _StarsectorFolderUrl.ValidityChecker = StarsectorValidityChecker.CheckSSFolderValidity;
                    if (Properties.Settings.Default.StarsectorUrl != "")
                        _StarsectorFolderUrl.Url = Properties.Settings.Default.StarsectorUrl;
                }
                return _StarsectorFolderUrl;
            }
        }

        EditableURLViewModel _TargetModUrl;
        public EditableURLViewModel TargetModUrl
        {
            get
            {
                if (_TargetModUrl == null)
                {
                    _TargetModUrl = new EditableURLViewModel("New Mod target folder", "Select path");
                    StarsectorFolderUrl.PropertyChanged += StarsectorFolderUrl_PropertyChanged;
                    if (StarsectorFolderUrl.UrlState == URLstate.Acceptable)
                        _TargetModUrl.Url = StarsectorFolderUrl.Url + "\\mods\\LMPC";
                }
                return _TargetModUrl;
            }
        }

        public ObservableCollection<SSMod> Mods { get; set; }
        SSDirectory directory = new SSDirectory();
        SSModWritable target;
        FactionEditor factionEditor;

        public MainWindow()
        {
            Mods = directory.Mods;
            DataContext = this;
            InitializeComponent();
        }

        RelayCommand<object> _ExploreSSCommand;
        public ICommand ExploreSSCommand
        {
            get
            {
                if (_ExploreSSCommand == null)
                {
                    _ExploreSSCommand = new RelayCommand<object>(param => this.ExploreSS_Execute());
                }
                return _ExploreSSCommand;
            }
        }
        RelayCommand<object> _ReplacePortraitsCommand;
        public ICommand ReplacePortraitsCommand
        {
            get
            {
                if (_ReplacePortraitsCommand == null)
                {
                    _ReplacePortraitsCommand = new RelayCommand<object>(param => this.ReplacePortraits_Execute());
                }
                return _ReplacePortraitsCommand;
            }
        }

        private void ReplacePortraits_Execute()
        {
            directory.PopulateMergedCollections();
            target = new SSModWritable(directory.InstallationUrl + new SSLinkUrl("mods\\lepg"));
            factionEditor = new FactionEditor(directory, target);
            List<SSFactionGroup> factions = factionEditor.GetFaction();
            foreach (SSFactionGroup f in factions)
            {
                f.MustOverwrite = true;
                f.ExtractMonitoredContent();
                f.MalePortraits?.ContentArray.Clear();
                f.MalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai1.png"));
                f.FemalePortraits?.ContentArray.Clear();
                f.FemalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai2.png"));
                
            }
            factionEditor.ReplaceFactionToWrite();
            target.WriteMod();
        }

        private void ExploreSS_Execute()
        {
            if (!(StarsectorFolderUrl.UrlState == URLstate.Acceptable))
                return;
            directory.InstallationUrl = new SSBaseUrl(StarsectorFolderUrl.Url);
            directory.ReadMods();
        }

        private void StarsectorFolderUrl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UrlState")
            {
                if (((EditableURLViewModel)sender).UrlState == (URLstate.Acceptable))
                {
                    TargetModUrl.ValidityChecker = StarsectorValidityChecker.GetCheckModFolderValidity(StarsectorFolderUrl.Url);
                    TargetModUrl.Url = StarsectorFolderUrl.Url + "\\mods\\LMPC";
                }
            }
        }


        public enum SSModFolderActions { Ignore, Use }
        SSModFolderActions _ModAction = (SSModFolderActions)Properties.Settings.Default.ModFoldAction;
        public SSModFolderActions ModAction
        {
            get => _ModAction;
            set
            {
                _ModAction = value;
                NotifyPropertyChanged("ModFolderRadioAsIgnore");
                NotifyPropertyChanged("ModFolderRadioAsUse");
            }

        }
        public bool ModFolderRadioAsIgnore
        {
            get => ModAction.Equals(SSModFolderActions.Ignore);
            set => ModAction = SSModFolderActions.Ignore;
        }
        public bool ModFolderRadioAsUse
        {
            get => ModAction.Equals(SSModFolderActions.Use);
            set => ModAction = SSModFolderActions.Use;
        }
    }
}
