namespace ActionCat {
    using UnityEngine;

    public class Item_SpArr : Item_Equipment {
        private GameObject spArrowPref = null;
        private ASInfo[] skillInfos    = null;
        public ASInfo[] GetSkillInfos {
            get {
                if(skillInfos == null) {
                    skillInfos = new ASInfo[0];
                }
                return skillInfos;
            }
        }

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            return new Ability[0]; 
        }

        public Item_SpArr(ItemDt_SpArr data) : base() {
            EquipType   = EQUIP_ITEMTYPE.EQUIP_ARROW;
            Item_Id     = data.Item_Id;
            Item_Name   = data.Item_Name;
            Item_Desc   = data.Item_Desc;
            Item_Sprite = data.Item_Sprite;
            Item_Grade  = data.Item_Grade;

            //Ability
            abilities = GetNewAbilities(data.abilityDatas);

            //Special Arrow Prefab
            spArrowPref = data.MainArrowObj;

            //Skills Info
            var tempList = new System.Collections.Generic.List<ASInfo>();
            if (data.ArrowSkillFst != null) tempList.Add(new ASInfo(data.ArrowSkillFst));
            if (data.ArrowSkillSec != null) tempList.Add(new ASInfo(data.ArrowSkillSec));
            skillInfos = tempList.ToArray();
        }
        #region ES3
        public Item_SpArr() { }
        ~Item_SpArr() { }
        #endregion
    }
}
