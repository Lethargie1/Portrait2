using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupValueViewModel:Screen
    {
        public SSFactionGroup FactionGroup { get; set; }

        protected List<IEventBinding> binding = new List<IEventBinding>();
        public FactionGroupValueViewModel(SSFactionGroup factionGroup)
        {
            FactionGroup = factionGroup;
            SSDisplayName = new MonitoredValueViewModel(FactionGroup?.DisplayName);
            DisplayNameWithArticle = new MonitoredValueViewModel(FactionGroup?.DisplayNameWithArticle);
            ShipNamePrefix = new MonitoredValueViewModel(FactionGroup?.ShipNamePrefix);
            Color = new MonitoredColorViewModel(FactionGroup?.Color);
            BaseUIColor = new MonitoredColorViewModel(FactionGroup?.BaseUIColor) {ReplacementSource = FactionGroup?.Color };
            DarkUIColor = new MonitoredColorViewModel(FactionGroup?.DarkUIColor)
            {
                ReplacementSource = FactionGroup?.Color,
                ReplacementSourceTransformation = delegate(JsonArray a) 
                {
                    JsonArray result = new JsonArray();
                        List<int> number = (from JsonToken j in a.Values
                                               select Convert.ToInt32( ((JsonValue)j).Content)   ).ToList();
                    
                    result.Values.Add(new JsonValue(number[0]*0.4));
                    result.Values.Add(new JsonValue(number[1]*0.4));
                    result.Values.Add(new JsonValue(number[2]*0.4));
                    JsonValue alpha = new JsonValue(175);
                    result.Values.Add(alpha);
                    return result;
                }
            };
            SecondaryUIColor = new MonitoredColorViewModel(FactionGroup?.SecondaryUIColor) { ReplacementSource = FactionGroup?.Color };
            SecondarySegments = new MonitoredValueViewModel(FactionGroup?.SecondarySegments);
            FleetCircleViewModel = new FactionGroupFleetCircleViewModel(Color,SecondaryUIColor, SecondarySegments);

        }
        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
        }

        public string Id
        {
            get { return FactionGroup?.Id?.Content.ToString(); }
        }

        public MonitoredValueViewModel SSDisplayName { get; }
        public MonitoredValueViewModel DisplayNameWithArticle { get; }

        public MonitoredValueViewModel ShipNamePrefix { get; }

        public MonitoredColorViewModel Color { get; }
        public MonitoredColorViewModel BaseUIColor { get; }
        public MonitoredColorViewModel DarkUIColor { get; }
        public MonitoredColorViewModel SecondaryUIColor { get; }

        public MonitoredValueViewModel SecondarySegments { get; }

        public FactionGroupFleetCircleViewModel FleetCircleViewModel { get; }

    }
}
