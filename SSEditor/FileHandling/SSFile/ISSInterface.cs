﻿using System;
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
        string FileName { get; }
        SSMod SourceMod { get; }
        void CopyTo(SSBaseLinkUrl NewPath);
    }

    public interface ISSJson : ISSMergable
    {
        //these contains json data, many type exist but the handling of the internal jsoncontent is the same
        string ReadValue(string JsonPath);
        List<string> ReadArray(string JsonPath);
    }

    public interface ISSMergable : ISSGenericFile
    {
        //just a qualifier for all type of file that can be merged and that should be handlable by SSFileGroup
    }
}
