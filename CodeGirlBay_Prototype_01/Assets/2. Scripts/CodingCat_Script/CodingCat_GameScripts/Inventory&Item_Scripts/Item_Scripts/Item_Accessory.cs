namespace CodingCat_Games
{
    using UnityEngine;

    public class Item_Accessory : Item_Equipment
    {
        private object effect;

        public Item_Accessory(int id, string name, string desc, Sprite sprite, ITEMGRADE grade) : base()
        {
            EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            this.Item_Id     = id;
            this.Item_Name   = name;
            this.Item_Desc   = desc;
            this.Item_Sprite = sprite;
            this.Item_Grade  = grade;

            this.effect = null;
        }

        public Item_Accessory() : base() { }

        private void AddEffect(object effect)
        {
            this.effect = effect;
        }
    }
}
