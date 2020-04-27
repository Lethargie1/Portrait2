using SSEditor.FileHandling;
using SSEditor.FileHandling.Editors;
using Stylet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class FactionEditorViewModel : Screen
    {
        public FactionEditor FactionEditor { get; private set; }
        public List<SSFactionGroup> Factions { get => FactionEditor.Factions; }
        public FactionEditorViewModel(FactionEditor factionEditor)
        {
            FactionEditor = factionEditor;
            FactionEditor.GetFaction();
        }

        protected override void OnActivate() 
        {
            FactionEditor.GetFaction();
            NotifyOfPropertyChange(nameof(Factions));
        }
    }
}
