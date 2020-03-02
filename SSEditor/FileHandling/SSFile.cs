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

        JObject _JsonContent;
        public JObject JsonContent { get => _JsonContent; }

        #endregion

        #region Constructors
        public SSFile(URLRelative url)
        {
            _Url = url ?? throw new ArgumentNullException("The Url cannot be null.");
            if (!Url.IsComplete)
                throw new ArgumentNullException("The Url must be complete.");
            FileInfo info = new FileInfo(Url.FullUrl);
            _FileName = info.Name ?? throw new ArgumentNullException("The FileName cannot be null.");
            string ReadResult = File.ReadAllText(Url.FullUrl);
            var result = Regex.Replace(ReadResult, "#.*", "");
            using (var jsonReader = new JsonTextReader(new StringReader(result)))
            {
                var serializer = JsonSerializer.Create(new JsonSerializerSettings { Error = HandleDeserializationError });
                dynamic value = serializer.Deserialize(jsonReader);
                _JsonContent = value as JObject;
            }
        }
        #endregion


        public void HandleDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }

    }
}