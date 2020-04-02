using Newtonsoft.Json.Linq;
using SSEditor.MonitoringField;
using SSEditor.TokenClass;
using SSEditor.JsonHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SSEditor.FileHandling
{
    class SSFactionGroup : SSFileGroup<SSFactionFile>
    {
        public MonitoredValue<Text,SSFactionFile> DisplayName { get; } = new MonitoredValue<Text, SSFactionFile>() { FieldPath = "displayName" };
        public MonitoredArrayValue<Color, SSFactionFile> FactionColor { get; } = new MonitoredArrayValue<Color, SSFactionFile>() { FieldPath = "color" };
        public MonitoredArray<Text, SSFactionFile> KnownHull { get; } = new MonitoredArray<Text, SSFactionFile>() { FieldPath = "knownShips.hulls" };

        public SSFactionGroup() : base ()
        {
            DisplayName.ReplaceFiles(base.CommonFiles);
            FactionColor.ReplaceFiles(base.CommonFiles);
            KnownHull.ReplaceFiles(base.CommonFiles);
        }

        public void WriteMergeTo(SSBaseLinkUrl newPath)
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
            JObject NewContent = new JObject();
            IEnumerable<MonitoredField<SSFactionFile>> TempList = MonitoredField<SSFactionFile>.ExtractFields(base.CommonFiles);

            foreach (MonitoredField<SSFactionFile> mf in TempList)
            {
                JObject a = mf.GetJsonEquivalent();
                NewContent.ConcatRecursive(a);
            }


        }


    }
    
}
