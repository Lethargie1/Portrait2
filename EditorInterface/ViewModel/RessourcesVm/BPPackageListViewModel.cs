using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

        private void Packages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NotifyOfPropertyChange(nameof(Packages));
        }

        private ObservableCollection<BPPackage> _Packages;
        public ObservableCollection<BPPackage> Packages
        {
            get => _Packages;
            set
            {
                SetAndNotify(ref _Packages, value);
                CollectionChangedEventManager.AddHandler(Packages, Packages_CollectionChanged);
            }
        }

        public int SelectedIndex { get; set; }
        public BPPackage SelectedPackage { get; set; }
    }
}
