using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class MonitoredColorViewModel
    {
        public MonitoredArrayValue Color { get; private set; }

        public MonitoredColorViewModel(MonitoredArrayValue color)
        {
            Color = color;
        }
    }
}
