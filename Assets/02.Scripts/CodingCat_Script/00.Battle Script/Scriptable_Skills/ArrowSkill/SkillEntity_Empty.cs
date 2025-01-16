namespace ActionCat {
    using UnityEngine;

    public class SkillEntity_Empty : ArrowSkillData {
        public float[] Values;
        public SkillEntity_Empty() {
            ActiveType = ARROWSKILL_ACTIVETYPE.EMPTY;
            SkillType = ARROWSKILL.NONE;
        }

        private void OnEnable() {
            skillData = new EmptyTypeArrowSkill(this);
        }
    }
}
