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

        public SSShipHullGroup() : base()
        { }

        protected override void AttachDefinedAttribute()
        {
            HullId = AttachOneAttribute<MonitoredValue>(".hullId", JsonToken.TokenType.String);
        }
    }
}
