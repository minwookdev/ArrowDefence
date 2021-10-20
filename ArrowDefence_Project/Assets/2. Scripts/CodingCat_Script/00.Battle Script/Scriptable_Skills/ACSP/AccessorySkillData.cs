namespace ActionCat
{
    using UnityEngine;

    public class AccessorySkillData : ScriptableObject
    {
        //Default Skill Data
        public string SkillId;
        public string SkillName;
        public string SkillDesc;
        public ACSP_TYPE EffectType;
        public SKILL_LEVEL SkillLevel;
        protected AccessorySPEffect SkillData;

        public AccessorySPEffect Skill()
        {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
