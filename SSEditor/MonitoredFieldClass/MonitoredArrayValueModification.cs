using FVJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredArrayValueModification : IMonitoredModification
    {
        public enum ModificationType { Unset, Replace,}

        public Type RessourceType { get; set; } = null;
        public ModificationType ModType { get; set; }
        public JsonArray Content { get; set; }

        public MonitoredArrayValueModification() { }
        private MonitoredArrayValueModification(ModificationType type)
        { ModType = type; }
        private MonitoredArrayValueModification(JsonArray content, ModificationType type)
        {
            ModType = type;
            Content = content;
        }

        public override string ToString()
        {
            string show = null;
            if (Content != null)
            {
                var parts = Content.Values.Select(v => ((JsonValue)v).ToString());
                show = String.Join(",", parts);
            }
            return $"ValueArray.{ModType}, content: [{show ?? "n/a"}]";
        }

        public object GetContentAsValue()
        {
             throw new InvalidOperationException("Monitored value array cannot contain a single value");
        }

        public static MonitoredArrayValueModification GetUnsetModification()
        {
            var result = new MonitoredArrayValueModification(ModificationType.Unset);
            return result;
        }
        public static MonitoredArrayValueModification GetReplaceModification(JsonArray NewContent)
        {
            if (NewContent == null)
                throw new ArgumentException("Can't make add mod from empty token");
            return new MonitoredArrayValueModification(NewContent, ModificationType.Replace);
        }

    }
}

