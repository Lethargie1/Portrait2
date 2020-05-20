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
        SSDirectory Directory { get; set; }
        VariantsRessources VariantsRessources { get; set; }

        public ShipHullRessources(SSDirectory directory)
        {
            Directory = directory;
            VariantsRessources = new VariantsRessources(Directory);
            
            Directory.GroupedFiles.TryGetValue("data\\hulls\\ship_data.csv", out ISSGroup dataGroup);
            SSCsvGroup ShipDataGroup = (SSCsvGroup)dataGroup;
            ShipDataGroup.ExtractMonitoredContent();

            ExtractHullIdUsedFromVariant();


            var BaseHullFiles = (from KeyValuePair<string, ISSGroup> kv in Directory.GroupedFiles
                                 where kv.Value is SSShipHullGroup
                                 select kv.Value).Select(g =>
                                 {
                                     SSShipHullGroup f = (SSShipHullGroup)g;
                                     if (f.MonitoredContent == null)
                                         f.ExtractMonitoredContent();
                                     return f;
                                 });
            //var a = HullFiles.GroupBy(x => x.HullId.Content.ToString())
            //.Where(x => x.Count() > 1);
            //var dic = HullFiles.ToDictionary(x => x.HullId.ToString());
            //var ShipHull = referencedShipHull.Select(x =>
            //{
            //    try
            //    { var a = HullFiles.First(g => g.HullId.Content.ToString() == x); }
            //    catch(Exception e)
            //    {

            //    }
            //    ShipHull localHull = new ShipHull(HullFiles.First(g => g.HullId.Content.ToString() == x));
            //    localHull.ShipDataLine = ShipDataGroup.Content.GetLineByColumnValue("id", x);
            //    return localHull;
            //});

            //ShipHull.ToList();
            //ok, using the referenced ship hull, i need to pull the data from ship data and myself a samwich
        }


        public List<String> referencedShipHullIdFromVariant { get; set; }
        public void ExtractHullIdUsedFromVariant()
        {
            Directory.GroupedFiles.TryGetValue("data\\world\\factions\\default_ship_roles.json", out ISSGroup roleGroup);
            SSJsonGroup jsonRoleGroup = (SSJsonGroup)roleGroup;
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
            referencedShipHullIdFromVariant = referencedShipVariant.Distinct().Select(x => VariantsRessources.GetHullIdFromVariantName(x)).Distinct().ToList();

        }
    }
}
