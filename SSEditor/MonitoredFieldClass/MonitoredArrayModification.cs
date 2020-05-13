using FVJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredArrayModification : IMonitoredModification
    {
        public enum ModificationType { Clear, Remove, Add}

        public Type RessourceType { get; set; } = null;
        public ModificationType ModType { get; set; }
        public JsonToken Content { get; set; }

        public MonitoredArrayModification() { }
        private MonitoredArrayModification(ModificationType type)
        { ModType = type; }
        private MonitoredArrayModification(JsonToken content, ModificationType type)
        {
            ModType = type;
            Content = content;
        }

        public override string ToString()
        {
            return $"Array.{ModType}, content type {Content?.GetType()?.ToString() ?? "n/a"}";
        }

        public static MonitoredArrayModification GetClearModification()
        {
            var result = new MonitoredArrayModification(ModificationType.Clear);
            return result;
        }
        public static MonitoredArrayModification GetAddModification(JsonToken NewContent, Type ressourceType=null)
        {
            if (NewContent == null)
                throw new ArgumentException("Can't make add mod from empty token");
            return new MonitoredArrayModification(NewContent, ModificationType.Add) {RessourceType = ressourceType };
        }
        public static MonitoredArrayModification GetRemoveModification(JsonToken NewContent)
        {
            if (NewContent == null)
                throw new ArgumentException("Can't make remove mod from empty token");
            return new MonitoredArrayModification(NewContent, ModificationType.Remove);
        }
    }
    public static class MonitoredArrayModificationExtension
    {
        public static void AddArrayMod(this ICollection<MonitoredArrayModification> modificationCollection, MonitoredArrayModification newMod)
        {
            MonitoredArrayModification opposite;
            switch (newMod.ModType)
            {
                case MonitoredArrayModification.ModificationType.Clear:
                    modificationCollection.Clear();
                    modificationCollection.Add(newMod);
                    return;
                case MonitoredArrayModification.ModificationType.Add:
                    opposite = (from MonitoredArrayModification m in modificationCollection
                                  where m.ModType == MonitoredArrayModification.ModificationType.Remove && m.Content.Equals(newMod.Content)
                                  select m).FirstOrDefault();
                    if (opposite != null)
                        modificationCollection.Remove(opposite);
                    else
                        modificationCollection.Add(newMod);
                    return;
                case MonitoredArrayModification.ModificationType.Remove:
                    opposite = (from MonitoredArrayModification m in modificationCollection
                                where m.ModType == MonitoredArrayModification.ModificationType.Add && m.Content.Equals(newMod.Content)
                                select m).FirstOrDefault();
                    if (opposite != null)
                        modificationCollection.Remove(opposite);
                    else
                        modificationCollection.Add(newMod);
                    return;
                default:
                    throw new ArgumentException("Unknown modification type");

            }

        }
        public static void ApplyModification(this ICollection<JsonToken> targetContent, MonitoredArrayModification newMod)
        {
            switch (newMod.ModType)
            {
                case MonitoredArrayModification.ModificationType.Add:
                    targetContent.Add(newMod.Content);
                    break;
                case MonitoredArrayModification.ModificationType.Clear:
                    targetContent.Clear();
                    break;
                case MonitoredArrayModification.ModificationType.Remove:
                    if (targetContent.Contains(newMod.Content))
                        targetContent.Remove(newMod.Content);
                    break;
            }
        }
    }
}
