namespace ActionCat {
    using UnityEngine;

    public class DataSplitDagger : ArrowSkillData {
        [RangeEx(3, 5, 1)]
        public int projectileCount = 3;
        public GameObject daggerPref;

        public DataSplitDagger() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.SPLIT_DAGGER;
        }

        private void OnEnable() {
            skillData = new SplitDagger(this);
        }
    }
}
