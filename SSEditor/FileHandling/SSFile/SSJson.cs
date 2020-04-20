
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


        #region Properties

        JsonObject _JsonContent;
        public JsonObject JsonContent
        {
            get
            {
                if (_JsonContent == null)
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
                    this.ExtractFile();
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
        public SSJson(SSMod mod, SSFullUrl fullUrl) : base(mod, fullUrl)
        {
            //this.ExtractFile(fullUrl);
            SourceMod = mod;
        }
        #endregion




        public void ExtractFile()
        {
            SSFullUrl fullUrl = base.BaseUrl + base.LinkRelativeUrl;
            base.LinkRelativeUrl = new SSLinkRelativeUrl(fullUrl?.Link ?? throw new ArgumentNullException("The Url cannot be null."), fullUrl?.Relative ?? throw new ArgumentNullException("The Url cannot be null."));
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
            
        }
        public void RefreshFields()
        {
            Fields = _JsonContent?.GetPathedChildrens() ?? new Dictionary<string, JsonToken>();
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
    }

    public class SSFaction : SSJson
    {
        public SSFaction(SSMod mod, SSFullUrl fullUrl) : base(mod, fullUrl)
        { }
        
    }

}

