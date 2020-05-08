
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredArrayValue : MonitoredField
    {
        public JsonArray ContentArray { get; } = new JsonArray();
        public override bool Modified { get => this.IsModified(); }


        public override void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.Fields.ContainsKey(FieldPath) == true
                                   select new { modName = f.SourceMod.ModName, value = f.Fields[FieldPath], file = f };
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file };
                JsonToken ValueResult = Ordered.FirstOrDefault()?.value;
                if (ValueResult is JsonArray jArray)
                {
                    if (jArray.Values.Count == 4)
                    {
                        ContentArray.Values.Clear();
                        foreach (JsonToken token in jArray.Values)
                            ContentArray.Values.Add(token);
                    }
                }
                ISSJson FileResult = Ordered.FirstOrDefault()?.file;
            }
        }
        public override JsonToken GetJsonEquivalent()
        {
            return ContentArray;
        }
        public override JsonToken GetJsonEquivalentNoOverwrite()
        {
            return null;
        }

        public override bool IsModified()
        {
            return false;
        }

        public override bool RequiresOverwrite()
        {
            return false;
        }
        protected override void ResolveAdd(ISSJson file)
        {
            Resolve();
        }

        protected override void ResolveRemove(ISSJson file)
        {
            Resolve();
        }

        public override string ToString()
        {
            return base.FieldPath + ": " + ContentArray.ToString();
        }

        public override Dictionary<string, MonitoredField> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField>() { { "", this } };
        }
    }
}
