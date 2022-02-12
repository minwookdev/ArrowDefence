namespace ActionCat {
    using ActionCat.Data;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour {
        PlayerAbilitySlot[] abilitySlots = null;

        //Is Initialized Ability Struct.
        bool isInitAbility = false;
        public bool IsInitEquipments { get; private set; } = false;

        /// <summary>
        /// Run Player Equipment Controller Initialize.
        /// </summary>
        public void Initialize() {
            var originAbility = CCPlayerData.ability;
            abilitySlots = new PlayerAbilitySlot[2] {
                new PlayerAbilitySlot(originAbility.GetAbilityMain()),
                new PlayerAbilitySlot(originAbility.GetAbilitySub()),
                //new PlayerAbilitySlot(originAbility.GetAbilitySpecial())
            };
            isInitAbility = true;
        }

        public void AddListnerToSkillDel(ref AD_BowController.BowSkillsDel bowskilldelegate) {
            var bowSkills = CCPlayerData.equipments.GetBowItem().GetSkillsOrNull();
            for (int i = 0; i < bowSkills.Length; i++) {
                if(bowSkills[i] != null) {
                    bowSkills[i].Init();
                    bowskilldelegate += bowSkills[i].BowSpecialSkill;
                    CatLog.Log($"Skill Slot {i} Init, Skill Name : {bowSkills[i].ToString()}");
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

        public PlayerAbilitySlot GetAbility(byte index) {
            return abilitySlots[index];
        }

        public void IsInitializedEquipments(Player_Equipments equipments, bool initialized) {
            IsInitEquipments = initialized;
        }
    }
}
