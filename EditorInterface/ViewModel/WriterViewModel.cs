using EditorInterface.Properties;
using EditorInterface.Validation;
using FluentValidation;
using Ookii.Dialogs.Wpf;
using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

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


}
