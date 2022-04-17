namespace ActionCat {
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Arrow_Asset", menuName = "Scriptable Object Asset/Item_Arrow_Asset")]
    public class ItemData_Equip_Arrow : ItemData_Equip {
        public GameObject MainArrowObj;
        public GameObject LessArrowObj;

        public ArrowSkillData ArrowSkillFst;
        public ArrowSkillData ArrowSkillSec;

        //Arr Inherence Ability
        [RangeEx(0f, 16f, 1.6f)] public float AdditionalSpeed = 0f;
        [RangeEx(0f, 1f, 0.1f)]  public float ArrowDamageInc = 0f;
        [RangeEx(0, 50, 5)]      public short ProjectileDamageInc = 0;
        [RangeEx(0, 150, 15)]    public short SpellDamageInc = 0;
        //[RangeEx(0, 30, 3)]      public short ElementalActivation = 0;

        //Additional Ability
        [RangeEx(0, 500, 50)]      public short BaseDamageInc = 0;
        [RangeEx(0, 30, 3)]        public byte CriticalChanceInc = 0;
        [RangeEx(0f, 1.5f, 0.15f)] public float CriticalDamageMultiplierInc = 0f;
        [RangeEx(0, 150, 15)]      public short ArmorPenetrationInc = 0;

        public ACEffector2D[] effects;

        public ItemData_Equip_Arrow() : base() {
            this.Equip_Type = EQUIP_ITEMTYPE.ARROW;
        }

        private void OnValidate() {
            CheckArrowSkils();
        }

        private void OnEnable() {
            InitAbility();
        }

        void CheckArrowSkils() {
            if(ArrowSkillFst != null) {
                if (ArrowSkillSec == null) return;
                if(ArrowSkillFst.ActiveType == ArrowSkillSec.ActiveType) {
                    CatLog.WLog($"Active Type이 중복된 Arrow Skill이 감지되었습니다. Type : {ArrowSkillFst.ActiveType}");
                }
            }
        }

        void InitAbility() {
            System.Collections.Generic.List<Ability> tempAbility = new System.Collections.Generic.List<Ability>();
            //=============================================== << ARROW INHERENCE ABILITY >> ===============================================
            //1. Add Ability Increase Arrow Speed.
            if (AdditionalSpeed > 0f) {
                if(AdditionalSpeed > AbilitySpeed.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new AbilitySpeed(AdditionalSpeed));
            }
            //2. Add Ability Damage Increase.
            if (ArrowDamageInc > 0f) {
                if(ArrowDamageInc > IncArrowDamageRate.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncArrowDamageRate(ArrowDamageInc));
            }
            //3. projectiel Damage Increase
            if (ProjectileDamageInc > 0) {
                if (ProjectileDamageInc > IncProjectileDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncProjectileDamage(ProjectileDamageInc));
            }
            //4. Add Spell Damage Increase
            if (SpellDamageInc > 0) {
                if (SpellDamageInc > IncSpellDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncSpellDamage(SpellDamageInc));
            }
            //5. Elemental Activation -> to Bow
            //if (ElementalActivation > 0) {
            //    if(ElementalActivation > ActionCat.ElementalActivation.MaxValue) {
            //        throw new System.Exception();
            //    }
            //    tempAbility.Add(new ElementalActivation(ElementalActivation));
            //}
            //=================================================== << PUBLIC ABILITY >> ====================================================
            //6. Base Damage Increase
            if (BaseDamageInc > 0) {
                if (BaseDamageInc > AbilityDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new AbilityDamage(BaseDamageInc));
            }
            //7. Crtitical Chance Increase
            if (CriticalChanceInc > 0) {
                if(CriticalChanceInc > AbilityCritChance.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new AbilityCritChance(CriticalChanceInc));
            }
            //8. Critical Damage Multiplier Increase
            if (CriticalDamageMultiplierInc > 0f) {
                if (CriticalDamageMultiplierInc > AbilityCritDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new AbilityCritDamage(CriticalDamageMultiplierInc));
            }
            //9. Armor Penetration Increase
            if (ArmorPenetrationInc > 0) {
                if (ArmorPenetrationInc > PenetrationArmor.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new PenetrationArmor(ArmorPenetrationInc));
            }
            //================================================ << ASSIGNMENT ABILITY >> ===================================================
            if (tempAbility.Count > 4) { //Assignment Abilities Data
                throw new System.Exception("Arrow Ability Max Length is less than 5.");
            }
            abilityDatas = tempAbility.ToArray();
        }
    }
}
