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

        public MonitoredValue SecondarySegments { get; private set; } = null;
        public MonitoredArray KnownShipsTag { get; private set; } = null;
        public MonitoredArray KnownShipsHulls { get; private set; } = null;
        public MonitoredArray PriorityShipsTag { get; private set; } = null;
        public MonitoredArray PriorityShipsHulls { get; private set; } = null;
        public MonitoredArray ShipsWhenImportingTag { get; private set; } = null;
        public MonitoredArray ShipsWhenImportingHulls { get; private set; } = null;

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
            BaseUIColor = AttachOneAttribute<MonitoredArrayValue>(".baseUIColor");
            //secondaryUIColor should be the color of segment in 2 color faction
            SecondaryUIColor = AttachOneAttribute<MonitoredArrayValue>(".secondaryUIColor");

            SecondarySegments = AttachOneAttribute<MonitoredValue>(".secondarySegments", JsonToken.TokenType.Double);
           
            
            
            Id = AttachOneAttribute<MonitoredValue>(".id",JsonToken.TokenType.Reference);
            DisplayNameWithArticle = AttachOneAttribute<MonitoredValue>(".displayNameWithArticle", JsonToken.TokenType.String);
            ShipNamePrefix = AttachOneAttribute<MonitoredValue>(".shipNamePrefix", JsonToken.TokenType.String);

            KnownShipsTag = AttachOneAttribute<MonitoredArray>(".knownShips.tags");
            KnownShipsHulls = AttachOneAttribute<MonitoredArray>(".knownShips.hulls");
            PriorityShipsTag = AttachOneAttribute<MonitoredArray>(".priorityShips.tags");
            PriorityShipsHulls = AttachOneAttribute<MonitoredArray>(".priorityShips.hulls");
            ShipsWhenImportingTag = AttachOneAttribute<MonitoredArray>(".shipsWhenImporting.tags");
            ShipsWhenImportingHulls = AttachOneAttribute<MonitoredArray>(".shipsWhenImporting.hulls");
        }
        


        public SSFactionGroup() : base ()
        {
        }

        
    }
    
}
