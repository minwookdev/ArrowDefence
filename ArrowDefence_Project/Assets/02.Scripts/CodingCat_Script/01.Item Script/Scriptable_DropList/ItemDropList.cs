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

            public void CheckQuantity()
            {
                if(QuantityRange.Length <= 0) QuantityRange = new int[] { 1 };
                if(ItemAsset.Item_Type == ITEMTYPE.ITEM_EQUIPMENT)
                {
                    if (QuantityRange.Length != 1)
                        QuantityRange = new int[] { 1 };

                    if (QuantityRange[0] != 1)
                        QuantityRange[0] = 1;
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

        private void OnEnable() {
            if (DropTableArray == null) return;

            //OnCheckTotalChanceValue();
            CalcTotalChance();
            OnCheckQuantityArray();
        }

        private void OnCheckTotalChanceValue()
        {
            float totalChance = 0f;

            if (DropTableArray.Length != 0)
            {
                for (int i = 0; i < DropTableArray.Length; i++)
                {
                    totalChance += DropTableArray[i].DropChance;
                }

                if (totalChance != 100) CatLog.WLog($"DropList : {this.name}, Total Item Chance is not match \n" +
                                                    $"TotalChance : {totalChance}%");
            }
            else CatLog.WLog($"DropList : {this.name} is Not have a Item Data's");
        }

        void CalcTotalChance() {
            float totalChance = 0f;
            if(DropTableArray.Length != 0) {
                for (int i = 0; i < DropTableArray.Length; i++) {
                    totalChance += DropTableArray[i].DropChance;
                }
            }

            CatLog.Log(StringColor.YELLOW, $"DROP TABLE : {this.name}, TOTAL DROP CHANCE : {totalChance}%");
        }

        private void OnCheckQuantityArray() {
            foreach (var item in DropTableArray) {
                item.CheckQuantity();
            }
        }

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
