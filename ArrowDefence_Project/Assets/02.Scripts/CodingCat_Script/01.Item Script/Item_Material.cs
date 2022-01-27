namespace ActionCat
{
    public class Item_Material : AD_item, IStackable    
    {
        public Item_Material(ItemData_Mat item)
        {
            //Item Type Field (Static)
            this.Item_Type = ITEMTYPE.ITEM_MATERIAL;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Amount = item.Item_Amount;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;
        }

        /// <summary>
        /// Constructor For Item Stack
        /// </summary>
        /// <param name="item">New Material Item Data</param>
        /// <param name="quantity">Item Quantity int</param>
        public Item_Material(ItemData_Mat item, int quantity)
        {
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

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this)
        /// </summary>
        public Item_Material() { }

        public override object GetItem() => this;

        public void SetAmount(int value) => this.Item_Amount = value;

        public void IncAmount(int value) => this.Item_Amount += value;

        public void DecAmount(int value) => this.Item_Amount -= value;
    }
}
