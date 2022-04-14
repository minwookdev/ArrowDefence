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
        public abstract float GetCount();
        public abstract byte GetGrade();
        public abstract string GetName();
        public abstract bool Upgrade();
    }
    #region ABILITY-BOW
    //=======================================================================================================================================================
    //================================================================== << BASE DAMAGE >> ==================================================================
    public class AbilityDamage : Ability {
        public const short MaxValue = 500;
        public const int UnitValue  = 50;
        private short increaseValue;

        public override float GetCount() {
            return increaseValue;
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.DAMAGE;
        }

        public override byte GetGrade() { // 1~500 [50]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / UnitValue));
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityDamage(int damage) {
            abilityType = ABILITY_TYPE.DAMAGE;
            if (damage > short.MaxValue) {
                damage = short.MaxValue;
            }
            increaseValue = System.Convert.ToInt16(damage);
        }
        #region ES3
        public AbilityDamage() { }
        ~AbilityDamage() { }
        #endregion

        public static int GetGradeCount(int number) => UnityEngine.Mathf.CeilToInt((float)number / UnitValue);
    }
    //=======================================================================================================================================================
    //================================================================ << CHARGED DAMAGE >> =================================================================
    public class AbilityChargedDamage : Ability {
        public const float MaxValue  = 1.25f;
        public const float UnitValue = .125f;
        float increaseValue;

        public override float GetCount() {
            return increaseValue;
        }

        public override byte GetGrade() { // 0.1~1.25, [0.125]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / UnitValue));
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

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / UnitValue);
    }
    //=======================================================================================================================================================
    //=============================================================== << CRITICAL CHANCE >> =================================================================
    public class AbilityCritChance : Ability {
        public const byte MaxValue = 30;
        public const int UnitValue = 3;
        byte increaseValue;
        public override float GetCount() {
            return increaseValue;
        }

        public override byte GetGrade() { //0~30 [3]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / UnitValue));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.CRIT_CHANCE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritChance(int value) {
            abilityType   = ABILITY_TYPE.CRITICALCHANCE;
            if(value > byte.MaxValue) {
                value = byte.MaxValue;
            }
            increaseValue = System.Convert.ToByte(value);
        }
        #region ES3
        public AbilityCritChance() { }
        #endregion

        public static int GetGradeCount(int number) => UnityEngine.Mathf.CeilToInt((float)number / UnitValue);
    }
    //=======================================================================================================================================================
    //=============================================================== << CRITICAL DAMAGE >> =================================================================
    public class AbilityCritDamage : Ability {
        public const float MaxValue  = 1.5f;
        public const float UnitValue = 0.15f;
        float increaseValue;
        public override float GetCount() {
            return increaseValue;
        }

        public override byte GetGrade() { //0f~1.5f [.3f]
            return System.Convert.ToByte(UnityEngine.Mathf.CeilToInt((float)increaseValue / 0.15f));
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.CRIT_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritDamage(float value) {
            abilityType          = ABILITY_TYPE.CRITICALDAMAGE;
            increaseValue = value;
        }
        #region ES3
        public AbilityCritDamage() { }
        #endregion

        public static int GetGradeCount(float number) => UnityEngine.Mathf.CeilToInt((float)number / 0.15f);
    }
    #endregion

    #region ABILITY-ARROW
    //=======================================================================================================================================================
    //================================================================= << ARROW SPEED >> ===================================================================
    public class AbilitySpeed : Ability {
        float speed;
        public override float GetCount() {
            return speed;
        }

        public override byte GetGrade() {
            if (speed <= 18f)      return 0;
            else if (speed <= 21f) return 1;
            else if (speed <= 24f) return 2;
            else if (speed <= 27f) return 3;
            else if (speed <= 30f) return 4;
            else if (speed <= 33f) return 5;
            else throw new System.Exception("Ability:Speed Value is RangeOver. [18~33]");
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.ARROWSPEED;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilitySpeed(float value) {
            abilityType = ABILITY_TYPE.ARROWSPEED;
            speed       = value;
        }
        #region ES3
        public AbilitySpeed() { }
        #endregion
    }
    //=======================================================================================================================================================
    //============================================================= << DAMAGE INCREASE RATE >> ==============================================================
    public class IncDamageRate : Ability {
        float increaseRate;

        public override float GetCount() {
            return increaseRate;
        }

        public override byte GetGrade() {
            if (increaseRate <= 1f)        return 0;
            else if (increaseRate <= 1.1f) return 1;
            else if (increaseRate <= 1.2f) return 2;
            else if (increaseRate <= 1.3f) return 3;
            else if (increaseRate <= 1.4f) return 4;
            else if (increaseRate <= 1.5f) return 5;
            else                           return 0;
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.INC_DAMAGE_RATE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public IncDamageRate(float value) {
            abilityType  = ABILITY_TYPE.DAMAGEINC;
            increaseRate = value;
        }
        #region ES3
        public IncDamageRate() { }
        #endregion
    }
    //=======================================================================================================================================================
    //=============================================================== << PROJECTILE DAMAGE >> ===============================================================
    public class IncProjectileDamage : Ability {
        short increaseValue; // 10~50

        public override float GetCount() {
            return increaseValue;
        }

        public override byte GetGrade() {
            if (increaseValue <= 10)      return 1;
            else if (increaseValue <= 20) return 2;
            else if (increaseValue <= 30) return 3;
            else if (increaseValue <= 40) return 4;
            else if (increaseValue <= 50) return 5;
            else {
                throw new System.NotImplementedException();
            }
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.PROJECTILE_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public IncProjectileDamage(short value) {
            abilityType  = ABILITY_TYPE.PROJECTILEDAMAGE;
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
        short increaseValue; // 60~240

        public override float GetCount() {
            return increaseValue;
        }

        public override byte GetGrade() {
            if (increaseValue <= 60)       return 1;
            else if (increaseValue <= 96)  return 1;
            else if (increaseValue <= 132) return 2;
            else if (increaseValue <= 168) return 3;
            else if (increaseValue <= 204) return 4;
            else if (increaseValue <= 240) return 5;
            else {
                throw new System.NotImplementedException();
            }
        }

        public override string GetName() {
            return I2.Loc.ScriptLocalization.ABILITY.SPELL_DAMAGE;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public IncSpellDamage(short value) {
            this.increaseValue = value;
            this.abilityType = ABILITY_TYPE.SPELLDAMAGE;
        }
        #region ES3
        public IncSpellDamage() {

        }
        #endregion
    }
    //=======================================================================================================================================================
    #endregion
}
