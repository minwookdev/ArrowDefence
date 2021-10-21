namespace ActionCat
{
    using UnityEngine;

    /// <summary>
    /// Create it through the ActionCat Menu.
    /// </summary>
    public class SkillDataAimSight : AccessorySkillData
    {
        public Material LineRenderMat;
        [Range(0.1f, 0.001f)]
        public float LineWidth = 0.050f;

        public SkillDataAimSight()
        {
            this.EffectType = ACSP_TYPE.SPEFFECT_AIMSIGHT;
        }

        private void OnEnable()
        {
            SkillData = new Acsp_AimSight(SkillId, SkillName, SkillDesc, EffectType, SkillLevel,
                                          LineRenderMat, LineWidth);
        }
    }
}
