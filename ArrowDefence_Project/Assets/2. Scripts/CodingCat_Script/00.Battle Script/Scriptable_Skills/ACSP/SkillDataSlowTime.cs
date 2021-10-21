namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataSlowTime : AccessorySkillData
    {
        public float TimeSlowRatio;

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
