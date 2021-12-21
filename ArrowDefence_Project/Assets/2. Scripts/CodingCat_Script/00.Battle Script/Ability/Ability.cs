namespace ActionCat {
    public abstract class Ability {
        public abstract float GetCount();
        public abstract byte GetGrade();
        public abstract string GetName();
    }
    public enum ABILITY_TYPE {
        DAMAGE         = 0,
        FULLCHARGING   = 1,
        CRITICALCHANCE = 2,
        CRITICALDAMAGE = 3,
    }

    public class AbilityDamage : Ability {
        private string abilityName = "DAMAGE";
        private float damageCount;

        public override float GetCount() {
            return damageCount;
        }

        public override string GetName() {
            return abilityName;
        }

        public override byte GetGrade() {
            if (damageCount <= 100)      return 1;
            else if (damageCount <= 200) return 2;
            else if (damageCount <= 300) return 3;
            else if (damageCount <= 400) return 4;
            else if (damageCount <= 500) return 5;
            else                         return 5;
        }

        public AbilityDamage(float damage) {
            damageCount = damage;
        }

        public AbilityDamage() { }
        ~AbilityDamage() { }
    }

    public class AbilityChargedDamage : Ability {
        string abilityName = "CHARGED SHOT DAMAGE";
        float chargedShotDamageMulti;

        public override float GetCount() {
            return chargedShotDamageMulti;
        }

        public override byte GetGrade() {
            //if(chargedShotDamageMulti > 1.2f) {
            //    if (chargedShotDamageMulti <= 1.5f) return 1;
            //    else if (chargedShotDamageMulti <= 1.8f) return 2;
            //    else if (chargedShotDamageMulti <= 2.1f) return 3;
            //    else if (chargedShotDamageMulti <= 2.5f) return 4;
            //}
            throw new System.NotImplementedException();
        }

        public override string GetName() {
            throw new System.NotImplementedException();
        }

        public AbilityChargedDamage() {

        }
    }

    public class AbilityCritChance : Ability {
        public override float GetCount()
        {
            throw new System.NotImplementedException();
        }

        public override byte GetGrade()
        {
            throw new System.NotImplementedException();
        }

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }
    }

    public class AbilityCritDamage : Ability
    {
        public override float GetCount()
        {
            throw new System.NotImplementedException();
        }

        public override byte GetGrade()
        {
            throw new System.NotImplementedException();
        }

        public override string GetName()
        {
            throw new System.NotImplementedException();
        }
    }
}
