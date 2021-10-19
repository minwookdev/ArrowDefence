namespace ActionCat
{
    using UnityEngine;

    public class BowSkillData : ScriptableObject
    {
        //Basic Skill Data
        public string SkillName;
        public string SkillDesc;
        public BOWSKILL_TYPE SkillType;
        public SKILL_LEVEL SkillLevel;
        protected AD_BowSkill SkillData;

        public AD_BowSkill Skill()
        {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
