namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AimSight_Asset", menuName = "Scriptable Object Asset/ACSP.SkillData_Asset/AimSight_Asset")]
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
