using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class Portraits
    {
        private ISSGenericFile BinarySource { get; set; }
        public string SourceModName { get => BinarySource.SourceMod.ModName; }
        public string FullPath { get => (BinarySource.SourceMod.ModUrl + BinarySource.RelativeUrl).ToString(); }

        public Portraits(ISSGenericFile binarySource)
        {
            BinarySource = binarySource;
        }
    }
}
