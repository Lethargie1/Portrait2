using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class MonitoredValueViewModel : Screen 
    {
        public MonitoredValue MonitoredValue { get; set; }
        private List<IEventBinding> binding = new List<IEventBinding>();
        public MonitoredValueViewModel(MonitoredValue monitoredValue)
        {
            MonitoredValue = monitoredValue;
            binding.Add(MonitoredValue.Bind(x => x.Content, (sender, arg) => 
            {
                NotifyOfPropertyChange(nameof(Value));
                NotifyOfPropertyChange(nameof(ValueWarning));
            }));
        }
        protected override void OnClose()
        {
            foreach (IEventBinding b in binding)
                b.Unbind();
            base.OnClose();
        }
        public string Value
        {
            get { return MonitoredValue?.Content?.ToString(); }
            set
            {
                switch (MonitoredValue?.GoalType)
                {
                    case JsonToken.TokenType.Double:
                    case JsonToken.TokenType.Integer:
                        MonitoredValue?.ApplyModification(new JsonValue(Convert.ToDouble(value)));
                        break;
                    case JsonToken.TokenType.Reference:
                        MonitoredValue?.ApplyModification(new JsonValue(value,JsonToken.TokenType.Reference));
                        break;
                    case JsonToken.TokenType.String:
                        MonitoredValue?.ApplyModification(new JsonValue(value));
                        break;
                    case JsonToken.TokenType.Boolean:
                        throw new NotImplementedException("Havent done boolean yet");
                        break;
                    default:
                        throw new InvalidOperationException("Value type is improperly set");
                }

            }
        }
        public void Reset()
        {
            MonitoredValue?.Reset();
        }
        public string ValueWarning { get => MonitoredValue?.HasMultipleSourceFile ?? false ? "Has multiple source" : null; }
    }
}
