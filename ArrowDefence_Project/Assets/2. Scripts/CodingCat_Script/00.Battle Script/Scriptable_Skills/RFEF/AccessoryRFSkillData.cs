namespace ActionCat
{
    using UnityEngine;

    public class AccessoryRFSkillData : ScriptableObject
    {
        [Header("BASIC SKILL DATA")]
        public string SkillId;
        public string SkillName;
        public string SkillDesc;
        public ACCESSORY_RFEFFECT_TYPE EffectType;
        protected AccessoryRFEffect SkillData;
    }
}
