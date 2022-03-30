namespace ActionCat {
    public enum EQUIP_ITEMTYPE {
        BOW      = 0,
        ARROW    = 1,
        ARTIFACT = 2
    }

    public abstract class Item_Equipment : AD_item {
        protected EQUIP_ITEMTYPE EquipType;
        protected Ability[] abilities = null;

        public Ability[] AbilitiesOrNull {
            get {
                if (abilities == null) {
                    return null;
                }
                return abilities;
            }
        }

        protected Item_Equipment(string nameTerms, string descTerms) : base(nameTerms, descTerms) {
            this.Item_Amount = 1;
            this.Item_Type = ITEMTYPE.ITEM_EQUIPMENT;
        }

        #region ES3
        protected Item_Equipment() {

        }
        #endregion

        public override object GetItem() => throw new System.NotImplementedException();

        protected abstract Ability[] GetNewAbilities(Ability[] abilities);

        public EQUIP_ITEMTYPE GetEquipType() => EquipType;

        //public abstract void Setup();
    }
}
