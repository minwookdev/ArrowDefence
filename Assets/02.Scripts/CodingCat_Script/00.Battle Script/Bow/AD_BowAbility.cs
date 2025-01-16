namespace ActionCat {
    using ActionCat.Data;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour {
        PlayerAbilitySlot[] abilitySlots = null;
        SpArrCondition spArrCondition    = null;
        AD_BowSkill[] skills = null;

        //Is Initialized Ability Struct.
        bool isInitAbility = false;
        bool isTimerUse    = false;
        public bool IsInitEquipments { get; private set; } = false;
        public SpArrCondition Condition {
            get {
                if(spArrCondition == null) {
                    throw new System.Exception();
                }
                return spArrCondition;
            }
        }

        /// <summary>
        /// Run Player Equipment Controller Initialize.
        /// </summary>
        public void Initialize() {
            var originAbility = CCPlayerData.ability;
            abilitySlots = new PlayerAbilitySlot[3] {
                new PlayerAbilitySlot(originAbility.GetAbilityMain),
                new PlayerAbilitySlot(originAbility.GetAbilitySub),
                new PlayerAbilitySlot(originAbility.GetAbilitySpecial)
            };
            isInitAbility = true;
        }

        public void AddListnerToSkillDel(ref AD_BowController.BowSkillsDel bowskilldelegate, Audio.ACSound audioSource) {
            var skillList = new System.Collections.Generic.List<AD_BowSkill>(GameManager.Instance.PlayerEquips.GetBowItem().GetSkillsOrNull()); //너무 많이 타긴하는데 중간에 체크안해도 문제는 없음
            for (int i = skillList.Count - 1; i >= 0; i--) {
                if (skillList[i] != null && skillList[i].Type != BOWSKILL_TYPE.SKILL_EMPTY) { //null 아니고, empty type이 아닌경우,
                    skillList[i].Init(audioSource);
                    bowskilldelegate += skillList[i].BowSpecialSkill;
                    CatLog.Log($"BowSkill Init. SkillName: {skillList[i].ToString()}");
                }
                else {
                    skillList.RemoveAt(i);
                }
            }
            this.skills = skillList.ToArray();
        }

        /// <summary>
        /// Ability Struct 변수로 계산하고, DamageStruct로 리턴.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public DamageStruct GetDamage(ARROWTYPE type, bool isCharged) {
            if(isInitAbility == false) {
                throw new System.Exception("Ability Not Initialized.");
            }

            switch (type) {
                case ARROWTYPE.ARROW_MAIN:    return new DamageStruct(abilitySlots[0], isCharged);
                case ARROWTYPE.ARROW_SUB:     return new DamageStruct(abilitySlots[1], isCharged);
                case ARROWTYPE.ARROW_SPECIAL: return new DamageStruct(abilitySlots[2], isCharged);
                default: throw new System.NotImplementedException();
            }
        }

        private void Update() {
            if (isTimerUse) {
                spArrCondition.CostIncByTime();
            }
        }

        public PlayerAbilitySlot GetAbility(byte index) {
            if(index < 0 || index > 2) {
                throw new System.NotImplementedException("this slot index is Not Implemented !");
            }
            return abilitySlots[index];
        }

        public void ArrowsSetupCompleted() {
            IsInitEquipments = true;
        }

        public void SetCondition(SpArrCondition condition, UI.SwapSlots slot) {
            spArrCondition = condition;
            spArrCondition.Initialize(slot);
            isTimerUse = spArrCondition.IsTypeTime;
        }

        private void OnDestroy() {
            abilitySlots = null;
            if (spArrCondition != null) {
                spArrCondition.Clear();
            }
            spArrCondition = null;

            for (int i = 0; i < skills.Length; i++) {
                if (skills[i] != null) {
                    skills[i].Release();
                }
            }

            skills.Foreach((skill) => {
                if (skill != null) {
                    skill.Release();
                }
            });
            skills = null;
        }
    }
}
