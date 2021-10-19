namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "SpreadShot_Asset", menuName = "Scriptable Object Asset/B.SkillData_Asset/SpreadShot_Asset")]
    public class SkillDataSpreadShot : BowSkillData
    {
        [Header("SPREAD SHOT")]
        [Range(1, 255)]
        public int ArrowShotCount;
        public float SpreadAngle;

        public SkillDataSpreadShot()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_SPREAD_SHOT;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Multiple_Shot(SkillName, SkillDesc, SkillLevel, SkillType,
                                                (byte)ArrowShotCount, SpreadAngle);
        }
    }
}
