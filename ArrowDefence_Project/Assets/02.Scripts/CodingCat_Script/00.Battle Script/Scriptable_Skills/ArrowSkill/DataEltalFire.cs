namespace ActionCat {
    using UnityEngine;

    /// <summary>
    /// ArrowSkill ScriptableObject
    /// </summary>
    public class DataEltalFire : ArrowSkillData {
        [RangeEx(10f, 70f, 5f)]
        public float ActivationProbability = 10f;
        public ProjectilePref firePref;
        [RangeEx(10, 500, 10)]
        public short ProjectileDamage = 0;

        public DataEltalFire() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.ELEMENTAL_FIRE;
        }

        private void OnEnable() {
            skillData = new ElementalFire(this);
        }
    }
}
