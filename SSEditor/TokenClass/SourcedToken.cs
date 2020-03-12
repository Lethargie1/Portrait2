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
        private string _Value;
        public string Value
        {
            get
            {
                if (_Value==null || !this.IsValid)
                {
                    return this.DefaultValue;
                }else
                {
                    return _Value;
                }
            }
            protected set => _Value = value;
        }
        public ISSFile Source { get; protected set; }
        public abstract string DefaultValue { get; }
        public abstract bool IsValid { get; }


        public SourcedToken() { }
        public SourcedToken(string value, ISSFile file)
        {
            _Value = value;
            Source = file;
        }

        public override string ToString()
        {
            return Value;
        }

    }
}
