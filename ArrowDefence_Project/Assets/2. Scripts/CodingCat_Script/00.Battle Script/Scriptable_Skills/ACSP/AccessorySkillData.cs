namespace ActionCat
{
    using UnityEngine;

    public class AccessorySkillData : ScriptableObject
    {
        [Header("BASIC SKILL DATA")]
        public string SkillName;
        public string SkillDesc;
        public ACCESSORY_SPEFFECT_TYPE EffectType;
        public SKILL_LEVEL SkillLevel;
        protected AccessorySPEffect SkillData;

        public AccessorySPEffect Skill()
        {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
}
