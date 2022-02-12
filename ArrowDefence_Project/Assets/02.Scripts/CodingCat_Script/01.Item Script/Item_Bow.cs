namespace ActionCat
{
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
        /// Bow Item Constructor (Value Type)
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
        /// Bow Item Constructor (Item Data Type)
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
            this.bowSkill_Fst  = item.SkillFst;
            this.bowSkill_Sec  = item.SkillSec;

            this.abilities = item.abilityDatas;
        }

        /// <summary>
        /// Bow Item Constructor (Item Type)
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

            this.abilities = item.abilities;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete and Use this)
        /// </summary>
        public Item_Bow() : base() { }

        public AD_BowSkill GetSkill(int idx)
        {
            if (idx == 0)
                return bowSkill_Fst;
            else if (idx == 1)
                return bowSkill_Sec;
            else
                CatLog.WLog($"GetSkill Method parameter idx is cannot exceed 1 [get parameter : {idx}]");
            return null;
        }

        public AD_BowSkill[] GetSkillsOrNull()
        {
            //AD_BowSkill[] skills = new AD_BowSkill[2] { this.bowSkill_Fst, 
            //                                            this.bowSkill_Sec};

            AD_BowSkill[] skills = { this.bowSkill_Fst, this.bowSkill_Sec };

            return skills;
        }

        public GameObject GetGameObjectOrNull() {
            return (bowGameObject != null) ? bowGameObject : null;
        }

        public override object GetItem() => this;

        public void Init(Transform initPos, Transform parentTr)
        {
            if(bowGameObject == null)
            {
                CatLog.ELog($"{Item_Name} is Bow GameObject is NULL, return Function");
                return;
            }

            var bowObject = GameObject.Instantiate(bowGameObject, initPos.position, Quaternion.Euler(0f, 0f, 90f), parentTr);
        }

        public AD_BowAbility Initialize(Transform initTr, Transform parentTr) {
            if(bowGameObject == null) {
                throw new System.Exception("Bow GameObejct Prefab is null.");
            }

            var prefab = GameObject.Instantiate(bowGameObject, initTr.position, Quaternion.Euler(0f, 0f, 90f), parentTr);
            if (prefab.TryGetComponent<AD_BowAbility>(out AD_BowAbility ability)) {
                ability.Initialize();
                return ability;
            }
            else {
                throw new System.Exception("Controller Ability Component is Null.");
            }
        }
    }
}
