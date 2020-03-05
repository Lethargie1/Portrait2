using SSEditor.MonitoredTokenClass;
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
        public ObservableCollection<SSFile> Files { get;} = new ObservableCollection<SSFile>();
        public List<MonitoredField> MonitoredFields { get; } = new List<MonitoredField>();

        public SSFileGroup()
        {
            foreach (MonitoredField field in MonitoredFields)
                field.ReplaceFiles(Files);
        }
    }
}
