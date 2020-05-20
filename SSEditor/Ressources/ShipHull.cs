using FVJson;
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
        string HullName { get; }
        string SpriteFullPath { get; }
        List<String> Tags { get;  }

        Dictionary<string, string> ShipDataLine { get; set; }
        
    }

    public class ShipHull : IShipHull
    {
        public ShipHull(SSShipHullGroup groupSource, SSDirectory directory)
        {
            GroupSource = groupSource;
            Directory = directory;
        }

        private SSShipHullGroup GroupSource { get; set; }
        private SSDirectory Directory { get; set; }

        public SSRelativeUrl RelativeUrl { get => GroupSource.RelativeUrl; }

        public Dictionary<string,string> ShipDataLine { get; set; }
        public string Id { get { return GroupSource.HullId?.Content?.ToString(); } }
        public string HullName { get { return GroupSource.HullName?.Content?.ToString();  } }
        public string SpriteFullPath
        {
            get
            {
                string relative = GroupSource.SpriteName?.Content?.ToString();
                Directory.GroupedFiles.TryGetValue(relative.Replace('/', '\\'), out ISSGroup source);
                if (source is SSBinaryGroup group)
                {
                    group.RecalculateFinal();
                    return group.FinalSourcePath;
                }
                else
                    return null;
            }
        }

        public List<String> Tags
        {
            get
            {
                ShipDataLine.TryGetValue("tags", out string csvCell);
                if (csvCell == "")
                    return new List<string>();
                return csvCell.Split(',').ToList();
            }
        }
    }

    public class ShipHullSkin : IShipHull
    {
        private SSShipHullGroup BaseHullGroup { get; set; }
        private SSShipHullSkinGroup GroupSource { get; set; }
        private SSDirectory Directory { get; set; }

        public ShipHullSkin(SSShipHullSkinGroup groupSource, SSShipHullGroup baseHullGroup, SSDirectory directory)
        {
            GroupSource = groupSource;
            BaseHullGroup = baseHullGroup;
            Directory = directory;
        }

        public Dictionary<string, string> ShipDataLine { get; set; }

        public SSRelativeUrl RelativeUrl { get => GroupSource.RelativeUrl; }
        public string Id { get => GroupSource?.SkinHullId?.Content?.ToString(); }
        public string HullName { get { return GroupSource.HullName?.Content?.ToString(); } }
        public string SpriteFullPath
        {
            get
            {
                string relative = GroupSource.SpriteName?.Content?.ToString();
                Directory.GroupedFiles.TryGetValue(relative.Replace('/', '\\'), out ISSGroup source);
                if (source is SSBinaryGroup group)
                {
                    group.RecalculateFinal();
                    return group.FinalSourcePath;
                }
                else
                    return null;
            }
        }

        public List<String> Tags
        {
            get
            {
                if (GroupSource.Tags.ContentArray.Count() == 0)
                {
                    ShipDataLine.TryGetValue("tags", out string csvCell);
                    if (csvCell == "")
                        return new List<string>();
                    return csvCell.Split(',').ToList();
                }
                else
                {
                    return GroupSource.Tags.ContentArray.Select(x =>
                    {
                        var value = (JsonValue)x;
                        return value.Content.ToString();
                    }).ToList();
                }
            }
        }

    }
}
