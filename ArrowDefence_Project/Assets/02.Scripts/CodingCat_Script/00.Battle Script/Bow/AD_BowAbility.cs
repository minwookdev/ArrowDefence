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
            this.skills = GameManager.Instance.PlayerEquips.GetBowItem().GetSkillsOrNull(); //너무 많이 타고감
            for (int i = 0; i < skills.Length; i++) {
                if (skills[i] != null && skills[i].Type != BOWSKILL_TYPE.SKILL_EMPTY) { //null이 아니고, empty type이 아닌경우,
                    skills[i].Init(audioSource);
                    bowskilldelegate += skills[i].BowSpecialSkill;
                    CatLog.Log($"Bow SkillSlot {i} Init, SkillName: {skills[i].ToString()}");
                }
            }
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

        public void IsInitializedEquipments(Player_Equipments equipments, bool initialized) {
            IsInitEquipments = initialized;
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
