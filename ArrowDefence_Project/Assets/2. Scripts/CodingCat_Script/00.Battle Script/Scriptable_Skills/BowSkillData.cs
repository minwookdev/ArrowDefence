namespace ActionCat
{
    using UnityEngine;

    public class BowSkillData : ScriptableObject
    {
        //Basic Skill Data
        public string SkillId;
        public string SkillName;
        public string SkillDesc;
        public BOWSKILL_TYPE SkillType;
        public SKILL_LEVEL SkillLevel;
        public Sprite SkillIconSprite;
        protected AD_BowSkill SkillData;

        public AD_BowSkill Skill()
        {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
