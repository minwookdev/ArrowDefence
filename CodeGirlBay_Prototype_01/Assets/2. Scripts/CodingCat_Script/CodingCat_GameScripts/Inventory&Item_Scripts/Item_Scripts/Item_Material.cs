namespace CodingCat_Games
{
    using UnityEngine;

    public class Item_Material : AD_item
    {
        public Item_Material(int itemId, int amount, string name, string desc, Sprite sprite, ITEMGRADE grade)
        {
            this.Item_Type = ITEMTYPE.ITEM_MATERIAL;

            this.Item_Grade  = grade;
            this.Item_Id     = itemId;
            this.Item_Amount = amount;
            this.Item_Name   = name;
            this.Item_Desc   = desc;
            this.Item_Sprite = sprite;
        }

        public Item_Material(ItemData_Mat item)
        {
            this.Item_Type = ITEMTYPE.ITEM_MATERIAL;

            this.Item_Id      = item.Item_Id;
            this.Item_Amount      = item.Item_Amount;
            this.Item_Name    = item.Item_Name;
            this.Item_Desc = item.Item_Desc;
            this.Item_Sprite  = item.Item_Sprite;
        }

        public Item_Material() { }

        public void SetAmount(int value) => this.Item_Amount = value;

        public void IncAmount(int value) => this.Item_Amount += value;
    }
}
