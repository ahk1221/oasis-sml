﻿using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SMLHelper.Patchers
{
    public class TechTypePatcher
    {
        private static Dictionary<TechType, string> customTechTypes = new Dictionary<TechType, string>();

        private static int currentIndex = 10001;
        public static TechType AddTechType(string name, string languageName, string languageTooltip)
        {
            var techType = (TechType)currentIndex;

            customTechTypes.Add(techType, name);
            currentIndex++;

            LanguagePatcher.customLines.Add(name, languageName);
            LanguagePatcher.customLines.Add("Tooltip_" + name, languageTooltip);

            return techType;
        }

        public static void Postfix_GetValues(Type enumType, ref Array __result)
        {
            if(enumType.Equals(typeof(TechType)))
            {
                var listArray = new List<TechType>();
                foreach (var obj in __result)
                {
                    listArray.Add((TechType)obj);
                }

                __result = listArray
                    .Concat(customTechTypes.Keys)
                    .ToArray();
            }
        }

        public static bool Prefix_IsDefined(Type enumType, object value, ref bool __result)
        {
            if(enumType.Equals(typeof(TechType)))
            {
                if(customTechTypes.Keys.Contains((TechType)value))
                {
                    __result = true;
                    return false;
                }
            }

            return true;
        }

        public static bool Prefix_Parse(Type enumType, string value, bool ignoreCase, ref object __result)
        {
            if(enumType.Equals(typeof(TechType)))
            {
                foreach(var techType in customTechTypes)
                {   
                    if(value.Equals(techType.Value, ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                    {
                        __result = techType.Key;
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool Prefix_ToString(Enum __instance, ref string __result)
        {
            if(__instance.GetType().Equals(typeof(TechType)))
            {
                foreach(var techType in customTechTypes)
                {
                    if(__instance.Equals(techType.Key))
                    {
                        __result = techType.Value;
                        return false;
                    }
                }
            }

            return true;
        }

        public static void Patch(HarmonyInstance harmony)
        {
            var enumType = typeof(Enum);
            var thisType = typeof(TechTypePatcher);
            var techTypeType = typeof(TechType);

            harmony.Patch(enumType.GetMethod("GetValues", BindingFlags.Public | BindingFlags.Static), null,
                new HarmonyMethod(thisType.GetMethod("Postfix_GetValues", BindingFlags.Public | BindingFlags.Static)));

            harmony.Patch(enumType.GetMethod("IsDefined", BindingFlags.Public | BindingFlags.Static),
                new HarmonyMethod(thisType.GetMethod("Prefix_IsDefined", BindingFlags.Public | BindingFlags.Static)), null);

            harmony.Patch(enumType.GetMethod("Parse", new Type[] { typeof(Type), typeof(string), typeof(bool) }),
                new HarmonyMethod(thisType.GetMethod("Prefix_Parse", BindingFlags.Public | BindingFlags.Static)), null);

            harmony.Patch(techTypeType.GetMethod("ToString", new Type[0]),
                new HarmonyMethod(thisType.GetMethod("Prefix_ToString", BindingFlags.Public | BindingFlags.Static)), null);

            var techTypeExtensions = typeof(TechTypeExtensions);
            var traverse = Traverse.Create(techTypeExtensions);

            var stringsNormal = traverse.Field("stringsNormal").GetValue<Dictionary<TechType, string>>();
            var stringsLowercase = traverse.Field("stringsLowercase").GetValue<Dictionary<TechType, string>>();
            var techTypesNormal = traverse.Field("techTypesNormal").GetValue<Dictionary<string, TechType>>();
            var techTypesIgnoreCase = traverse.Field("techTypesIgnoreCase").GetValue<Dictionary<string, TechType>>();
            var techTypeKeys = traverse.Field("techTypeKeys").GetValue<Dictionary<TechType, string>>();
            var keyTechTypes = traverse.Field("keyTechTypes").GetValue<Dictionary<string, TechType>>();

            foreach (var techValue in customTechTypes)
            {
                var key = techValue.Key;
                var value = techValue.Value;

                stringsNormal[key] = value;
                stringsLowercase[key] = value.ToLowerInvariant();
                techTypesNormal[value] = key;
                techTypesIgnoreCase[value] = key;
                string key3 = ((int)key).ToString();
                techTypeKeys[key] = key3;
                keyTechTypes[key3] = key;
            }
        }
    }
}
