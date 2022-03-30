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

        protected string termsName;
        protected string termsDesc;

        #region PROPERTY_FIELD

        public string    GetID            { get { return Item_Id; } }
        public string    GetName          { get { return Item_Name; } }
        public string    GetDesc          { get { return Item_Desc; } }
        public int       GetAmount        { get { return Item_Amount; } }
        public Sprite    GetSprite        { get { return Item_Sprite; } }
        public ITEMTYPE  GetItemType      { get { return Item_Type; } }
        public ITEMGRADE GetGrade         { get { return Item_Grade; } }

        public string GetNameByTerms {
            get {
                if (string.IsNullOrEmpty(termsName)) {
                    return "";
                }

                I2.Loc.LocalizedString localString = termsName;
                return localString;
            }
        }

        public string GetDescByTerms {
            get {
                if (string.IsNullOrEmpty(termsDesc)) {
                    return "";
                }

                I2.Loc.LocalizedString localString = termsDesc;
                return localString;
            }
        }

        public string GetNameTerms {
            get => termsName;
        }

        public string GetDescTerms {
            get => termsDesc;
        }

        #endregion

        /// <summary>
        /// NEVER USE CASTING
        /// </summary>
        /// <returns></returns>
        public abstract object GetItem();

        //기본적인 아이템 정보는 이걸로 다 받는건 어떰??
        protected AD_item(string nameTerms, string descTerms) {
            this.termsName = nameTerms;
            this.termsDesc = descTerms;
        }

        #region ES3
        protected AD_item() {

        }
        ~AD_item() {

        }
        #endregion
    }
}
