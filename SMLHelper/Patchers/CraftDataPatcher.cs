﻿using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections;

namespace SMLHelper.Patchers
{
    public class CraftDataPatcher
    {
        public static Dictionary<TechType, TechDataHelper> customTechData = new Dictionary<TechType, TechDataHelper>();
        public static Dictionary<TechType, TechType> customHarvestOutputList = new Dictionary<TechType, TechType>();
        public static Dictionary<TechType, HarvestType> customHarvestTypeList = new Dictionary<TechType, HarvestType>();
        public static Dictionary<TechType, Vector2int> customItemSizes = new Dictionary<TechType, Vector2int>();
        public static Dictionary<TechType, EquipmentType> customEquipmentTypes = new Dictionary<TechType, EquipmentType>();

        private static readonly Type CraftDataType = typeof(CraftData);

        public static void Patch(HarmonyInstance harmony)
        {
            var techData = CraftDataType.GetField("techData", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
            var addMethod = techData.GetType().GetMethod("Add");

            foreach(var customTech in customTechData)
            {
                addMethod.Invoke(techData, new object[2] { customTech.Key, customTech.Value.GetTechDataObj() });
            }

            PatchDictionary("harvestOutputList", customHarvestOutputList, BindingFlags.Static | BindingFlags.Public);
            PatchDictionary("harvestTypeList", customHarvestTypeList);
            PatchDictionary("itemSizes", customItemSizes);
            PatchDictionary("equipmentTypes", customEquipmentTypes);

            var prepareEntTechCache = CraftDataType.GetMethod("PrepareEntTechCache", BindingFlags.NonPublic | BindingFlags.Static);

            harmony.Patch(prepareEntTechCache, null,
                new HarmonyMethod(typeof(CraftDataPatcher).GetMethod("Postfix")));
        }

        public static void PatchDictionary(string name, IDictionary dictionary)
        {
            PatchDictionary(name, dictionary, BindingFlags.NonPublic | BindingFlags.Static);
        }

        public static void PatchDictionary(string name, IDictionary dictionary, BindingFlags flags)
        {
            var dictionaryField = CraftDataType.GetField(name, flags);
            var craftDataDict = dictionaryField.GetValue(null) as IDictionary;

            foreach (DictionaryEntry key in dictionary)
            {
                craftDataDict[key.Key] = key.Value;
            }
        }

        public static void Postfix()
        {
            var entTechMap = CraftDataType.GetField("entTechMap", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, TechType>;

            foreach (var prefab in CustomPrefabHandler.customPrefabs)
            {
                entTechMap[Path.GetFileName(prefab.PrefabFileName).ToLowerInvariant()] = prefab.TechType;
            }
        }
    }

    public class TechDataHelper : ITechData
    {
        public int _craftAmount;
        public TechType _techType;
        public List<IngredientHelper> _ingredients = new List<IngredientHelper>();
        public List<TechType> _linkedItems = new List<TechType>();

        public static Type TechDataType = typeof(CraftData).GetNestedType("TechData", BindingFlags.NonPublic);

        public int craftAmount { get { return _craftAmount; } }

        public int ingredientCount
        {
            get
            {
                if (_ingredients != null) return _ingredients.Count;
                else return 0;
            }
        }

        public int linkedItemCount
        {
            get
            {
                if (_linkedItems != null) return _linkedItems.Count;
                else return 0;
            }
        }

        public IIngredient GetIngredient(int index)
        {
            if(_ingredients == null || index > (_ingredients.Count - 1) || index < 0)
            {
                return _ingredients[index];
            }

            return new IngredientHelper(TechType.None, 0);
        }

        public TechType GetLinkedItem(int index)
        {
            if (_linkedItems == null || index > (_linkedItems.Count - 1) || index < 0)
            {
                return _linkedItems[index];
            }

            return TechType.None;
        }

        private object GetIngredientsObj()
        {
            var ingredientsType = typeof(CraftData).GetNestedType("Ingredients", BindingFlags.NonPublic);
            var ingredientsObj = Activator.CreateInstance(ingredientsType);
            var addMethod = ingredientsType.GetMethod("Add", new Type[] { IngredientHelper.IngredientType } );

            foreach (var ingredient in _ingredients)
            {
                addMethod.Invoke(ingredientsObj, new object[] { ingredient.GetIngredientObj() });
            }

            return ingredientsObj;
        }

        public object GetTechDataObj()
        {
            var techDataObj = Activator.CreateInstance(TechDataType);
            var ingredientsObj = GetIngredientsObj();

            TechDataType.GetField("_craftAmount").SetValue(techDataObj, _craftAmount);
            TechDataType.GetField("_ingredients").SetValue(techDataObj, ingredientsObj);
            TechDataType.GetField("_linkedItems").SetValue(techDataObj, _linkedItems);
            TechDataType.GetField("_techType").SetValue(techDataObj, _techType);

            return techDataObj;
        }

    }

    public class IngredientHelper : IIngredient
    {
        public TechType _techType;
        public int _amount;

        public TechType techType => _techType;
        public int amount => _amount;

        public static Type IngredientType = typeof(CraftData).GetNestedType("Ingredient", BindingFlags.NonPublic);

        public IngredientHelper(TechType techType, int amount)
        {
            _amount = amount;
            _techType = techType;
        }

        public object GetIngredientObj()
        {
            return Activator.CreateInstance(IngredientType, _techType, _amount);
        }
    }
}
