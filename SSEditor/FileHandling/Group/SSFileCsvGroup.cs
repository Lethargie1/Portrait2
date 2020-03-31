using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSFileCsvGroup : SSFileGroup<SSFileCsv>
    {
        public void WriteMergeTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl TargetUrl = newPath + this.CommonRelativeUrl;

            using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
            {
                foreach (SSFileCsv file in this.CommonFiles)
                {
                    SSFullUrl SourceUrl = InstallationUrl + file.LinkRelativeUrl;
                    using (StreamReader sr = File.OpenText(SourceUrl.ToString()))
                    {
                        String s = "";

                        while ((s = sr.ReadLine()) != null)
                        {
                            sw.WriteLine(s);
                        }
                    }
                }
            }
        }
    }
}
