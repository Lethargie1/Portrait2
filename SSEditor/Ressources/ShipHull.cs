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
        public ShipHull(SSShipHullGroup groupSource)
        {
            GroupSource = groupSource;
        }

        private SSShipHullGroup GroupSource { get; set; }

        public SSRelativeUrl RelativeUrl { get => GroupSource.RelativeUrl; }

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
