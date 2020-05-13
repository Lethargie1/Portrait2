using EditorInterface.ViewModel;
using FVJson;
using SSEditor.FileHandling;
using SSEditor.MonitoringField;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private JsonObject _DummyFileContent;
        public JsonObject DummyFileContent
        {
            get
            {
                if (_DummyFileContent== null)
                {
                    string ReadResult = File.ReadAllText("TestObject/hegemony.faction");
                    var result = Regex.Replace(ReadResult, "#.*", "");
                    using (StringReader reader = new StringReader(result))
                    {
                        JsonReader jreader = new JsonReader(reader);
                        JsonToken read = jreader.UnJson();
                        _DummyFileContent = read as JsonObject;
                    }
                }
                return _DummyFileContent;
            }
        }

        private SSFaction _DummyFaction;
        public SSFaction DummyFaction
        {
            get
            {
                if (_DummyFaction == null)
                {
                    _DummyFaction = new SSFaction(DummyMod, new SSRelativeUrl("hegemony.faction"));
                    _DummyFaction.JsonType = SSJson.JsonFileType.NotExtrated;
                    _DummyFaction.JsonContent = DummyFileContent;
                }
                return _DummyFaction;
            }
        }

        private ISSMod _DummyMod;
        public ISSMod DummyMod
        {
            get
            {
                if (_DummyMod == null)
                {
                    SSModWritable dum = new SSModWritable();
                    SSBaseUrl url = new SSBaseUrl("c://star");
                    SSLinkUrl url2 = new SSLinkUrl("mods\\dummy");
                    dum.ModUrl = url + url2;
                    dum.MakeModInfoBase();
                    _DummyMod = dum;
                }
                return _DummyMod;
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
                monitorValue.Modify(MonitoredValueModification.GetReplaceModification(new JsonValue("hegemony2")));
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

        public ISSWritableViewModel ISSWritableViewModel
        {
            get
            {
                return new ISSWritableViewModel(DummyFaction);
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
