namespace ActionCat {
    using UnityEngine;

    public class SkillEntity_ElementalAmp : ArrowSkillData {
        [RangeEx(1f, 1.5f, 0.1f, "Value")] public float ElementalAmplificationValue = 1f;
        public SkillEntity_ElementalAmp() {
            ActiveType = ARROWSKILL_ACTIVETYPE.EMPTY;
            SkillType  = ARROWSKILL.BUFF;
        }

        private void OnEnable() {
            skillData = new ElementalAmplification(this);
        }
    }
}
