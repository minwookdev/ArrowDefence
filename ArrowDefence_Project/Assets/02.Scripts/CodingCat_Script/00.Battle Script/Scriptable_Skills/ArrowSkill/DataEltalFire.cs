namespace ActionCat {
    using UnityEngine;

    /// <summary>
    /// ArrowSkill ScriptableObject
    /// </summary>
    public class DataEltalFire : ArrowSkillData {
        [RangeEx(10f, 30f, 5f)]
        public float ActivationProbability = 10f;
        public GameObject firePref;

        public DataEltalFire() {
            ActiveType = ARROWSKILL_ACTIVETYPE.ADDPROJ;
            SkillType  = ARROWSKILL.ELEMENTAL_FIRE;
        }

        private void OnEnable() {
            skillData = new ElementalFire(this);
        }
    }
}
