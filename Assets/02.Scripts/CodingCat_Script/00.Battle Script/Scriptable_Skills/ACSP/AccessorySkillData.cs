namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    public class AccessorySkillData : ScriptableObject {
        //Default Skill Data
        public string SkillId;
        public ACSP_TYPE EffectType;
        public SKILL_LEVEL SkillLevel;
        public Sprite SkillIconSprite;
        protected AccessorySPEffect SkillData; // <-- not using this..

        [TermsPopup] public string NameTerms;
        [TermsPopup] public string DescTerms;

        //Condition
        public ARTCONDITION ConditionType = ARTCONDITION.NONE;
        public int MaxStack = 0;
        public float MaxCost = 0f;
        public float IncreaseCostCount = 0f;
        public float CoolDownTime = 0f;

        //Sound Effect
        public AudioClip SoundEffect;

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

        protected AccessorySkillData(ACSP_TYPE type) {
            EffectType = type;
        }
    }
}
