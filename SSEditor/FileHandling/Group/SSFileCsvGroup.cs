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
            //we do not merge core csv on the patch, it would be pointless
            IEnumerable<SSFileCsv> NonCoreFile = from SSFileCsv file in this.CommonFiles
                                                 where file.SourceMod.CurrentType != SSMod.ModType.Core
                                                 select file;
            if (NonCoreFile.Count() == 0)
                return;

            //we need to make sure the directory exist
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            //csv merge is just an append
            using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
            {
                bool columnNameCopied = false;
                foreach (SSFileCsv file in this.CommonFiles)
                {
                    SSFullUrl SourceUrl = InstallationUrl + file.LinkRelativeUrl;
                    using (StreamReader sr = File.OpenText(SourceUrl.ToString()))
                    {
                        String s = "";
                        //skip first line since its column name
                        //we would need not to skip it on first file ....
                        if (columnNameCopied)
                            s = sr.ReadLine();
                        else
                            columnNameCopied = true;
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
