namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "RapidShot_Asset", menuName = "Scriptable Object Asset/B.SkillData_Asset/RapidShot_Asset")]
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
            SkillData = new Skill_Rapid_Shot(SkillId, SkillName, SkillDesc, SkillLevel, SkillType, 
                                             (byte)ArrowShotCount, ShotInterval);
        }
    }
}
