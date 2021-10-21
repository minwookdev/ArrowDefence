namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Bow Skill Scriptable Object
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillData_Empty : BowSkillData
    {
        public SkillData_Empty()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_EMPTY;
        }

        public void OnEnable()
        {
            SkillData = new Skill_Empty(SkillId, SkillName, SkillDesc, SkillLevel, SkillType);
        }
    }
}
