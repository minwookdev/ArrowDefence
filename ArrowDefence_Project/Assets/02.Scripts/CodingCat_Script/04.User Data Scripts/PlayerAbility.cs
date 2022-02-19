namespace ActionCat
{
    using System.Runtime.InteropServices;
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DamageStruct { //Keep Less 16Byte.
        public short Damage { get; private set; }
        public short ArmorPene { get; private set; }
        public byte CritChance { get; private set; }
        public float CritMulti { get; private set; }

        public DamageStruct(PlayerAbilitySlot ability, bool isCharged) {
            Damage     = (isCharged) ? System.Convert.ToInt16(ability.RawDamage * Data.CCPlayerData.ability.GlobalAbilityField.ChargedShotMultiplier) : System.Convert.ToInt16(ability.RawDamage);
            CritChance = ability.CritChance;
            CritMulti  = ability.CritDamageMultiplier;
            ArmorPene  = ability.ArmorPenetRate;
        }

        public void MultiplierDamage(float value) {
            Damage = System.Convert.ToInt16(Damage * value);
        }

        public void DivisionDamage(float value) {
            Damage = System.Convert.ToInt16(Damage / value);
        }

        public float GetFinalCalcDamageOut(short monsterArmorating, byte monsterCritResist, out bool isCritical) {
            isCritical = (GameGlobal.GetCritChance() < CritChance - monsterCritResist) ? true : false;                                //   out critical result.
            var globalAbility = Data.CCPlayerData.ability.GlobalAbilityField;                                                         //   get Global Ability
            float finCalcDamage = UnityEngine.Random.Range(Damage * globalAbility.MinDamagePer, Damage * globalAbility.MaxDamagePer); // * Min ~ * Max per
            finCalcDamage = (isCritical) ? finCalcDamage * CritMulti : finCalcDamage;                                                 // * Calc Critical result
            finCalcDamage = (monsterArmorating - ArmorPene > 0f) ? finCalcDamage - (monsterArmorating - ArmorPene) : finCalcDamage;   //   Calc Armor Penetrating
            finCalcDamage = (finCalcDamage < 1f) ? 1f : UnityEngine.Mathf.Round(finCalcDamage);                                       //   Damage Rounding Calc. (Round -> Ceil)
            return finCalcDamage;
        }

        public float GetFinalCalcDamage(short monsterArmorating, byte monsterCritResist) {
            var globalAbility = Data.CCPlayerData.ability.GlobalAbilityField;                                                              //   get Global Ability
            float finCalcDamage = UnityEngine.Random.Range(Damage * globalAbility.MinDamagePer, Damage * globalAbility.MaxDamagePer);      // * Min ~ * Max
            finCalcDamage = (GameGlobal.GetCritChance() < CritChance - monsterCritResist) ? finCalcDamage * CritMulti : finCalcDamage;     // * Calc Critical
            finCalcDamage = (monsterArmorating - ArmorPene > 0f) ? finCalcDamage - (monsterArmorating - ArmorPene) : finCalcDamage;        //   Calc Armor Penetrating
            finCalcDamage = (finCalcDamage < 1f) ? 1f : UnityEngine.Mathf.Round(finCalcDamage);                                            //   Damage Rounding Calc.
            return finCalcDamage;
        }

        public float GetElementalDamage(bool isUseMinMaxDamagePer = false) {
            var ability = Data.CCPlayerData.ability.GlobalAbilityField;
            float correction = (isUseMinMaxDamagePer) ? UnityEngine.Random.Range(ability.MinDamagePer, ability.MaxDamagePer) : 1f;
            float finCalcDamage = Damage * correction;
            finCalcDamage = (finCalcDamage < 1f) ? 1f : UnityEngine.Mathf.Round(finCalcDamage);
            return finCalcDamage;
        }

        public void SetDamage(short damage) {
            Damage = damage;
        }
    }

    public class PlayerAbilitySlot {
        public float RawDamage { private set; get; } = 0f;
        public float DamageIncRate { private set; get; } = 0f;
        public short ArmorPenetRate { private set; get; } = 0;
        public byte CritChance { private set; get; } = 0;
        public float CritDamageMultiplier { private set; get; } = 1.5f;
        public float ElementalActivationRateIncrease { private set; get; } = 0f;

        public void UpdateSlotAbility(float damage, float arrowIncDamage, byte critChance, float critMultiplier) {
            RawDamage            = damage * arrowIncDamage;
            DamageIncRate        = arrowIncDamage;
            CritChance           = critChance;
            CritDamageMultiplier = critMultiplier;
            ArmorPenetRate       = 0;
        }

        /// <summary>
        /// Init Class Constructor
        /// </summary>
        /// <param name="damage"></param>
        /// <param name="arrowIncDamage"></param>
        /// <param name="critchance"></param>
        /// <param name="critMultiplier"></param>
        public PlayerAbilitySlot(float damage, float arrowIncDamage, byte critchance, float critMultiplier) {
            RawDamage            = damage * arrowIncDamage;
            DamageIncRate        = arrowIncDamage;
            CritChance           = critchance; 
            CritDamageMultiplier = critMultiplier;
            ArmorPenetRate       = 0;
        }

        /// <summary>
        /// Clone Class Constructor
        /// </summary>
        /// <param name="origin"></param>
        public PlayerAbilitySlot(PlayerAbilitySlot origin) {
            RawDamage            = origin.RawDamage;
            DamageIncRate        = origin.DamageIncRate;
            CritChance           = origin.CritChance;
            CritDamageMultiplier = origin.CritDamageMultiplier;
            ArmorPenetRate       = origin.ArmorPenetRate;
        }

        public short GetProjectileDamage(short projectileDamage) {
            return System.Convert.ToInt16(projectileDamage * DamageIncRate);
        }
    }

    public class GlobalAbility {
        public float IncreaseDropRate { private set; get; } = .0f;
        public float ChargedShotMultiplier { private set; get; } = 1.2f;
        public float MinDamagePer { private set; get; } = 0.9f; // 90~
        public float MaxDamagePer { private set; get; } = 1.1f; // ~110
        public float IncreaseSpArrCost { private set; get; } = 0f;
    }

    public class PlayerAbility {
        //Global Ability Class
        public GlobalAbility GlobalAbilityField;

        //Slot Ability
        PlayerAbilitySlot mainSlotAbility = null;
        PlayerAbilitySlot subSlotAbility  = null;
        PlayerAbilitySlot specialSlotAbility = null;

        //Bow Ability Properties
        float tempDamage;
        float tempCritDmgMultiplier;
        float tempChargedDmgMultiplier;
        byte tempCritChance;

        #region PROPERTY
        public PlayerAbilitySlot GetAbilityMain {
            get {
                if(mainSlotAbility == null) {
                    throw new System.Exception("this slot not assignment !");
                }
                return mainSlotAbility;
            }
        }
        public PlayerAbilitySlot GetAbilitySub {
            get {
                if(subSlotAbility == null) {
                    throw new System.Exception("this slot not assignment !");
                }
                return subSlotAbility;
            }
        }
        public PlayerAbilitySlot GetAbilitySpecial {
            get {
                if(specialSlotAbility == null) {
                    throw new System.Exception("this slot not assignment !");
                }
                return specialSlotAbility;
            }
        }
        #endregion

        public void UpdateAbility(Player_Equipments equip) {
            //Update Bow Abilities // ↓ Default Value.
            tempDamage = 0f; tempCritChance = 0; tempCritDmgMultiplier = 1.5f; tempChargedDmgMultiplier = 1.2f;
            if (equip.IsEquippedBow() == true) {
                var abilities = equip.GetBowItem().AbilitiesOrNull;
                if (abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityDamage damage: tempDamage = damage.GetCount(); break;
                            case AbilityChargedDamage chargedDamage: tempChargedDmgMultiplier = chargedDamage.GetCount(); break;
                            case AbilityCritChance critChance: tempCritChance = System.Convert.ToByte(critChance.GetCount()); break;
                            case AbilityCritDamage critDamage: tempCritDmgMultiplier = critDamage.GetCount(); break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            //Update Arrow Ability : Main
            float tempMainArrowIncDamage = 1f;
            if (equip.IsEquippedArrowMain() == true) {
                var abilities = equip.GetMainArrow().AbilitiesOrNull;
                if (abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityIncDamageRate incDamage: tempMainArrowIncDamage = incDamage.GetCount(); break;
                            case AbilitySpeed speed: break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            //Update Arrow Ability : Sub
            float tempSubArrowIncDamage = 1f;
            if (equip.IsEquippedArrowSub() == true) {
                var abilities = equip.GetSubArrow().AbilitiesOrNull;
                if (abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            case AbilityIncDamageRate incDamage: tempSubArrowIncDamage = incDamage.GetCount(); break;
                            case AbilitySpeed speed: break;
                            default: throw new System.NotImplementedException("This Ability Type is Not Implemented !");
                        }
                    }
                }
            }

            //Update Arrow Ability: Special
            float tempSpecialArrowIncDamage = 1f;
            if(equip.IsEquippedSpArr == true) {
                var abilities = equip.GetSpArrOrNull.AbilitiesOrNull;
                if(abilities != null) {
                    for (int i = 0; i < abilities.Length; i++) {
                        switch (abilities[i]) {
                            default: throw new System.NotImplementedException("this ability type is Not Implamented !");
                        }
                    }
                }
            }

            //Init-Ability Slots
            mainSlotAbility    = new PlayerAbilitySlot(tempDamage, tempMainArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
            subSlotAbility     = new PlayerAbilitySlot(tempDamage, tempSubArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
            specialSlotAbility = new PlayerAbilitySlot(tempDamage, tempSpecialArrowIncDamage, tempCritChance, tempCritDmgMultiplier);

            //Init-Global Ability
            GlobalAbilityField = new GlobalAbility();
        }
    }
}
