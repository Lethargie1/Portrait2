using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoredTokenClass
{
    abstract class MonitoredField
    {
        public List<string> FieldPath { get; set; }
        public string FieldName { get; set; }

        public void Resolve(SSFile file)
        {
            Resolve(new List<SSFile> { file });
        }
        abstract public void Resolve(List<SSFile> fileList);
    }
}
