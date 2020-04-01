using Newtonsoft.Json.Linq;
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
    abstract class MonitoredField<T> where T:SSFile
    {
        public string FieldPath { get; set; }
        public string FieldName { get; set; }
        private ObservableCollection<T> _Files;
        public ObservableCollection<T> Files
        {
            get
            {
                if (_Files == null)
                {
                    _Files = new ObservableCollection<T>();
                    _Files.CollectionChanged += this.OnFilesChanged;
                }
                return _Files;
            }
        }

        
        abstract public void Resolve();
        abstract protected void ResolveAdd(T file);
        abstract protected void ResolveRemove(T file);

        public void ReplaceFiles(ObservableCollection<T> newFiles)
        {
            Files.CollectionChanged -= this.OnFilesChanged;
            _Files = newFiles;
            _Files.CollectionChanged += this.OnFilesChanged;
            this.Resolve();
        }
        private void OnFilesChanged(object sender, NotifyCollectionChangedEventArgs e )
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (T file in e.NewItems)
                    this.ResolveAdd(file);

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (T file in e.OldItems)
                    this.ResolveRemove(file);
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Resolve();
            }
        }

        public static IEnumerable<MonitoredField<T>> ExtractFields(List<T> files)
        {
            List<MonitoredField<T>> result = new List<MonitoredField<T>>();
            foreach (SSFile file in files)
            {
                List<String> possiblePath = RecursiveExtractPossibleFieldsPath(file.JsonContent);
            }
            return result;
        }

        private static List<string> RecursiveExtractPossibleFieldsPath(JObject JsonContent, string CommonBase = "")
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, JToken> x in JsonContent)
            {
                if (x.Value.HasValues && x.Value.Type != JTokenType.Array)
                    result.AddRange(RecursiveExtractPossibleFieldsPath(x.Value as JObject));
                else
                    result.Add(x.Value.Path);
            }
            return result;
        }
    }
}
