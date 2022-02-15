namespace ActionCat {
    using UnityEngine;

    /// <summary>
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataAimSight : AccessorySkillData {
        [ReadOnly] [Tooltip("this field is deprecated.")] public Material LineRenderMat;
        [ReadOnly] [Tooltip("this field is deprecated.")] public float LineWidth = 0.050f;

        public GameObject AimSightPref = null;

        public SkillDataAimSight() {
            this.EffectType = ACSP_TYPE.SPEFFECT_AIMSIGHT;
        }

        private void OnEnable() {
            SkillData = new Acsp_AimSight(this);
        }
    }
}
