namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Bow Skill Scriptable Object
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataSpreadShot : BowSkillData
    {
        [RangeEx(1, 255, 1)] public byte ArrowShotCount;
        public float SpreadAngle;

        public SkillDataSpreadShot()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_SPREAD_SHOT;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Multiple_Shot(this);
        }
    }
}
