namespace ActionCat
{
    using UnityEngine;
    using CodingCat_Scripts;

    #region SKILL_DATA_PARENT
    public class BowSkillData : ScriptableObject
    {
        [Header("BASIC SKILL DATA")]
        public string SkillName;
        public string SkillDesc;
        public BOWSKILL_TYPE SkillType;
        public SKILL_LEVEL SkillLevel;
        public AD_BowSkill SkillData;

        public AD_BowSkill Skill()
        {
            if (SkillData != null) return SkillData;
            else                   return null;
        }
    }
    #endregion

    #region SPREAD_SHOT
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
            SkillData = new Skill_Multiple_Shot((byte)ArrowShotCount, SpreadAngle,
                                                SkillName, SkillDesc, SkillLevel, SkillType);
        }
    }
    #endregion

    #region RAPID_SHOT
    [CreateAssetMenu(fileName = "RapidShot_Asset", menuName = "Scriptable Object Asset/B.SkillData_Asset/RapidShot_Asset")]
    public class SkillDataRapidShot : BowSkillData
    {
        [Header("RAPID SHOT")]
        [Range(1, 255)]
        public int ArrowShotCount;
        public float ShotInterval;

        public SkillDataRapidShot()
        {
            this.SkillType = BOWSKILL_TYPE.SKILL_RAPID_SHOT;
        }

        private void OnEnable()
        {
            SkillData = new Skill_Rapid_Shot((byte)ArrowShotCount, ShotInterval, 
                                             SkillName, SkillDesc, SkillLevel, SkillType);
        }
    }
    #endregion

    #region ARROW_RAIN
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
    #endregion

    #region EMPTY_SLOT
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

    #endregion
}
