namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    public class AccessorySkillData : ScriptableObject {
        //Default Skill Data
        public string SkillId;
        public ACSP_TYPE EffectType;
        public SKILL_LEVEL SkillLevel;
        public Sprite SkillIconSprite;
        protected AccessorySPEffect SkillData;

        [TermsPopup] public string NameTerms;
        [TermsPopup] public string DescTerms;

        public string NameByTerms {
            get {
                LocalizedString loc = NameTerms;
                return loc;
            }
        }

        public string DescByTerms {
            get {
                LocalizedString loc = DescTerms;
                return loc;
            }
        }

        public AccessorySPEffect Skill() {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
