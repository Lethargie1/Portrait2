using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public static class SSRessource
    {

        /// <summary>From a list of file implementing the generic file interface a relative reference finds a ressource.</summary>
        /// <param name="files">list of all files accesible</param>
        /// <param name="url">Relative Url of the referenced Url</param>
        public static ISSGenericFile FindFromFiles(IEnumerable<ISSGenericFile> files, SSRelativeUrl url)
        {
            var match = from f in files
                        where f.RelativeUrl == url
                        select f;
            return match.Single();

        }

        /// <summary>From a list of file creates a maping between a jsonvalue relative path and the actual file</summary>
        /// <param name="files">list of all files accesible</param>
        /// <param name="referencedRessources">Ressources referenced</param>
        public static Dictionary<JsonValue,string> FindFromFiles(IEnumerable<ISSGenericFile> files, IEnumerable<JsonValue> referencedRessources)
        {
            Dictionary<JsonValue, string> result = new Dictionary<JsonValue, string>();
            foreach (JsonValue reference in referencedRessources)
            {
                SSRelativeUrl url = new SSRelativeUrl(reference.ToString());
                ISSGenericFile matching = FindFromFiles(files, url);
                SSFullUrl full = matching.SourceMod.ModUrl + url;
                result.Add(reference, full.ToString());
            }
            return result;
        }
    }
}
