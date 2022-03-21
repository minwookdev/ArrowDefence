namespace ActionCat {
    using UnityEngine;

    public class Item_Bow : Item_Equipment {
        private AD_BowSkill bowSkill_Fst;
        private AD_BowSkill bowSkill_Sec;
        private GameObject bowGameObject;

        //장비하고 있는 BowItem 에서 BowSkill을 가져오기 또는 Equipment 부분에서 또 세분화해서 나누기
        //1. 활 2. 화살 3. 활 악세서리 장비 (스코프, 충전기 등의 기능성 장비아이템)
        //public void Awake() => BowSkill = new Skill_Arrow_Rain();

        //AD_Equipment를 상속 받는 아이템에서 활 스킬 함수 자체에 먹여주는 값 (ex : Arrow Num 변수, Angle 변수)을 부여해서 
        //PlayerData로 들어가는 함수를 가볍게 해주기

        /// <summary>
        /// Bow Item Constructor (Item Data Type)
        /// </summary>
        /// <param name="item">Bow Item Data Address</param>
        public Item_Bow(ItemData_Equip_Bow item) : base() {
            this.EquipType = EQUIP_ITEMTYPE.BOW;

            this.Item_Id      = item.Item_Id;
            this.Item_Name    = item.Item_Name;
            this.Item_Desc    = item.Item_Desc;
            this.Item_Sprite  = item.Item_Sprite;
            this.Item_Grade   = item.Item_Grade;

            this.bowGameObject = item.BowGameObject;
            this.bowSkill_Fst  = GetNewSkill(item.SkillAsset_f);
            this.bowSkill_Sec  = GetNewSkill(item.SkillAsset_s);

            this.abilities = GetNewAbilities(item.abilityDatas);
        }

        /// <summary>
        /// Bow Item Constructor (Item Type)
        /// </summary>
        /// <param name="item"></param>
        public Item_Bow(Item_Bow item) : base() {
            this.EquipType = EQUIP_ITEMTYPE.BOW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.bowGameObject = item.bowGameObject;
            this.bowSkill_Fst  = item.bowSkill_Fst;
            this.bowSkill_Sec  = item.bowSkill_Sec;

            this.abilities = item.abilities;
        }
        #region ES3
        public Item_Bow() : base() { }
        #endregion

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            var tempList = new System.Collections.Generic.List<Ability>();
            for (int i = 0; i < abilities.Length; i++) {
                switch (abilities[i].AbilityType) {
                    case ABILITY_TYPE.DAMAGE:
                        var damage = abilities[i] as AbilityDamage;
                        if (damage != null) {
                            tempList.Add(new AbilityDamage(System.Convert.ToInt16(damage.GetCount())));
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
                            tempList.Add(new AbilityCritChance(System.Convert.ToByte(criticalChance.GetCount())));
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
                CatLog.ELog($"{Item_Name} is Bow GameObject is NULL, return Function");
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
