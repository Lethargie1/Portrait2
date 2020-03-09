using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSEditor.FileHandling.Filing;

namespace SSEditor.FileHandling
{
    class SSFileGroup
    {
        public ReadOnlyObservableCollection<SSFile> CommonFilesReadOnly { get; private set; }
        protected ObservableCollection<SSFile> CommonFiles { get;} = new ObservableCollection<SSFile>();

        public SSRelativeUrl CommonRelativeUrl { get; private set; }

        public SSFileGroup()
        {
            CommonFilesReadOnly = new ReadOnlyObservableCollection<SSFile>(CommonFiles);
        }

        public void Add(SSFile file)
        {
            if (file.LinkRelativeUrl == null || file.LinkRelativeUrl.Link == null || file.LinkRelativeUrl.Relative == null)
                throw new ArgumentException("Cannot add file with no path to group");
            if (CommonFiles.Count() == 0)
            {
                CommonRelativeUrl = file.LinkRelativeUrl.GetRelative();
                CommonFiles.Add(file);
            } else
            {
                if (!file.LinkRelativeUrl.GetRelative().Equals(CommonRelativeUrl))
                    throw new ArgumentException("Cannot add file with unrelated path to group");
                CommonFiles.Add(file);
            }
        }

        public void Remove(SSFile file)
        {
            CommonFiles.Remove(file);
            if (CommonFiles.Count() == 0)
                CommonRelativeUrl = null;
        }

        public override string ToString()
        {
            return "Group of (" + CommonFilesReadOnly.Count.ToString()+ ") " + (CommonRelativeUrl?.ToString() ?? "no file");
        }
    }
}
