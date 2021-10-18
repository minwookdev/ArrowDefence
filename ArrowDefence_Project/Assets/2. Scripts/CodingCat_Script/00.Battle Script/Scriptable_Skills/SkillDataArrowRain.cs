namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "ArrowRain_Asset", menuName = "Scriptable Object Asset/B.SkillData_Asset/ArrowRain_Asset")]
    public class SkillDataArrowRain : BowSkillData
    {
        [Header("ARROW RAIN")]
        [Range(1, 255)]
        public int ArrowShotCount;
        public float ShotInterval;

        public SkillDataArrowRain()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_ARROW_RAIN;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Arrow_Rain((byte)ArrowShotCount, ShotInterval,
                                             SkillName, SkillDesc, SkillLevel, SkillType);
        }
    }
}
