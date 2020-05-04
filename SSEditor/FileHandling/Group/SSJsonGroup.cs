using FVJson;
using SSEditor.FileHandling;
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
    public class SSJsonGroup : SSJsonGroup<SSJson>
    {

    }
    public class SSJsonGroup<T> : SSGroup<T>, ISSJsonGroup where T: SSJson
    {

        public override bool MustOverwrite
        {
            get => MonitoredContent.RequiresOverwrite();      
        }
        public MonitoredObject<T> MonitoredContent { get; set; } = null;
        public Dictionary<string, MonitoredField<T>> PathedContent { get; private set; } = new Dictionary<string, MonitoredField<T>>();

        public override bool IsModified 
        {
            get
            {
                if (MonitoredContent.IsModified() == true)
                    return true;
                return false;
            }
            set => base.IsModified = value; 
        }
        public override bool WillCreateFile
        {
            get
            {
                if (MonitoredContent.Files.Count == 0)
                    return false;
                if (MonitoredContent.IsModified() == true)
                    return true;
                return false;
            }
        }

        public SSJsonGroup() : base() { }

        public void ExtractMonitoredContent()
        {
            MonitoredObject<T> rootMonitoredObject = new MonitoredObject<T>() { FieldPath = "" };
            
            T core = (from T file in base.CommonFiles
                      where file.SourceMod.CurrentType == ModType.Core
                      select file).SingleOrDefault();
            List<T> modAdded = (from T file in base.CommonFiles
                                where file.SourceMod.CurrentType == ModType.Mod
                                select file).ToList();
            ObservableCollection<T> fileUsed = new ObservableCollection<T>(modAdded);
            if ( core != null)
                fileUsed.Add(core);


            rootMonitoredObject.ReplaceFiles(fileUsed);
            MonitoredContent = rootMonitoredObject;
            PopulatePathedContent();
            AttachDefinedAttribute();
        }
        protected virtual void AttachDefinedAttribute()
        { }

        public void PopulatePathedContent()
        {
            Dictionary<string, MonitoredField<T>> temp = MonitoredContent?.GetPathedChildrens();
            PathedContent.Clear();
            if (temp !=null)
            {
                foreach (KeyValuePair<string, MonitoredField<T>> kv in temp)
                {
                    PathedContent.Add(kv.Key, kv.Value);
                }
            }
        }

        public override void WriteTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);
            SSFullUrl TargetUrl = newPath + this.RelativeUrl;

            //we need to make sure the directory exist
            FileInfo targetInfo = new FileInfo(TargetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            if (MonitoredContent == null)
                this.ExtractMonitoredContent();
            if (MonitoredContent.Files.Count == 0)
                return;

            string result;
            if (this.MustOverwrite == true)
                result = MonitoredContent.GetJsonEquivalent()?.ToJsonString();
            else
                result = MonitoredContent.GetJsonEquivalentNoOverwrite()?.ToJsonString();
            if (result != null)
            {
                using (StreamWriter sw = File.CreateText(TargetUrl.ToString()))
                {
                    sw.Write(result);
                }
            }
        }
        
        public static void WriteJsonTo(SSFullUrl targetUrl, JsonObject content)
        {
            FileInfo targetInfo = new FileInfo(targetUrl.ToString());
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            using (StreamWriter sw = File.CreateText(targetUrl.ToString()))
            {
                string result = content.ToJsonString();
                sw.Write(result);
            }
        }



        public ReadOnlyObservableCollection<SSJson> GetJSonFiles()
        {
            ObservableCollection<SSJson> BaseCollection = new ObservableCollection<SSJson>();
            foreach (T file in base.CommonFiles)
                BaseCollection.Add(file as SSJson);
            return new ReadOnlyObservableCollection<SSJson>(BaseCollection);
        }

        //public void CopyFilesToMonitored( MonitoredField<T> monitor)
        //{
        //    monitor.ReplaceFiles(base.CommonFiles);
        //}
    }
}
