using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class BPPackageListViewModel : Screen
    {

        public BPPackageListViewModel()
        {

        }

        private ObservableCollection<BPPackage> _Packages;
        public ObservableCollection<BPPackage> Packages
        {
            get => _Packages;
            set => SetAndNotify(ref _Packages, value);
        }

        public int SelectedIndex { get; set; }
        public BPPackage SelectedPackage { get; set; }
    }
}
