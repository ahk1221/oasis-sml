using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Logger = OasisModLoader.Logging.Logger;
using OasisModLoader.Patching;
using SMLHelper;
using SMLHelper.Patchers;

namespace OasisModLoader
{
    public class Main
    {
        private static string ModFolder = @"./Subnautica_Data/SMods";

        private static List<Mod> mods = new List<Mod>();

        private static bool patched = false;

        // Called when GameInput.Awake happens.
        public static void OnGameStart()
        {
            try
            {
                if (patched) return;

                patched = true;

                Logger.Initialize("SMLLog.log");

                Logger.Log("Logging started!");

                if (!Directory.Exists(ModFolder))
                    Directory.CreateDirectory(ModFolder);

                FileInfo[] modFiles = new DirectoryInfo(ModFolder).GetFiles();
                Logger.Log("Found mod files! Amount of mod files: " + modFiles.Length);

                foreach (FileInfo file in modFiles)
                {
                    if (file.Extension != ".dll")
                        continue;

                    if (file.Name.Contains("0Harmony"))
                        continue;

                    AssemblyName an = AssemblyName.GetAssemblyName(file.FullName);
                    Assembly assembly = Assembly.Load(an);
                    Type modType = typeof(Mod);
                    foreach (Type t in assembly.GetTypes())
                    {
                        if (t.Name == "EntryPoint")
                        {
                            Mod mod = (Mod)Activator.CreateInstance(t);
                            mod.OnGameStart();

                            mods.Add(mod);

                            Logger.Log("Loaded mod: " + assembly.FullName + "!");
                        }
                    }
                }

                var propulsionCannon = Resources.Load<GameObject>("WorldEntities/Tools/RepulsionCannon");
                foreach(var component in propulsionCannon.GetComponents<Component>())
                {
                    Logger.Log("Component: " + component.GetType().Name);
                }

                Patcher.Patch();
                Initializer.Patch();
            }
            catch(Exception e)
            {
                Logger.Log("Caught exception! " + e.Message);
                Logger.Log(e.StackTrace);
            }
        }

        public static void OnGameFinishLoad()
        {
            foreach(Mod m in mods)
            {
                Logger.Log("Game finished loading!");
                m.OnGameFinishLoad();
            }

            Initializer.Postpatch();
        }

    }
    
}
