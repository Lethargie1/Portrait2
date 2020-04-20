using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public abstract class SSGroup<T> : ISSGroup, ISSWritable where T: ISSMergable
    {
        public ReadOnlyObservableCollection<T> CommonFilesReadOnly { get; private set; }
        protected ObservableCollection<T> CommonFiles { get;} = new ObservableCollection<T>();

        public SSRelativeUrl RelativeUrl { get; private set; }

        public bool MustOverwrite { get; set; } = true;

        public SSGroup()
        {
            CommonFilesReadOnly = new ReadOnlyObservableCollection<T>(CommonFiles);
        }

        public void Add(T file)
        {
            if (file.LinkRelativeUrl == null || file.LinkRelativeUrl.Link == null || file.LinkRelativeUrl.Relative == null)
                throw new ArgumentException("Cannot add file with no path to group");
            if (CommonFiles.Count() == 0)
            {
                RelativeUrl = file.LinkRelativeUrl.GetRelative();
                CommonFiles.Add(file);
            } else
            {
                if (!file.LinkRelativeUrl.GetRelative().Equals(RelativeUrl))
                    throw new ArgumentException("Cannot add file with unrelated path to group");
                CommonFiles.Add(file);
            }
        }

        public void Remove(T file)
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
