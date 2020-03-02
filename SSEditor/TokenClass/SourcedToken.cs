using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.TokenClass
{
    abstract class SourcedToken
    {
        public string Value { get; set; }
        public SSFile Source { get; set; } 
        public abstract bool CheckValidity();

        public SourcedToken() { }
        public SourcedToken(string value, SSFile file)
        {
            Value = value;
            Source = file;
        }
    }
}
