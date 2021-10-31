namespace ActionCat
{
    using UnityEngine;

    public class Item_Arrow : Item_Equipment
    {
        private GameObject MainArrowObject;
        private GameObject LessArrowObject;

        //private Arrow_Skill arrowSkill;
        //화살 전용 특수효과를 어떻게 담을지

        public Item_Arrow (ItemData_Equip_Arrow item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.MainArrowObject = item.MainArrowObj;
            this.LessArrowObject = item.LessArrowObj;
        }

        public Item_Arrow(int id, string name, Sprite sprite, ITEMGRADE grade, GameObject main, GameObject less) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;
            this.Item_Id = id;
            this.Item_Name = name;
            this.Item_Sprite = sprite;
            this.Item_Grade = grade;

            this.MainArrowObject = main;
            this.LessArrowObject = less;
        }

        public Item_Arrow(Item_Arrow item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            this.MainArrowObject = item.MainArrowObject;
            this.LessArrowObject = item.LessArrowObject;
        }

        /// <summary>
        /// Constructor With no Parameters. (Used Saving Function. Don't Delete this) 
        /// </summary>
        public Item_Arrow() : base() { }

        public override object GetItem() => this;

        public GameObject GetObject_MainArrow()
        {
            if(MainArrowObject == null)
            {
                CatLog.ELog($"{Item_Name} Item is Not have a Main Arrow Object");
                return null;
            }
            else
            {
                return MainArrowObject;
            }
        }

        public GameObject GetObject_LessArrow()
        {
            if(LessArrowObject == null)
            {
                CatLog.ELog($"{Item_Name} Item is Not have a Less Arrow Object");
                return null;
            }
            else
            {
                return LessArrowObject;
            }
        }

        public void Init(string mainArrowObjTag, string lessArrowObjTag, int poolQuantity)
        {
            if(MainArrowObject == null || LessArrowObject == null)
            {
                CatLog.ELog($"{Item_Name} is Arrow GameObject is NULL, return Function");
                return;
            }

            CCPooler.AddPoolList(mainArrowObjTag, poolQuantity, MainArrowObject);
            CCPooler.AddPoolList(lessArrowObjTag, poolQuantity, LessArrowObject);
        }
    }
}
