namespace ActionCat
{
    using CodingCat_Scripts;
    using UnityEngine;

    [CreateAssetMenu(fileName = "", menuName = "")]
    public class ItemDropList : ScriptableObject
    {
        [System.Serializable]
        public class DropTable
        {
            public ItemData ItemAsset;
            public float DropChance;
            public int[] QuantityRange;

            public void CheckQuantity()
            {
                if(QuantityRange.Length <= 0) QuantityRange = new int[] { 1 };
            }
        }

        [Header("ITEM DROP LIST")]
        public DropTable[] DropTableArray;

        private void OnEnable()
        {
            if (DropTableArray == null) return;

            OnCheckTotalChanceValue();
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

        private void OnCheckQuantityArray()
        {
            foreach (var item in DropTableArray)
            {
                item.CheckQuantity();
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("ActionCat/Scriptable Object/ItemDropList Asset")]
        public static void CreateItemDropListAssetInstance()
        {
            var asset = ScriptableObject.CreateInstance<ItemDropList>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/05. Scriptable_Object/DropListAssets/DropList.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
#endif
    }
}
