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


        public static ISSGenericFile BuildFile(SSMod mod, SSFullUrl fullUrl)
        {
            FileInfo info = new FileInfo(fullUrl?.ToString() ?? throw new ArgumentNullException("The Url cannot be null."));
            string FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
            string Extension = info.Extension;
            switch (Extension)
            {
                case ".json":
                    return new SSJson(mod, fullUrl);
                case ".faction":
                    return new SSFaction(mod, fullUrl);
                case ".csv":
                    return new SSCsv(mod, fullUrl);
                case ".jar":
                case ".java":
                    return new SSNoMerge(mod, fullUrl);
                default:
                    return new SSBinary(mod, fullUrl);
            }
        }

    }
}
