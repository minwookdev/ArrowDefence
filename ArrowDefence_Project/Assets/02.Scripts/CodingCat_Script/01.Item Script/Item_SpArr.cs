namespace ActionCat {
    using UnityEngine;

    public class Item_SpArr : Item_Equipment {
        //SAVED VALUES
        private GameObject spArrowPref = null;
        private ASInfo[] skillInfos = null;      //Skill이 존재하지 않더라도 Length = 0인 상태
        private SpArrCondition condition = null;
        private float specialArrDefaultSpeed = 12f;
        private float additionalSpeed = 0f;

        //NON_SAVED VALUES
        private ArrowSkillSet tempSkillSet = null;

        #region PROPERTY
        public ASInfo[] GetSkillInfos {
            get {
                if (skillInfos == null) {
                    skillInfos = new ASInfo[0];
                }
                return skillInfos;
            }
        }
        public SpArrCondition Condition {
            get {
                if (condition == null) {
                    throw new System.Exception("Special Arrow Condition is Null.");
                }
                return condition;
            }
        }
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

        public bool TryGetSkillSet(out ArrowSkillSet skillset) {
            if (tempSkillSet == null) {
                skillset = null;
                return false;
            }

            skillset = tempSkillSet;
            return true;
        }

        /// <summary>
        /// SP Arrow Battle Scene Setup
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="poolAmount"></param>
        public void Setup(PlayerAbilitySlot ability, int poolAmount) {
            if (spArrowPref.TryGetComponent<AD_Arrow>(out AD_Arrow arrow) == false) {
                CatLog.ELog("Arrow Component is Null.");
                return;
            }
            else {
                arrow.SetSpeed(specialArrDefaultSpeed + additionalSpeed);
            }

            // Set Skill and Add to Object Pool
            tempSkillSet = ArrowSkillSet.GetSpecialSkillSet(skillInfos, AD_Data.POOLTAG_SPECIAL_ARROW, ability);
            CCPooler.AddPoolList(AD_Data.POOLTAG_SPECIAL_ARROW, poolAmount, spArrowPref, false);
        }

        public override T GetItem<T>() {
            throw new System.NotImplementedException();
        }

        public Item_SpArr(ItemDt_SpArr entity) : base(entity) {
            EquipType = EQUIP_ITEMTYPE.ARROW;
            abilities = GetNewAbilities(entity.abilityDatas);
            spArrowPref = entity.MainArrowObj;
            var tempList = new System.Collections.Generic.List<ASInfo>();
            if (entity.ArrowSkillFst != null) tempList.Add(new ASInfo(entity.ArrowSkillFst));
            if (entity.ArrowSkillSec != null) tempList.Add(new ASInfo(entity.ArrowSkillSec));
            if (entity.ArrowSkillTrd != null) tempList.Add(new ASInfo(entity.ArrowSkillTrd));
            skillInfos = tempList.ToArray();
            condition = new SpArrCondition(entity.ChargeType, entity.MaxCost, entity.MaxStackCount, entity.CostIncrease);
            additionalSpeed = (IsExistAbility(ABILITY_TYPE.ARROWSPEED, out Ability speedIncrease)) ? speedIncrease.GetValueToSingle() : 0f;
        }
        #region ES3
        public Item_SpArr() { }
        ~Item_SpArr() { }
        #endregion
    }

    public sealed class SpArrCondition {
        //SAVED VALUE
        private CHARGETYPE chargeType;
        private float costIncrease;
        private int maxCost;
        private int maxStackCount;

        //NON-SAVED VALUE
        private float tempCost;
        private float currentCost;
        private float addIncCost;
        private int currentStackedCount;
        private UI.SwapSlots spSlot = null;

        #region PROPERTY
        public bool IsTypeTime {
            get {
                if (chargeType == CHARGETYPE.TIME) return true;
                else return false;
            }
        }

        public bool IsInitSlot {
            get {
                if (spSlot == null) return false;
                else return true;
            }
        }

        public bool IsReadyToLoad {
            get {
                if (currentStackedCount >= 1) {
                    currentStackedCount--;
                    UpdateInterface();
                    return true;
                }
                else
                    return false;
            }
        }
        #endregion

        public SpArrCondition(CHARGETYPE type, int cost, int count, float increase) {
            if (type == CHARGETYPE.NONE || count <= 0) {
                throw new System.Exception("New Condition instnace: Missing condition");
            }

            this.chargeType = type;
            this.maxStackCount = count;
            this.maxCost = cost;
            this.costIncrease = increase;
        }
        #region ES3
        public SpArrCondition() { }
        #endregion

        public void Initialize(UI.SwapSlots slot) {
            addIncCost = GameManager.Instance.GetGoAbility().IncreaseSpArrCost;
            switch (chargeType) {
                case CHARGETYPE.KILL: InitTypeKill(); break;
                case CHARGETYPE.TIME: InitTypeAtck(); break;
                case CHARGETYPE.ATCK: InitTypeTime(); break;
                default: throw new System.NotImplementedException();
            }

            if (slot == null) throw new System.Exception("Arrow Swap Slot is Null.");
            spSlot = slot;
            UpdateInterface();
        }

        #region INITIALIZE
        void InitTypeKill() {
            BattleProgresser.OnMonsterDeathByAttack += CostIncByKill;
        }

        void InitTypeAtck() {
            BattleProgresser.OnMonsterHit += CostIncByAtck;
        }

        void InitTypeTime() {

        }
        #endregion

        #region INCREASE
        void CostIncByAtck() {
            if (currentStackedCount >= maxStackCount) return;

            tempCost = currentCost + (costIncrease + addIncCost);
            if (tempCost >= maxCost) {
                currentStackedCount++;
                if (currentStackedCount < maxStackCount) {
                    tempCost -= maxCost;
                }
                else {
                    tempCost = 0f;
                }
                spSlot.PlayNotify();
            }
            currentCost = tempCost;
            UpdateInterface();
        }
        void CostIncByKill() {
            if (currentStackedCount >= maxStackCount) return;

            tempCost = currentCost + (costIncrease + addIncCost);
            if (tempCost >= maxCost) {
                currentStackedCount++;
                if (currentStackedCount < maxStackCount) {
                    tempCost -= maxCost;
                }
                else {
                    tempCost = 0f;
                }
                spSlot.PlayNotify();
            }
            currentCost = tempCost;
            UpdateInterface();
        }
        public void CostIncByTime() {
            if (currentStackedCount >= maxStackCount) return;
            currentCost = Time.deltaTime + (addIncCost * 0.01f);
            if (currentCost >= maxCost) {
                currentStackedCount++;
                currentCost = 0f;
                spSlot.PlayNotify();
            }
            UpdateInterface();
        }
        #endregion

        public void Clear() {
            currentCost = 0f;
            addIncCost = 0f;
            currentStackedCount = 0;
            tempCost = 0f;
            spSlot = null;

            switch (chargeType) {
                case CHARGETYPE.NONE: break;
                case CHARGETYPE.KILL: BattleProgresser.OnMonsterDeathByAttack -= CostIncByKill; break;
                case CHARGETYPE.TIME: break;
                case CHARGETYPE.ATCK: BattleProgresser.OnMonsterHit -= CostIncByAtck; break;
                default: break;
            }
        }

        void UpdateInterface() {
            if (!IsInitSlot) return;
            spSlot.SSlotUpdateCost(currentCost / maxCost);
            spSlot.SSSlotUpdateStack(currentStackedCount);
        }

        void CostInterfaceUpdate() {

        }

        void StackInterfaceUdpate() {

        }
    }
}
