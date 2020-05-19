using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public abstract class SSGroup<T> : PropertyChangedBase, ISSGroup, ISSWritable where T: ISSGenericFile
    {
        public ReadOnlyObservableCollection<T> CommonFilesReadOnly { get; private set; }
        protected ObservableCollection<T> CommonFiles { get;} = new ObservableCollection<T>();

        public SSRelativeUrl RelativeUrl { get; private set; }

        
        public virtual bool MustOverwrite { get => ForceOverwrite; }

        private bool _ForceOverwrite = false;
        public bool ForceOverwrite { get => _ForceOverwrite; set { SetAndNotify(ref _ForceOverwrite, value); NotifyOfPropertyChange(nameof(MustOverwrite)); } } 
        public virtual bool IsModified { get; set; } = false;
        public virtual bool WillCreateFile { get; } = true;
        public SSGroup()
        {
            CommonFilesReadOnly = new ReadOnlyObservableCollection<T>(CommonFiles);
        }
        
        public virtual void Add(ISSGenericFile file)
        {
            if (file is T typed)
            {
                Add(typed);
            }
            else
                throw new ArgumentException($"cannot add file of type {file.GetType()} to this group");
        }
        public virtual void Add(T file)
        {
            if (file.RelativeUrl == null )
                throw new ArgumentException("Cannot add file with no path to group");
            if (CommonFiles.Count() == 0)
            {
                RelativeUrl = file.RelativeUrl;
                CommonFiles.Add(file);
            } else
            {
                if (!file.RelativeUrl.Equals(RelativeUrl))
                    throw new ArgumentException("Cannot add file with unrelated path to group");
                CommonFiles.Add(file);
            }
        }

        public virtual void Remove(T file)
        {
            CommonFiles.Remove(file);
            if (CommonFiles.Count() == 0)
                RelativeUrl = null;
        }

        public abstract void WriteTo(SSBaseLinkUrl newPath);

        public override string ToString()
        {
            return "Group of (" + CommonFilesReadOnly.Count.ToString()+ ") " + (RelativeUrl?.ToString() ?? "no file");
        }
    }

    
}
