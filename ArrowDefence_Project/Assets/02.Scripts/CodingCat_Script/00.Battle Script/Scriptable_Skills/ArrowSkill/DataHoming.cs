namespace ActionCat
{
    using UnityEngine;

    public class DataHoming : ArrowSkillData {
        [ReadOnly] [Tooltip("This Field is Not Used.")]
        [Range(0.1f, 0.3f)] public float TargetSearchInterval = .1f;
        [Range(2f, 5f)]     public float ScanRadius           = 3f;
        public float HomingSpeed       = 6f;
        public float HomingRotateSpeed = 800f;

        public DataHoming() {
            ActiveType = ARROWSKILL_ACTIVETYPE.AIR;
            SkillType  = ARROWSKILL.SKILL_HOMING;
        }

        public void OnEnable()
        {
            skillData = new HomingArrow(this);
        }
    }
}
