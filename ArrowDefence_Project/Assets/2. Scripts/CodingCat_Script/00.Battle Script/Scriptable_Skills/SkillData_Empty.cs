namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "EmptySlot_Asset", menuName = "Scriptable Object Asset/B.SkillData_Asset/EmptySlot_Asset")]
    public class SkillData_Empty : BowSkillData
    {
        public SkillData_Empty()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_EMPTY;
        }

        public void OnEnable()
        {
            SkillData = new Skill_Empty(SkillName, SkillDesc, SkillLevel, SkillType);
        }
    }
}
