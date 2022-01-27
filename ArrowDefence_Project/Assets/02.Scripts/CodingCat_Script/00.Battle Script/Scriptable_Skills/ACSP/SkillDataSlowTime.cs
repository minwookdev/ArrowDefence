namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataSlowTime : AccessorySkillData
    {
        [Range(0.1f, 0.9f)]
        public float TimeSlowRatio = 0.5f;
        public float Duration = 3f;
        public float CoolDown = 10f;

        public SkillDataSlowTime()
        {
            this.EffectType = ACSP_TYPE.SPEEFECT_SLOWTIME;
        }

        public void OnEnable()
        {
            this.SkillData = new Acsp_SlowTime(this);
        }
    }
}
