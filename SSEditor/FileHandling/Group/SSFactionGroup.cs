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
        public MonitoredArrayValue Color { get; private set; } = null;
        public MonitoredArrayValue DarkUIColor { get; private set; } = null;
        public MonitoredArrayValue BaseUIColor { get; private set; } = null;
        public MonitoredArrayValue SecondaryUIColor { get; private set; } = null;

        protected override void AttachDefinedAttribute()
        {
            DisplayName = AttachOneAttribute<MonitoredValue>(".displayName", JsonToken.TokenType.String);
            MalePortraits = AttachOneAttribute<MonitoredArray>(".portraits.standard_male");
            FemalePortraits = AttachOneAttribute<MonitoredArray>(".portraits.standard_female");

            //should be the base color for the faction
            Color = AttachOneAttribute<MonitoredArrayValue>(".color");
            //darkUIColor is the background color for text header
            DarkUIColor = AttachOneAttribute<MonitoredArrayValue>(".darkUIColor");
            //baseUIColor is the color of text
            BaseUIColor = AttachOneAttribute<MonitoredArrayValue>(".baseUIcolor");
            //secondaryUIColor should be the color of segment in 2 color faction
            SecondaryUIColor = AttachOneAttribute<MonitoredArrayValue>(".secondaryUIcolor");
            

           
            
            
            Id = AttachOneAttribute<MonitoredValue>(".id",JsonToken.TokenType.Reference);
            DisplayNameWithArticle = AttachOneAttribute<MonitoredValue>(".displayNameWithArticle", JsonToken.TokenType.String);
            ShipNamePrefix = AttachOneAttribute<MonitoredValue>(".shipNamePrefix", JsonToken.TokenType.String);
        }
        private T AttachOneAttribute<T>(string path, JsonToken.TokenType goalType = JsonToken.TokenType.String) where T:MonitoredField, new()
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
                    throw new InvalidOperationException($"Existing field {path} in file {this.RelativeUrl.ToString()} is different type than {typeof(T)}");
            }
            else
            {
                extracted = new T();
                if (extracted is MonitoredValue mv)
                    mv.GoalType = goalType;
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
