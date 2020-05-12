
using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSEditor.MonitoringField
{
    public class MonitoredValue: MonitoredField 
    {
        public JsonToken.TokenType GoalType { get; private set; }
        public void SetGoal(JsonToken.TokenType type)
        {
            GoalType = type;
            GoalExternalySet = true;
        }

        private bool GoalExternalySet { get; set; } = false;
        private JsonValue _Content;
        public JsonValue Content { get => _Content; private set=>SetAndNotify(ref _Content,value); }
        public override bool Modified { get => this.IsModified(); }


        public MonitoredValueModification Modification { get; set; }
        

        public bool HasMultipleSourceFile { get; private set; } = false;
        public MonitoredValue() : base()
        {
            Content = null;
        }
        public MonitoredValue(JsonValue content)
        {
            Content = content;
            GoalType = content.Type;
        }

        public void Modify(MonitoredValueModification mod)
        {
            if (mod != null && mod.Content.Type != this.GoalType)
                throw new ArgumentException($"Wrong type of modification ({Enum.GetName(typeof(JsonToken.TokenType), mod.Content.Type)}) for monitored Value <{this.FieldPath}> with goaltype ({Enum.GetName(typeof(JsonToken.TokenType), this.GoalType)})");
            Modification = mod;
            NotifyOfPropertyChange(nameof(Modified));
            ApplyModification();
        }
        public void ApplyModification()
        {
            if (Modification == null)
                return;
            if (Modification.ModType == MonitoredValueModification.ModificationType.Unset)
                Content = null;
            else if (Modification.ModType == MonitoredValueModification.ModificationType.Replace)
                Content = Modification.Content;
        }
        public void Reset()
        {
            if (Modification != null)
                Modification = null;
            NotifyOfPropertyChange(nameof(Modified));
            Resolve();
        }
        override public void Resolve()
        {
            if (FieldPath != null)
            {
                var ModValuePair = from f in Files
                                   where f.Fields.ContainsKey(FieldPath) == true
                                   select new { modName = f.SourceMod.ModName , value = f.Fields[FieldPath], file = f};
                var Ordered = from p in ModValuePair
                              orderby p.modName
                              select new { p.value, p.file } ;
                JsonToken TokenResult = Ordered.FirstOrDefault()?.value;
                if (Ordered.Count() >1)
                {
                    HasMultipleSourceFile = true;
                    NotifyOfPropertyChange(nameof(HasMultipleSourceFile));
                }
                if (Modification != null)
                {
                    ApplyModification();
                }
                else if (TokenResult is JsonValue value)
                {
                    Content = value;
                    if (!GoalExternalySet)
                        GoalType = value.Type;
                    else
                    {
                        if (value.Type != GoalType)
                            throw new InvalidOperationException($"Cannot put value of type {value.Type.ToString()} in Monitored with goal {GoalType.ToString()}");
                    }
                }
                else if (TokenResult == null)
                    Content = null;
                else
                    throw new ArgumentException("Path leads to wrong type of token");

                ISSJson FileResult = Ordered.FirstOrDefault()?.file;
            }
        }
        public override JsonToken GetJsonEquivalent()
        {
            return Content;
        }
        public override JsonToken GetJsonEquivalentNoOverwrite()
        {
            return Modification.Content;
        }

        public override bool IsModified()
        {
            return Modification != null;
        }
        public override bool RequiresOverwrite()
        {
            if (Modification != null && HasMultipleSourceFile)
                return true;
            else
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
            return base.FieldPath+": "+ Content.ToString();
        }

        public override Dictionary<string, MonitoredField> GetPathedChildrens()
        {
            return new Dictionary<String, MonitoredField>() { { "", this } };
        }
    }
}
