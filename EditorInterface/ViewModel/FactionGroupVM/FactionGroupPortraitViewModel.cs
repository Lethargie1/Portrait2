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
    public class FactionGroupPortraitViewModel : Conductor<PortraitsRessourcesViewModel>
    {
        public string LongDisplayName { get; set; }
        public PortraitsRessourcesViewModel PortraitsRessourcesVM { get; private set; }
        public MonitoredArray TargetMonitor { get; private set; }
        public ObservableCollection<JsonToken> MonitoredArray { get => TargetMonitor?.ContentArray; }

        public JsonToken SelectedPortraitArray { get; set; }
        public FactionGroupPortraitViewModel(MonitoredArray targetMonitor, PortraitsRessourcesViewModel portraitsRessourcesVM)
        {
            TargetMonitor = targetMonitor;
            PortraitsRessourcesVM = portraitsRessourcesVM;
            ActivateItem(PortraitsRessourcesVM);
        }
        protected override void OnViewLoaded()
        {
            base.OnViewLoaded();
            ActivateItem(PortraitsRessourcesVM);
        }
        //public bool CanAddPortrait
        //{
        //    get { return SelectedPortraitRessource != null ? true : false; }
        //}

        public void AddPortrait()
        {
            TargetMonitor?.Modify(MonitoredArrayModification.GetAddModification(new JsonValue(PortraitsRessourcesVM.SelectedPortraitRessource.RelativeUrl.SSStyleString)));
        }

        public void ClearPortrait()
        {
            TargetMonitor?.Modify(MonitoredArrayModification.GetClearModification());
        }

        public void RemovePortrait()
        {
            if (SelectedPortraitArray != null)
                TargetMonitor?.Modify(MonitoredArrayModification.GetRemoveModification(SelectedPortraitArray));
        }

        public void ResetPortrait()
        {
            TargetMonitor?.ResetModification();
        }
    }
}
