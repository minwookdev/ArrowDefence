namespace ActionCat {
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;

    [System.Serializable]
    public class UpgradeRecipeSO : ScriptableObject {
        public Dictionary<string, UpgradeRecipe> recipeDictionary = null;
        public UpgradeRecipe[] recipeArray = null;

        public string[] DictionaryKeys {
            get {
                if (recipeDictionary == null) {
                    throw new System.Exception("Recipe Dictionary is Null.");
                }

                return recipeDictionary.Keys.ToArray();
            }
        }

        // Scriptable Object의 Awakw는 Run-Time에서 호출되지 않는다
        //public void Awake() {
        //    CatLog.Log("Upgrade Recipe Awake Call !");
        //}

        private void OnEnable() {
            recipeDictionary = new Dictionary<string, UpgradeRecipe>();
            if (recipeArray != null && recipeArray.Length > 0) {
                for (int i = 0; i < recipeArray.Length; i++) {
                    if (recipeDictionary.ContainsKey(recipeArray[i].KeyItem.Item_Id) == false) {
                        recipeDictionary.Add(recipeArray[i].KeyItem.Item_Id, recipeArray[i]);
                    }
                    else {
                        CatLog.WLog($"[UpgradeRecipe Dictionary] Attempt to Insert Duplicate Key, Check Detail: \n " +
                                    $"Item ID: {recipeArray[i].KeyItem.Item_Id}, \n" +
                                    $"Item Name: {recipeArray[i].KeyItem.Item_Name}");
                    }
                }
            }
        }

        #region EDITOR_ONLY
#if UNITY_EDITOR
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/UpgradeRecipe Asset")]
        public static void CreateUpgradeRecipeScriptableObject() {
            string assetCreatePath = "Assets/05.SO/Tables/UpgadeRecipe_(version).asset";
            var asset = ScriptableObject.CreateInstance<UpgradeRecipeSO>();
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
    public sealed class UpgradeRecipe {
        public ItemData_Equip KeyItem = null;
        public Material[] Materials = null;
        public float FailedProb = 0f;
        public ItemData_Equip Result = null;

        [System.Serializable]
        public sealed class Material {
            public ItemData Mat = null;
            public int Required = 0;
        }
    }
}
