using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SSEditor.FileHandling;

namespace SSEditor.FileHandling
{
    public class SSJson : SSGeneric, ISSJson, ISSMergable
    {


        #region Properties

        JObject _JsonContent;
        public JObject JsonContent { get => _JsonContent; }

        string _ModName;
        public string ModName { get => _ModName; }

        public string JSonStatusError { get; private set; } = "";
        public bool ExtractedProperly { get; private set; } = false;

        #endregion

        #region Constructors
        public SSJson(SSMod mod, SSFullUrl fullUrl) : base (mod, fullUrl)
        {
            this.ExtractFile(fullUrl);
            SourceMod = mod;
        }
        #endregion

        


        public void ExtractFile(SSFullUrl fullUrl)
        {
            
            base.LinkRelativeUrl = new SSLinkRelativeUrl(fullUrl?.Link ?? throw new ArgumentNullException("The Url cannot be null."), fullUrl?.Relative ?? throw new ArgumentNullException("The Url cannot be null."));
            _ModName = fullUrl.Link;

            FileInfo info = new FileInfo(fullUrl.ToString());
            
            FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
            if (!info.Exists)
            {
                _JsonContent = null;
                return;
            }
            string ReadResult = File.ReadAllText(fullUrl.ToString());
            var result = Regex.Replace(ReadResult, "#.*", "");
            using (var jsonReader = new JsonTextReader(new StringReader(result)))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings { Error = HandleDeserializationError });
                ExtractedProperly = true;
                dynamic value = serializer.Deserialize(jsonReader);
                _JsonContent = value as JObject;
            }
        }

        public string ReadValue(string JsonPath)
        {
            string result;

            if (JsonContent == null)
                return null;
            JToken FoundToken = JsonContent.SelectToken(JsonPath);

            if (FoundToken==null || FoundToken.Type == JTokenType.Object || FoundToken.Count()>1)
            {
                result = null;
            }
            else
            { 
                result = FoundToken.Value<string>();
            }
            return result;
        }

        public List<string> ReadArray(string JsonPath)
        {
            List<string> result;

            if (JsonContent == null)
                return null;
            JToken FoundToken = JsonContent.SelectToken(JsonPath);


            if (FoundToken == null || !FoundToken.HasValues)
            {
                result = null;
            }
            else
            {
                result = FoundToken.Values<string>().ToList<string>();
            }


            return result;
        }

        public void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            JSonStatusError = errorArgs.ErrorContext.Error.Message;
            ExtractedProperly = false;
            errorArgs.ErrorContext.Handled = true;
        }

    }

    public class SSFaction: SSJson, ISSJson
    {
        //nothing special, its just a marker of the type of file
        public SSFaction(SSMod mod, SSFullUrl url) : base (mod, url) { }
    }
}