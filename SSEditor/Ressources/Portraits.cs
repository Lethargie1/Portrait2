using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class Portraits
    {
        public static IEnumerable<JsonValue> GetOriginalPortraits(IEnumerable<SSFactionGroup> factionGroups)
        {
            List<JsonToken> Original = factionGroups.SelectMany(g =>
            {
                List<JsonToken> result = new List<JsonToken>();
                if (g.MalePortraits != null)
                    result.AddRange(g.MalePortraits.GetOriginalContent());
                if (g.FemalePortraits != null)
                    result.AddRange(g.FemalePortraits?.GetOriginalContent());
                return result;
            }).Distinct().ToList();
            return Original.Cast<JsonValue>();
        }
    }
}
