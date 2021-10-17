namespace ActionCat
{
    using CodingCat_Scripts;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Material_Asset", menuName = "Scriptable Object Asset/Item_Mat_Asset")]
    public class ItemData_Mat : ItemData
    {
        //[ReadOnly]
        //public Enum_Itemtype Item_Type = Enum_Itemtype.ITEM_MATERIAL;
        //public int Item_Amount;

        ItemData_Mat()
        {
            Item_Type = ITEMTYPE.ITEM_MATERIAL;
        }
    }
}
