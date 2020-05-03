using EditorInterface.ViewModel;
using SSEditor.FileHandling;
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

    }
}
