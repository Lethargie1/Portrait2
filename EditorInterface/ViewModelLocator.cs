using EditorInterface.ViewModel;
using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface
{
    public class ViewModelLocator
    {
        private SSDirectory _DummyDirectory;
        public SSDirectory DummyDirectory
        {
            get
            {
                if (_DummyDirectory == null)
                {
                    SSDirectory dummyDirectory = new SSDirectory();
                    dummyDirectory.InstallationUrl = new SSBaseUrl("C:\\Program Files (x86)\\Fractal Softworks\\Starsector");
                    dummyDirectory.ReadMods();
                    dummyDirectory.PopulateMergedCollections();
                    _DummyDirectory = dummyDirectory;
                }
                return _DummyDirectory;

            }
        }

        public ViewModelLocator()
        {

        }
        public DirectoryViewModel DirectoryViewModel
        {
            get
            {
                var validator = new DirectoryViewModelValidator();
                var fv = new FluentModelValidator<DirectoryViewModel>(validator);
                var vm = new DirectoryViewModel(DummyDirectory, fv);
                return vm;
            }
        }

        public MonitoredValueViewModel MonitoredValueViewModel
        {
            get
            {
                JsonValue value = new JsonValue("Hegemon");
                MonitoredValue monitorValue = new MonitoredValue();
                MonitoredValueViewModel result = new MonitoredValueViewModel(monitorValue);
                monitorValue.ApplyModification(new JsonValue("hegemony2"));
                return result;
            }
        }

        public MonitoredColorViewModel MonitoredColorViewModel
        {
            get
            {
                JsonArray value = new JsonArray();
                value.Values.Add(new JsonValue(55));
                value.Values.Add(new JsonValue(45));
                value.Values.Add(new JsonValue(35));
                value.Values.Add(new JsonValue(205));
                MonitoredArrayValue monitorValue = new MonitoredArrayValue();
                MonitoredColorViewModel result = new MonitoredColorViewModel(monitorValue);
                monitorValue.Modify(MonitoredArrayValueModification.GetReplaceModification(value));
                return result;
            }
        }

        //public FactionGroupFleetCircleViewModel FactionGroupFleetCircleViewModel
        //{
        //    get
        //    {
        //        return new FactionGroupFleetCircleViewModel();
        //    }
        //}

    }
}
