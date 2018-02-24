using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Harmony;
using System.Reflection;
using OasisModLoader.Logging;

namespace OasisModLoader.Patching
{
    public class Patcher
    {
        public static HarmonyInstance harmony = HarmonyInstance.Create("com.ahk1221.sml");

        public static List<PatchProcessor> patches = new List<PatchProcessor>();

        public static void Patch()
        {
            //foreach(Type type in Assembly.GetExecutingAssembly().GetTypes())
            //{
            //    object[] attributes = type.GetCustomAttributes(typeof(HarmonyPatch), true);

            //    if(attributes.Length == 2)
            //    {
            //        Type originalType = (attributes[0] as HarmonyPatch).info.originalType;
            //        MethodInfo originalMethod = originalType.GetMethod((attributes[1] as HarmonyPatch).info.methodName);

            //        MethodInfo prefixInfo = type.GetMethod("Prefix", BindingFlags.Public | BindingFlags.Static);
            //        MethodInfo postfixInfo = type.GetMethod("Postfix", BindingFlags.Public | BindingFlags.Static);
            //        MethodInfo transpilerInfo = type.GetMethod("Transpiler", BindingFlags.Public | BindingFlags.Static);
            //        HarmonyMethod prefixMethod = TryHarmonyMethod(prefixInfo);
            //        HarmonyMethod postfixMethod = TryHarmonyMethod(postfixInfo);
            //        HarmonyMethod transpilerMethod = TryHarmonyMethod(transpilerInfo);
            //        patches.Add(harmony.Patch(originalMethod, prefixMethod, postfixMethod, transpilerMethod));
            //        harmony.Pat
            //    }
            //}

            patches = harmony.PatchAll(Assembly.GetExecutingAssembly()).ToList();
            Logger.Log("Patched " + patches.Count + " patches!");
        }
    }
}
