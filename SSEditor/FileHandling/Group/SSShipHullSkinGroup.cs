using FVJson;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSShipHullSkinGroup: SSJsonGroup<SSShipHullSkin>
    {
        public SSShipHullSkinGroup() : base()
        { }

        public MonitoredValue BaseHullId { get; private set; } = null;
        public MonitoredValue SkinHullId { get; private set; } = null;
        public MonitoredArray Tags { get; private set; } = null;

        protected override void AttachDefinedAttribute()
        {
            BaseHullId = AttachOneAttribute<MonitoredValue>(".baseHullId", JsonToken.TokenType.String);
            SkinHullId = AttachOneAttribute<MonitoredValue>(".skinHullId", JsonToken.TokenType.String);
            Tags =       AttachOneAttribute<MonitoredArray>(".tags");
        }
    
    }
}
