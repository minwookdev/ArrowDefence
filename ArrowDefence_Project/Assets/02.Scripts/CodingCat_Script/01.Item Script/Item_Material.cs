namespace ActionCat
{
    public class Item_Material : AD_item, IStackable {
        /// <summary>
        /// Material Item Default Constructor
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="amount"></param>
        public Item_Material(ItemData_Mat entity, int amount) : base(entity, amount) { }
        #region ES3
        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this)
        /// </summary>
        public Item_Material() { }
        #endregion

        public void SetAmount(int value) => this.Item_Amount = value;

        public void IncAmount(int value) => this.Item_Amount += value;

        public void DecAmount(int value) => this.Item_Amount -= value;

        public override T GetItem<T>() {
            throw new System.NotImplementedException();
        }
    }
}
