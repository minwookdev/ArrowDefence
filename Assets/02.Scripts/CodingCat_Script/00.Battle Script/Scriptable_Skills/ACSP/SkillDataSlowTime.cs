namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataSlowTime : AccessorySkillData {
        [RangeEx(0.1f, 0.9f, 0.1f)] 
        [Tooltip("값이 증가할수록 더 느려짐")]
        public float TimeSlowRatio = 0.5f;
        [Tooltip("지속시간")]
        public float Duration = 3f;

        public SkillDataSlowTime() : base(ACSP_TYPE.SPEEFECT_SLOWTIME) { }

        public void OnEnable() {
            this.SkillData = new Acsp_SlowTime(this);
        }
    }
}
