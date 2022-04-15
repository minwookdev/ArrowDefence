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

        /// <summary>
        /// [ Critiacal - X ] [ Min~Max - Choose ] [ Calculate Defence - X ]
        /// </summary>
        /// <param name="isUseMinMaxDamagePer"></param>
        /// <returns></returns>
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

    public class GlobalAbility {                                 // [Default Global Ability Values]
        public float IncreaseDropRate { private set; get; }      = .0f;
        public float ChargedShotMultiplier { private set; get; } = 1.2f;
        public float MinDamagePer { private set; get; }          = 0.9f; // 90~
        public float MaxDamagePer { private set; get; }          = 1.1f; // ~110
        public float IncreaseSpArrCost { private set; get; }     = 0f;

        public GlobalAbility(Ability[] bowAbilities, Ability[] mainArrowAbility, Ability[] subArrowAbility, Ability[] artifactAbility, Ability[] specialArrowAbilities) {
            if (bowAbilities != null) {
                foreach (var ability in bowAbilities) {
                    switch (ability.AbilityType) {
                        case ABILITY_TYPE.CHARGEDAMAGE: ChargedShotMultiplier += ability.GetValueToSingle(); break;
                        default: break;
                    }
                }
            }
        }
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
            Ability[] bowAbilities = (equip.IsEquippedBow()) ? equip.GetBowItem().AbilitiesOrNull : null;
            if (bowAbilities != null) {
                for (int i = 0; i < bowAbilities.Length; i++) {
                    switch (bowAbilities[i]) {
                        case AbilityDamage damage:               tempDamage               = damage.GetValueToSingle(); break;
                        case AbilityChargedDamage chargedDamage: tempChargedDmgMultiplier = chargedDamage.GetValueToSingle(); break;
                        case AbilityCritChance critChance:       tempCritChance           = System.Convert.ToByte(critChance.GetValueToSingle()); break;
                        case AbilityCritDamage critDamage:       tempCritDmgMultiplier    = critDamage.GetValueToSingle(); break;
                        default: throw new System.NotImplementedException();
                    }
                }
            }

            //Update Arrow Ability : Main
            float tempMainArrowIncDamage = 1f;
            Ability[] mainArrowAbilities = (equip.IsEquippedArrowMain()) ? equip.GetMainArrow().AbilitiesOrNull : null;
            if (mainArrowAbilities != null) {
                for (int i = 0; i < mainArrowAbilities.Length; i++) {
                    switch (mainArrowAbilities[i]) {
                        case IncArrowDamageRate incDamage: tempMainArrowIncDamage = incDamage.GetValueToSingle(); break;
                        case AbilitySpeed speed:                                                     break;
                        default: throw new System.NotImplementedException();
                    }
                }
            }

            //Update Arrow Ability : Sub
            float tempSubArrowIncDamage = 1f;
            Ability[] subArrowAbilities = (equip.IsEquippedArrowSub()) ? equip.GetSubArrow().AbilitiesOrNull : null;
            if (subArrowAbilities != null) {
                for (int i = 0; i < subArrowAbilities.Length; i++) {
                    switch (subArrowAbilities[i]) {
                        case IncArrowDamageRate incDamage: tempSubArrowIncDamage = incDamage.GetValueToSingle(); break;
                        case AbilitySpeed speed:                                                    break;
                        default: throw new System.NotImplementedException();
                    }
                }
            }

            //Update Arrow Ability: Special
            float tempSpecialArrowIncDamage = 1f;
            Ability[] specialArrowAbilities = (equip.IsEquippedSpArr) ? equip.GetSpArrow().AbilitiesOrNull : null;
            if (specialArrowAbilities != null) {
                for (int i = 0; i < specialArrowAbilities.Length; i++) {
                    switch (specialArrowAbilities[i]) {
                        default: throw new System.NotImplementedException();
                    }
                }
            }

            //Get Artifact 
            System.Collections.Generic.List<Ability> artifactAbilities = new System.Collections.Generic.List<Ability>();
            var artifacts = equip.GetAccessories(); //각 유물 장착 슬롯마다 분류해줘야하는 경우에, 각각 slot에 해당하는 list로 분류해주기
            for (int i = 0; i < artifacts.Length; i++) {
                if (artifacts[i] != null) {
                    var abilities = artifacts[i].AbilitiesOrNull;
                    if (abilities != null) {
                        for (int j = 0; j < abilities.Length; j++) { //작성시점: 유물의 Ability 분류진행 하지않는 상태
                            artifactAbilities.Add(abilities[i]);
                        }
                    }
                }
            }

            //Init-Ability Slots
            mainSlotAbility    = new PlayerAbilitySlot(tempDamage, tempMainArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
            subSlotAbility     = new PlayerAbilitySlot(tempDamage, tempSubArrowIncDamage, tempCritChance, tempCritDmgMultiplier);
            specialSlotAbility = new PlayerAbilitySlot(tempDamage, tempSpecialArrowIncDamage, tempCritChance, tempCritDmgMultiplier);

            //Init-Global Ability
            GlobalAbilityField = new GlobalAbility(bowAbilities, mainArrowAbilities, subArrowAbilities, artifactAbilities.ToArray(), specialArrowAbilities);
        }
    }
}
