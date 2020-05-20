using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public interface IShipHull
    {
        SSRelativeUrl RelativeUrl { get; }
        string Id { get; }

        Dictionary<string, string> ShipDataLine { get; set; }
        
    }

    public class ShipHull : IShipHull
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
                return GroupSource.HullId?.Content?.ToString();
            }
        }
    }

    public class ShipHullSkin : IShipHull
    {
        private SSShipHullGroup BaseHullGroup { get; set; }
        private SSShipHullSkinGroup GroupSource { get; set; }

        public ShipHullSkin(SSShipHullSkinGroup groupSource, SSShipHullGroup baseHullGroup)
        {
            GroupSource = groupSource;
            BaseHullGroup = baseHullGroup;
        }

        public SSRelativeUrl RelativeUrl { get => GroupSource.RelativeUrl; }
        public string Id { get => GroupSource?.SkinHullId?.Content?.ToString(); }

        public Dictionary<string, string> ShipDataLine { get; set; }
    }
}
