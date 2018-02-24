using System;
using Harmony;

namespace OasisModLoader.Patching.Patches
{
    [HarmonyPatch(typeof(MainMenuMusic))]
    [HarmonyPatch("Stop")]
    public class Patch_MainMusicStop
    {
        public static void Postfix()
        {
            Main.OnGameFinishLoad();
        }
    }
}
