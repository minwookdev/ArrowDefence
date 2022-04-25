namespace ActionCat {
    using UnityEngine;

    public class Item_SpArr : Item_Equipment {
        //SAVED VALUES
        private GameObject spArrowPref = null;
        private ASInfo[] skillInfos    = null;
        private SpArrCondition condition = null;
        private float specialArrDefaultSpeed = 12f;
        private float additionalSpeed = 0f;

        //NON_SAVED VALUES
        private ArrowSkillSet tempSkillSet = null;

        #region PROPERTY
        public ASInfo[] GetSkillInfos {
            get {
                if(skillInfos == null) {
                    skillInfos = new ASInfo[0];
                }
                return skillInfos;
            }
        }
        public SpArrCondition Condition {
            get {
                if(condition == null) {
                    throw new System.Exception("Special Arrow Condition is Null.");
                }
                return condition;
            }
        }
        #endregion
        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            return new Ability[0] { };
        }

        public bool TryGetSkillSet(out ArrowSkillSet skillset) {
            if(tempSkillSet == null) {
                skillset = null;
                return false;
            }

            skillset = tempSkillSet;
            return true;
        }

        public void Init(PlayerAbilitySlot ability, int poolQuatity) {
            if(spArrowPref.TryGetComponent<AD_Arrow>(out AD_Arrow arrow)) {
                arrow.SetSpeed(specialArrDefaultSpeed + additionalSpeed);
            }
            else {
                throw new System.Exception("Not Added Component in Arrow Prefab");
            }

            if (skillInfos.Length > 0) tempSkillSet = new ArrowSkillSet(skillInfos, AD_Data.POOLTAG_SPECIAL_ARROW, ability);
            else                       tempSkillSet = null;

            CCPooler.AddPoolList(AD_Data.POOLTAG_SPECIAL_ARROW, poolQuatity, spArrowPref, false);
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
                else                               return false;
            }
        }

        public bool IsInitSlot {
            get {
                if (spSlot == null) return false;
                else                return true;
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
            if(type == CHARGETYPE.NONE || count <= 0) {
                throw new System.Exception("New Condition instnace: Missing condition");
            }

            this.chargeType    = type;
            this.maxStackCount = count;
            this.maxCost       = cost;
            this.costIncrease  = increase;
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
            BattleProgresser.OnMonsterDeath += CostIncByKill;
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
            if(tempCost >= maxCost) {
                currentStackedCount++;
                if(currentStackedCount < maxStackCount) {
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
            if(currentCost >= maxCost) {
                currentStackedCount++;
                currentCost = 0f;
                spSlot.PlayNotify();
            }
            UpdateInterface();
        }
        #endregion

        public void Clear() {
            currentCost = 0f;
            addIncCost  = 0f;
            currentStackedCount = 0;
            tempCost = 0f;
            spSlot = null;

            switch (chargeType) {
                case CHARGETYPE.NONE:                                                   break;
                case CHARGETYPE.KILL: BattleProgresser.OnMonsterDeath -= CostIncByKill; break;
                case CHARGETYPE.TIME:                                                   break;
                case CHARGETYPE.ATCK: BattleProgresser.OnMonsterHit   -= CostIncByAtck; break;
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
