using SSEditor.MonitoringField;
using SSEditor.JsonHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FVJson;

namespace SSEditor.FileHandling
{
    class SSFactionGroup : SSJsonGroup<SSFaction>
    {
        public MonitoredValue<SSFaction> DisplayName { get; } = new MonitoredValue< SSFaction>(new JsonValue(null,JsonToken.TokenType.String)) { FieldPath = "displayName" };
        public MonitoredArrayValue<SSFaction> FactionColor { get; } = new MonitoredArrayValue<SSFaction>() { FieldPath = "color" };
        public MonitoredArray< SSFaction> KnownHull { get; } = new MonitoredArray<SSFaction>() { FieldPath = "knownShips.hulls" };


        public SSFactionGroup() : base ()
        {
            DisplayName.ReplaceFiles(base.CommonFiles);
            FactionColor.ReplaceFiles(base.CommonFiles);
            KnownHull.ReplaceFiles(base.CommonFiles);
        }
    }
    
}
