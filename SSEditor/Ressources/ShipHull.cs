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
        string HullSize { get; }

        string Tech { get; }
        string Designation { get; }
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
        //read from the .ship file
        public string Id { get { return GroupSource.HullId?.Content?.ToString(); } }
        public string HullName { get { return GroupSource.HullName?.Content?.ToString();  } }
        public string HullSize { get { return GroupSource.HullSize?.Content?.ToString(); } }
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

        //stuff that requires the shipdataline from the csv
        public string Tech
        {
            get
            {
                ShipDataLine.TryGetValue("tech/manufacturer", out string tech);
                if (tech == null || tech == "")
                    tech = "none set";
                return tech;
            }
        }
        public string Designation
        {
            get
            {
                ShipDataLine.TryGetValue("designation", out string designation);
                if (designation == null || designation == "")
                    designation = "none set";
                return designation;
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
        public string HullSize { get { return BaseHullGroup.HullSize?.Content?.ToString(); } }

        //stuff that requires the shipdataline from the csv
        public string Tech
        {
            get
            {
                string tech = GroupSource?.Tech?.Content?.ToString();
                if (tech != null)
                    return tech;
                ShipDataLine.TryGetValue("tech/manufacturer", out tech);
                if (tech == null || tech == "")
                    tech = "none set";
                return tech;
            }
        }
        public string Designation
        {
            get
            {
                ShipDataLine.TryGetValue("designation", out string designation);
                if (designation == null || designation == "")
                    designation = "none set";
                return designation;
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
