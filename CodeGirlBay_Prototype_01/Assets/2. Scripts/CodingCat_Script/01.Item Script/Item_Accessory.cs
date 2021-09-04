namespace CodingCat_Games
{
    using UnityEngine;

    public class Item_Accessory : Item_Equipment
    {
        private object AccessEffect;

        public Item_Accessory(ItemData_Equip_Accessory item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.AccessEffect = null;
        }

        public Item_Accessory(Item_Accessory item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.AccessEffect = null;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Accessory() : base() { }

        private void AddEffect(object effect)
        {
            this.AccessEffect = effect;
        }
    }
}
