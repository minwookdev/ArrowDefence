namespace ActionCat {
    using UnityEngine;

    public class Item_Arrow : Item_Equipment {
        //Required Saving Variables
        //Arrow Prefab
        private GameObject MainArrowObject;
        private GameObject LessArrowObject;
        float speed;
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
        /// Arrow Item Entity Constructor
        /// </summary>
        /// <param name="entity"></param>
        public Item_Arrow(ItemData_Equip_Arrow entity) : base(entity) {
            this.EquipType = EQUIP_ITEMTYPE.ARROW;

            this.MainArrowObject = entity.MainArrowObj;
            this.LessArrowObject = entity.LessArrowObj;

            this.arrowSkillInfoFst = (entity.ArrowSkillFst != null) ? new ASInfo(entity.ArrowSkillFst) : null;
            this.arrowSkillInfoSec = (entity.ArrowSkillSec != null) ? new ASInfo(entity.ArrowSkillSec) : null;
            this.abilities         = GetNewAbilities(entity.abilityDatas);
            this.effects           = entity.effects;

            //assignment arrow speed value
            this.speed = (IsExistAbility(ABILITY_TYPE.ARROWSPEED, out Ability speedIncrease)) ?
                GameGlobal.DefaultArrowSpeed + speedIncrease.GetValueToSingle() : GameGlobal.DefaultArrowSpeed;
        }

        /// <summary>
        /// Item Clone Constructor
        /// </summary>
        /// <param name="origin"></param>
        public Item_Arrow(Item_Arrow origin) : base(origin) {
            this.EquipType         = origin.EquipType;
            this.MainArrowObject   = origin.MainArrowObject;
            this.LessArrowObject   = origin.LessArrowObject;
            this.speed             = origin.speed;
            this.arrowSkillInfoFst = origin.arrowSkillInfoFst;
            this.arrowSkillInfoSec = origin.arrowSkillInfoSec;
            this.abilities         = origin.abilities;
            this.effects           = origin.effects;
        }

        #region ES3
        public Item_Arrow() : base() { }
        #endregion

        protected override Ability[] GetNewAbilities(Ability[] abilities) {
            var tempList = new System.Collections.Generic.List<Ability>();
            for (int i = 0; i < abilities.Length; i++) {
                switch (abilities[i].AbilityType) {
                    case ABILITY_TYPE.ARROWDAMAGEINC:
                        var damageIncrease = abilities[i] as IncArrowDamageRate;
                        if (damageIncrease != null) {
                            tempList.Add(new IncArrowDamageRate(damageIncrease.GetValueToSingle()));
                        }
                        else {
                            CatLog.ELog("New Ability Type Not Matched !");
                        } break;
                    case ABILITY_TYPE.ARROWSPEED:
                        var speed = abilities[i] as AbilitySpeed;
                        if (speed != null) {
                            tempList.Add(new AbilitySpeed(speed.GetValueToSingle()));
                        }
                        else {
                            CatLog.ELog("New Ability Type Not Matched !");
                        } break;
                    default: throw new System.NotImplementedException();
                }
            }
            return tempList.ToArray();
        }


        public override object GetItem() => this;

        public GameObject GetObject_MainArrow()
        {
            if(MainArrowObject == null)
            {
                CatLog.ELog($"{termsName} Item is Not have a Main Arrow Object");
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
                CatLog.ELog($"{termsName} Item is Not have a Less Arrow Object");
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
                        throw new System.Exception($"Item Initialize Failed, {termsName} has Duplicate skill type.");
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

        public void Init(string mainArrowObjTag, string lessArrowObjTag, int poolQuantity, PlayerAbilitySlot ability) {
            if(MainArrowObject == null || LessArrowObject == null) {
                throw new System.Exception($"ArrowItem : {termsName} is ArrowPrefab NULL.");
            }

            bool isMainArr = MainArrowObject.TryGetComponent<AD_Arrow>(out AD_Arrow mainArrow);
            bool isLessArr = LessArrowObject.TryGetComponent<AD_Arrow_less>(out AD_Arrow_less lessArrow);

            if(isMainArr == false || isLessArr == false) {
                throw new System.Exception($"ArrItem : {termsName} is Component Not Assignment.");
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
                arrowSkillSets = new ArrowSkillSet(arrowSkillInfoFst, arrowSkillInfoSec, mainArrowObjTag, ability);
            }

            mainArrow.SetEffectInfo(effectPoolTags.ToArray());
            lessArrow.SetEffectInfo(effectPoolTags.ToArray());

            mainArrow.PowerFactor = speed;
            lessArrow.PowerFactor = speed;

            //Create Pools of Arrow Object
            CCPooler.AddPoolList(mainArrowObjTag, poolQuantity, MainArrowObject, isTracking: false);
            CCPooler.AddPoolList(lessArrowObjTag, poolQuantity, LessArrowObject, isTracking: false);
        }

        public void Release() {
            //Clear Arrow Skill Sets Class 
            if (arrowSkillSets == null)
                return;
            arrowSkillSets = null;

            //Release Data Arrow Skill
            if (arrowSkillInfoFst != null) arrowSkillInfoFst.Release();
            if (arrowSkillInfoSec != null) arrowSkillInfoSec.Release();
            
            //Remove Origin Prefab EffectTags string Array
            MainArrowObject.GetComponent<AD_Arrow>().RemoveEffectInfo();
            LessArrowObject.GetComponent<AD_Arrow_less>().RemoveEffectInfo();
        }
    }
}
