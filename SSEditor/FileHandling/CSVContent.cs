using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;


namespace SSEditor.FileHandling
{
    public class CSVContent
    {
        public List<string> Headers { get; set; }
        public List<Dictionary<string,string>> Content { get; private set; }

        public CSVContent() { }

        public static CSVContent ExtractFromText(TextReader source)
        {
            Factory csvFactory = new Factory();
            IParser parser = csvFactory.CreateParser(source, CultureInfo.InvariantCulture);
            parser.Configuration.AllowComments = true;
            parser.Configuration.BadDataFound = null;

            CSVContent result = new CSVContent();
            result.Headers = parser.Read().ToList<string>();
            string[] line = parser.Read();
            int count = result.Headers.Count;
            List<Dictionary<string, string>> contentToSet = new List<Dictionary<string, string>>();
            while (line != null)
            {
                if (line.Count() != count)
                    throw new FormatException("Csv line has different number of element then header");

                //now i need to write my line to the local array
                Dictionary<string, string> linecontent = new Dictionary<string, string>();
                bool isEmpty = true;
                for (int index =0; index < count; index++)
                {
                    if (isEmpty == false || line[index] != "")
                        isEmpty = false;
                    linecontent.Add(result.Headers[index], line[index]);
                }
                if (!isEmpty)
                    contentToSet.Add(linecontent);
                line = parser.Read();
            }
            result.Content = contentToSet;
            return result;
        }


        public static CSVContent Merge(IEnumerable<CSVContent> ToMerge )
        {
            CSVContent result = new CSVContent();
            result.Headers = ToMerge.SelectMany(x => x.Headers)
                                               .Distinct().ToList<string>();
            result.Content = new List<Dictionary<string, string>>();
            foreach(CSVContent csv in ToMerge)
            {
                foreach (Dictionary<string,string> line in csv.Content)
                {
                    var MissingKeysValue = result.Headers.Where(x => !line.ContainsKey(x))
                                                         .Select(x => new KeyValuePair<string, string>(x, ""));
                    foreach (KeyValuePair<string,string> kv in MissingKeysValue)
                    { line.Add(kv.Key, kv.Value); }
                    result.Content.Add(line);
                }
            }


            return result;
        }
    }
}
