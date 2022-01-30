namespace ActionCat {
    public class Item_Accessory : Item_Equipment {
        private int maxNumberOfEffect;
        private AccessoryRFEffect[] effects;
        private AccessorySPEffect specialEffect;

        public AccessorySPEffect SPEffectOrNull {
            get {
                if (specialEffect != null) return specialEffect;
                else                       return null;
            }
        }

        public Item_Accessory(ItemData_Equip_Accessory item) : base() {
            //Set Equipment Item Type
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            //Set Default Item Data
            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Set Accessory Item Data
            //specialEffect = item.SpecialEffect;
            specialEffect = item.SPEffect;
        }

        public Item_Accessory(Item_Accessory item) : base() {
            //Set Equipment Item Type
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ACCESSORY;

            //Set Default Item Data
            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Set Accessory Item Data 
            specialEffect = item.specialEffect;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Accessory() : base() { }

        public void Init() {
            if (specialEffect != null) specialEffect.Init();
        }
    }
}
