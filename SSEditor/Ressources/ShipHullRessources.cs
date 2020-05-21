using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.Ressources
{
    public class ShipHullRessources
    {
        public static List<string> ShipRoles = new List<string>()
        {
            "combatSmallForSmallFleet",
            "combatSmall",
            "combatMedium",
            "combatLarge",
            "combatCapital",
            "combatFreighterSmall",
            "combatFreighterMedium",
            "combatFreighterLarge",
            "civilianRandom",
            "carrierSmall",
            "carrierMedium",
            "carrierLarge",
            "phaseSmall",
            "phaseMedium",
            "phaseLarge",
            "phaseCapital",
            "freighterSmall",
            "freighterMedium",
            "freighterLarge",
            "tankerSmall",
            "tankerMedium",
            "tankerLarge",
            "personnelSmall",
            "personnelMedium",
            "personnelLarge",
            "linerSmall",
            "linerMedium",
            "linerLarge",
            "tug",
            "crig",
            "utility",
            "orbitalStationLowTech",
            "battlestationLowTech"
        };
        //dependency injected object
        SSDirectory Directory { get; set; }
        VariantsRessources VariantsRessources { get; set; }

        public ShipHullRessources(SSDirectory directory, VariantsRessources variantsResources)
        {
            Directory = directory;
            VariantsRessources = variantsResources;

            RefreshRessource();

        }

        SSCsvGroup ShipDataGroup { get; set; }
        List<SSShipHullGroup> AvailableShipHullGroup { get; set; } = new List<SSShipHullGroup>();
        List<SSShipHullSkinGroup> AvailableShipHullSkinGroup { get; set; } = new List<SSShipHullSkinGroup>();
        List<String> ReferencedShipHullIdFromVariant { get; set; } = new List<string>();

        public Dictionary<string, IShipHull> UsableShipHull { get; private set; }
        public Dictionary<string, List<string>> TagToHullId { get; private set; }


        public void ExtractHullIdUsedFromVariant()
        {
            Directory.GroupedFiles.TryGetValue("data\\world\\factions\\default_ship_roles.json", out ISSGroup roleGroup);
            SSJsonGroup jsonRoleGroup = (SSJsonGroup)roleGroup;
            if (jsonRoleGroup == null)
                return;
            if (jsonRoleGroup.MonitoredContent == null)
                jsonRoleGroup.ExtractMonitoredContent();
            List<string> referencedShipVariant = new List<string>();
            foreach (string role in ShipRoles)
            {
                jsonRoleGroup.MonitoredContent.MonitoredProperties.TryGetValue(new JsonValue(role), out MonitoredField FieldVariantList);
                if (FieldVariantList is MonitoredObject shipList)
                {
                    referencedShipVariant.AddRange(shipList.MonitoredProperties.Where(x => x.Value is MonitoredValue).Select(x => x.Key.ToString()));
                }
                else
                    throw new Exception("Default ship role contains non object field with expected name");
            }
            ReferencedShipHullIdFromVariant = referencedShipVariant.Distinct().Select(x => VariantsRessources.GetHullIdFromVariantName(x)).Distinct().ToList();

        }
       
        public void ExtractAvailableHullAndSkin()
        {
            AvailableShipHullGroup = Directory.GetAndReadJsonGroupsByType<SSShipHullGroup>().ToList();
            AvailableShipHullSkinGroup = Directory.GetAndReadJsonGroupsByType<SSShipHullSkinGroup>().ToList();
        }

        public void ExtractUsableShipHull()
        {
            UsableShipHull = new Dictionary<string, IShipHull>();
            foreach (string referencedId in ReferencedShipHullIdFromVariant)
            {
                var hullGroup = AvailableShipHullGroup.FirstOrDefault(x => x.HullId.Content.Content.ToString() == referencedId);
                IShipHull localResult;
                if (hullGroup == null)
                {
                    var skinGroup = AvailableShipHullSkinGroup.First(x => x.SkinHullId.Content.Content.ToString() == referencedId);
                    string hullId = skinGroup.BaseHullId.Content.Content.ToString();
                    hullGroup = AvailableShipHullGroup.First(x => x.HullId.Content.Content.ToString() == hullId);
                    localResult = new ShipHullSkin(skinGroup, hullGroup, Directory);
                    localResult.ShipDataLine = ShipDataGroup.Content.GetLineByColumnValue("id", hullId);
                }
                else
                {
                    localResult = new ShipHull(hullGroup,Directory);
                    localResult.ShipDataLine = ShipDataGroup.Content.GetLineByColumnValue("id", referencedId);
                }

                
                UsableShipHull.Add(referencedId, localResult);
            }
        }

        public void ExtractTags()
        {
            TagToHullId = UsableShipHull.SelectMany(x => x.Value.Tags.Select(tag => new {x.Key,tag }))
                                  .GroupBy(x => x.tag, x => x.Key)
                                  .ToDictionary(g => g.Key, g => g.ToList());
        }

        public void RefreshRessource()
        {
            Directory.GroupedFiles.TryGetValue("data\\hulls\\ship_data.csv", out ISSGroup dataGroup);
            ShipDataGroup = (SSCsvGroup)dataGroup;
            ShipDataGroup?.ExtractMonitoredContent();

            ExtractHullIdUsedFromVariant();
            ExtractAvailableHullAndSkin();
            ExtractUsableShipHull();
            ExtractTags();
        }

        public List<IShipHull> MakeShipHullListFromTagAndId(IEnumerable<string> tags , IEnumerable<string> ids)
        {
            List<string> idResult = new List<string>();
            if (tags != null)
                foreach (string tag in tags)
                {
                    TagToHullId.TryGetValue(tag, out List<String> tagedIds);
                    if (tagedIds != null)
                        idResult.AddRange(tagedIds);
                }
            if (ids != null)
                idResult.AddRange(ids);
            return idResult.Distinct().Select(id => UsableShipHull[id]).ToList();
        }
    }
}
