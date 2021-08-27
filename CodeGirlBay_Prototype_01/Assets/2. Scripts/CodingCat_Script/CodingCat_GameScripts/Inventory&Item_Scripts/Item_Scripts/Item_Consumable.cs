namespace CodingCat_Games
{
    using UnityEngine;

    public class Item_Consumable : AD_item
    {
        public Item_Consumable(int itemId, int amount, string name, string desc, Sprite sprite, ITEMGRADE grade)
        {
            //Consume Item Static Field
            this.Item_Type = ITEMTYPE.ITEM_CONSUMABLE;

            this.Item_Id     = itemId;
            this.Item_Name   = name;
            this.Item_Amount = amount;
            this.Item_Desc   = desc;
            this.Item_Sprite = sprite;
            this.Item_Grade  = grade;
        }

        public Item_Consumable(ItemData_Con item)
        {
            //Consume Item Static Field
            this.Item_Type = ITEMTYPE.ITEM_CONSUMABLE;

            this.Item_Id      = item.Item_Id;
            this.Item_Name    = item.Item_Name;
            this.Item_Amount      = item.Item_Amount;
            this.Item_Desc = item.Item_Desc;
            this.Item_Sprite  = item.Item_Sprite;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this)
        /// </summary>
        public Item_Consumable() { }

        public void SetAmount(int value) => this.Item_Amount = value;

        public void IncAmount(int value) => this.Item_Amount += value;

        public override object GetItem() => this;
    }
}
