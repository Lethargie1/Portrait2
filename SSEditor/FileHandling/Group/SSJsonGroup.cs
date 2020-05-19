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
using Stylet;

namespace SSEditor.FileHandling
{
    public class SSJsonGroup : SSJsonGroup<SSJson>
    {

    }
    public class SSJsonGroup<T> : SSGroup<T>, ISSJsonGroup where T: ISSJson
    {

        public override bool MustOverwrite
        {
            get => base.ForceOverwrite ? true : MonitoredContent.RequiresOverwrite();      
        }


        public MonitoredObject MonitoredContent { get; set; } = null;
        public Dictionary<string, MonitoredField> PathedContent { get; private set; } = new Dictionary<string, MonitoredField>();

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
                if (MustOverwrite == true)
                    return true;
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
            MonitoredObject rootMonitoredObject = new MonitoredObject() { FieldPath = "" };
            
            T core = (from T file in base.CommonFiles
                      where file.SourceMod.CurrentType == ModType.Core
                      select file).SingleOrDefault();
            List<ISSJson> modAdded = (from T file in base.CommonFiles
                                where file.SourceMod.CurrentType == ModType.Mod
                                select file as ISSJson).ToList();
            ObservableCollection<ISSJson> fileUsed = new ObservableCollection<ISSJson>(modAdded);
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
            Dictionary<string, MonitoredField> temp = MonitoredContent?.GetPathedChildrens();
            PathedContent.Clear();
            if (temp !=null)
            {
                foreach (KeyValuePair<string, MonitoredField> kv in temp)
                {
                    PathedContent.Add(kv.Key, kv.Value);
                }
            }
        }

        public override void WriteTo(SSBaseLinkUrl newPath)
        {
            if (!this.WillCreateFile)
                return;
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

        public ICollection<GroupModification> GetModifications()
        {
            var Unsourced = this.MonitoredContent.GetModification();
            var Sourced = Unsourced.Select(x => { x.GroupUrl = this.RelativeUrl; return x; });
            return Sourced.ToList();
        }

        protected G AttachOneAttribute<G>(string path, JsonToken.TokenType goalType = JsonToken.TokenType.String) where G : MonitoredField, new()
        {
            MonitoredField extracted;
            if (PathedContent.TryGetValue(path, out extracted))
            {
                if (extracted is G typed)
                {
                    typed.Bind(x => x.Modified, (sender, arg) => SubPropertyModified(sender, arg));
                    if (typed is MonitoredValue mv)
                        mv.SetGoal(goalType);
                    return typed;
                }
                else
                    throw new InvalidOperationException($"Existing field {path} in file {this.RelativeUrl.ToString()} is different type than {typeof(G)}");
            }
            else
            {
                extracted = new G();
                if (extracted is MonitoredValue mv)
                    mv.SetGoal(goalType);
                MonitoredContent.AddSubMonitor(path, extracted);
                extracted.Bind(x => x.Modified, (sender, arg) => SubPropertyModified(sender, arg));
                PopulatePathedContent();
                return extracted as G;
            }
        }
        public void SubPropertyModified(object sender, EventArgs e)
        {
            NotifyOfPropertyChange(nameof(IsModified));
            NotifyOfPropertyChange(nameof(MustOverwrite));
        }
        //public void CopyFilesToMonitored( MonitoredField<T> monitor)
        //{
        //    monitor.ReplaceFiles(base.CommonFiles);
        //}
    }
}
