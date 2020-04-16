using SSEditor.MonitoringField;
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
        public MonitoredValue<SSFaction> DisplayName { get; private set; } = null;
        public MonitoredArray<SSFaction> MalePortraits { get; private set; } = null;
        public MonitoredArray<SSFaction> FemalePortraits { get; private set; } = null;
        public MonitoredArrayValue<SSFaction> FactionColor { get; private set; } = new MonitoredArrayValue<SSFaction>() { FieldPath = "color" };
        protected override void AttachDefinedAttribute()
        {
            DisplayName = AttachOneAttribute<MonitoredValue<SSFaction>>(".displayName");
            MalePortraits = AttachOneAttribute<MonitoredArray<SSFaction>>(".portraits.standard_male");
            FemalePortraits = AttachOneAttribute<MonitoredArray<SSFaction>>(".portraits.standard_female");
            FactionColor = AttachOneAttribute<MonitoredArrayValue<SSFaction>>(".color");

        }
        private T AttachOneAttribute<T>(string path) where T:MonitoredField<SSFaction>
        {
            MonitoredField<SSFaction> extracted;
            if (PathedContent.TryGetValue(path, out extracted))
            {
                if (extracted is T typed)
                    return typed;
                else
                    throw new InvalidOperationException("Existing field is different type than defined one");
            }
            else
                return null;
        }


        public SSFactionGroup() : base ()
        {
        }

    }
    
}
