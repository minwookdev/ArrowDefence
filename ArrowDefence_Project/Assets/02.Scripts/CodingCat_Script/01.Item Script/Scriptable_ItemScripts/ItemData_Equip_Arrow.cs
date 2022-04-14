namespace ActionCat {
    using UnityEngine;

    [CreateAssetMenu(fileName = "Item_Arrow_Asset", menuName = "Scriptable Object Asset/Item_Arrow_Asset")]
    public class ItemData_Equip_Arrow : ItemData_Equip {
        public GameObject MainArrowObj;
        public GameObject LessArrowObj;

        public ArrowSkillData ArrowSkillFst;
        public ArrowSkillData ArrowSkillSec;

        //Inherence Ability
        [Range(1f, 1.5f)] public float DamageInc = 1f;
        [Range(18f, 28f)] public float Speed = 18f;

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
            //1. Add Ability Damage Increase.
            if (DamageInc > 0f) tempAbility.Add(new IncDamageRate(DamageInc));
            //2. Add Ability Increase Arrow Speed.
            if (Speed > 18f)    tempAbility.Add(new AbilitySpeed(Speed));

            //3. Temp Ability List to item Abilities.
            abilityDatas = tempAbility.ToArray();
            //if (tempAbility.Count > 0) {
            //    abilityDatas = tempAbility.ToArray();
            //}
        }
    }
}
