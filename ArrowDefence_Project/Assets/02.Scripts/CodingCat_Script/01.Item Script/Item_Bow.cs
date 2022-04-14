namespace ActionCat {
    using UnityEngine;

    public class Item_Bow : Item_Equipment {
        private AD_BowSkill bowSkill_Fst;
        private AD_BowSkill bowSkill_Sec;
        private GameObject bowGameObject;

        /// <summary>
        /// Clone Constructor
        /// </summary>
        /// <param name="origin"></param>
        public Item_Bow(Item_Bow origin) : base(origin) {
            this.EquipType = origin.EquipType;

            this.bowGameObject = origin.bowGameObject;
            this.bowSkill_Fst  = origin.bowSkill_Fst;
            this.bowSkill_Sec  = origin.bowSkill_Sec;
            this.abilities     = origin.abilities;
        }

        /// <summary>
        /// Bow Item Constructor
        /// </summary>
        /// <param name="entity"></param>
        public Item_Bow(ItemData_Equip_Bow entity) : base(entity) {
            this.EquipType = EQUIP_ITEMTYPE.BOW;

            this.bowGameObject = entity.BowGameObject;
            this.bowSkill_Fst  = GetNewSkill(entity.SkillAsset_f);
            this.bowSkill_Sec  = GetNewSkill(entity.SkillAsset_s);
            this.abilities     = GetNewAbilities(entity.abilityDatas);
        }
        #region ES3
        public Item_Bow() { }
        #endregion

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            var tempList = new System.Collections.Generic.List<Ability>();
            for (int i = 0; i < abilities.Length; i++) {
                switch (abilities[i].AbilityType) {
                    case ABILITY_TYPE.DAMAGE:
                        var damage = abilities[i] as AbilityDamage;
                        if (damage != null) {
                            tempList.Add(new AbilityDamage(System.Convert.ToInt32(damage.GetCount())));
                        }
                        else {
                            CatLog.ELog("Ability Type Not Matched.");
                        } break;
                    case ABILITY_TYPE.CHARGEDAMAGE:
                        var chargedDamage = abilities[i] as AbilityChargedDamage;
                        if (chargedDamage != null) {
                            tempList.Add(new AbilityChargedDamage(chargedDamage.GetCount()));
                        }
                        else {
                            CatLog.ELog("Ability Type Not Matched.");
                        } break;
                    case ABILITY_TYPE.CRITICALCHANCE:
                        var criticalChance = abilities[i] as AbilityCritChance;
                        if (criticalChance != null) {
                            tempList.Add(new AbilityCritChance(System.Convert.ToInt32(criticalChance.GetCount())));
                        }
                        else {
                            CatLog.ELog("Ability Type Not Matched.");
                        } break;
                    case ABILITY_TYPE.CRITICALDAMAGE:
                        var criticalDamage = abilities[i] as AbilityCritDamage;
                        if (criticalDamage != null) {
                            tempList.Add(new AbilityCritDamage(criticalDamage.GetCount()));
                        }
                        else {
                            CatLog.ELog("Ability Type Not Matched.");
                        } break;
                    case ABILITY_TYPE.ARMORPENETRATE: throw new System.NotImplementedException();
                    default:                          throw new System.NotImplementedException();
                }
            } 
            return tempList.ToArray();
        }

        private AD_BowSkill GetNewSkill(BowSkillData data) {
            if (data == null) return null;
            switch (data.SkillType) {
                case BOWSKILL_TYPE.SKILL_EMPTY: 
                    throw new System.NotImplementedException();
                case BOWSKILL_TYPE.SKILL_SPREAD_SHOT:
                    var spreadShot = data as SkillDataSpreadShot;
                    if (spreadShot != null) {
                        return new Skill_Multiple_Shot(spreadShot);
                    }
                    else {
                        throw new System.Exception("This Bow Skill Type Not Matched.");
                    } 
                case BOWSKILL_TYPE.SKILL_RAPID_SHOT:
                    var rapidShot = data as SkillDataRapidShot;
                    if (rapidShot != null) {
                        return new Skill_Rapid_Shot(rapidShot);
                    }
                    else {
                        throw new System.Exception("This Bow Skill Type Not Matched.");
                    } 
                case BOWSKILL_TYPE.SKILL_ARROW_RAIN:
                    var rainArrow = data as SkillDataArrowRain;
                    if (rainArrow != null) {
                        return new Skill_Arrow_Rain(rainArrow);
                    }
                    else {
                        throw new System.Exception("This Bow Skill Type Not Matched.");
                    } 
                default: throw new System.NotImplementedException();
            }
        }

        public AD_BowSkill GetSkill(int idx)
        {
            if (idx == 0)
                return bowSkill_Fst;
            else if (idx == 1)
                return bowSkill_Sec;
            else
                CatLog.WLog($"GetSkill Method parameter idx is cannot exceed 1 [get parameter : {idx}]");
            return null;
        }

        public AD_BowSkill[] GetSkillsOrNull()
        {
            //AD_BowSkill[] skills = new AD_BowSkill[2] { this.bowSkill_Fst, 
            //                                            this.bowSkill_Sec};

            AD_BowSkill[] skills = { this.bowSkill_Fst, this.bowSkill_Sec };

            return skills;
        }

        public GameObject GetGameObjectOrNull() {
            return (bowGameObject != null) ? bowGameObject : null;
        }

        public override object GetItem() => this;

        public void Init(Transform initPos, Transform parentTr)
        {
            if(bowGameObject == null)
            {
                CatLog.ELog($"{termsName} is Bow GameObject is NULL, return Function");
                return;
            }

            var bowObject = GameObject.Instantiate(bowGameObject, initPos.position, Quaternion.Euler(0f, 0f, 90f), parentTr);
        }

        public AD_BowAbility Initialize(Transform initTr, Transform parentTr) {
            if(bowGameObject == null) {
                throw new System.Exception("Bow GameObejct Prefab is null.");
            }

            var prefab = GameObject.Instantiate(bowGameObject, initTr.position, Quaternion.Euler(0f, 0f, 90f), parentTr);
            if (prefab.TryGetComponent<AD_BowAbility>(out AD_BowAbility ability)) {
                ability.Initialize();
                return ability;
            }
            else {
                throw new System.Exception("Controller Ability Component is Null.");
            }
        }
    }
}
