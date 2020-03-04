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
    public class SSFile
    {


        #region Properties
        string _FileName;
        public string FileName { get => _FileName; }

        URLRelative _Url;
        public URLRelative Url { get => _Url; }
        public SSLinkRelativeUrl RelativePath { get; private set; }

        JObject _JsonContent;
        public JObject JsonContent { get => _JsonContent; }

        string _ModName;
        public string ModName { get => _ModName; }
        #endregion

        #region Constructors
        public SSFile(SSFullUrl fullUrl)
        {
            this.ExtractFile(fullUrl);
        }
        #endregion

        public void ExtractFile(SSFullUrl fullUrl)
        {
            
            RelativePath = new SSLinkRelativeUrl(fullUrl?.Link ?? throw new ArgumentNullException("The Url cannot be null."), fullUrl?.Relative ?? throw new ArgumentNullException("The Url cannot be null."));
            _ModName = fullUrl.Link;

            FileInfo info = new FileInfo(fullUrl.ToString());
            _FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
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
}