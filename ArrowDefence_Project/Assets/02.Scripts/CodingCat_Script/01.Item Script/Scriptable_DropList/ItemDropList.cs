namespace ActionCat {
    using UnityEngine;

    public class ItemDropList : ScriptableObject {
        [System.Serializable]
        public class DropTable {
            public ItemData ItemAsset;
            public float DropChance;
            public int[] QuantityRange = new int[1] { 1 };

            public int DefaultQuantity {
                get {
                    return RandomEx.GetRandomItemAmount(QuantityRange);
                }
            }

            public void CheckQuantity(string tableName) {
                if (QuantityRange.Length <= 0) {
                    CatLog.WLog($"Quantity Range is Not Set. TableName: {tableName}");
                }

                if (ItemAsset.Item_Type == ITEMTYPE.ITEM_EQUIPMENT) {
                    if (QuantityRange.Length != 1 || QuantityRange[0] != 1) {
                        CatLog.WLog($"Equipment Item Drop Quantity is Always 1, TableName: {tableName}, ItemName: {ItemAsset.NameByTerms}");
                    }
                }
            }
        }

        public DropTable[] DropTableArray;
        public DropTable[] RewardTableArray;

        public int TotalTableSize {
            get {
                return ((DropTableArray == null) ? 0 : DropTableArray.Length) + ((RewardTableArray == null) ? 0 : RewardTableArray.Length);
            }
        }

        public DropTable[] GetRewardTable {
            get { //if value is null, return empty droptable array
                return (RewardTableArray != null) ? RewardTableArray : new DropTable[0] { };
            }
        }

        public int GetTotalTableSize(bool includeRewardTableSize) {
            return (includeRewardTableSize) ? ((DropTableArray == null ? 0 : DropTableArray.Length) + (RewardTableArray == null ? 0 : RewardTableArray.Length)) :
                                              ((DropTableArray == null ? 0 : DropTableArray.Length));
        }

        private void Awake() {
            if (DropTableArray == null || DropTableArray.Length <= 0) {
                return;
            }

            CheckTable();
        }

        private void CheckTable() {
            foreach (var item in DropTableArray) {
                item.CheckQuantity(this.name);
            }
        }

        //어디서 지워버리는 듯?
        //droptable_dungeon_entrance_0

#if UNITY_EDITOR
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/ItemDropList Asset")]
        public static void CreateItemDropListAssetInstance() {
            var asset = ScriptableObject.CreateInstance<ItemDropList>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/05.SO/SO.DropTable/(name)_(stage).asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
#endif
    }
}
