namespace ActionCat {
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class CraftingRecipeSO : ScriptableObject {
        public Dictionary<string, CraftingRecipe> RecipeDictionary = null;
        public CraftingRecipe[] RecipeArray = null;
        public string version = "";

        private void OnEnable() {
            if (RecipeArray != null || RecipeArray.Length > 0) {
                RecipeDictionary = new Dictionary<string, CraftingRecipe>();
                for (int i = 0; i < RecipeArray.Length; i++) {
                    if (RecipeDictionary.ContainsKey(RecipeArray[i].BluePrint.Item_Id)) {
                        CatLog.WLog($"중복등록된 제작 레시피가 있습니다. 도면 아이템 아이디: {RecipeArray[i].BluePrint.Item_Id}");
                        CatLog.WLog("해당 중복 레시피는 등록되지 않습니다.");
                        continue;
                    }
                    RecipeDictionary.Add(RecipeArray[i].BluePrint.Item_Id, RecipeArray[i]);
                }
            }

            CatLog.Log($"Crafting Recipe Initialized, Check the Init Report" + '\n' +
                       $"Title: {this.name}" + '\n' +
                       $"Total Assigned Items: {((RecipeDictionary != null || RecipeDictionary.Count > 0) ? RecipeDictionary.Count.ToString() : "0")}" + '\n' +
                       $"Success Items: {RecipeDictionary.Count}" + '\n' +
                       $"Failed Items: {RecipeArray.Length - RecipeDictionary.Count}");
        }

        public bool TryGetRecipe(string itemId, out CraftingRecipe recipe) {
            return RecipeDictionary.TryGetValue(itemId, out recipe);
        }

        public string[] GetMatStrings(string itemId) {
            var materialStringList = new List<string>();
            if (RecipeDictionary.ContainsKey(itemId)) {
                var mats = RecipeDictionary[itemId].Mats;
                for (int i = 0; i < mats.Length; i++) {
                    materialStringList.Add(mats[i].Mateiral.Item_Id);
                }
            }
            else {
                throw new System.Exception();
            }

            return materialStringList.ToArray();
        }

        private void OnValidate() {
            if(RecipeArray != null) {
                foreach (var recipe in RecipeArray) {
                    if (recipe.Result.Item.Item_Type == ITEMTYPE.ITEM_EQUIPMENT) {
                        if (recipe.Result.Count > 1) {
                            CatLog.ELog("Equipment Type Result Item Amount Over 1, Check this");
                        }
                    }

                    if(recipe.Mats.Length > 5) {
                        CatLog.ELog($"Material Array Size Always Less than 6, BluePrint: {recipe.BluePrint.NameByTerms}");
                    }
                }
            }
        }

        #region EDITOR_ONLY
#if UNITY_EDITOR
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/CraftingRecipe Asset")]
        public static void CreateCraftingRecipeScriptableObject() {
            string assetCreatePath = "Assets/05.SO/Tables/CraftingRecipe_(version).asset";  
            var asset = ScriptableObject.CreateInstance<CraftingRecipeSO>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetCreatePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
#endif
        #endregion
    }

    [System.Serializable]
    public sealed class CraftingRecipe {
        public ItemData_Mat BluePrint  = null;
        public Materials[] Mats = null;
        public Results Result   = null;
        public int CraftingTime = 3; // <- Default

        [System.Serializable]
        public class Materials {
            public ItemData Mateiral = null;
            public int Required = 0;
        }

        [System.Serializable]
        public class Results {
            public ItemData Item = null;
            public int Count = 1;
        }

    }
}
