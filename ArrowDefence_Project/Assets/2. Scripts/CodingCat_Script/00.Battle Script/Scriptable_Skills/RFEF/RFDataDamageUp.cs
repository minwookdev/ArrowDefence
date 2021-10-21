namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DamageUp_Asset", menuName = "Scriptable Object Asset/RFEF.SkillData_Asset/DamageUp_Asset")]
    public class RFDataDamageUp : AccessoryRFSkillData
    {
        public int DamageIncreaseValue;

        public RFDataDamageUp()
        {
            this.EffectType = RFEF_TYPE.RFEFFECT_INCREASE_DAMAGE;
        }

        public void OnEnable()
        {
            SkillData = new RFEffectDamage();
        }
    }
}
