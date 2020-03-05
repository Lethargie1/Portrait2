using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSFileGroup
    {
        public ReadOnlyObservableCollection<SSFile> CommonFilesReadOnly { get; private set; }
        protected ObservableCollection<SSFile> CommonFiles { get;} = new ObservableCollection<SSFile>();
        protected List<MonitoredField> MonitoredFields { get; } = new List<MonitoredField>();

        public SSFileGroup()
        {
            CommonFilesReadOnly = new ReadOnlyObservableCollection<SSFile>(CommonFiles);
        }

        public void SynchroniseMonitored()
        {
            foreach (MonitoredField field in MonitoredFields)
                field.ReplaceFiles(CommonFiles);
        }
    }
}
