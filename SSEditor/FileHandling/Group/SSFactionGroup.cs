using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FVJson;
using Stylet;

namespace SSEditor.FileHandling
{
    public class SSFactionGroup : SSJsonGroup<SSFaction>
    {
        public MonitoredValue<SSFaction> Id { get; private set; } = null;
        public MonitoredValue<SSFaction> DisplayName { get; private set; } = null;
        public MonitoredValue<SSFaction> DisplayNameWithArticle { get; private set; } = null;
        public MonitoredValue<SSFaction> ShipNamePrefix { get; private set; } = null;
        public MonitoredArray<SSFaction> MalePortraits { get; private set; } = null;
        public MonitoredArray<SSFaction> FemalePortraits { get; private set; } = null;
        public MonitoredArrayValue<SSFaction> FactionColor { get; private set; } = null;
        
        protected override void AttachDefinedAttribute()
        {
            DisplayName = AttachOneAttribute<MonitoredValue<SSFaction>>(".displayName");
            MalePortraits = AttachOneAttribute<MonitoredArray<SSFaction>>(".portraits.standard_male");
            FemalePortraits = AttachOneAttribute<MonitoredArray<SSFaction>>(".portraits.standard_female");
            FactionColor = AttachOneAttribute<MonitoredArrayValue<SSFaction>>(".color");
            Id = AttachOneAttribute<MonitoredValue<SSFaction>>(".id");
            DisplayName = AttachOneAttribute<MonitoredValue<SSFaction>>(".displayName");
            DisplayNameWithArticle = AttachOneAttribute<MonitoredValue<SSFaction>>(".displayNameWithArticle");
            ShipNamePrefix = AttachOneAttribute<MonitoredValue<SSFaction>>(".shipNamePrefix");
        }
        private T AttachOneAttribute<T>(string path) where T:MonitoredField<SSFaction>
        {
            MonitoredField<SSFaction> extracted;
            if (PathedContent.TryGetValue(path, out extracted))
            {
                if (extracted is T typed)
                {
                    typed.Bind(x => x.Modified, (sender, arg) => SubPropertyModified(sender, arg));
                    return typed;
                }
                else
                    throw new InvalidOperationException("Existing field is different type than defined one");
            }
            else
                return null;
        }


        public SSFactionGroup() : base ()
        {
        }

        public void SubPropertyModified(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(nameof(IsModified));
            NotifyOfPropertyChange(nameof(MustOverwrite));
        }
    }
    
}
