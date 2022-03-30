namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    public class AccessorySkillData : ScriptableObject {
        //Default Skill Data
        public string SkillId;
        public string SkillName;
        public string SkillDesc;
        public ACSP_TYPE EffectType;
        public SKILL_LEVEL SkillLevel;
        public Sprite SkillIconSprite;
        protected AccessorySPEffect SkillData;

        [TermsPopup] public string NameTerms;
        [TermsPopup] public string DescTerms;

        public AccessorySPEffect Skill() {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
