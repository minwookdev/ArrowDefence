namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "SlowTime_Asset", menuName = "Scriptable Object Asset/ACSP.SkillData_Asset/SlowTime_Asset")]
    public class SkillDataSlowTime : AccessorySkillData
    {
        public float TimeSlotRatio;

        public SkillDataSlowTime()
        {
            this.EffectType = ACSP_TYPE.SPEEFECT_SLOWTIME;
        }

        public void OnEnable()
        {
            this.SkillData = new Acsp_SlowTime(SkillId, SkillName, SkillDesc, EffectType, SkillLevel);
        }
    }
}
