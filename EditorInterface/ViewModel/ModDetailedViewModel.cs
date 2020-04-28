using FVJson;
using SSEditor.FileHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EditorInterface.ViewModel
{
    public class ModDetailedViewModel
    {
        public SSMod Mod { get; private set; }
        public ModDetailedViewModel(SSMod mod)
        {
            Mod = mod;
        }

        public string Id
        { 
            get 
            {
                if (Mod.ModInfo == null)
                    return "no ID";
                else
                {
                    return ((JsonValue)Mod.ModInfo.Fields[".id"]).ToString();
                }
            } 
        }

        public string Identifier
        {
            get 
            {

                if (Mod.ModInfo == null)
                    return Mod.ModName;
                else
                {
                    return Mod.ModName + " " + ((JsonValue)Mod.ModInfo.Fields[".version"]).ToString();
                }
            }
        }

        public string Description
        {
            get
            {
                if (Mod.ModInfo == null)
                    return "This folder does not have a mod-info and cannot be handled as a mod";
                else
                {
                    return ((JsonValue)Mod.ModInfo.Fields[".description"]).ToString();
                }
            }
        }
    }
}
