namespace ActionCat {
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class CraftingRecipeSO : ScriptableObject {
        public Dictionary<string, CraftingRecipe> RecipeDictionary = null;
        public CraftingRecipe[] RecipeArray = null;
        public string version = "";

        private void Awake() {
            if (RecipeArray != null || RecipeArray.Length > 0) {
                for (int i = 0; i < RecipeArray.Length; i++) {

                }
            }

            CatLog.Log(StringColor.YELLOW, $"Crafting Recipe Initialized, Check the Init Report" + '\n' +
                                           $"Title: {this.name}" + '\n' +
                                           $"Total Assigned Items: {((RecipeArray != null || RecipeArray.Length > 0) ? RecipeArray.Length.ToString() : "0")}" + '\n' +
                                           $"Success Items: " + '\n' + 
                                           $"Failed Items: ");
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
        public CraftingMaterial[] Mats = null;
        public ItemData ResultItem     = null;

        [System.Serializable]
        public class CraftingMaterial {
            public ItemData Mateiral = null;
            public int Required = 0;
        }
    }
}
