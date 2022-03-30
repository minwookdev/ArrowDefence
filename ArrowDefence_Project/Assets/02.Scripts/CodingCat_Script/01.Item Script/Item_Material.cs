namespace ActionCat
{
    public class Item_Material : AD_item, IStackable {
        /// <summary>
        /// Constructor For Item Stack
        /// </summary>
        /// <param name="item">New Material Item Data</param>
        /// <param name="quantity">Item Quantity int</param>
        public Item_Material(ItemData_Mat item, int quantity) : base(item.NameTerms, item.DescTerms) {
            //Item Type Field (Static)
            this.Item_Type = ITEMTYPE.ITEM_MATERIAL;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Init Item Amount Quantity Value
            this.Item_Amount = quantity;
        } 
        #region ES3
        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this)
        /// </summary>
        public Item_Material() { }
        #endregion

        public override object GetItem() => this;

        public void SetAmount(int value) => this.Item_Amount = value;

        public void IncAmount(int value) => this.Item_Amount += value;

        public void DecAmount(int value) => this.Item_Amount -= value;
    }
}
