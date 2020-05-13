
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSEditor.FileHandling;
using FVJson;

namespace SSEditor.FileHandling
{
    public class SSJson : SSGeneric, ISSJson, ISSMergable
    {
        public bool MustOverwrite { get; set; } = false;
        public enum JsonFileType { Extracted, NotExtrated};
        public bool WillCreateFile { get; } = true;
        #region Properties
        public JsonFileType JsonType { get; set; } = JsonFileType.Extracted;
        JsonObject _JsonContent;
        public JsonObject JsonContent
        {
            get
            {
                if (_JsonContent == null && this.JsonType == JsonFileType.Extracted)
                {
                    this.ExtractFile();
                    this.RefreshFields();
                }
                return _JsonContent;
            }
            set
            {
                _JsonContent = value;
                this.RefreshFields();
            }
        }
        Dictionary<string, JsonToken> _Fields;
        public Dictionary<string,JsonToken> Fields
        {
            get
            {
                if (_Fields == null)
                {
                    JsonObject test = this.JsonContent;
                    this.RefreshFields();
                }
                return _Fields;
            }
            private set { _Fields = value; }
        }


        string _ModName;
        public string ModName { get => _ModName; }

        public string JSonStatusError { get; private set; } = "";
        public bool ExtractedProperly { get; private set; } = false;
        
        #endregion

        #region Constructors
        public SSJson(ISSMod mod, SSRelativeUrl Url) : base(mod, Url)
        {
            //this.ExtractFile(fullUrl);
            SourceMod = mod;
        }
        #endregion




        public void ExtractFile()
        {
            SSFullUrl fullUrl = base.SourceMod.ModUrl + this.RelativeUrl;
            _ModName = base.SourceMod.ModName;

            FileInfo info = new FileInfo(fullUrl.ToString());

            FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
            if (!info.Exists)
            {
                _JsonContent = null;
                return;
            }
            string ReadResult = File.ReadAllText(fullUrl.ToString());
            var result = Regex.Replace(ReadResult, "#.*", "");
            using (StringReader reader = new StringReader(result))
            {
                JsonReader jreader = new JsonReader(reader);
                JsonToken read = jreader.UnJson();
                _JsonContent = read as JsonObject;
            }
            if (_JsonContent == null)
                return;
            
        }
        public void RefreshFields()
        {
            Fields = _JsonContent?.GetPathedChildrens() ?? throw new InvalidOperationException("Attempt to get field of empty SSJson File");
        }

        public JsonToken ReadToken(string JsonPath)
        {
            JsonToken result;

            if (JsonContent == null)
                return null;
            if (JsonContent.ExistPath(JsonPath))
                result = JsonContent.SelectToken(JsonPath);
            else
                return null;
            return result;
        }

        public void WriteTo(SSBaseLinkUrl newPath)
        {
            SSBaseUrl InstallationUrl = new SSBaseUrl(newPath.Base);

            SSFullUrl targetUrl = newPath + this.RelativeUrl;

            FileInfo targetInfo = new FileInfo(targetUrl.ToString());
            if (targetInfo.Exists)
            {
                targetInfo = new FileInfo(targetUrl.ToString() + SourceMod.ModName);
            }
            DirectoryInfo targetDir = targetInfo.Directory;
            if (!targetDir.Exists)
            {
                targetDir.Create();
            }
            using (StreamWriter sw = File.CreateText(targetUrl.ToString()))
            {
                string result = JsonContent.ToJsonString();
                sw.Write(result);
            }
        }
    }

    public class SSFaction : SSJson
    {
        public SSFaction(ISSMod mod, SSRelativeUrl Url) : base(mod, Url)
        { }
        
    }

}

