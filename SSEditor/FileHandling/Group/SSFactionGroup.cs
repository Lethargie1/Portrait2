using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSFactionGroup : SSFileGroup<SSFactionFile>
    {
        public MonitoredValue<Text,SSFactionFile> DisplayName { get; } = new MonitoredValue<Text, SSFactionFile>() { FieldPath = new List<string> { "displayName" } };
        public MonitoredArrayValue<Color, SSFactionFile> FactionColor { get; } = new MonitoredArrayValue<Color, SSFactionFile>() { FieldPath = new List<string> { "color" } };
        public MonitoredArray<Text, SSFactionFile> KnownHull { get; } = new MonitoredArray<Text, SSFactionFile>() { FieldPath = new List<string> { "knownShips", "hulls" } };

        public SSFactionGroup() : base ()
        {
            DisplayName.ReplaceFiles(base.CommonFiles);
            FactionColor.ReplaceFiles(base.CommonFiles);
            KnownHull.ReplaceFiles(base.CommonFiles);
        }

        
    }
    
}
