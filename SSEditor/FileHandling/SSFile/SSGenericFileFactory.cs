using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public static class SSGenericFileFactory
    {


        public static ISSGenericFile BuildFile(SSMod mod, SSRelativeUrl Url)
        {
            FileInfo info = new FileInfo(Url?.ToString() ?? throw new ArgumentNullException("The Url cannot be null."));
            string FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
            string Extension = info.Extension;
            switch (Extension)
            {
                case ".json":
                    return new SSJson(mod, Url);
                case ".faction":
                    return new SSFaction(mod, Url);
                case ".variant":
                    return new SSVariant(mod, Url);
                case ".ship":
                    return new SSShipHull(mod, Url);
                case ".csv":
                    return new SSCsv(mod, Url);
                case ".jar":
                case ".java":
                    return new SSNoMerge(mod, Url);
                default:
                    return new SSBinary(mod, Url);
            }
        }

    }
}
