
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
        private JsonArray _ContentArray;
        public JsonArray ContentArray { get => _ContentArray; set => SetAndNotify(ref _ContentArray, value); }

        public override bool Modified { get => this.IsModified(); }

        private JsonArray _Modification;
        public JsonArray Modification
        {
            get => _Modification;
            set
            {

                if (value != null && value.Values.Count != 4)
                    throw new ArgumentException("Cant set an array with the wrong count");
                SetAndNotify(ref _Modification, value);
                NotifyOfPropertyChange(nameof(Modified));
            }

        }
        public void ApplyModification(JsonArray mod)
        {
            Modification = mod;
            ContentArray = mod;
        }
        public void Reset()
        {
            if (Modification != null)
                Modification = null;
            Resolve();
        }

        public bool HasMultipleSourceFile { get; private set; } = false;

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
                if (Ordered.Count() > 1)
                    HasMultipleSourceFile = true;
                else
                    HasMultipleSourceFile = false;
                NotifyOfPropertyChange(nameof(HasMultipleSourceFile));
                if (Modification != null)
                    ValueResult = Modification;

                if (ValueResult is JsonArray value)
                    ContentArray = value;
                else if (ValueResult == null)
                    ContentArray = null;
                else
                    throw new ArgumentException("Path leads to wrong type of token");


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
            return Modification != null; 
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
