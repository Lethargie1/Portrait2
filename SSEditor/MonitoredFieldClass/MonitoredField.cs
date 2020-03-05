using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    abstract class MonitoredField
    {
        public List<string> FieldPath { get; set; }
        public string FieldName { get; set; }
        private ObservableCollection<SSFile> _Files;
        public ObservableCollection<SSFile> Files
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new ObservableCollection<SSFile>();
                    _Files.CollectionChanged += this.OnFilesChanged;
                }
                return _Files;
            }
        }

        
        abstract public void Resolve();
        abstract protected void ResolveAdd(SSFile file);
        abstract protected void ResolveRemove(SSFile file);

        public void ReplaceFiles(ObservableCollection<SSFile> newFiles)
        {
            Files.CollectionChanged -= this.OnFilesChanged;
            _Files = newFiles;
            _Files.CollectionChanged += this.OnFilesChanged;
            this.Resolve();
        }
        private void OnFilesChanged(object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (SSFile file in e.NewItems)
                    this.ResolveAdd(file);

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (SSFile file in e.OldItems)
                    this.ResolveRemove(file);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Resolve();
            }
        }
    }
}
