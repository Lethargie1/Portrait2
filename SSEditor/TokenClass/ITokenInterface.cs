using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.TokenClass
{
    interface ISourcedToken
    {
        string Value { get; }
        SSFile Source { get; }
    }

    interface ITokenValue : ISourcedToken
    {
        void SetContent(string value, SSFile source);
    }
    
    interface ITokenAsArray : ISourcedToken
    {
        List<string> ValueArray { get; }
        void SetContent(List<string> valueList, SSFile source);
    }
}
