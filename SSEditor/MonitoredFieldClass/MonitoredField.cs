
using FVJson;
using SSEditor.FileHandling;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public abstract class MonitoredField : PropertyChangedBase 
    {
        public string FieldPath { get; set; }
        private ObservableCollection<ISSJson> _Files;
        public ObservableCollection<ISSJson> Files
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new ObservableCollection<ISSJson>();
                    _Files.CollectionChanged += this.OnFilesChanged;
                }
                return _Files;
            }
        }

        abstract public bool Modified { get; }
        abstract public JsonToken GetJsonEquivalent();
        abstract public JsonToken GetJsonEquivalentNoOverwrite();
        abstract public bool RequiresOverwrite();
        abstract public bool IsModified();
        abstract public void Resolve();
        abstract protected void ResolveAdd(ISSJson file);
        abstract protected void ResolveRemove(ISSJson file);
        public abstract Dictionary<string, MonitoredField> GetPathedChildrens();

        public void ReplaceFiles(ObservableCollection<ISSJson> newFiles)
        {
            Files.CollectionChanged -= this.OnFilesChanged;
            _Files = newFiles;
            _Files.CollectionChanged += this.OnFilesChanged;
            this.Resolve();
        }
        private void OnFilesChanged(object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (ISSJson file in e.NewItems)
                    this.ResolveAdd(file);

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (ISSJson file in e.OldItems)
                    this.ResolveRemove(file);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Resolve();
            }
        }

    }
}
