using Newtonsoft.Json;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    class SSJsonGroup : SSJsonGroup<SSJson>
    {

    }
    class SSJsonGroup<T> : SSGroup<T> where T: SSJson
    {
        public SSJsonGroup() : base() { }

        public override void WriteMergeTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl TargetUrl = newPath + this.CommonRelativeUrl;

            //we need to make sure the directory exist
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            MonitoredPropertyArray<T> TempList = new MonitoredPropertyArray<T>() { FieldPath = "" };
            if (base.MustOverwrite)
                TempList.ReplaceFiles(base.CommonFiles);
            else
            {
                List<T> ModList = (from T file in base.CommonFiles
                                               where file.SourceMod.CurrentType != SSMod.ModType.Core
                                               select file).ToList();
                TempList.ReplaceFiles(new ObservableCollection<T>(ModList));
            }

            using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
            {
                using (JsonTextWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    TempList.GetJsonEquivalent().WriteTo(writer);
                }
            }
        }
    }
}
