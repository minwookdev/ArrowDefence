namespace ActionCat {
    using UnityEngine;
    public class Dt_Explosion : ArrowSkillData {
        [RangeEx(100, 2000, 100)] public short ExplosionDamage = 100;
        [RangeEx(50, 1000, 50)]   public short AddExDamage = 50;
        public GameObject ExplosionPref;
        public GameObject SmallExPref;
        public float ExplosionRange;
        [Tooltip("Ignore default Explosion Range, Explosion Range + Add Explosion Range")]
        public float AddExplosionRange;

        public Dt_Explosion() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.EXPLOSION;
        }

        private void OnEnable() {
            if(SkillLevel == SKILL_LEVEL.LEVEL_LOW) {
                CatLog.WLog("Special Skill Level is Upper than (SKILL_LEVEL_LOW)");
            }

            skillData = new Explosion(this);
        }
    }
}
