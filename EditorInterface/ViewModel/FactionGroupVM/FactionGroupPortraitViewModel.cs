using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using SSEditor.Ressources;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionGroupPortraitViewModel : Screen
    {
        public PortraitsRessources PortraitsRessources { get; private set; }
        public MonitoredArray<SSFaction> TargetMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TargetMonitor?.ContentArray; }
        public List<Portraits> AvailablePortraits
        {
            get
            {
                return PortraitsRessources.RessourceCorrespondance.Select(kv => kv.Value).ToList();
            }
        }

        private Portraits _SelectedPortraitRessource;
        public Portraits SelectedPortraitRessource { get => _SelectedPortraitRessource; set { _SelectedPortraitRessource = value; NotifyOfPropertyChange(nameof(CanAddPortrait)); } }

        public JsonToken SelectedPortraitArray { get; set; }
        public FactionGroupPortraitViewModel(MonitoredArray<SSFaction> targetMonitor, PortraitsRessources portraitsRessources)
        {
            TargetMonitor = targetMonitor;
            PortraitsRessources = portraitsRessources;

        }

        public bool CanAddPortrait
        {
            get { return SelectedPortraitRessource != null ? true : false; }
        }

        public void AddPortrait()
        {
            TargetMonitor.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(SelectedPortraitRessource.RelativeUrl.SSStyleString)));
        }

        public void ClearPortrait()
        {
            TargetMonitor?.Modify(MonitoredArrayModification.GetClearModification());
        }
    }
}
