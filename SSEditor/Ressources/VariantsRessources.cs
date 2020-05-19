using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class VariantsRessources
    {
        public SSDirectory Directory { get; set; }
        public List<SSVariantGroup> VariantFiles { get; private set; }
        public VariantsRessources(SSDirectory directory)
        {
            Directory = directory;
            VariantFiles = (from KeyValuePair<string, ISSGroup> kv in Directory.GroupedFiles
                            where kv.Value is SSVariantGroup
                            select kv.Value).Select(g =>
                           {
                               SSVariantGroup f = (SSVariantGroup)g;
                               if (f.MonitoredContent == null)
                                   f.ExtractMonitoredContent();
                               return g as SSVariantGroup;
                           }).ToList();
        }

        public string GetHullIdFromVariantName(string variantName)
        {
            return VariantFiles.FirstOrDefault(x => x.VariantId.Content.ToString() == variantName)?.HullId.Content.ToString();
        }
    }
}
