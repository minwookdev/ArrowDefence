namespace ActionCat {
    using UnityEngine;
    using I2.Loc;

    public class AccessoryRFSkillData : ScriptableObject
    {
        //Default Skill Data
        public string SkillId;
        public string SkillName;
        [TextArea(3, 7)] 
        public string SkillDesc;
        public RFEF_TYPE EffectType;
        protected AccessoryRFEffect SkillData;

        [TermsPopup] public string NameTerms;
        [TermsPopup] public string DescTerms;
    }
}
