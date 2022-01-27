namespace ActionCat {
    using UnityEngine;
    public class AbilityData : ScriptableObject {
        //Ability Type
        ABILITY_TYPE AbilityType;

        //Ability : Damage Property
        public float Damage = 0;

        //Ability : Charging Damage
        public float ChargeDamage = 0;
    }
}
