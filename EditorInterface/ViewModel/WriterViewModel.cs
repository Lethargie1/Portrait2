using EditorInterface.Properties;
using EditorInterface.Validation;
using FluentValidation;
using Ookii.Dialogs.Wpf;
using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;

namespace EditorInterface.ViewModel
{

    public class WriterViewModel : Screen
    {
        private readonly IWindowManager windowManager;

        private string _TargetFolder;
        public string TargetFolder { get => _TargetFolder; set { SetAndNotify(ref _TargetFolder, value);} }


        public SSDirectory Directory { get; private set; }

        public SSModWritable Receiver { get; } = new SSModWritable();

        private FactionEditorFactory FactionEditorFactory { get; set; }

        public ObservableCollection<ISSWritable> FilesToWrite { get => Receiver.FileList; }

        CollectionView _FilesToWriteView;
        public CollectionView FilesToWriteView
        {
            get
            {
                if (_FilesToWriteView == null)
                {

                    _FilesToWriteView = (CollectionView)CollectionViewSource.GetDefaultView(FilesToWrite);
                    //_FilesToWriteView = new CollectionView(FilesToWrite);
                    _FilesToWriteView.Filter = x => ((ISSWritable)x).WillCreateFile;
                    //PropertyGroupDescription groupDescription = new PropertyGroupDescription("SourceMod", new PortraitModToGroupConverter());
                    //_PortraitsView.GroupDescriptions.Add(groupDescription);
                }
                return _FilesToWriteView;
            }
        }

        public WriterViewModel(SSDirectory directory, FactionEditorFactory factionEditorFactory, IWindowManager windowManager, IModelValidator<WriterViewModel> validator) : base(validator)
        {
            Directory = directory;
            FactionEditorFactory = factionEditorFactory;
            this.windowManager = windowManager;
            Validate();
        }

        protected override void OnActivate()
        {
            if (Directory.InstallationUrl !=null)
            {
                if (TargetFolder == null)
                {
                    SSBaseLinkUrl url= (Directory.InstallationUrl + new SSLinkUrl(Settings.Default.PatchTarget));
                    TargetFolder = url.ToString();
                }
            }
            Receiver.ModUrl = new SSBaseLinkUrl() { Base = TargetFolder, Link = "" };
            var factionEditor = FactionEditorFactory.CreateFactionEditor();
            factionEditor.ReplaceFactionToWrite(Receiver);
            base.OnActivate();
        }

        public void SelectNewUrl()
        {
            VistaFolderBrowserDialog OpenRootFolder = new VistaFolderBrowserDialog();
            if (OpenRootFolder.ShowDialog() == true)
            {
                this.TargetFolder = OpenRootFolder.SelectedPath;

            }
            return;
        }

        public void WriteToDisk()
        {
            Receiver.ModUrl = new SSBaseLinkUrl() { Base = TargetFolder, Link = "" };

            var factionEditor = FactionEditorFactory.CreateFactionEditor();
            factionEditor.ReplaceFactionToWrite(Receiver);

            try
            {  
                Receiver.WriteMod();
                windowManager.ShowMessageBox("Patch created successfully, do not forget to activate it before starting a new game");
            }
            catch (IOException)
            {
                windowManager.ShowMessageBox("Target directory is not accesibles (Is it open in the Windows explorer?)");
            }
            catch (System.UnauthorizedAccessException)
            {
                windowManager.ShowMessageBox("Target directory are not accesibles (Is it open in the Windows explorer?)");
            }

        }
    }

    public class WriterViewModelValidator : AbstractValidator<WriterViewModel>
    {
        public WriterViewModelValidator()
        {
            RuleFor(x => x.TargetFolder).Cascade(CascadeMode.StopOnFirstFailure).NotEmpty().WithMessage("You must have a folder").Must(x =>
            {
                return StarsectorValidityChecker.CheckModFolderEmpty(x);
            }).WithMessage("Folder must be empty");
        }
    }

    [ValueConversion(typeof(ISSWritable), typeof(string))]
    public class ISSWritableToDescriptiveString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ISSWritable writ = (ISSWritable)value;
            return writ.RelativeUrl.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
