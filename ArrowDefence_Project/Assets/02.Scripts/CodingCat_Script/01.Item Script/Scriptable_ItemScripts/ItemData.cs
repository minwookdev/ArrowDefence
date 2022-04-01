namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    [System.Serializable]
    public abstract class ItemData : ScriptableObject {
        [ReadOnly]
        public ITEMTYPE Item_Type;
        public ITEMGRADE Item_Grade;
        public string Item_Id;
        public int DefaultAmount;
        public Sprite Item_Sprite;

        [TermsPopup] public string NameTerms;
        [TermsPopup] public string DescTerms;

        public string NameByTerms {
            get {
                I2.Loc.LocalizedString loc = NameTerms;
                return loc;
            }
        }

        public string DescByTerms {
            get {
                I2.Loc.LocalizedString loc = DescTerms;
                return loc;
            }
        }

        public virtual string GetItemTypeStr(bool toUpper = false) {
            string itemtypestr = "";
            switch (Item_Type) {
                case ITEMTYPE.ITEM_MATERIAL:   itemtypestr = "material";   return (toUpper) ? itemtypestr.ToUpper() : itemtypestr;
                case ITEMTYPE.ITEM_CONSUMABLE: itemtypestr = "consumable"; return (toUpper) ? itemtypestr.ToUpper() : itemtypestr;
                case ITEMTYPE.ITEM_EQUIPMENT:  itemtypestr = "equipment";  return (toUpper) ? itemtypestr.ToUpper() : itemtypestr;
                default: throw new System.NotImplementedException();
            }
        }
    }
}
