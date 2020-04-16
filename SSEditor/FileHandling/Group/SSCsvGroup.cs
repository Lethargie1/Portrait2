using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace SSEditor.FileHandling
{
    class SSCsvGroup : SSGroup<SSCsv>
    {
        public override void WriteMergeTo(SSBaseLinkUrl newPath)
        {

            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl TargetUrl = newPath + this.CommonRelativeUrl;
            //we do not merge core csv on the patch, it would be pointless
            List<SSCsv> NonCoreFile = (from SSCsv file in this.CommonFiles
                                             where file.SourceMod.CurrentType != SSMod.ModType.Core
                                             select file).ToList<SSCsv>();
            if (NonCoreFile.Count() == 0)
                return;

            //we need to make sure the directory exist
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            Factory csvFactory = new Factory();
            //csv merge requires us to match column tutle
            using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
            {
                CsvWriter csvWriter = new CsvWriter(sw, CultureInfo.InvariantCulture);
                List<List<String>> allHeaders = new List<List<string>>();
                foreach (SSCsv file in NonCoreFile)
                {
                    SSFullUrl SourceUrl = InstallationUrl + file.LinkRelativeUrl;

                    using (StreamReader sr = File.OpenText(SourceUrl.ToString()))
                    {
                        IParser parser = csvFactory.CreateParser(sr, CultureInfo.InvariantCulture);
                        allHeaders.Add(parser.Read().ToList<string>());
                    }
                }
                List<string> finalHeaders = allHeaders.SelectMany(c => c).Distinct().ToList<string>();
                //lets write column title once
                foreach (string head in finalHeaders)
                {
                        csvWriter.WriteField(head);
                }
                csvWriter.NextRecord();

                for (int fileIndex = 0; fileIndex < NonCoreFile.Count; fileIndex++)
                {
                    SSFullUrl SourceUrl = InstallationUrl + NonCoreFile[fileIndex].LinkRelativeUrl;
                    List<string> localHeaders = allHeaders[fileIndex];
                    int[] finalSourceFromLocal = new int[finalHeaders.Count];
                    for (int finalIndex = 0; finalIndex < finalHeaders.Count; finalIndex++)
                    {
                        finalSourceFromLocal[finalIndex] = localHeaders.FindIndex(c => c == finalHeaders[finalIndex]);
                    }
                    using (StreamReader sr = File.OpenText(SourceUrl.ToString()))
                    {
                        IParser parser = csvFactory.CreateParser(sr, CultureInfo.InvariantCulture);
                        parser.Configuration.AllowComments = true;
                        parser.Configuration.BadDataFound = null;

                        string[] line = parser.Read(); //skip first line that contain headers
                        line = parser.Read();
                        while (line != null)
                        {
                            if (line.Count() != localHeaders.Count)
                            {
                                line = parser.Read();
                                continue;
                            }
                            foreach (int source in finalSourceFromLocal)
                            {
                                if (source == -1)
                                    csvWriter.WriteField("");
                                else
                                    csvWriter.WriteField(line[source]);
                            }
                            csvWriter.NextRecord();
                            line = parser.Read();
                        }

                    }
                }

            }
        }
    }
}

