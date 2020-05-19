using FVJson;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public class SSVariantGroup: SSJsonGroup<SSVariant>
    {
        public MonitoredValue HullId { get; private set; } = null;
        public MonitoredValue VariantId { get; private set; } = null;
        public SSVariantGroup() : base()
        { }

        protected override void AttachDefinedAttribute()
        {
            HullId = AttachOneAttribute<MonitoredValue>(".hullId", JsonToken.TokenType.String);
            VariantId = AttachOneAttribute<MonitoredValue>(".variantId", JsonToken.TokenType.String);
        }
    }
}
