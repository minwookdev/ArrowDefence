namespace ActionCat
{
    using System.Runtime.InteropServices;
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct DamageStruct { //Keep Less 16Byte.
        public short Damage { get; private set; }
        public short ArmorPene { get; private set; }
        public byte CritChance { get; private set; }
        public float CritMulti { get; private set; } // <--- 글로벌 어빌리리티로 뺄 수도

        public DamageStruct(PlayerAbilitySlot ability, bool isCharged) {
            Damage     = (isCharged) ? System.Convert.ToInt16(ability.BaseDamage * Data.CCPlayerData.ability.GlobalAbilityField.ChargedShotMultiplier) : System.Convert.ToInt16(ability.BaseDamage);
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
            isCritical = GameGlobal.IsCritical(CritChance, monsterCritResist);                                                        //   out critical result.
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
            finCalcDamage = GameGlobal.IsCritical(CritChance, monsterCritResist) ? finCalcDamage * CritMulti : finCalcDamage;              // * Calc Critical
            finCalcDamage = (monsterArmorating - ArmorPene > 0f) ? finCalcDamage - (monsterArmorating - ArmorPene) : finCalcDamage;        //   Calc Armor Penetrating
            finCalcDamage = (finCalcDamage < 1f) ? 1f : UnityEngine.Mathf.Round(finCalcDamage);                                            //   Damage Rounding Calc.
            return finCalcDamage;
        }

        /// <summary>
        /// [ Critiacal - F ] [ Min~Max - SELECT ] [ Calculate Defence - F ]
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

        /// <summary>
        /// [ Critiacal - T ] [ Min~Max - T ] [ Calculate Defence - T ]
        /// </summary>
        /// <returns></returns>
        public float GetProjectileDamage() {
            throw new System.NotImplementedException();
        }

        public void SetDamage(short damage) {
            Damage = damage;
        }
    }

    public class PlayerAbilitySlot {                            //Default Value Field
        public float BaseDamage { private set; get; }           = 0f;
        public float ArrowDamageIncRate { private set; get; }   = 1f;
        public short ArmorPenetRate { private set; get; }       = 0;
        public byte CritChance { private set; get; }            = 0;
        public float CritDamageMultiplier { private set; get; } = 1.5f;
        public short ProjectileDamageInc { private set; get; }  = 0;
        public short SpellDamageInc { private set; get; }       = 0;

        /// <summary>
        /// Clone Class Constructor
        /// </summary>
        /// <param name="origin"></param>
        public PlayerAbilitySlot(PlayerAbilitySlot origin) {
            BaseDamage             = origin.BaseDamage;
            CritChance             = origin.CritChance;
            CritDamageMultiplier   = origin.CritDamageMultiplier;
            ArmorPenetRate         = origin.ArmorPenetRate;
            ArrowDamageIncRate     = origin.ArrowDamageIncRate;
            ProjectileDamageInc    = origin.ProjectileDamageInc;
            SpellDamageInc         = origin.SpellDamageInc;
        }

        public short GetProjectileDamage(short projectileDamageCount) {
            return (short)(projectileDamageCount + ProjectileDamageInc);
        }

        public short GetSpellDamage(short spellDamageCount) {
            return (short)(spellDamageCount + SpellDamageInc);
        }

        /// <summary>
        /// Main && Sub Ability Slot
        /// </summary>
        /// <param name="bow"></param>
        /// <param name="arrow"></param>
        public PlayerAbilitySlot(Item_Bow bow, Item_Arrow arrow, Ability[] artifacts) {
            if (bow != null) {
                foreach (var ability in bow.AbilitiesOrNull) {
                    switch (ability.AbilityType) {
                        case ABILITY_TYPE.DAMAGE:              BaseDamage             += ability.GetValueToInt16();  break;
                        case ABILITY_TYPE.CRITICALCHANCE:      CritChance             += ability.GetValueToByte();   break;
                        case ABILITY_TYPE.CRITICALDAMAGE:      CritDamageMultiplier   += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.ARMORPENETRATE:      ArmorPenetRate         += ability.GetValueToInt16();  break;
                    }
                }
            }

            if (arrow != null) {
                foreach (var ability in arrow.AbilitiesOrNull) {
                    switch (ability.AbilityType) {
                        case ABILITY_TYPE.DAMAGE:          BaseDamage           += ability.GetValueToInt16(); break;
                        case ABILITY_TYPE.CRITICALCHANCE:  CritChance           += ability.GetValueToByte();   break;
                        case ABILITY_TYPE.CRITICALDAMAGE:  CritDamageMultiplier += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.ARMORPENETRATE:  ArmorPenetRate       += ability.GetValueToInt16();  break;

                        case ABILITY_TYPE.ARROWDAMAGEINC:   ArrowDamageIncRate  += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.PROJECTILEDAMAGE: ProjectileDamageInc += ability.GetValueToInt16();  break;
                        case ABILITY_TYPE.SPELLDAMAGE:      SpellDamageInc      += ability.GetValueToInt16();  break;
                    }
                }
            }

            for (int i = 0; i < artifacts.Length; i++) {

            }

            //Calc BaseDamage -> '증가율' 로 적용되는 옵션들은 요렇게 처리되도록 구현
            BaseDamage = BaseDamage * ArrowDamageIncRate;
        }

        /// <summary>
        /// Special Ability Slot
        /// </summary>
        /// <param name="bow"></param>
        /// <param name="arrow"></param>
        public PlayerAbilitySlot(Item_Bow bow, Item_SpArr arrow, Ability[] artifacts) {
            if (bow != null) {
                foreach (var ability in bow.AbilitiesOrNull) {
                    switch (ability.AbilityType) {
                        case ABILITY_TYPE.DAMAGE:              BaseDamage             += ability.GetValueToInt16();  break;
                        case ABILITY_TYPE.CRITICALCHANCE:      CritChance             += ability.GetValueToByte();   break;
                        case ABILITY_TYPE.CRITICALDAMAGE:      CritDamageMultiplier   += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.ARMORPENETRATE:      ArmorPenetRate         += ability.GetValueToInt16();  break;
                    }
                }
            }

            if (arrow != null) {
                foreach (var ability in arrow.AbilitiesOrNull) {
                    switch (ability.AbilityType) {
                        case ABILITY_TYPE.DAMAGE:         BaseDamage           += ability.GetValueToInt16(); break;
                        case ABILITY_TYPE.CRITICALCHANCE: CritChance           += ability.GetValueToByte();   break;
                        case ABILITY_TYPE.CRITICALDAMAGE: CritDamageMultiplier += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.ARMORPENETRATE: ArmorPenetRate       += ability.GetValueToInt16();  break;

                        case ABILITY_TYPE.ARROWDAMAGEINC:   ArrowDamageIncRate  += ability.GetValueToSingle(); break;
                        case ABILITY_TYPE.PROJECTILEDAMAGE: ProjectileDamageInc += ability.GetValueToInt16();  break;
                        case ABILITY_TYPE.SPELLDAMAGE:      SpellDamageInc      += ability.GetValueToInt16();  break;
                    }
                }
            }

            for (int i = 0; i < artifacts.Length; i++) {

            }

            //Calc BaseDamage -> '증가율' 로 적용되는 옵션들은 요렇게 처리되도록 구현
            BaseDamage = BaseDamage * ArrowDamageIncRate;
        }

        //여기서 바로 DamageStruct를 밷어내개 할 수 있을까
        //Projectile SetDamage좀 더 직관적으로 할 수 있도록 만들기
    }

    public class GlobalAbility {                                 // [Default Global Ability Values]
        public float MaxPlayerHealth { private set; get; }       = 100f;
        public float IncreaseDropRate { private set; get; }      = .0f;
        public float ChargedShotMultiplier { private set; get; } = 1.25f;
        public float MinDamagePer { private set; get; }          = 0.9f; // 90 % ~
        public float MaxDamagePer { private set; get; }          = 1.1f; // ~ 110 %
        public float IncreaseSpArrCost { private set; get; }     = 0f;
        public byte AdditionalArrowFire { private set; get; }    = 0;
        public short ElementalActivation { private set; get; }   = 0;
        public float ElementalDamageAmplification { private set; get; } = 1f; // <-- 아직 적용안되고 있음
        public float PhysicalDamageAmplification { private set; get; }  = 1f; // <-- 아직 적용안되고 있음

        public GlobalAbility(Item_Bow bow, Item_SpArr specialArr, Ability[] artifacts) {
            if (bow != null) {
                ChargedShotMultiplier = bow.IsExistAbility(ABILITY_TYPE.CHARGEDAMAGE, out Ability chargedShotMutliplier) ? ChargedShotMultiplier + chargedShotMutliplier.GetValueToSingle() : ChargedShotMultiplier;
                AdditionalArrowFire   = bow.IsExistAbility(ABILITY_TYPE.ADDITIONALFIRE, out Ability additionalFire) ? (byte)(AdditionalArrowFire + additionalFire.GetValueToByte()) : AdditionalArrowFire;
                ElementalActivation   = bow.IsExistAbility(ABILITY_TYPE.ELEMENTALACTIVATION, out Ability elementalActivationAbility) ? (short)(ElementalActivation + elementalActivationAbility.GetValueToInt16()) : ElementalActivation;
            }

            if (specialArr != null) {
                var skills = specialArr.GetSkillInfos;
                foreach (var skill in skills) {
                    if(skill.SkillType == ARROWSKILL.BUFF) {
                        var buffTypeSkill = skill.SkillData as BuffTypeArrowSkill;
                        if (buffTypeSkill == null) {
                            throw new System.Exception();
                        }
                        switch (buffTypeSkill.BuffType) {
                            case ARROWBUFFTYPE.ELEMENTALAMPLIFICATION: ElementalDamageAmplification = buffTypeSkill.GetValue(); break;
                            //case ARROWBUFFTYPE.PHYSICALAMPLIFICATION:  PhysicalDamageAmplification  = buffTypeSkill.GetValue(); break;
                            default: throw new System.NotImplementedException();
                        }
                    }
                }
            }
        }
    }

    public class PlayerAbility {
        //Global Ability Class
        public GlobalAbility GlobalAbilityField = new GlobalAbility(null, null, null);

        //Slot Ability
        PlayerAbilitySlot mainSlotAbility = null;
        PlayerAbilitySlot subSlotAbility  = null;
        PlayerAbilitySlot specialSlotAbility = null;

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
        public void UpdateAbility(Player_Equipments equipments) {
            //Get All Artifact Abilities.
            System.Collections.Generic.List<Ability> allArtifactAbilites = new System.Collections.Generic.List<Ability>();
            foreach (var artifact in equipments.GetArtifacts()) {
                if (artifact != null) {
                    var abilities = artifact.AbilitiesOrNull;
                    for (int i = 0; i < abilities.Length; i++) {
                        allArtifactAbilites.Add(abilities[i]);
                    }
                }
            }
            var artifactAbilitiesArray = allArtifactAbilites.ToArray();

            //Assignment New-Ability Slots
            mainSlotAbility    = new PlayerAbilitySlot(equipments.GetBowItem(), equipments.GetMainArrow(), artifactAbilitiesArray);
            subSlotAbility     = new PlayerAbilitySlot(equipments.GetBowItem(), equipments.GetSubArrow(), artifactAbilitiesArray);
            specialSlotAbility = new PlayerAbilitySlot(equipments.GetBowItem(), equipments.GetSpArrOrNull, artifactAbilitiesArray);

            //Assignment New-Global Ability
            GlobalAbilityField = new GlobalAbility(equipments.GetBowItem(), equipments.GetSpArrOrNull, artifactAbilitiesArray);  
        }
    }
}
