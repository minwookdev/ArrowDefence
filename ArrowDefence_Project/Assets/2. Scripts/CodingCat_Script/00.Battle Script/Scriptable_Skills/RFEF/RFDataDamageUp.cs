namespace ActionCat
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "DamageUp_Asset", menuName = "Scriptable Object Asset/RFEF.SkillData_Asset/DamageUp_Asset")]
    public class RFDataDamageUp : AccessoryRFSkillData
    {
        public RFDataDamageUp()
        {
            this.EffectType = ACCESSORY_RFEFFECT_TYPE.RFEFFECT_INCREASE_DAMAGE;
        }

        public void OnEnable()
        {
            SkillData = new RFEffectDamage();
        }
    }
}
