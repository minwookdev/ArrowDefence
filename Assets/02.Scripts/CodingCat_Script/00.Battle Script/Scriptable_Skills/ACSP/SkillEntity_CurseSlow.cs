namespace ActionCat {
    using UnityEngine;

    public class SkillEntity_CurseSlow : AccessorySkillData {
        public float RangeRadius = 3f;
        public float SlowRatio   = 0.5f;
        public float Duration    = 8f;
        
        public SkillEntity_CurseSlow() : base(ACSP_TYPE.CURSE_SLOW) { }
    }
}
