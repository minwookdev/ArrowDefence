namespace CodingCat_Games
{
    using UnityEngine;

    public enum Equipment_Item_Type
    {
        Equip_Bow,
        Equip_Arrow,
        Equip_Accessory
    }

    public class Item_Equipment : AD_item
    {
        protected Equipment_Item_Type EquipType;

        public Item_Equipment()
        {
            this.Item_Amount = 1;
            this.Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
        }

        public override object GetItem()
        {
            throw new System.NotImplementedException();
        }
    }
}
