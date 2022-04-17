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

        public Item_Accessory(ItemData_Equip_Accessory entity) : base(entity) {
            this.EquipType     = EQUIP_ITEMTYPE.ARTIFACT;
            this.specialEffect = GetNewEffect(entity.SPEffectAsset);

            //assignment new temp abilities
            this.abilities = new Ability[] { };
            if (entity.abilityDatas != null) {
                this.abilities = GetNewAbilities(entity.abilityDatas);
            }
        }

        public Item_Accessory(Item_Accessory origin) : base(origin) {
            this.EquipType     = origin.EquipType;
            this.specialEffect = origin.specialEffect;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Accessory() : base() { }

        public void Init() {
            if (specialEffect != null) specialEffect.Init();
        }

        AccessorySPEffect GetNewEffect(AccessorySkillData data) {
            switch (data) {
                case SkillDataAimSight newData: return new Acsp_AimSight(newData);
                case SkillDataSlowTime newData: return new Acsp_SlowTime(newData);
                case null:                      return null;
                default: throw new System.NotImplementedException();
            }
        }

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            throw new System.NotImplementedException();
        }
    }
}
