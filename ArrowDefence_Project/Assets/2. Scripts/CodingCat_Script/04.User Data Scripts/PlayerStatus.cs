namespace ActionCat {
    public struct DamageStruct {
        public float Damage { get; private set; }
        public byte CritChance { get; private set; }
        public float CritMulti { get; private set; }
        public byte ArmorPene { get; private set; }

        /// <summary>
        /// Bow Ability에서 생성될 DamageStruct원본.
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="critchance"></param>
        /// <param name="critmulti"></param>
        /// <param name="armorpene"></param>
        public DamageStruct(float damage, byte critchance, float critmulti, byte armorpene) {
            Damage        = damage;
            CritChance    = critchance;
            CritMulti     = critmulti;
            ArmorPene     = armorpene;
        }

        /// <summary>
        /// Arrow, Less Arrow에 뿌려줄 때 사용될 생성자.
        /// </summary>
        /// <param name="origin"></param>
        public DamageStruct(DamageStruct origin) {
            Damage     = origin.Damage;
            CritChance = origin.CritChance;
            CritMulti  = origin.CritMulti;
            ArmorPene  = origin.ArmorPene;
        }

        public float GetFinalCalcDamageOut(short monsterArmorating, byte monsterCritResist, out bool isCritical) {
            isCritical = (GameGlobal.GetCriticalChance() < CritChance - monsterCritResist) ? true : false;
            float finCalcDamage = (isCritical) ? Damage * CritMulti : Damage;
            finCalcDamage = (monsterArmorating - ArmorPene > 0f) ? finCalcDamage - (monsterArmorating - ArmorPene) : finCalcDamage;
            finCalcDamage = (finCalcDamage < 1f) ? 1f : finCalcDamage; //소숫점 처리.
            return finCalcDamage;
        }

        public float GetFinalCalcDamage(short monsterArmorating, byte monsterCritResist) {
            float finCalcDamage = (GameGlobal.GetCriticalChance() < CritChance - monsterCritResist) ? Damage * CritMulti : Damage;
            finCalcDamage = (monsterArmorating - ArmorPene > 0f) ? finCalcDamage - (monsterArmorating - ArmorPene) : finCalcDamage;
            finCalcDamage = (finCalcDamage < 1f) ? 1f : finCalcDamage; //소숫점 처리.
            return finCalcDamage;
        }
    }

    public struct AbilityStruct {
        public float RawDamage { get; private set; }
        public byte ArmorPenetrating { get; private set; }
        public byte CriticalChance { get; private set; }
        public float CriticalMultiplier { get; private set; }
        public float MinDamagePer { get; private set; }
        public float MaxDamagePer { get; private set; }
        
        public AbilityStruct(float rawdamage) {
            RawDamage = rawdamage;
            ArmorPenetrating = 0;
            CriticalChance   = 0;
            CriticalMultiplier = 0f;
            MinDamagePer = 0f;
            MaxDamagePer = 0f;
        }

        public AbilityStruct(AbilityStruct origin) {
            RawDamage          = origin.RawDamage;
            ArmorPenetrating   = origin.ArmorPenetrating;
            CriticalChance     = origin.CriticalChance;
            CriticalMultiplier = origin.CriticalMultiplier;
            MinDamagePer       = origin.MinDamagePer;
            MaxDamagePer       = origin.MaxDamagePer;
        }

        public void UpdateAbility(float damage, float arrowIncDamage, byte critChance, float critMultiplier) {
            RawDamage          = damage * arrowIncDamage;
            CriticalChance     = critChance;
            CriticalMultiplier = critMultiplier;
        }
    }

    public class PlayerStatus {
        AbilityStruct MainSlotStr;
        AbilityStruct SubSlotStr;

        //Bow Ability Properties
        float tempDamage;
        float tempCritDmgMultiplier;
        float tempChargedDmgMultiplier;
        byte  tempCritChance;

        //Arrow Ability Properties


        public void InitDamageStruct(Player_Equipments equip) {
            float bowDamage = 0f;
            if (equip.GetBowItem() != null) {
                var abilites = equip.GetBowItem().AbilitiesOrNull;
                if (abilites != null) {
                    for (int i = 0; i < abilites.Length; i++) {
                        if (abilites[i] is AbilityDamage abilityDamage) {
                            bowDamage = abilityDamage.GetCount();
                        }
                    }
                }
            }

            MainSlotStr = new AbilityStruct(bowDamage);
            SubSlotStr  = new AbilityStruct(bowDamage);
        }

        public void UpdateAbility(Player_Equipments equip) {
            //Update Bow Abilities
            tempDamage = 0f; tempCritChance = 0; tempCritDmgMultiplier = 1.5f; tempChargedDmgMultiplier = 1.2f;
            if(equip.IsEquippedBow() == true) {
                var abilities = equip.GetBowItem().AbilitiesOrNull;
                if(abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityDamage damage: tempDamage                             = damage.GetCount();                            break;
                            case AbilityChargedDamage chargedDamage: tempChargedDmgMultiplier = chargedDamage.GetCount();                     break;
                            case AbilityCritChance critChance: tempCritChance                 = System.Convert.ToByte(critChance.GetCount()); break;
                            case AbilityCritDamage critDamage: tempCritDmgMultiplier          = critDamage.GetCount();                        break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            //Update Arrow Ability : Main
            float tempMainArrowIncDamage = 1f;
            if(equip.IsEquippedArrowMain() == true) {
                var abilities = equip.GetMainArrow().AbilitiesOrNull;
                if(abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityIncDamageRate incDamage: tempMainArrowIncDamage = incDamage.GetCount(); break;
                            case AbilitySpeed speed:                                                            break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            //Update Arrow Ability : Sub
            float tempSubArrowIncDamage = 1f;
            if(equip.IsEquippedArrowSub() == true) {
                var abilities = equip.GetSubArrow().AbilitiesOrNull;
                if(abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityIncDamageRate incDamage: tempSubArrowIncDamage = incDamage.GetCount(); break;
                            case AbilitySpeed speed:                                                           break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            MainSlotStr.UpdateAbility(tempDamage, tempMainArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
            SubSlotStr.UpdateAbility(tempDamage, tempSubArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
        }

        public void UpdateSubWeaponAbiltiy() {

        }

        public AbilityStruct GetMainSlotAbility() {
            return MainSlotStr;
        }

        public AbilityStruct GetSubSlotAbility() {
            return SubSlotStr;
        }
    }
}
