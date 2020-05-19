using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class ShipHull
    {
        public ShipHull(ISSGenericFile binarySource)
        {
            BinarySource = binarySource;
        }

        private ISSGenericFile BinarySource { get; set; }
        public string SourceModName { get => BinarySource.SourceMod.ModName; }
        public string FullPath { get => (BinarySource.SourceMod.ModUrl + BinarySource.RelativeUrl).ToString(); }
        public SSRelativeUrl RelativeUrl { get => BinarySource.RelativeUrl; }

        public Dictionary<string,string> ShipDataLine { get; set; }
        public string Id
        {
            get
            {
                string result = null;
                ShipDataLine?.TryGetValue("Id", out result);
                return result;
            }
        }
    }
}
