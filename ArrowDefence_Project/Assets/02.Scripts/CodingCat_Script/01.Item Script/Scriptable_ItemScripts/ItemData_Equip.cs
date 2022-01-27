using UnityEngine;

namespace ActionCat
{
    public class ItemData_Equip : ItemData
    {
        [Header("Equipment Item Data")]
        [Space(15)]
        [ReadOnly] public EQUIP_ITEMTYPE Equip_Type;
        public Ability[] abilityDatas = null;

        protected ItemData_Equip() {
            Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
            Item_Amount = 1;
        }
    }
}
