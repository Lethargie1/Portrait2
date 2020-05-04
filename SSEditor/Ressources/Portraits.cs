using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SSEditor.Ressources
{
    public class Portraits
    {
        private ISSGenericFile BinarySource { get; set; }
        public string SourceModName { get => BinarySource.SourceMod.ModName; }

        public object GroupingObject { get => BinarySource.SourceMod; }
        public string FullPath { get => (BinarySource.SourceMod.ModUrl + BinarySource.RelativeUrl).ToString(); }

        public SSRelativeUrl RelativeUrl { get => BinarySource.RelativeUrl; }

        public Portraits(ISSGenericFile binarySource)
        {
            BinarySource = binarySource;
        }
    }
}

    
