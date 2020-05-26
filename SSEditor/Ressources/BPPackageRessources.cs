using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class BPPackageRessources
    {
        SSDirectory Directory { get; set; }
        ShipHullRessources ShipHullRessources { get; set; }

        public BPPackageRessources(SSDirectory directory, ShipHullRessources shipHullRessources)
        {
            Directory = directory;
            ShipHullRessources = shipHullRessources;
            RefreshRessource();
        }

        public Dictionary<string,BPPackage> AvailableBPPackages { get; private set; }

        public void RefreshRessource()
        {
            ReadDefinedBluePrint();
        }

        protected void ReadDefinedBluePrint()
        {
            Directory.GroupedFiles.TryGetValue("data\\campaign\\special_items.csv", out ISSGroup SpecialItemFile);
            if (!(SpecialItemFile is SSCsvGroup SpecialItemGroup))
                return;
            if (SpecialItemGroup.Content == null)
                SpecialItemGroup.ExtractMonitoredContent();
            var tagged = SpecialItemGroup.Content.GetTaggedLines("tags", "package_bp");
            AvailableBPPackages = tagged.ToDictionary(x=> x["id"], x => new BPPackage(x, this.Directory));

            var BaseLine = new Dictionary<string, string>() { {"plugin params","base_bp"}, { "name", "Base Blueprint" }, { "icon", "graphics/icons/cargo/blueprint_basic.png" } };
            AvailableBPPackages.Add("base_package", new BPPackage(BaseLine, this.Directory));
            foreach (KeyValuePair<string,BPPackage> kv in AvailableBPPackages)
            {
                var UsingShip = ShipHullRessources.UsableShipHull.Where(skv => skv.Value.Tags.Contains(kv.Value.BluePrintTag))
                                                                               .Select(skv => skv.Value);
                kv.Value.BluePrints.AddRange( UsingShip);
            }
        }

        public BPPackage IdToRessource(string id)
        {
            AvailableBPPackages.TryGetValue(id, out BPPackage result);
            return result;
        }
        public BPPackage TagToRessource(string tag)
        {
            return AvailableBPPackages.Select(x => x.Value).SingleOrDefault(x => x.BluePrintTag == tag); 
        }
    }

    public class BPPackage
    {
        public Dictionary<string,string> SpecialItemLine { get; set; }
        public SSDirectory Directory { get; set; }

        public BPPackage(Dictionary<string,string> line)
        {
            SpecialItemLine = line;
        }

        public BPPackage(Dictionary<string, string> line, SSDirectory directory)
        {
            SpecialItemLine = line;
            Directory = directory;
        }
        public string BluePrintTag
        {
            get
            {
                string cellContent = SpecialItemLine["plugin params"];
                if (cellContent=="")
                    { return null; }
                var splited = cellContent.Split(',');
                return splited.First();
            }
        } 

        public string IconPath
        {
            get
            {
                string cellContent = SpecialItemLine["icon"];
                if (cellContent == "")
                { return null; }
                string rela = cellContent.Replace('/', '\\');
                Directory.GroupedFiles.TryGetValue(rela, out ISSGroup binaryGroup);
                SSBinaryGroup binary = (SSBinaryGroup)binaryGroup;
                binary?.RecalculateFinal();
                return binary.FinalSourcePath;
                
            }
        }

        public string Name
        {
            get
            {
                string cellContent = SpecialItemLine["name"];
                if (cellContent == "")
                { return null; }
                return cellContent;
            }
        }
        public List<IShipHull> BluePrints { get; private set; } = new List<IShipHull>();
    }
}
