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

        public string GetTermsName {
            get => termsName;
        }

        public string GetTermsDesc {
            get => termsDesc;
        }

        #endregion

        /// <summary>
        /// NEVER USE CASTING
        /// </summary>
        /// <returns></returns>
        public abstract object GetItem();

        
        /// <summary>
        /// Only Terms Constructor
        /// </summary>
        /// <param name="nameTerms"></param>
        /// <param name="descTerms"></param>
        protected AD_item(string nameTerms, string descTerms) {
            this.termsName = nameTerms;
            this.termsDesc = descTerms;
        }

        /// <summary>
        /// Default Amount Constructor
        /// </summary>
        /// <param name="entity"></param>
        protected AD_item(ItemData entity) {
            this.Item_Id     = entity.Item_Id;
            this.Item_Amount = entity.DefaultAmount;
            this.Item_Sprite = entity.Item_Sprite;
            this.Item_Type   = entity.Item_Type;
            this.Item_Grade  = entity.Item_Grade;
            this.termsName   = entity.NameTerms;
            this.termsDesc   = entity.DescTerms;
        }

        /// <summary>
        /// Custom Amount Constructor
        /// </summary>
        /// <param name="entity">Item Entity</param>
        /// <param name="amount">Amount</param>
        protected AD_item(ItemData entity, int amount) {
            this.Item_Id     = entity.Item_Id;
            this.Item_Amount = amount;
            this.Item_Sprite = entity.Item_Sprite;
            this.Item_Type   = entity.Item_Type;
            this.Item_Grade  = entity.Item_Grade;
            this.termsName   = entity.NameTerms;
            this.termsDesc   = entity.DescTerms;
        }
        #region ES3
        protected AD_item() {

        }
        ~AD_item() {

        }
        #endregion
    }
}
