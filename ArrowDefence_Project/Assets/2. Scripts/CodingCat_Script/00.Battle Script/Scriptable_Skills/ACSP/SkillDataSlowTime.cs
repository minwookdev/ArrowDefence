namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "SlowTime_Asset", menuName = "Scriptable Object Asset/ACSP.SkillData_Asset/SlowTime_Asset")]
    public class SkillDataSlowTime : AccessorySkillData
    {
        [Header("SLOW TIME")]
        public float TimeSlotRatio;

        public SkillDataSlowTime()
        {
            this.EffectType = ACCESSORY_SPEFFECT_TYPE.SPEEFECT_SLOWTIME;
        }

        public void OnEnable()
        {
            this.SkillData = new Acsp_SlowTime();
        }
    }
}
