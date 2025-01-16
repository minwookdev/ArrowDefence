namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    public class ArrowSkillData : ScriptableObject {
        //Basic Arrow Skill Data
        //MEMBER
        public string SkillId;
        public string SkillName;
        public string SkillDesc;
        public SKILL_LEVEL SkillLevel;
        public ARROWSKILL SkillType;
        public Sprite IconSprite;
        protected ArrowSkill skillData = null;
        public ARROWSKILL_ACTIVETYPE ActiveType;
        public ACEffector2D[] effects = null;
        public AudioClip[] Sounds;

        [TermsPopup] public string NameTerms = null;
        [TermsPopup] public string DescTerms = null;

        //PROPERTIES
        public ArrowSkill ArrowSkill {
            get {
                if (skillData != null)
                    return skillData;
                else
                    return null;
            }
        }
    }
}
