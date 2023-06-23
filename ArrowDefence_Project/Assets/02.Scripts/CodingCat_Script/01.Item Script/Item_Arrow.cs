﻿namespace ActionCat {
    using UnityEngine;

    public class Item_Arrow : Item_Equipment {
        //Required Saving Variables
        //Arrow Prefab
        private GameObject MainArrowObject;
        private GameObject LessArrowObject;
        float speed;
        //Arrow Skill Data Class
        ASInfo arrowSkillInfoFst;
        ASInfo arrowSkillInfoSec;
        //Arrow Default Hit Effect
        ACEffector2D[] effects;

        //↘Not Save This SkillSet Class [TEMP]
        ArrowSkillSet arrowSkillSets = null;
        public ArrowSkillSet ArrowSkillSets { 
            get {
                if (arrowSkillSets != null)
                    return arrowSkillSets;
                else
                    return null;
            }
        }
        public bool TryGetSkillSet(out ArrowSkillSet set) {
            if (this.arrowSkillSets == null) {
                set = null;
                return false;
            }

            set = this.arrowSkillSets;
            return true;
        }
        
        public ASInfo[] SkillInfosOrNull {
            get {
                ASInfo[] skillinfo = new ASInfo[2] {
                    arrowSkillInfoFst, arrowSkillInfoSec
                };
                return skillinfo;
            }
        }

        /// <summary>
        /// Arrow Item Entity Constructor
        /// </summary>
        /// <param name="entity"></param>
        public Item_Arrow(ItemData_Equip_Arrow entity) : base(entity) {
            this.EquipType = EQUIP_ITEMTYPE.ARROW;

            this.MainArrowObject = entity.MainArrowObj;
            this.LessArrowObject = entity.LessArrowObj;

            this.arrowSkillInfoFst = (entity.ArrowSkillFst != null) ? new ASInfo(entity.ArrowSkillFst) : null;
            this.arrowSkillInfoSec = (entity.ArrowSkillSec != null) ? new ASInfo(entity.ArrowSkillSec) : null;
            this.abilities         = GetNewAbilities(entity.abilityDatas);
            this.effects           = entity.effects;

            //assignment arrow speed value
            this.speed = (IsExistAbility(ABILITY_TYPE.ARROWSPEED, out Ability speedIncrease)) ?
                GameGlobal.DefaultArrowSpeed + speedIncrease.GetValueToSingle() : GameGlobal.DefaultArrowSpeed;
        }

        /// <summary>
        /// Item Clone Constructor
        /// </summary>
        /// <param name="origin"></param>
        public Item_Arrow(Item_Arrow origin) : base(origin) {
            this.EquipType         = origin.EquipType;
            this.MainArrowObject   = origin.MainArrowObject;
            this.LessArrowObject   = origin.LessArrowObject;
            this.speed             = origin.speed;
            this.arrowSkillInfoFst = origin.arrowSkillInfoFst;
            this.arrowSkillInfoSec = origin.arrowSkillInfoSec;
            this.abilities         = origin.abilities;
            this.effects           = origin.effects;
        }

        /// <summary>
        /// 현 Arrow Item Class의 First, Seconds Skill이 Battle Stage에서 Init되어야하는 Type의 Skill인지 확인하고,
        /// BattleStage에서 사용되는 Skill이면 매개변수로 들어온 Index에 따른 skill class를 반환하거나, null반환.
        /// </summary>
        /// <param name="index"> 0 == first arrow skill, 0 != seconds arrow skill </param>
        /// <returns></returns>
        ASInfo GetBattleStageInitSkill(byte index) {
            var skill = (index == 0) ? arrowSkillInfoFst : arrowSkillInfoSec;
            return (skill != null && skill.ActiveType != ARROWSKILL_ACTIVETYPE.EMPTY && skill.ActiveType != ARROWSKILL_ACTIVETYPE.BUFF) ? skill : null;
        }

        #region ES3
        public Item_Arrow() : base() { }
        #endregion

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            var tempList = new System.Collections.Generic.List<Ability>();
            string log = "";
            for (int i = 0; i < abilities.Length; i++) {
                Ability newAbility = null;  //New Ability Assignment 시험적용
                switch (abilities[i].AbilityType) {
                    //======================================================================= <<< INHERENCE ABILITY >>> =======================================================================
                    case ABILITY_TYPE.ARROWDAMAGEINC: // ★★ 구형로직
                        var incArrDamage = abilities[i] as IncArrowDamageRate;
                        if (incArrDamage != null) {
                            tempList.Add(new IncArrowDamageRate(incArrDamage.Value));
                            continue;
                        }
                        else {
                            log = $"TypeEnum: {nameof(ABILITY_TYPE.ARROWDAMAGEINC)}, Class: {nameof(IncArrowDamageRate)}";
                        }
                        break; 
                    case ABILITY_TYPE.ARROWSPEED:     // ★★ 구형로직
                        var speed = abilities[i] as AbilitySpeed;
                        if (speed != null) {
                            tempList.Add(new AbilitySpeed(speed.Value));
                            continue;
                        }
                        else {
                            log = $"TypeEnum: {(nameof(ABILITY_TYPE.ARROWSPEED))}, Class: {nameof(AbilitySpeed)}";
                        }
                        break;
                    case ABILITY_TYPE.PROJECTILEDAMAGE:
                        var incProjectileDamage = abilities[i] as IncProjectileDamage;
                        newAbility = (incProjectileDamage != null) ? new IncProjectileDamage(incProjectileDamage.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum: {(nameof(ABILITY_TYPE.PROJECTILEDAMAGE))}, Class: {(nameof(IncProjectileDamage))}";
                        break;
                    case ABILITY_TYPE.SPELLDAMAGE:
                        var incSpellDamage = abilities[i] as IncSpellDamage;
                        newAbility = (incSpellDamage != null) ? new IncSpellDamage(incSpellDamage.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum: {(nameof(ABILITY_TYPE.SPELLDAMAGE))}, Class: {nameof(IncSpellDamage)}";
                        break;
                    //========================================================================= <<< PUBLIC ABILITY >>> ========================================================================
                    case ABILITY_TYPE.DAMAGE:
                        var abilityDamage = abilities[i] as AbilityDamage;
                        newAbility = (abilityDamage != null) ? new AbilityDamage(abilityDamage.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum: {(nameof(ABILITY_TYPE.DAMAGE))}, Class: {nameof(AbilityDamage)}";
                        break;
                    case ABILITY_TYPE.CRITICALCHANCE:
                        var abilityCritChance = abilities[i] as AbilityCritChance;
                        newAbility = (abilityCritChance != null) ? new AbilityCritChance(abilityCritChance.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum : {nameof(ABILITY_TYPE.CRITICALCHANCE)}, Class: {nameof(AbilityCritChance)}";
                        break;
                    case ABILITY_TYPE.CRITICALDAMAGE:
                        var abilityCritDamage = abilities[i] as AbilityCritDamage;
                        newAbility = (abilityCritDamage != null) ? new AbilityCritDamage(abilityCritDamage.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum: {nameof(ABILITY_TYPE.CRITICALDAMAGE)}, Class: {nameof(AbilityCritDamage)}";
                        break;
                    case ABILITY_TYPE.ARMORPENETRATE:
                        var abilityPenetration = abilities[i] as PenetrationArmor;
                        newAbility = (abilityPenetration != null) ? new PenetrationArmor(abilityPenetration.Value) : null;
                        log = (newAbility != null) ? "" : $"TypeEnum: {nameof(ABILITY_TYPE.ARMORPENETRATE)}, Class: {nameof(PenetrationArmor)}";
                        break;
                    //========================================================================== <<< OTHER ABILITY >>> ========================================================================
                    default: throw new System.NotImplementedException(); // 허용안함 
                }

                //
                if (newAbility != null) {
                    //Type이 매치되어 새로운 어빌리티 class가 할당된 상태
                    tempList.Add(newAbility);
                }
                else { 
                    //Ability Type과 class가 Match되지 않아서 NewAbility에 새롭게 할당되지 않았음
                    CatLog.ELog($"Arrow Ability Type Not Matched !. Exception Message: \n" + log);
                    continue;
                }
            }
            return tempList.ToArray();
        }

        public GameObject GetObject_MainArrow()
        {
            if(MainArrowObject == null)
            {
                CatLog.ELog($"{termsName} Item is Not have a Main Arrow Object");
                return null;
            }
            else
            {
                return MainArrowObject;
            }
        }

        public GameObject GetObject_LessArrow()
        {
            if(LessArrowObject == null)
            {
                CatLog.ELog($"{termsName} Item is Not have a Less Arrow Object");
                return null;
            }
            else
            {
                return LessArrowObject;
            }
        }

        bool TryFindEffect(out ACEffector2D[] array, ref int arraySize) {
            if (arrowSkillInfoFst == null && arrowSkillInfoSec == null) {
                // Non-Skill - Use ArrItem Default Effects
                array = null; 
                return false;
            }
            else {
                // Has-Skill - Use HitType Skill inherence Effects
                if (arrowSkillInfoFst != null && arrowSkillInfoSec != null) {
                    // two-skill
                    var arrSkill_f = arrowSkillInfoFst.SkillData as AttackActiveTypeAS;
                    var arrSkill_s = arrowSkillInfoSec.SkillData as AttackActiveTypeAS;
                    if (arrSkill_f != null && arrSkill_s != null) {
                        throw new System.Exception($"Item Initialize Failed, {termsName} has Duplicate skill type.");
                    }
                    else if (arrSkill_f != null) {  // firstslot is HitType skill
                        if(arrSkill_f.IsEffectUser == true) {
                            array = arrSkill_f.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                    else if (arrSkill_s != null) {  // secondsSlot is HitType skill
                        if(arrSkill_s.IsEffectUser == true) {
                            array = arrSkill_s.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                }
                else {
                    // one-skill
                    if (arrowSkillInfoFst != null && arrowSkillInfoFst.SkillData is AttackActiveTypeAS hitSkillFst) {
                        if(hitSkillFst.IsEffectUser == true) { // first slot is HitType Skill
                            array = hitSkillFst.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                    else if(arrowSkillInfoSec != null && arrowSkillInfoSec.SkillData is AttackActiveTypeAS hitSkillSec) {
                        if(hitSkillSec.IsEffectUser == true) { // seconds slot is HitType Skill
                            array = hitSkillSec.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                }
            }

            array = null; 
            return false;
        }

        /// <summary>
        /// Battle Scene Setup
        /// </summary>
        /// <param name="mainArrowObjTag"></param>
        /// <param name="lessArrowObjTag"></param>
        /// <param name="poolAmount"></param>
        /// <param name="ability"></param>
        /// <exception cref="System.Exception"></exception>
        public void Setup(string mainArrowObjTag, string lessArrowObjTag, int poolAmount, PlayerAbilitySlot ability) {
            if (MainArrowObject == null || LessArrowObject == null) {
                throw new System.Exception($"ArrowItem : {termsName} is ArrowPrefab NULL.");
            }

            bool isMainArr = MainArrowObject.TryGetComponent<AD_Arrow>(out AD_Arrow mainArrow);
            bool isLessArr = LessArrowObject.TryGetComponent<AD_Arrow_less>(out AD_Arrow_less lessArrow);

            if(isMainArr == false || isLessArr == false) {
                throw new System.Exception($"ArrItem : {termsName} is Component Not Assignment.");
            }

            System.Collections.Generic.List<string> effectPoolTags = new System.Collections.Generic.List<string>();
            int effectArrayLenght = 0;

            // Add PoolList Effects 
            if(TryFindEffect(out ACEffector2D[] effectArray, ref effectArrayLenght)) {
                for (int i = 0; i < effectArray.Length; i++) {
                    string effectPoolTag = mainArrowObjTag + AD_Data.POOLTAG_HITEFFECT + i.ToString();
                    CCPooler.AddPoolList(effectPoolTag, 3, effectArray[i].gameObject, isTracking: false);
                    effectPoolTags.Add(effectPoolTag);
                }
            }
            else {
                if(effects == null || effects.Length == 0) {
                    effectArrayLenght = 0;
                }
                else {
                    effectArrayLenght = effects.Length;
                    for (int i = 0; i < effects.Length; i++) {
                        string effectPoolTag = mainArrowObjTag + AD_Data.POOLTAG_HITEFFECT + i.ToString();
                        CCPooler.AddPoolList(effectPoolTag, 3, effects[i].gameObject, isTracking: false);
                        effectPoolTags.Add(effectPoolTag);
                    }
                }
            }

            // Init ArrSkillSets
            //arrowSkillSets = new ArrowSkillSet(GetBattleStageInitSkill(0), GetBattleStageInitSkill(1), mainArrowObjTag, ability);
            arrowSkillSets = ArrowSkillSet.GetSkillSet(GetBattleStageInitSkill(0), GetBattleStageInitSkill(1), mainArrowObjTag, ability);

            mainArrow.SetEffectInfo(effectPoolTags.ToArray());
            lessArrow.SetEffectInfo(effectPoolTags.ToArray());

            mainArrow.PowerFactor = speed;
            lessArrow.PowerFactor = speed;

            //Set Arrow Prefabs Speed
            //bool isExistsSpeedAbility = IsExistAbility(ABILITY_TYPE.ARROWSPEED, out Ability incSpeed);
            //mainArrow.PowerFactor = (isExistsSpeedAbility) ? speed + incSpeed.GetValueToSingle() : speed;
            //lessArrow.PowerFactor = (isExistsSpeedAbility) ? speed + incSpeed.GetValueToSingle() : speed;

            //Create Pools of Arrow Object
            CCPooler.AddPoolList(mainArrowObjTag, poolAmount, MainArrowObject, isTracking: false);
            CCPooler.AddPoolList(lessArrowObjTag, poolAmount, LessArrowObject, isTracking: false);
        }

        public void Release() {
            //Clear Arrow Skill Sets Class 
            if (arrowSkillSets == null)
                return;
            arrowSkillSets = null;

            //Release Data Arrow Skill
            if (arrowSkillInfoFst != null) arrowSkillInfoFst.Release();
            if (arrowSkillInfoSec != null) arrowSkillInfoSec.Release();
            
            //Remove Origin Prefab EffectTags string Array
            MainArrowObject.GetComponent<AD_Arrow>().RemoveEffectInfo();
            LessArrowObject.GetComponent<AD_Arrow_less>().RemoveEffectInfo();
        }

        public override T GetItem<T>() {
            throw new System.NotImplementedException();
        }
    }
}
