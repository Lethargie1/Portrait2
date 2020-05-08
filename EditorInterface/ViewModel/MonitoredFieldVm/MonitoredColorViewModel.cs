using FVJson;
using SSEditor.Converters;
using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class MonitoredColorViewModel: Screen
    {
        private JsonArrayToColorConverter colorConverter = new JsonArrayToColorConverter();
        public MonitoredArrayValue MonitoredColor { get; private set; }

        public MonitoredColorViewModel(MonitoredArrayValue color)
        {
            MonitoredColor = color;
        }

        public string Color
        {
            get => (string)colorConverter.Convert(MonitoredColor.ContentArray);

            set
            {

                if (value != Color)
                {
                    MonitoredColor.ApplyModification((JsonArray)colorConverter.ConvertBack(value));
                    NotifyOfPropertyChange(nameof(Color));
                }
            }
        }


        public void Reset()
        {
            MonitoredColor?.Reset();
            NotifyOfPropertyChange(nameof(Color));
        }
        public string ValueWarning { get => MonitoredColor?.HasMultipleSourceFile ?? false ? "Has multiple source" : null; }
    }
}
