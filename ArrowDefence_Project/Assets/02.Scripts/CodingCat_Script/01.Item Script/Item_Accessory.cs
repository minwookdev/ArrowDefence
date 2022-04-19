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

        public bool IsExistEffect {
            get {
                return (specialEffect != null);
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

        AccessorySPEffect GetNewEffect(AccessorySkillData entity) {
            switch (entity.EffectType) {
                case ACSP_TYPE.SPEFFECT_NONE:     throw new System.Exception();
                //=============================================================================================================
                case ACSP_TYPE.SPEFFECT_AIMSIGHT: 
                    if (entity is SkillDataAimSight aimsight) {
                        return new Acsp_AimSight(aimsight);
                    }
                    else {
                        throw new System.Exception("Type <-> Class Not Match !");
                    }
                //=============================================================================================================
                case ACSP_TYPE.SPEEFECT_SLOWTIME:
                    if (entity is SkillDataSlowTime slowtime) {
                        return new Acsp_SlowTime(slowtime);
                    }
                    else {
                        throw new System.Exception("Type <-> Class Not Match !");
                    }
                //=============================================================================================================
                default: throw new System.NotImplementedException();
            }
        }

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            throw new System.NotImplementedException();
        }
    }
}
