using FVJson;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSShipHullGroup : SSJsonGroup<SSShipHull>
    {
        public MonitoredValue HullId { get; private set; } = null;
        public MonitoredValue HullName { get; private set; } = null;
        public MonitoredValue SpriteName { get; private set; } = null;
        public MonitoredValue HullSize{ get; private set; } = null;

        public SSShipHullGroup() : base()
        { }

        protected override void AttachDefinedAttribute()
        {
            HullId = AttachOneAttribute<MonitoredValue>(".hullId", JsonToken.TokenType.String);
            HullName = AttachOneAttribute<MonitoredValue>(".hullName", JsonToken.TokenType.String);
            SpriteName = AttachOneAttribute<MonitoredValue>(".spriteName", JsonToken.TokenType.String);
            HullSize = AttachOneAttribute<MonitoredValue>(".hullSize", JsonToken.TokenType.String);
        }
    }
}
