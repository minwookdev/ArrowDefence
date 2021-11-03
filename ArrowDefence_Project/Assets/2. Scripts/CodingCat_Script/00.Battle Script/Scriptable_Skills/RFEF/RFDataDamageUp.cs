namespace ActionCat
{
    using UnityEngine;

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
