namespace ActionCat {
    using UnityEngine;
    using System.Collections.Generic;

    public class SaleTable : ScriptableObject {
        [Header("SALE TABLE")]
        [SerializeField] string tableVersion = "1.0";
        [SerializeField] List<SaleInfo> saleInfoList;
        Dictionary<string, SaleInfo> saleInfoDictionary = null;

        [System.Serializable]
        class SaleInfo {
            [SerializeField] ItemData itemAsset;
            [SerializeField] int price;

            public string ItemId {
                get {
                    if (string.IsNullOrEmpty(itemAsset.Item_Id)) {
                        CatLog.ELog($"Null ID Item Finded: {itemAsset.NameByTerms}");
                    }
                    return itemAsset.Item_Id;
                }
            }

            public string ItemName {
                get {
                    return itemAsset.NameByTerms;
                }
            }

            public int Price {
                get => price;
            }
        }

        private void Awake() {
            // Init New Sale Dictionary
            saleInfoDictionary = new Dictionary<string, SaleInfo>();

            // SaleList to SaleDictioanry
            saleInfoList.ForEach((item) => {
                if (saleInfoDictionary.ContainsKey(item.ItemId)) { 
                    // 이미 Dictionary에 동일한 아이템 아이디가 존재하는 경우
                    CatLog.ELog($"Detected Same Item Id. input: {item.ItemName}, inDictionary: {saleInfoDictionary[item.ItemId].ItemName}");
                    return;
                }

                saleInfoDictionary.Add(item.ItemId, item);
            });

            // Clear Sale Information List
            saleInfoList.Clear();

            // throw Init Message
            CatLog.Log($"Initialized SaleTable. version: {"v" + tableVersion}");
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/Sale Table Asset")]
        public static void CreateSaleTable() {
            string assetCreatePath = "Assets/05.SO/Tables/SaleTable_(version).asset";
            var asset = ScriptableObject.CreateInstance<SaleTable>();
            UnityEditor.AssetDatabase.CreateAsset(asset, assetCreatePath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
#endif
    }
}
