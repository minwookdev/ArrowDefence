namespace ActionCat {
    using ActionCat.Data;
    using UnityEngine;

    public class AD_BowAbility : MonoBehaviour {
        //Ability Struct (Main, Sub).
        private AbilityStruct mainSlotStr;
        private AbilityStruct subSlotStr;

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
        public DamageStruct GetDamage(LOAD_ARROW_TYPE type) {
            if(isInitAbility == false) {
                isInitAbility = InitAbilityReturnBool();
            }

            switch (type) {
                case LOAD_ARROW_TYPE.ARROW_MAIN: //Return Damage Struct used Ability Slot Values..
                    return new DamageStruct(mainSlotStr.RawDamage, 
                                            mainSlotStr.CriticalChance,
                                            mainSlotStr.CriticalMultiplier, 
                                            mainSlotStr.ArmorPenetrating);
                case LOAD_ARROW_TYPE.ARROW_SUB:
                    return new DamageStruct(subSlotStr.RawDamage, 
                                            subSlotStr.CriticalChance,
                                            subSlotStr.CriticalMultiplier, 
                                            subSlotStr.ArmorPenetrating);
                default: 
                    return new DamageStruct(0f, 0, 0f, 0);
            }
        }

        private bool InitAbilityReturnBool() {
            if (CCPlayerData.equipments.IsEquippedArrowMain()) {
                mainSlotStr = new AbilityStruct(CCPlayerData.status.GetMainSlotAbility());
                CatLog.Log($"Main Arrow Slot Damage is {mainSlotStr.RawDamage}");
            }
            if(CCPlayerData.equipments.IsEquippedArrowSub()) {
                subSlotStr = new AbilityStruct(CCPlayerData.status.GetSubSlotAbility());
                CatLog.Log($"Sub Arrow Slot Damage is {subSlotStr.RawDamage}");
            }
            return true;
        }
    }
}
