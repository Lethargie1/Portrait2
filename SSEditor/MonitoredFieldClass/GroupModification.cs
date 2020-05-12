using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class GroupModification
    {
        public SSRelativeUrl GroupUrl { get; set; }
        public string FieldPath { get; set; }
        public IMonitoredModification Modification { get; set; }
    }
}
