namespace ActionCat
{
    using UnityEngine;

    [System.Serializable]
    public abstract class ItemData : ScriptableObject {
        [ReadOnly]
        public ITEMTYPE Item_Type;
        public ITEMGRADE Item_Grade;
        public string Item_Id;
        public int Item_Amount;
        public string Item_Name;
        public string Item_Desc;
        public Sprite Item_Sprite;

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
