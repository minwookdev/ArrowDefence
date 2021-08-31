namespace CodingCat_Games
{
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(fileName ="Item_Accessory_Asset", menuName = "Scriptable Object Asset/Item_Accessory_Asset")]
    public class ItemData_Equip_Accessory : ItemData_Equip
    {
        [Header("Accessory Item Data")]
        public string effect = "";

        public ItemData_Equip_Accessory() : base()
        {
            this.Equip_Type = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;
        }
#if UNITY_EDITOR
        [MenuItem("CodingCat/Item Asset/Accessory Item Asset")]
        static void CreateAsset()
        {
            var asset = CreateInstance<ItemData_Equip_Accessory>();
            AssetDatabase.CreateAsset(asset, "Assets/05. Scriptable_Object/Equipment_Items/Accessory Item Asset");
            AssetDatabase.Refresh();
        }
#endif
    }
}
