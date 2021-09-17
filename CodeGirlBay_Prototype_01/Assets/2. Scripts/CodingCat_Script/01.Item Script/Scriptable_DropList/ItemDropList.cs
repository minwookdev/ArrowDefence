namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    [CreateAssetMenu(fileName = "", menuName = "")]
    public class ItemDropList : ScriptableObject
    {
        [System.Serializable]
        public class DropItems
        {
            public ItemData ItemAsset;
            public float DropChance;
            public int[] QuantityRange;

            //public int GetQuantityInRange() 원본
            //{
            //    if (QuantityRange.Length <= 1) return ItemAsset.Item_Amount;
            //
            //    int randomIndexInRange = Random.Range(0, QuantityRange.Length + 1);
            //    return QuantityRange[randomIndexInRange];
            //}
        }

        [Header("ITEM DROP LIST")]
        public DropItems[] DropListArray;

        private void OnEnable()
        {
            float totalChance = 0f;

            if (DropListArray.Length != 0)
            {
                for (int i = 0; i < DropListArray.Length; i++)
                {
                    totalChance += DropListArray[i].DropChance;
                }

                if (totalChance != 100) CatLog.WLog($"DropList : {this.name}, Total Item Chance is not match \n" +
                                                    $"TotalChance : {totalChance}%");
            }
            else CatLog.WLog($"DropList : {this.name} is Not have a Item Data's");
        }

        [UnityEditor.MenuItem("CodingCat/Scriptable Object/ItemDropList Asset")]
        public static void CreateItemDropListAssetInstance()
        {
            var asset = ScriptableObject.CreateInstance<ItemDropList>();
            UnityEditor.AssetDatabase.CreateAsset(asset, "Assets/05. Scriptable_Object/DropListAssets/DropList.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();

            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.Selection.activeObject = asset;
        }
    }
}
