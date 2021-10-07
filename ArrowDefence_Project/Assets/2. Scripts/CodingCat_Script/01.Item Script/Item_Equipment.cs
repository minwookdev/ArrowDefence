using UnityEngine;

namespace CodingCat_Games
{
    public enum EQUIP_ITEMTYPE
    {
        EQUIP_BOW       = 0,
        EQUIP_ARROW     = 1,
        EQUIP_ACCESSORY = 2
    }

    public abstract class Item_Equipment : AD_item
    {
        protected EQUIP_ITEMTYPE EquipType;

        protected Item_Equipment()
        {
            this.Item_Amount = 1;
            this.Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
        }

        public override object GetItem() => throw new System.NotImplementedException();

        public EQUIP_ITEMTYPE GetEquipType() => EquipType;

        //public abstract void Setup();
    }
}
