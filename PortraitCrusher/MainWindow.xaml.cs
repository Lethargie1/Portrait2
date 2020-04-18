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
                    _StarsectorFolderUrl.ValidityChecker = CheckSSFolderValidity;
                    if (Properties.Settings.Default.StarsectorUrl != "")
                        _StarsectorFolderUrl.CommonUrl = Properties.Settings.Default.StarsectorUrl;
                }
                return _StarsectorFolderUrl;
            }
        }


        public MainWindow()
        {
            InitializeComponent();
        }


        public bool CheckSSFolderValidity(SSFullUrl url)
        {
            if (url == null)
                return false;
                
            DirectoryInfo CoreFactionDirectory = new DirectoryInfo(url.ToString());
            if (!CoreFactionDirectory.Exists)
            {
                return false;
            }
            IEnumerable<DirectoryInfo> DirList = CoreFactionDirectory.EnumerateDirectories();
            List<DirectoryInfo> SSCoreFolder = (from dir in DirList
                                                where dir.Name == "starsector-core"
                                                select dir).ToList();
            IEnumerable<FileInfo> FileList = CoreFactionDirectory.EnumerateFiles();
            List<FileInfo> SSExecutable = (from file in FileList
                                           where file.Name == "starsector.exe"
                                           select file).ToList();
            if (SSCoreFolder.Count == 1 && SSExecutable.Count == 1)
                return true;
            else
                return false;


        }
    }
