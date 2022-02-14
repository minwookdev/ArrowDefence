namespace ActionCat {
    public enum EQUIP_ITEMTYPE
    {
        EQUIP_BOW       = 0,
        EQUIP_ARROW     = 1,
        EQUIP_ACCESSORY = 2
    }

    public abstract class Item_Equipment : AD_item
    {
        protected EQUIP_ITEMTYPE EquipType;
        protected Ability[] abilities = null;

        public Ability[] AbilitiesOrNull {
            get {
                if (abilities == null) {
                    return null;
                }
                else if (abilities.Length == 0) {
                    CatLog.WLog($"{Item_Name} Ability Size is 0");
                    return null;
                }
                return abilities;
            }
        }

        protected Item_Equipment() {
            this.Item_Amount = 1;
            this.Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
        }

        public override object GetItem() => throw new System.NotImplementedException();

        protected abstract Ability[] GetNewAbilities(Ability[] abilities);

        public EQUIP_ITEMTYPE GetEquipType() => EquipType;

        //public abstract void Setup();
    }
}
