namespace ActionCat
{
    public enum ACCESSORY_RFEFFECT_TYPE
    {
        RFEFFECT_NONE,
        RFEFFECT_INCREASE_DAMAGE,
        RFEFFECT_INCREASE_PHYSICAL_DAMAGE,
        RFEFFECT_INCREASE_MAGICAL_DAMAGE,
        RFEFFECT_INCREASE_HEALTH
    }

    public abstract class AccessoryRFEffect
    {

    }

    public class RFEffectDamage : AccessoryRFEffect
    {

    }

    public class RFEffectPhysicalDamage : AccessoryRFEffect
    {

    }

    public class RFEffectMagicalDamage : AccessoryRFEffect
    {
    
    }
}
