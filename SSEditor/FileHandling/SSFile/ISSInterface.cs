using FVJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.FileHandling
{
    public interface ISSGenericFile
    {
        //all possible file should fit in a generic
        SSLinkRelativeUrl LinkRelativeUrl { get; }
        SSRelativeUrl RelativeUrl { get; }
        string FileName { get; }
        SSMod SourceMod { get; }
        void CopyTo(SSBaseLinkUrl NewPath);
    }

    public interface ISSJson : ISSMergable, ISSWritable
    {
        //these contains json data, many type exist but the handling of the internal jsoncontent is the same
        bool ExtractedProperly { get;}
        JsonToken ReadToken(string path);
    }

    public interface ISSMergable : ISSGenericFile
    {
        //just a qualifier for all type of file that can be merged and that should be handlable by SSFileGroup
    }
}
