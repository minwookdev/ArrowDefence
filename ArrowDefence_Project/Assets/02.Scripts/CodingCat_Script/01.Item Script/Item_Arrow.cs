namespace ActionCat
{
    using UnityEngine;

    public class Item_Arrow : Item_Equipment {
        //Required Saving Variables
        //Arrow Prefab
        private GameObject MainArrowObject;
        private GameObject LessArrowObject;
        //Arrow Skill Data Class
        ASInfo arrowSkillInfoFst;
        ASInfo arrowSkillInfoSec;
        //Arrow Default Hit Effect
        ACEffector2D[] effects;

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
        
        public ASInfo[] SkillInfosOrNull {
            get {
                ASInfo[] skillinfo = new ASInfo[2] {
                    arrowSkillInfoFst, arrowSkillInfoSec
                };
                return skillinfo;
            }
        }

        /// <summary>
        /// Constructor for Item ScriptableObject
        /// </summary>
        /// <param name="item"></param>
        public Item_Arrow (ItemData_Equip_Arrow item) : base() {
            this.EquipType = EQUIP_ITEMTYPE.EQUIP_ARROW;

            this.Item_Id     = item.Item_Id;
            this.Item_Name   = item.Item_Name;
            this.Item_Desc   = item.Item_Desc;
            this.Item_Sprite = item.Item_Sprite;
            this.Item_Grade  = item.Item_Grade;

            //Init Arrow Prefab
            this.MainArrowObject = item.MainArrowObj;
            this.LessArrowObject = item.LessArrowObj;

            //Init-Arrow Skill Info
            if (item.ArrowSkillFst != null) {
                this.arrowSkillInfoFst = new ASInfo(item.ArrowSkillFst);
            }
            if (item.ArrowSkillSec != null) {
                this.arrowSkillInfoSec = new ASInfo(item.ArrowSkillSec);
            }

            //Init-Ability Info
            this.abilities = item.abilityDatas;

            this.effects = item.effects;
        }

        /// <summary>
        /// Constructor for Item Clone
        /// </summary>
        /// <param name="item"></param>
        public Item_Arrow(Item_Arrow item) : base() {
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

            //Init-Ability Info
            this.abilities = item.abilities;

            this.effects = item.effects;
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

        bool TryFindEffect(out ACEffector2D[] array, ref int arraySize) {
            if (arrowSkillInfoFst == null && arrowSkillInfoSec == null) {
                // Non-Skill - Use ArrItem Default Effects
                array = null; 
                return false;
            }
            else {
                // Has-Skill - Use HitType Skill inherence Effects
                if (arrowSkillInfoFst != null && arrowSkillInfoSec != null) {
                    // two-skill
                    var arrSkill_f = arrowSkillInfoFst.SkillData as AttackActiveTypeAS;
                    var arrSkill_s = arrowSkillInfoSec.SkillData as AttackActiveTypeAS;
                    if (arrSkill_f != null && arrSkill_s != null) {
                        throw new System.Exception($"Item Initialize Failed, {Item_Name} has Duplicate skill type.");
                    }
                    else if (arrSkill_f != null) {  // firstslot is HitType skill
                        if(arrSkill_f.IsEffectUser == true) {
                            array = arrSkill_f.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                    else if (arrSkill_s != null) {  // secondsSlot is HitType skill
                        if(arrSkill_s.IsEffectUser == true) {
                            array = arrSkill_s.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                }
                else {
                    // one-skill
                    if (arrowSkillInfoFst != null && arrowSkillInfoFst.SkillData is AttackActiveTypeAS hitSkillFst) {
                        if(hitSkillFst.IsEffectUser == true) { // first slot is HitType Skill
                            array = hitSkillFst.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                    else if(arrowSkillInfoSec != null && arrowSkillInfoSec.SkillData is AttackActiveTypeAS hitSkillSec) {
                        if(hitSkillSec.IsEffectUser == true) { // seconds slot is HitType Skill
                            array = hitSkillSec.Effects;
                            arraySize = array.Length;
                            return true;
                        }
                    }
                }
            }

            array = null; 
            return false;
        }

        public void Init(string mainArrowObjTag, string lessArrowObjTag, int poolQuantity) {
            if(MainArrowObject == null || LessArrowObject == null) {
                throw new System.Exception($"ArrowItem : {Item_Name} is ArrowPrefab NULL.");
            }

            bool isMainArr = MainArrowObject.TryGetComponent<AD_Arrow>(out AD_Arrow mainArrow);
            bool isLessArr = LessArrowObject.TryGetComponent<AD_Arrow_less>(out AD_Arrow_less lessArrow);

            if(isMainArr == false || isLessArr == false) {
                throw new System.Exception($"ArrItem : {Item_Name} is Component Not Assignment.");
            }

            System.Collections.Generic.List<string> effectPoolTags = new System.Collections.Generic.List<string>();
            int effectArrayLenght = 0;

            // Add PoolList Effects 
            if(TryFindEffect(out ACEffector2D[] effectArray, ref effectArrayLenght)) {
                for (int i = 0; i < effectArray.Length; i++) {
                    string effectPoolTag = mainArrowObjTag + AD_Data.POOLTAG_HITEFFECT + i.ToString();
                    CCPooler.AddPoolList(effectPoolTag, 3, effectArray[i].gameObject, isTracking: false);
                    effectPoolTags.Add(effectPoolTag);
                }
            }
            else {
                if(effects == null || effects.Length == 0) {
                    effectArrayLenght = 0;
                }
                else {
                    effectArrayLenght = effects.Length;
                    for (int i = 0; i < effects.Length; i++) {
                        string effectPoolTag = mainArrowObjTag + AD_Data.POOLTAG_HITEFFECT + i.ToString();
                        CCPooler.AddPoolList(effectPoolTag, 3, effects[i].gameObject, isTracking: false);
                        effectPoolTags.Add(effectPoolTag);
                    }
                }
            }

            // Init ArrSkillSets
            if(arrowSkillInfoFst == null && arrowSkillInfoSec == null) {
                arrowSkillSets = null;
            }
            else {
                arrowSkillSets = new ArrowSkillSet(arrowSkillInfoFst, arrowSkillInfoSec, mainArrowObjTag);
                CatLog.Log($"Arritem : {Item_Name} is initialized skills.");
            }

            mainArrow.SetEffectInfo(effectPoolTags.ToArray());
            lessArrow.SetEffectInfo(effectPoolTags.ToArray());

            //Create Pools of Arrow Object
            CCPooler.AddPoolList(mainArrowObjTag, poolQuantity, MainArrowObject, isTracking: false);
            CCPooler.AddPoolList(lessArrowObjTag, poolQuantity, LessArrowObject, isTracking: false);
        }

        public void Release() {
            //Clear Arrow Skill Sets Class 
            if (arrowSkillSets == null)
                return;
            arrowSkillSets = null;

            //Remove Origin Prefab EffectTags string Array
            MainArrowObject.GetComponent<AD_Arrow>().RemoveEffectInfo();
            LessArrowObject.GetComponent<AD_Arrow_less>().RemoveEffectInfo();
        }
    }
}
