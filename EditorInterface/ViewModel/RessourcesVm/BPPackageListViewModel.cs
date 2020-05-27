using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EditorInterface.ViewModel
{
    public class BPPackageListViewModel : Screen
    {

        public BPPackageListViewModel()
        {

        }

        public event EventHandler ItemShiftClicked;

        protected virtual void OnItemShiftClicked()
        {
            EventHandler handler = ItemShiftClicked;
            handler?.Invoke(this, null);
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

        public void HandleListViewClick(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
                OnItemShiftClicked();
        }
    }
}
