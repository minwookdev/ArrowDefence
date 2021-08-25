namespace CodingCat_Games
{
    using System;
    using UnityEngine;

    public enum ITEMTYPE
    {
        ITEM_CONSUMABLE,
        ITEM_MATERIAL,
        ITEM_EQUIPMENT
    }

    public enum ITEMGRADE
    { 
        GRADE_NORMAL    = 0,
        GRADE_MAGIC     = 1,
        GRADE_RARE      = 2,
        GRADE_HERO      = 3,
        GRADE_EPIC      = 4,
        GRADE_LEGENDARY = 5,
        GRADE_UNIQUE    = 6
    }

    [Serializable] //Abstract Class 는 Serailizable 속성 달려있어도 Inspector에서 표기되지 않는다
    public abstract class AD_item
    {
        public int Item_Id             { get; protected set; }
        public string Item_Name        { get; protected set; }
        public string Item_Desc        { get; protected set; }
        public int Item_Amount         { get; protected set; }
        public Sprite Item_Sprite      { get; protected set; }
        public ITEMTYPE Item_Type      { get; protected set; }
        public ITEMGRADE Item_Grade    { get; protected set; }

        //public int itemID;
        //public string itemName;
        //public string description;
        //public int amount;
        //public Sprite itemSprite;
        //public Enum_Itemtype itemType;
    }
}
