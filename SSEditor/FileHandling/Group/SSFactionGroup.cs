using Newtonsoft.Json.Linq;
using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using SSEditor.JsonHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SSEditor.FileHandling
{
    class SSFactionGroup : SSJsonGroup<SSFaction>
    {
        public MonitoredValue<Text,SSFaction> DisplayName { get; } = new MonitoredValue<Text, SSFaction>() { FieldPath = "displayName" };
        public MonitoredArrayValue<Color, SSFaction> FactionColor { get; } = new MonitoredArrayValue<Color, SSFaction>() { FieldPath = "color" };
        public MonitoredArray<Text, SSFaction> KnownHull { get; } = new MonitoredArray<Text, SSFaction>() { FieldPath = "knownShips.hulls" };


        public SSFactionGroup() : base ()
        {
            DisplayName.ReplaceFiles(base.CommonFiles);
            FactionColor.ReplaceFiles(base.CommonFiles);
            KnownHull.ReplaceFiles(base.CommonFiles);
        }
    }
    
}
