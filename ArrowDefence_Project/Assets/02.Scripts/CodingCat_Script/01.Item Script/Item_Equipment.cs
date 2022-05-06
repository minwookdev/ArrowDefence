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

        /// <summary>
        /// Equipment Item Clone Constructor
        /// </summary>
        /// <param name="origin"></param>
        protected Item_Equipment(Item_Equipment origin) : base(origin.termsName, origin.termsDesc) {
            this.Item_Id     = origin.Item_Id;
            this.Item_Amount = 1;
            this.Item_Sprite = origin.Item_Sprite;
            this.Item_Type   = ITEMTYPE.ITEM_EQUIPMENT;
            this.Item_Grade  = origin.Item_Grade;
        }

        /// <summary>
        /// Constructor for Item Entity
        /// </summary>
        /// <param name="entity"></param>
        protected Item_Equipment(ItemData_Equip entity) : base(entity.NameTerms, entity.DescTerms) {
            this.Item_Id     = entity.Item_Id;
            this.Item_Amount = 1;
            this.Item_Sprite = entity.Item_Sprite;
            this.Item_Type   = ITEMTYPE.ITEM_EQUIPMENT;
            this.Item_Grade  = entity.Item_Grade;
        }
        #region ES3
        protected Item_Equipment() {

        }
        #endregion

        public override object GetItem() => throw new System.NotImplementedException();

        protected abstract Ability[] GetNewAbilities(Ability[] abilities);

        public EQUIP_ITEMTYPE GetEquipType() => EquipType;

        public bool IsExistAbility(ABILITY_TYPE type, out Ability findAbility) {
            foreach (var ability in abilities) {
                if (ability.AbilityType == type) {
                    findAbility = ability;
                    return true;
                }
            }
            findAbility = null;
            return false;
        }
    }
}
