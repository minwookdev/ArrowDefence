namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Bow Skill Scriptable Object
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataArrowRain : BowSkillData
    {
        [Range(1, 255)]
        public byte ArrowShotCount;
        public float ShotInterval;

        public SkillDataArrowRain()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_ARROW_RAIN;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Arrow_Rain(SkillId, SkillName, SkillDesc, SkillLevel, SkillType,
                                             ArrowShotCount, ShotInterval);
        }
    }
}
