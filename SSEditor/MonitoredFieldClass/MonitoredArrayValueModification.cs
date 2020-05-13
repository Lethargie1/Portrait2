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

        public ModificationType ModType { get; private set; }
        public JsonArray Content { get; private set; }

        private MonitoredArrayValueModification(ModificationType type)
        { ModType = type; }
        private MonitoredArrayValueModification(JsonArray content, ModificationType type)
        {
            ModType = type;
            Content = content;
        }

        public override string ToString()
        {
            return $"ValueArray.{ModType}, content (description not implemented)";
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

