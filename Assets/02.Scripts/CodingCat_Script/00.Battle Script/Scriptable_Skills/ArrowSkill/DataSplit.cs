namespace ActionCat {
    using UnityEngine;

    public class DataSplit : ArrowSkillData {
        [RangeEx(5f, 20f, 5f)] public float ScanRange = 5f;
        [RangeEx(3, 5, 1)] public int MaxTarget = 3;

        public DataSplit() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.SKILL_SPLIT;
        }

        private void OnEnable() {
            skillData = new SplitArrow(this);
        }
    }
}
