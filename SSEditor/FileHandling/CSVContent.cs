using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace SSEditor.FileHandling
{
    public class CSVContent
    {
        public List<string> Headers { get; set; }
        public List<Dictionary<string,string>> Content { get; private set; }

        public CSVContent() { }


        public List<Dictionary<string,string>> GetTaggedLines(string tagColumnHead, string tag)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            if (!Headers.Contains(tagColumnHead))
                throw new ArgumentException($"Column header name: {tagColumnHead} does not exist in current Content");
            foreach (Dictionary<string,string> line in Content)
            {
                line.TryGetValue(tagColumnHead, out string csvCell);
                if (csvCell == "")
                    continue;
                var CellContent = csvCell.Split(',').ToList();
                if (CellContent.Contains(tag))
                    result.Add(line);
            }

            return result;
        }

        public Dictionary<string,string> GetLineByColumnValue(string ColumnHead, string ColumnValue)
        {
            return Content.Where(x => x[ColumnHead] == ColumnValue).SingleOrDefault();
        }

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
                    Dictionary<string, string> newLine = new Dictionary<string, string>();
                    foreach (string head in result.Headers)
                    {
                        if (line.ContainsKey(head))
                            newLine.Add(head, line[head]);
                        else
                            newLine.Add(head, "");
                    }

                    result.Content.Add(newLine);
                }
            }


            return result;
        }


    }
}
