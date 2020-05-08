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
        public MonitoredValue Id { get; private set; } = null;
        public MonitoredValue DisplayName { get; private set; } = null;
        public MonitoredValue DisplayNameWithArticle { get; private set; } = null;
        public MonitoredValue ShipNamePrefix { get; private set; } = null;
        public MonitoredArray MalePortraits { get; private set; } = null;
        public MonitoredArray FemalePortraits { get; private set; } = null;
        public MonitoredArrayValue FactionColor { get; private set; } = null;
        
        protected override void AttachDefinedAttribute()
        {
            DisplayName = AttachOneAttribute<MonitoredValue>(".displayName");
            MalePortraits = AttachOneAttribute<MonitoredArray>(".portraits.standard_male");
            FemalePortraits = AttachOneAttribute<MonitoredArray>(".portraits.standard_female");
            FactionColor = AttachOneAttribute<MonitoredArrayValue>(".color");
            Id = AttachOneAttribute<MonitoredValue>(".id");
            DisplayNameWithArticle = AttachOneAttribute<MonitoredValue>(".displayNameWithArticle");
            ShipNamePrefix = AttachOneAttribute<MonitoredValue>(".shipNamePrefix");
        }
        private T AttachOneAttribute<T>(string path) where T:MonitoredField, new()
        {
            MonitoredField extracted;
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
            {
                extracted = new T();
                MonitoredContent.AddSubMonitor(path, extracted);
                extracted.Bind(x => x.Modified, (sender, arg) => SubPropertyModified(sender, arg));
                return extracted as T;
            }
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
