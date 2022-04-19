namespace ActionCat {
    //==================================================================== << PARENT >> =====================================================================
    public abstract class Ability {
        //Type은 따로 저장하지는 않는데, Save Data Load해서 확인해보기.
        protected ABILITY_TYPE abilityType = ABILITY_TYPE.NONE;
        public ABILITY_TYPE AbilityType {
            get {
                return abilityType;
            }
        }
        public abstract byte GetGrade();
        public abstract string GetName();
        public abstract bool Upgrade();
        protected static float GetUnitValue(float maxvalue) {
            return (float)maxvalue / 10;
        }
        public virtual byte GetValueToByte() => throw new System.NotImplementedException();
        public virtual short GetValueToInt16() => throw new System.NotImplementedException();
        public virtual float GetValueToSingle() => throw new System.NotImplementedException();
    }
    #region ABILITY-BOW
    //=======================================================================================================================================================
    //================================================================== << BASE DAMAGE >> ==================================================================
    public class AbilityDamage : Ability {
        public const short MaxValue = 500;
        private short increaseValue;
        public short Value {
            get {
                return increaseValue;
            }
        }

        public override short GetValueToInt16() {
            return increaseValue;
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.DAMAGE;
        }

        public override byte GetGrade() { // 1~500 [50]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityDamage(short value) {
            abilityType = ABILITY_TYPE.DAMAGE;
            increaseValue = value;
        }
        #region ES3
        public AbilityDamage() { }
        ~AbilityDamage() { }
        #endregion

        public static int GetGradeCount(int number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
    }
    //=======================================================================================================================================================
    //================================================================ << CHARGED DAMAGE >> =================================================================
    public class AbilityChargedDamage : Ability {
        public const float MaxValue  = 1.25f;
        float increaseValue;
        public float Value {
            get {
                return increaseValue;
            }
        }

        public override float GetValueToSingle() {
            return increaseValue;
        }

        public override byte GetGrade() { // 0.1~1.25, [0.125]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.CHARGEDSHOT_DMG;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityChargedDamage(float count) {
            abilityType   = ABILITY_TYPE.CHARGEDAMAGE;
            increaseValue = count;
        }
        #region ES3
        public AbilityChargedDamage() { }
        ~AbilityChargedDamage() { }
        #endregion

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
    }
    //=======================================================================================================================================================
    //=============================================================== << CRITICAL CHANCE >> =================================================================
    public class AbilityCritChance : Ability {
        public const byte MaxValue = 30;
        byte increaseValue;
        public byte Value {
            get {
                return increaseValue;
            }
        }

        public override byte GetValueToByte() {
            return increaseValue;
        }

        public override byte GetGrade() { //0~30 [3]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.CRIT_CHANCE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritChance(byte value) {
            abilityType   = ABILITY_TYPE.CRITICALCHANCE;
            increaseValue = value;
        }
        #region ES3
        public AbilityCritChance() { }
        #endregion

        public static int GetGradeCount(int number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
    }
    //=======================================================================================================================================================
    //=============================================================== << CRITICAL DAMAGE >> =================================================================
    public class AbilityCritDamage : Ability {
        public const float MaxValue  = 1.5f;
        float increaseValue;
        public float Value {
            get {
                return increaseValue;
            }
        }
        public override float GetValueToSingle() {
            return increaseValue;
        }

        public override byte GetGrade() { //0f~1.5f [.3f]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.CRIT_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritDamage(float value) {
            abilityType = ABILITY_TYPE.CRITICALDAMAGE;
            increaseValue = value;
        }
        #region ES3
        public AbilityCritDamage() { }
        #endregion

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
    }
    #endregion

    //=======================================================================================================================================================
    //================================================================= << ARMOR BREAK >> ===================================================================
    public class PenetrationArmor : Ability {
        private short increaseValue = 0;
        public const short MaxValue = 150;
        public short Value {
            get {
                return increaseValue;
            }
        }

        public override short GetValueToInt16() {
            return increaseValue;
        }

        public override byte GetGrade() { //0~150 [15]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.ARMOR_PENETRATION;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));

        public PenetrationArmor(short value) {
            abilityType = ABILITY_TYPE.ARMORPENETRATE;
            increaseValue = value;
        }
        #region ES3
        public PenetrationArmor() { }
        #endregion
    }
    //============================================================== << ADDITIONAL ARROW >> =================================================================
    //=======================================================================================================================================================
    public class AdditionalFire : Ability {
        private byte increaseValue = 0;
        public const byte MaxValue = 5;
        public byte Value {
            get {
                return increaseValue;
            }
        }

        public override byte GetValueToByte() {
            return increaseValue;
        }

        public override byte GetGrade() { //0~5 [0.5f]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.ADDITIONAL_FIRE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));

        public AdditionalFire(byte value) {
            abilityType   = ABILITY_TYPE.ADDITIONALFIRE;
            increaseValue = value;
        }
        public AdditionalFire() {}
    }

    #region ABILITY-ARROW
    //=======================================================================================================================================================
    //================================================================= << ARROW SPEED >> ===================================================================
    public class AbilitySpeed : Ability {
        public const float MaxValue = 16;
        float increaseValue = 0f;
        public float Value {
            get {
                return increaseValue;
            }
        }

        public override float GetValueToSingle() {
            return increaseValue;
        }

        public override byte GetGrade() { //0f~16f [1.6f]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.ARROWSPEED;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public static int GetGradeCount(float number) {
            return UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
        }

        public AbilitySpeed(float value) {
            abilityType   = ABILITY_TYPE.ARROWSPEED;
            increaseValue = value;
        }
        #region ES3
        public AbilitySpeed() { }
        #endregion
    }
    //=======================================================================================================================================================
    //============================================================= << DAMAGE INCREASE RATE >> ==============================================================
    public class IncArrowDamageRate : Ability {
        public const float MaxValue = 1f;
        float increaseRate = 0f;
        public float Value {
            get {
                return increaseRate;
            }
        }

        public override float GetValueToSingle() {
            return increaseRate + StNum.floatOne;
        }

        public override byte GetGrade() {
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseRate / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.INC_DAMAGE_RATE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public static int GetGradeCount(float number) {
            return UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
        }

        public IncArrowDamageRate(float value) {
            abilityType  = ABILITY_TYPE.ARROWDAMAGEINC;
            increaseRate = value;
        }
        #region ES3
        public IncArrowDamageRate() { }
        #endregion
    }
    //=======================================================================================================================================================
    //=============================================================== << PROJECTILE DAMAGE >> ===============================================================
    public class IncProjectileDamage : Ability {
        public const short MaxValue = 50;
        short increaseValue = 0; // 10~50
        public short Value {
            get {
                return increaseValue;
            }
        }

        public override short GetValueToInt16() {
            return increaseValue;
        }

        public override byte GetGrade() {
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.PROJECTILE_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public static int GetGradeCount(float number) {
            return UnityEngine.Mathf.CeilToInt((float)number / MaxValue);
        }

        public IncProjectileDamage(short value) {
            abilityType   = ABILITY_TYPE.PROJECTILEDAMAGE;
            increaseValue = value;
        }
        #region ES3
        public IncProjectileDamage() {

        }
        #endregion
    }
    //=======================================================================================================================================================
    //================================================================= << SPELL DAMAGE >> ==================================================================
    public class IncSpellDamage : Ability {
        public const short MaxValue = 150;
        short increaseValue = 0; // 0~150
        public short Value {
            get {
                return increaseValue;
            }
        }

        public override short GetValueToInt16() {
            return increaseValue;
        }

        public override byte GetGrade() {
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public static int GetGradeCount(float number) {
            return UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.SPELL_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public IncSpellDamage(short value) {
            this.abilityType   = ABILITY_TYPE.SPELLDAMAGE;
            this.increaseValue = value;
        }
        #region ES3
        public IncSpellDamage() {

        }
        #endregion
    }
    //=======================================================================================================================================================
    //============================================================ << ELEMENTAL ACTIVATION >> ===============================================================
    public class ElementalActivation : Ability {
        public const short MaxValue = 30;
        short increaseValue = 0;
        public short Value {
            get {
                return increaseValue;
            }
        }

        public override byte GetGrade() {
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / GetUnitValue(MaxValue)));
        }

        public static int GetGradeCount(float number) {
            return UnityEngine.Mathf.CeilToInt((float)number / GetUnitValue(MaxValue));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.ELEMENTAL_ACTIVATION;        
        }

        public override short GetValueToInt16() {
            return increaseValue;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public ElementalActivation(short value) {
            abilityType   = ABILITY_TYPE.ELEMENTALACTIVATION;
            increaseValue = value;
        }
        public ElementalActivation() { }
    }
    #endregion
}
