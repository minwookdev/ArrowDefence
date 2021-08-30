namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    public class Item_Bow : Item_Equipment
    {
        private AD_BowSkill bowSkill_Fst;
        private AD_BowSkill bowSkill_Sec;
        private GameObject bowGameObject;

        //장비하고 있는 BowItem 에서 BowSkill을 가져오기 또는 Equipment 부분에서 또 세분화해서 나누기
        //1. 활 2. 화살 3. 활 악세서리 장비 (스코프, 충전기 등의 기능성 장비아이템)
        //public void Awake() => BowSkill = new Skill_Arrow_Rain();

        //AD_Equipment를 상속 받는 아이템에서 활 스킬 함수 자체에 먹여주는 값 (ex : Arrow Num 변수, Angle 변수)을 부여해서 
        //PlayerData로 들어가는 함수를 가볍게 해주기

        /// <summary>
        /// 통합 Bow Item Constructor
        /// </summary>
        /// <param name="id">Item Id</param>
        /// <param name="name">Item Name</param>
        /// <param name="desc">Item Description</param>
        /// <param name="sprite">Item Sprite</param>
        /// <param name="grade">Item Grade</param>
        /// <param name="obj">Bow GameObject</param>
        /// <param name="skill1">First Bow Skill</param>
        /// <param name="skill2">Second Bow Skill</param>
        public Item_Bow(int id, string name, string desc, Sprite sprite, ITEMGRADE grade, 
                        GameObject obj, AD_BowSkill skill1, AD_BowSkill skill2) : base()
        {
            //Equip Item Type Set
            EquipType = EQUIP_ITEMTYPE.EQUIP_BOW;

            //Default Item Data Set
            Item_Id     = id;
            Item_Name   = name;
            Item_Desc   = desc;
            Item_Sprite = sprite;
            Item_Grade  = grade;

            //Bow Item Data Set
            bowGameObject = obj;
            bowSkill_Fst  = skill1;
            bowSkill_Sec  = skill2;
        }

        /// <summary>
        /// Item Address Constructor
        /// </summary>
        /// <param name="item">Bow Item Data Address</param>
        public Item_Bow(ItemData_Equip_Bow item) : base()
        {
            this.EquipType    = EQUIP_ITEMTYPE.EQUIP_BOW;

            this.Item_Id      = item.Item_Id;
            this.Item_Name    = item.Item_Name;
            this.Item_Desc    = item.Item_Desc;
            this.Item_Sprite  = item.Item_Sprite;
            this.Item_Grade   = item.Item_Grade;

            this.bowGameObject = item.BowGameObject;
            this.bowSkill_Fst  = item.BowSkill_First;
            this.bowSkill_Sec  = item.BowSkill_Second;
        }

        /// <summary>
        /// Equip Item Test Constructor
        /// </summary>
        /// <param name="item"></param>
        public Item_Bow(Item_Bow item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_BOW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.bowGameObject = item.bowGameObject;
            this.bowSkill_Fst  = item.bowSkill_Fst;
            this.bowSkill_Sec  = item.bowSkill_Sec;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete and Use this)
        /// </summary>
        public Item_Bow() : base() { }

        public AD_BowSkill GetBowSkill()
        {
            if (bowSkill_Fst != null) return bowSkill_Fst;
            else                      return null;

            //bool isSkill = (bowSkill_Fst != null) ? return bowSkill_Fst : false;
        }

        public AD_BowSkill[] GetBowSkills()
        {
            //AD_BowSkill[] skills = new AD_BowSkill[2] { this.bowSkill_Fst, 
            //                                            this.bowSkill_Sec};

            AD_BowSkill[] skills = { this.bowSkill_Fst, this.bowSkill_Sec };

            return skills;
        }

        public bool IsBowSkill() //Skill이 여러개 일때는 어떻게?
        {
            if (bowSkill_Fst != null) return true;
            else                      return false;
        }

        public GameObject GetBowObject()
        {
            if (this.bowGameObject != null) return bowGameObject;
            else
            {
                CatLog.WLog($"{this.Item_Name}의 Bow GameObject가 NULL 입니다.");
                return null;
            }
        }

        public override object GetItem() => this;
    }
}
