namespace ActionCat
{
    using UnityEngine;

    public class Item_Arrow : Item_Equipment
    {
        //Required Saving Variables
        //Arrow Prefab
        private GameObject MainArrowObject;
        private GameObject LessArrowObject;
        //Arrow Skill Data Class
        ASInfo arrowSkillInfoFst;
        ASInfo arrowSkillInfoSec;

        //↘Not Save This SkillSet Class [TEMP]
        ArrowSkillSet arrowSkillSets = null;
        public ArrowSkillSet ArrowSkillSets { 
            get {
                if (arrowSkillSets != null)
                    return arrowSkillSets;
                else
                    return null;
            }
        }

        public Item_Arrow (ItemData_Equip_Arrow item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Init Arrow Prefab
            this.MainArrowObject = item.MainArrowObj;
            this.LessArrowObject = item.LessArrowObj;

            //Init Arrow Skill Information
            //Init-Arrow Skill Info
            if (item.ArrowSkillFst != null)
                this.arrowSkillInfoFst = new ASInfo(item.ArrowSkillFst);
            if (item.ArrowSkillSec != null)
                this.arrowSkillInfoSec = new ASInfo(item.ArrowSkillSec);
        }

        public Item_Arrow(Item_Arrow item) : base()
        {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Init Arrow Prefab
            this.MainArrowObject = item.MainArrowObject;
            this.LessArrowObject = item.LessArrowObject;

            //Init Arrow Skill Data
            this.arrowSkillInfoFst = item.arrowSkillInfoFst;
            this.arrowSkillInfoSec = item.arrowSkillInfoSec;
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

            //Create Arrow SkillSets Instances
            if (arrowSkillInfoFst == null && arrowSkillInfoSec == null)
                arrowSkillSets = null;
            else
            {
                arrowSkillSets = new ArrowSkillSet(arrowSkillInfoFst, arrowSkillInfoSec);
                CatLog.Log($"Arrow Item : {Item_Name} is Init Skills");
            }

            //Create Pools of Arrow Object
            CCPooler.AddPoolList(mainArrowObjTag, poolQuantity, MainArrowObject);
            CCPooler.AddPoolList(lessArrowObjTag, poolQuantity, LessArrowObject);
        }

        public void Release()
        {
            //Clear Arrow Skill Sets Class 
            if (arrowSkillSets == null)
                return;
            arrowSkillSets = null;
        }
    }
}
