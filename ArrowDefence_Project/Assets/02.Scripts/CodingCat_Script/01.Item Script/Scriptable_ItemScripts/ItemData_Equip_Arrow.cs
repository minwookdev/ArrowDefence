namespace ActionCat {
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Arrow_Asset", menuName = "Scriptable Object Asset/Item_Arrow_Asset")]
    public class ItemData_Equip_Arrow : ItemData_Equip {
        public GameObject MainArrowObj;
        public GameObject LessArrowObj;

        public ArrowSkillData ArrowSkillFst;
        public ArrowSkillData ArrowSkillSec;

        //Inherence Ability
        [RangeEx(0f, 16f, 1.6f)] public float additionalSpeed = 0f;
        [RangeEx(0f, 1f, 0.1f)]  public float arrowDamageIncRate = 0f;
        [RangeEx(0, 50, 5)]      public short projectileDamageInc = 0;
        [RangeEx(0, 150, 15)]    public short spellDamageInc = 0;
        [RangeEx(0, 30, 3)]      public short elementalActivation = 0;

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
            //1. Add Ability Increase Arrow Speed.
            if (additionalSpeed > 0f) {
                if(additionalSpeed > AbilitySpeed.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new AbilitySpeed(additionalSpeed));
            }

            //2. Add Ability Damage Increase.
            if (arrowDamageIncRate > 0f) {
                if(arrowDamageIncRate > IncArrowDamageRate.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncArrowDamageRate(arrowDamageIncRate));
            }

            //3. projectiel Damage Increase
            if (projectileDamageInc > 0) {
                if (projectileDamageInc > IncProjectileDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncProjectileDamage(projectileDamageInc));
            }

            //4. Add Spell Damage Increase
            if (spellDamageInc > 0) {
                if (spellDamageInc > IncSpellDamage.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new IncSpellDamage(spellDamageInc));
            }

            //5. Elemental Activation
            if (elementalActivation > 0) {
                if(elementalActivation > ElementalActivation.MaxValue) {
                    throw new System.Exception();
                }
                tempAbility.Add(new ElementalActivation(elementalActivation));
            }

            if (tempAbility.Count > 4) { //Assignment Abilities Data
                throw new System.Exception("Arrow Ability Max Length is less than 5.");
            }
            abilityDatas = tempAbility.ToArray();
        }
    }
}
