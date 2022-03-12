namespace ActionCat {
    using System;
    using UnityEngine;

    public enum ITEMTYPE {
        ITEM_MATERIAL,
        ITEM_CONSUMABLE,
        ITEM_EQUIPMENT
    }

    public enum ITEMGRADE { 
        GRADE_NORMAL    = 0,
        GRADE_MAGIC     = 1,
        GRADE_RARE      = 2,
        GRADE_HERO      = 3,
        GRADE_EPIC      = 4,
        GRADE_LEGENDARY = 5,
        GRADE_UNIQUE    = 6
    }

    interface IStackable  {
        void SetAmount(int value);
        void IncAmount(int value);
        void DecAmount(int value);
    }

    [Serializable] 
    public abstract class AD_item {
        protected string    Item_Id;
        protected string    Item_Name;
        protected string    Item_Desc;
        protected int       Item_Amount;
        protected Sprite    Item_Sprite;
        protected ITEMTYPE  Item_Type;
        protected ITEMGRADE Item_Grade;

        #region PROPERTY_FIELD

        public string    GetID            { get { return Item_Id; } }
        public string    GetName          { get { return Item_Name; } }
        public string    GetDesc          { get { return Item_Desc; } }
        public int       GetAmount        { get { return Item_Amount; } }
        public Sprite    GetSprite        { get { return Item_Sprite; } }
        public ITEMTYPE  GetItemType      { get { return Item_Type; } }
        public ITEMGRADE GetGrade         { get { return Item_Grade; } }

        #endregion

        /// <summary>
        /// NEVER USE CASTING
        /// </summary>
        /// <returns></returns>
        public abstract object GetItem();
    }


}
