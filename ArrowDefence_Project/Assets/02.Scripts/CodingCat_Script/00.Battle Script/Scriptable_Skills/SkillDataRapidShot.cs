namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Bow Skill Scriptable Object
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataRapidShot : BowSkillData
    {
        [Range(1, 255)]
        public byte ArrowShotCount;
        public float ShotInterval;

        public SkillDataRapidShot()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_RAPID_SHOT;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Rapid_Shot(this);
        }
    }
}
