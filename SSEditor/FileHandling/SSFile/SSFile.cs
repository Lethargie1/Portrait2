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
    public class SSFile :ISSGenericFile
    {


        #region Properties
        public string FileName { get; private set; }
        public SSLinkRelativeUrl LinkRelativeUrl { get; private set; }
        public SSMod SourceMod { get; private set; }

        JObject _JsonContent;
        public JObject JsonContent { get => _JsonContent; }

        string _ModName;
        public string ModName { get => _ModName; }
        #endregion

        #region Constructors
        public SSFile() { }
        public SSFile(SSFullUrl fullUrl)
        {
            this.ExtractFile(fullUrl);
        }
        public SSFile(SSMod mod, SSFullUrl fullUrl) : this(fullUrl)
        {
            SourceMod = mod;
        }
        #endregion

        public override string ToString()
        {
            return FileName+" from "+ModName;
        }


        public void ExtractFile(SSFullUrl fullUrl)
        {
            
            LinkRelativeUrl = new SSLinkRelativeUrl(fullUrl?.Link ?? throw new ArgumentNullException("The Url cannot be null."), fullUrl?.Relative ?? throw new ArgumentNullException("The Url cannot be null."));
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
                dynamic value = serializer.Deserialize(jsonReader);
                _JsonContent = value as JObject;
            }
        }

        public string ReadValue(List<string> JsonPath)
        {
            string result;

            if (JsonContent == null)
                return null;
            JObject localDepth = JsonContent;
            JToken FoundToken = new JValue(null as string);
            foreach (string field in JsonPath)
            {
                if (localDepth.TryGetValue(field, out FoundToken))
                {
                    if (FoundToken.Type == JTokenType.Object)
                    {
                        localDepth = FoundToken as JObject;
                    }
                }
            }
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

        public List<string> ReadArray(List<string> JsonPath)
        {
            List<string> result;

            if (JsonContent == null)
                return null;
            JObject localDepth = JsonContent;
            JToken FoundToken = new JValue(null as string);
            foreach (string field in JsonPath)
            {
                if (localDepth.TryGetValue(field, out FoundToken))
                {
                    if (FoundToken.Type == JTokenType.Object)
                    {
                        localDepth = FoundToken as JObject;
                    }
                }
            }

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
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

    }

    public class SSFactionFile: SSFile
    {
        //nothing special, its just a marker of the type of file
    }
}