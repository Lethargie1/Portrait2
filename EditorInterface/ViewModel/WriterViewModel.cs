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
        public EditableUrlViewModel TargetModFolderUrl { get; set; } = new EditableUrlViewModel("Folder where patch will be written");

        public SSDirectory Directory { get; private set; }

        public SSModWritable Receiver { get; } = new SSModWritable();

        private FactionEditorFactory FactionEditorFactory { get; set; }

        public WriterViewModel(SSDirectory directory, FactionEditorFactory factionEditorFactory, IWindowManager windowManager)
        {
            Directory = directory;
            FactionEditorFactory = factionEditorFactory;
            this.windowManager = windowManager;
        }

        public void WriteToDisk()
        {
            var match = Regex.Match(TargetModFolderUrl.Url, @"(?:" + Directory.InstallationUrl.ToString().Replace(@"\", @"\\") + @"\\)(.*)");
            string linkPart = match.Groups[1].ToString();
            Receiver.ModUrl = Directory.InstallationUrl + new SSLinkUrl(linkPart);

            var factionEditor = FactionEditorFactory.CreateFactionEditor();
            factionEditor.ReplaceFactionToWrite(Receiver);

            //MessageBoxButton Button = MessageBoxButton.OK;
            try
            {  
                Receiver.WriteMod();
                windowManager.ShowMessageBox("Patch created successfully, do not forget to activate it before starting a new game");
                //MessageBox.Show(this, "Patch created successfully, do not forget to activate it before starting a new game", "success", Button);
            }
            catch (IOException)
            {
                windowManager.ShowMessageBox("Target directory is not accesibles (Is it open in the Windows explorer?)");
                //MessageBox.Show(this, "Target directory is not accesibles (Is it open in the Windows explorer?)", "Access error", Button);
            }
            catch (System.UnauthorizedAccessException)
            {
                windowManager.ShowMessageBox("Target directory are not accesibles (Is it open in the Windows explorer?)");
               //MessageBox.Show(this, "Target directory are not accesibles (Is it open in the Windows explorer?)", "Access error", Button);
            }

        }
    }
}
