namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Consumable_Asset", menuName = "Scriptable Object Asset/Item_Consume_Asset")]
    public class ItemData_Con : ItemData
    {
        ItemData_Con()
        {
            Item_Type = ITEMTYPE.ITEM_CONSUMABLE;
        }
    }
}
