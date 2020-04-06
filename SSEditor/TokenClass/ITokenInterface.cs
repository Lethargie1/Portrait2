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
        ISSJson Source { get; }
    }

    interface ITokenValue : ISourcedToken
    {
        void SetContent(string value, ISSJson source);
    }
    
    interface ITokenAsArray : ISourcedToken
    {
        List<int> ValueArray { get; }
        void SetContent(List<string> valueList, ISSJson source);
    }
}
