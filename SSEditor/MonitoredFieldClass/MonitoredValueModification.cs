using FVJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredValueModification : IMonitoredModification
    {
        public enum ModificationType { Unset, Replace, }

        public Type RessourceType { get; set; } = null;
        public ModificationType ModType { get; set; }
        public JsonValue Content { get; set; }

        public MonitoredValueModification()
        { }
        private MonitoredValueModification(ModificationType type)
        { ModType = type; }
        private MonitoredValueModification(JsonValue content, ModificationType type)
        {
            ModType = type;
            Content = content;
        }

        public override string ToString()
        {
            return $"Value.{ModType}, content {Content.ToString()}";
        }

        public static MonitoredValueModification GetUnsetModification()
        {
            var result = new MonitoredValueModification(ModificationType.Unset);
            return result;
        }
        public static MonitoredValueModification GetReplaceModification(JsonValue NewContent)
        {
            if (NewContent == null)
                throw new ArgumentException("Can't make replace mod from empty token");
            return new MonitoredValueModification(NewContent, ModificationType.Replace);
        }
    }
}
