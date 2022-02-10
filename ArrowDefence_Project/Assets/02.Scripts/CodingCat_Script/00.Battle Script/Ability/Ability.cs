namespace ActionCat {
    public abstract class Ability {
        //Type은 따로 저장하지는 않는데, Save Data Load해서 확인해보기.
        protected ABILITY_TYPE abilityType = ABILITY_TYPE.NONE;
        public abstract float GetCount();
        public abstract byte GetGrade();
        public abstract string GetName();
        public abstract bool Upgrade();
    }


    public class AbilityDamage : Ability {
        private short damageCount;

        public override float GetCount() {
            return damageCount;
        }

        public override string GetName() {
            return "Damage";
        }

        public override byte GetGrade() {
            if (damageCount <= 100)      return 1;
            else if (damageCount <= 200) return 2;
            else if (damageCount <= 300) return 3;
            else if (damageCount <= 400) return 4;
            else if (damageCount <= 500) return 5;
            else                         return 0;
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityDamage(int damage) {
            abilityType = ABILITY_TYPE.DAMAGE;
            damageCount = System.Convert.ToInt16(damage);
        }

        public AbilityDamage() {
            abilityType = ABILITY_TYPE.DAMAGE;
        }
        ~AbilityDamage() { }
    }

    public class AbilityChargedDamage : Ability {
        float chargedShotDamageMulti;

        public override float GetCount() {
            return chargedShotDamageMulti;
        }

        public override byte GetGrade() {
            if (chargedShotDamageMulti <= 1.2f)      return 0;
            else if (chargedShotDamageMulti <= 1.5f) return 1;
            else if (chargedShotDamageMulti <= 1.8f) return 2;
            else if (chargedShotDamageMulti <= 2.1f) return 3;
            else if (chargedShotDamageMulti <= 2.5f) return 4;
            else if (chargedShotDamageMulti <= 3f)   return 5;
            else                                     return 0;
        }

        public override string GetName() {
            return "Charged Shot";
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityChargedDamage(float count) {
            abilityType = ABILITY_TYPE.CHARGEDAMAGE;
            chargedShotDamageMulti = count;
        }

        public AbilityChargedDamage() {
            abilityType = ABILITY_TYPE.CHARGEDAMAGE;
        }
        ~AbilityChargedDamage() { }
    }

    public class AbilityCritChance : Ability {
        byte critHitChance;
        public override float GetCount() {
            return critHitChance;
        }

        public override byte GetGrade() {
            if (critHitChance <= 0)       return 0;
            else if (critHitChance <= 6)  return 1;
            else if (critHitChance <= 12) return 2;
            else if (critHitChance <= 18) return 3;
            else if (critHitChance <= 24) return 4;
            else if (critHitChance <= 30) return 5;
            else                          return 0;
        }

        public override string GetName() {
            return "Critical Chance";
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritChance(byte value) {
            abilityType = ABILITY_TYPE.CRITICALCHANCE;
            critHitChance = value;
        }

        public AbilityCritChance() {
            abilityType = ABILITY_TYPE.CRITICALCHANCE;
        }
    }

    public class AbilityCritDamage : Ability
    {
        float critDamageMultiplier;
        public override float GetCount() {
            return critDamageMultiplier;
        }

        public override byte GetGrade() {
            if (critDamageMultiplier <= 1.5f)      return 0;
            else if (critDamageMultiplier <= 1.8f) return 1;
            else if (critDamageMultiplier <= 2.1f) return 2;
            else if (critDamageMultiplier <= 2.4f) return 3;
            else if (critDamageMultiplier <= 2.7f) return 4;
            else if (critDamageMultiplier <= 3.0f) return 5;
            else                                   return 0;
        }

        public override string GetName() {
            return "Critical Damage";
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityCritDamage(float value) {
            abilityType = ABILITY_TYPE.CRITICALDAMAGE;
            critDamageMultiplier = value;
        }

        public AbilityCritDamage() {
            abilityType = ABILITY_TYPE.CRITICALDAMAGE;
        }
    }

    public class AbilitySpeed : Ability {
        float speed;
        public override float GetCount() {
            return speed;
        }

        public override byte GetGrade() {
            if (speed <= 18f)      return 0;
            else if (speed <= 20f) return 1;
            else if (speed <= 22f) return 2;
            else if (speed <= 24f) return 3;
            else if (speed <= 26f) return 4;
            else if (speed <= 28f) return 5;
            else                   return 0;
        }

        public override string GetName() {
            return "Arrow Speed";
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilitySpeed(float value) {
            abilityType = ABILITY_TYPE.ARROWSPEED;
            speed = value;
        }

        public AbilitySpeed() {
            abilityType = ABILITY_TYPE.ARROWSPEED;
        }
    }

    public class AbilityIncDamageRate : Ability {
        float incDamageRate;

        public override float GetCount() {
            return incDamageRate;
        }

        public override byte GetGrade() {
            if (incDamageRate <= 1f)        return 0;
            else if (incDamageRate <= 1.1f) return 1;
            else if (incDamageRate <= 1.2f) return 2;
            else if (incDamageRate <= 1.3f) return 3;
            else if (incDamageRate <= 1.4f) return 4;
            else if (incDamageRate <= 1.5f) return 5;
            else                            return 0;
        }

        public override string GetName() {
            return "Increase Damage";
        }

        public override bool Upgrade() {
            throw new System.NotImplementedException();
        }

        public AbilityIncDamageRate(float value) {
            abilityType = ABILITY_TYPE.DAMAGEINC;
            incDamageRate = value;
        }

        public AbilityIncDamageRate() {
            abilityType = ABILITY_TYPE.DAMAGEINC;
        }
    }
}
