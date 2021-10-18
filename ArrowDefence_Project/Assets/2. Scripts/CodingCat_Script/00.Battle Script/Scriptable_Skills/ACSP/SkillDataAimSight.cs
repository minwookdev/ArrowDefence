namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "AimSight_Asset", menuName = "Scriptable Object Asset/ACSP.SkillData_Asset/AimSight_Asset")]
    public class SkillDataAimSight : AccessorySkillData
    {
        [Header("AIM SIGHT")]
        public Material LineRenderMat;
        [Range(0.1f, 0.001f)]
        public float LineWidth = 0.050f;

        public SkillDataAimSight()
        {
            this.EffectType = ACCESSORY_SPEFFECT_TYPE.SPEFFECT_AIMSIGHT;
        }

        private void OnEnable()
        {
            SkillData = new Acsp_AimSight(LineRenderMat, LineWidth);
        }
    }
}
