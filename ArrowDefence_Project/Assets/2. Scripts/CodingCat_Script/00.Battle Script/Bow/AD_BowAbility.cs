namespace ActionCat {
    using ActionCat.Data;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour {
        //Slot Ability Class.
        PlayerAbilitySlot mainSlotAbility = null;
        PlayerAbilitySlot subSlotAbility  = null;

        //Is Initialized Ability Struct.
        bool isInitAbility = false;

        public void AddListnerToSkillDel(ref AD_BowController.BowSkillsDel bowskilldelegate) {
            var bowSkills = CCPlayerData.equipments.GetBowItem().GetSkillsOrNull();
            for (int i = 0; i < bowSkills.Length; i++) {
                if(bowSkills[i] != null) {
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
                isInitAbility = InitAbilityReturnBool();
            }

            switch (type) { //Return Damage Struct used Ability Slot Values..
                case ARROWTYPE.ARROW_MAIN: return new DamageStruct(mainSlotAbility, isCharged);
                case ARROWTYPE.ARROW_SUB:  return new DamageStruct(subSlotAbility,  isCharged);
                default:                         return new DamageStruct();
            }
        }

        private bool InitAbilityReturnBool() {
            if (CCPlayerData.equipments.IsEquippedArrowMain()) {
                mainSlotAbility = new PlayerAbilitySlot(CCPlayerData.ability.GetAbilitySlotMain());
            }
            if(CCPlayerData.equipments.IsEquippedArrowSub()) {
                subSlotAbility  = new PlayerAbilitySlot(CCPlayerData.ability.GetAbilitySubSlot());
            }
            return true;
        }
    }
}
