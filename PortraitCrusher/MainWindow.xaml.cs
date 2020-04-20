using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public partial class MainWindow : Window
    {
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

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }



    }
}
