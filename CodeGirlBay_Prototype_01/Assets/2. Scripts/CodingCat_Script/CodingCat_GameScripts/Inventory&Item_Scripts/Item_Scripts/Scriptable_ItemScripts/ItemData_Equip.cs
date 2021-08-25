using CodingCat_Scripts;
using UnityEngine;

namespace CodingCat_Games
{
    public class ItemData_Equip : ItemData
    {
        [Header("Equipment Item Data")]
        [Space(15)]
        [ReadOnly] public Equipment_Item_Type Equip_Type;
        
        protected ItemData_Equip()
        {
            Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
            Item_Amount = 1;
        }
    }
}
