namespace ActionCat {
    using UnityEngine;

    /// <summary>
    /// ArrowSkill ScriptableObject
    /// </summary>
    public class DataSplitDagger : ArrowSkillData {
        [RangeEx(3, 5, 1)]
        public int projectileCount = 3;
        public ProjectilePref daggerPref;
        [RangeEx(0, 500, 10)]
        public short ProjectileDamage = 0;

        public DataSplitDagger() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.SPLIT_DAGGER;
        }

        private void OnEnable() {
            skillData = new SplitDagger(this);
        }
    }
}
