using FVJson;
using SSEditor.SSRessources;
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
using System.Text.RegularExpressions;
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
                    {
                        _TargetModUrl.ValidityChecker = StarsectorValidityChecker.GetCheckModFolderValidity(StarsectorFolderUrl.Url);
                        _TargetModUrl.Url = StarsectorFolderUrl.Url + "\\mods\\LMPC";
                    }
                }
                return _TargetModUrl;
            }
        }

        public ObservableCollection<SSMod> Mods { get; set; }
        public ObservableCollection<JsonValue> Portraits { get; } = new ObservableCollection<JsonValue>();
        SSDirectory directory = new SSDirectory();
        SSModWritable target;
        FactionEditor factionEditor = new FactionEditor();

        public MainWindow()
        {
            Mods = directory.Mods;
            factionEditor.Directory = directory;
            DataContext = this;
            InitializeComponent();
        }

        #region command
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
            var match = Regex.Match(TargetModUrl.Url, @"(?:" + StarsectorFolderUrl.Url.Replace(@"\", @"\\") + @"\\)(.*)");
            string linkPart = match.Groups[1].ToString();
            target = new SSModWritable(directory.InstallationUrl + new SSLinkUrl(linkPart));
            factionEditor.Receiver = target;
            Properties.Settings.Default.ReceiverUrl = linkPart;
            Properties.Settings.Default.Save();





            List<SSFactionGroup> factions = factionEditor.GetFaction();
            foreach (SSFactionGroup f in factions)
            {
                f.MustOverwrite = true;
                f.MalePortraits?.ContentArray.Clear();
                f.MalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai1.png"));
                f.FemalePortraits?.ContentArray.Clear();
                f.FemalePortraits?.ContentArray.Add(new JsonValue("graphics/portraits/portrait_ai2.png"));
                
            }
            factionEditor.ReplaceFactionToWrite();
            try
            {
                target.WriteMod();
            }
            catch(IOException)
            {
                MessageBoxButton Button = MessageBoxButton.OK;
                MessageBox.Show(this, "Target directory is not accesibles (Is it open in the Windows explorer?)", "Access error", Button);
            }
            catch (System.UnauthorizedAccessException)
            {
                MessageBoxButton Button = MessageBoxButton.OK;
                MessageBox.Show(this, "Target directory are not accesibles (Is it open in the Windows explorer?)","Access error", Button);
            }
        }

        private void ExploreSS_Execute()
        {
            if (!(StarsectorFolderUrl.UrlState == URLstate.Acceptable))
                return;
            Properties.Settings.Default.StarsectorUrl = StarsectorFolderUrl.Url;
            Properties.Settings.Default.Save();
            directory.InstallationUrl = new SSBaseUrl(StarsectorFolderUrl.Url);
            if (TargetModUrl.UrlState == URLstate.Acceptable)
            {
                var targetFolder = new DirectoryInfo(TargetModUrl.Url);
                directory.ReadMods(targetFolder.Name);
            }
            else
                directory.ReadMods();
            directory.PopulateMergedCollections();
            
            ModAction = (SSModFolderActions)Properties.Settings.Default.ModAction;
        }
        #endregion

        private void StarsectorFolderUrl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "UrlState")
            {
                if (((EditableURLViewModel)sender).UrlState == (URLstate.Acceptable))
                {
                    TargetModUrl.ValidityChecker = StarsectorValidityChecker.GetCheckModFolderValidity(StarsectorFolderUrl.Url);
                    TargetModUrl.Url = StarsectorFolderUrl.Url + Properties.Settings.Default.ReceiverUrl;
                    NotifyPropertyChanged("StarsectorFolderUrl");
                }
            }
        }

        #region radio button
        public enum SSModFolderActions { Ignore, Use }
        SSModFolderActions _ModAction = (SSModFolderActions)Properties.Settings.Default.ModAction;
        public SSModFolderActions ModAction
        {
            get => _ModAction;
            set
            {
                _ModAction = value;
                Properties.Settings.Default.ModAction = (int)value;
                Properties.Settings.Default.Save();
                NotifyPropertyChanged("ModFolderRadioAsIgnore");
                NotifyPropertyChanged("ModFolderRadioAsUse");
                if (directory.Mods.Count>0)
                {
                    IEnumerable<SSMod> modused = from SSMod m in directory.Mods
                                                 where m.CurrentType == ModType.Mod
                                                 select m;
                    IEnumerable<SSMod> modskip = from SSMod m in directory.Mods
                                                 where m.CurrentType == ModType.Skip
                                                 select m;
                    if (ModAction == SSModFolderActions.Ignore)
                    {
                        foreach (SSMod m in modused)
                        {
                            m.ChangeType(ModType.Skip);
                        }
                    }
                    else
                    {
                        foreach (SSMod m in modskip)
                        {
                            m.ChangeType(ModType.Mod);
                        }
                    }
                    factionEditor.GetFaction();
                    if ((factionEditor?.Factions?.Count ?? 0) > 0)
                    {
                        IEnumerable<JsonValue> portraits = SSEditor.SSRessources.Portraits.GetOriginalPortraits(factionEditor.Factions);
                        Portraits.Clear();
                        foreach (JsonValue j in portraits)
                            Portraits.Add(j);
                    }
                }
                
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
        #endregion
    }
}
