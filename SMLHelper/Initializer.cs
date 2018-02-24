using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SMLHelper.Patchers;

namespace SMLHelper
{
    public class Initializer
    {
        private static HarmonyInstance harmony;

        public static void Patch()
        {
            harmony = HarmonyInstance.Create("com.ahk1221.smlhelper");

            TechTypePatcher.Patch(harmony);
            CraftDataPatcher.Patch(harmony);
            CraftTreePatcher.Patch(harmony);
            LanguagePatcher.Patch(harmony);
            ResourcesPatcher.Patch(harmony);
            PrefabDatabasePatcher.Patch(harmony);
            SpritePatcher.Patch(harmony);
        }

        public static void Postpatch()
        {
            CraftTreePatcher.Postpatch();

            var inventory = Inventory.Get();
            inventory.quickSlots.Select(1);
            var go = Inventory.Get().GetHeldObject();
            if(go != null)
            {
                //SubnauticaModLoader.Logging.Logger.Log("Local Position: " + go.transform.localPosition + " Scale: " + go.transform.lossyScale);
            }
        }
    }
}
