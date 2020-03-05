using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSFactionGroup : SSFileGroup
    {
        public MonitoredValue<Text> DisplayName { get; } = new MonitoredValue<Text>() { FieldPath = new List<string> { "displayName" } };
        public MonitoredArrayValue<Color> FactionColor { get; } = new MonitoredArrayValue<Color>() { FieldPath = new List<string> { "color" } };
        public MonitoredArray<Text> KnownHull { get; } = new MonitoredArray<Text>() { FieldPath = new List<string> { "knownShips", "hulls" } };

        public SSFactionGroup()
        {
            base.MonitoredFields.Add(DisplayName);
            base.MonitoredFields.Add(FactionColor);
            base.MonitoredFields.Add(KnownHull);
            base.SynchroniseMonitored();
        }
    }
    
}
