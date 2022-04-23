namespace ActionCat {
    using UnityEngine;

    public class SkillEntity_Cure : AccessorySkillData {
        public float HealAmount       = 0f;
        public int HealRepeatTime     = 0;
        public float RepeatIntervalTime = 2f;

        public SkillEntity_Cure() : base(ACSP_TYPE.CURE) { }
    }
}
