using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SSEditor.Ressources
{
    public class PortraitsRessources
    {
        public static PropertyGroupDescription GroupDescription { get; } = new PropertyGroupDescription("SourceModName");

        private List<SSFactionGroup> ReferencedFiles{ get; set; }

        public Dictionary<string, Portraits> RessourceCorrespondance { get; } = new Dictionary<string, Portraits>();
        public List<Portraits> Ressouces { get; } = new List<Portraits>();
        private SSDirectory Directory { get; set; }
        public PortraitsRessources(SSDirectory directory)
        {
            Directory = directory;
            ReferencedFiles = (from KeyValuePair<string, ISSGroup> kv in Directory.GroupedFiles
                        where kv.Value is SSFactionGroup
                        select kv.Value as SSFactionGroup).ToList();
            IEnumerable<JsonValue> ReferencedPortrait = ReferencedFiles.SelectMany(g =>
            {
                List<JsonToken> result = new List<JsonToken>();
                if (g.MalePortraits != null)
                    result.AddRange(g.MalePortraits.GetOriginalContent());
                if (g.FemalePortraits != null)
                    result.AddRange(g.FemalePortraits.GetOriginalContent());
                return result;
            }).Distinct().Cast<JsonValue>();

            foreach (JsonValue referenced in ReferencedPortrait)
            {
                string pathRelative = referenced.ToString().Replace('/', '\\');
                Portraits port = this.FindBinaryFromDirectory(pathRelative);
                if (port != null)
                    RessourceCorrespondance.Add(pathRelative, port);

            }


        }

        public Portraits FindBinaryFromDirectory(string pathRelative)
        {
                ISSGroup source;
                Directory.GroupedFiles.TryGetValue(pathRelative, out source);
                if (source is SSBinaryGroup group)
                {
                    group.RecalculateFinal();
                return new Portraits(group.FinalFile);
                }
            return null;
            
        }
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

        public static IEnumerable<JsonValue> GetCurrentPortraits(IEnumerable<SSFactionGroup> factionGroups)
        {
            List<JsonToken> Original = factionGroups.SelectMany(g =>
            {
                List<JsonToken> result = new List<JsonToken>();
                if (g.MalePortraits != null)
                    result.AddRange(g.MalePortraits.ContentArray);
                if (g.FemalePortraits != null)
                    result.AddRange(g.FemalePortraits?.ContentArray);
                return result;
            }).Distinct().ToList();
            return Original.Cast<JsonValue>();
        }
    }

    [ValueConversion(typeof(JsonValue), typeof(Portraits))]
    public class JsonRelativeToPortraits : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            JsonValue source = (JsonValue)value[0];
            PortraitsRessources portraitsRessources = (PortraitsRessources)value[1];
            Portraits result;
            portraitsRessources.RessourceCorrespondance.TryGetValue(source.ToString().Replace('/', '\\'), out result);
            return result;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
