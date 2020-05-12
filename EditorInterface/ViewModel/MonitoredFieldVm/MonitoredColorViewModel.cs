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
    public class MonitoredColorViewModel : Screen
    {
        private JsonArrayToColorConverter colorConverter = new JsonArrayToColorConverter();
        public MonitoredArrayValue MonitoredColor { get; private set; }
        private List<IEventBinding> binding = new List<IEventBinding>();
        public MonitoredColorViewModel(MonitoredArrayValue color )
        {
            MonitoredColor = color;
            if (MonitoredColor != null)
            binding.Add(MonitoredColor.Bind(x => x.ContentArray, (sender, arg) =>
            {
                NotifyOfPropertyChange(nameof(Color));
                NotifyOfPropertyChange(nameof(ValueWarning));
            }));

        }
        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            replacementBinding?.Unbind();
            base.OnClose();
        }
        public string Color
        {
            get
            {
                if (MonitoredColor?.ContentArray != null)
                {
                    if (replacementBinding != null)
                    {
                        replacementBinding.Unbind();
                        replacementBinding = null;
                    }
                    return (string)colorConverter.Convert(MonitoredColor?.ContentArray);
                }
                else if (ReplacementSource?.ContentArray != null)
                {
                    if (replacementBinding == null)
                        replacementBinding = ReplacementSource.Bind(x => x.ContentArray, (sender, arg) =>
                        {
                            NotifyOfPropertyChange(nameof(Color));
                            NotifyOfPropertyChange(nameof(ValueWarning));
                        });
                    return (string)colorConverter.Convert(ReplacementSourceTransformation(ReplacementSource?.ContentArray));
                }
                else
                    return DefaultColor;
            }

            set
            {

                if (value != Color)
                {
                    MonitoredColor.Modify(MonitoredArrayValueModification.GetReplaceModification((JsonArray)colorConverter.ConvertBack(value)));
                }
            }
        }
        public string DefaultColor { get; set; } = "#FFFFFFFF";

        IEventBinding replacementBinding;
        public MonitoredArrayValue ReplacementSource { get; set; }
        public Func<JsonArray, JsonArray> ReplacementSourceTransformation { get; set; } = x => x;
        
        public void Reset()
        {
            MonitoredColor?.Reset();
        }
        public string ValueWarning
        {
            get
            {
                if (MonitoredColor?.ContentArray == null)
                    return "Value not set";
                else
                 return MonitoredColor?.HasMultipleSourceFile ?? false ? "Has multiple source" : null;
            }
        }
    }
}
