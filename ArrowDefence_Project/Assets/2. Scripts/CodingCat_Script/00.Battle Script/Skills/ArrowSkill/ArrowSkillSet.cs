namespace ActionCat
{
    using UnityEngine;

    public class ArrowSkillSet
    {
        ARROWSKILL_ACTIVETYPE activeType;
        AttackActiveTypeAS hitSkill = null;
        AirActiveTypeAS airSkill    = null;
        AddProjTypeAS addProjSkill  = null;

        //temp
        Transform tempTr;

        #region CONSTRUCTOR

        /// <summary>
        /// Constructor I. Arrow Item Class에서 Origin Arrow Skill Sets Class 생성.
        /// </summary>
        /// <param name="skillInfoFst"></param>
        /// <param name="skillInfoSec"></param>
        public ArrowSkillSet(ASInfo skillInfoFst, ASInfo skillInfoSec)
        {
            //First Arrow SkillData Init
            if(skillInfoFst != null) {
                switch (skillInfoFst.ActiveType)
                {
                    case ARROWSKILL_ACTIVETYPE.ATTACK:  InitHitTypeSkill(skillInfoFst.SkillData);     break;
                    case ARROWSKILL_ACTIVETYPE.AIR:     InitAirTypeSkill(skillInfoFst.SkillData);     break;
                    case ARROWSKILL_ACTIVETYPE.ADDPROJ: InitAddProjTypeSkill(skillInfoFst.SkillData); break;
                    default: break;
                }
            }
            //Seconds Arrow SkillData Init
            if(skillInfoSec != null) {
                switch (skillInfoSec.ActiveType)
                {
                    case ARROWSKILL_ACTIVETYPE.ATTACK:  InitHitTypeSkill(skillInfoSec.SkillData);     break;
                    case ARROWSKILL_ACTIVETYPE.AIR:     InitAirTypeSkill(skillInfoSec.SkillData);     break;
                    case ARROWSKILL_ACTIVETYPE.ADDPROJ: InitAddProjTypeSkill(skillInfoSec.SkillData); break;
                    default: break;
                }
            }

            //Start SkillSets Activate Type Init
            activeType = InitArrowSkillActiveType(0);
            CatLog.Log($"Arrow SkillSets Active Type : {activeType.ToString()}");
        }


        /// <summary>
        /// Constructor II. Copy origin ArrowSkillSets Class.
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="num"></param>
        public ArrowSkillSet(ArrowSkillSet origin)
        {
            //Clone-Active Type
            activeType = origin.activeType;

            //Clone-Hit Type Skill
            if(origin.hitSkill != null) {
                switch (origin.hitSkill)
                {
                    case ReboundArrow rebound:   hitSkill = new ReboundArrow(rebound);   break;
                    case PiercingArrow piercing: hitSkill = new PiercingArrow(piercing); break;
                    default: break;
                }
            }

            //Clone-Air Type Skill
            if(origin.airSkill != null) {
                switch (origin.airSkill)
                {
                    case HomingArrow homing: airSkill = new HomingArrow(homing); break;
                    default: break;
                }
            }

            //Clone-Additional Projectiles Typ Skil
            if (origin.addProjSkill != null) {
                switch (origin.addProjSkill)
                {
                    case SplitArrow split: addProjSkill = new SplitArrow(split); break;
                    default: break;
                }
            }
        }

        #endregion

        #region INIT

        void InitHitTypeSkill(ArrowSkill skillData) {
            if(hitSkill != null) {
                CatLog.ELog($"Error : 중복된 ActiveType의 ArrowSkill {skillData}이(가) 할당되었습니다. [ATTACK]"); 
                return;
            }

            if(skillData is AttackActiveTypeAS hitTypeSkill) {
                hitSkill = hitTypeSkill;
            }
            else {
                CatLog.WLog($"{skillData}는(은) AttackActivate Type이 아닙니다.");
            }
        }

        void InitAirTypeSkill(ArrowSkill skillData) {
            if (airSkill != null) {
                CatLog.ELog($"Error : 중복된 ActiveType의 ArrowSkill {skillData}이(가) 할당되었습니다. [AIR]");
                return;
            }

            if (skillData is AirActiveTypeAS airTypeSkill) {
                airSkill = airTypeSkill;
            }
            else {
                CatLog.WLog($"{skillData}는(은) AirActivate Type이 아닙니다.");
            }
        }

        void InitAddProjTypeSkill(ArrowSkill skillData) {
            if(addProjSkill != null) {
                CatLog.ELog($"Error : 중복된 ActiveType의 ArrowSkill {skillData}이(가) 할당되었습니다. [ADD PROJECTILES]");
                return;
            }

            if(skillData is AddProjTypeAS addProjTypeSkill) {
                addProjSkill = addProjTypeSkill;
            }
            else {
                CatLog.WLog($"{skillData}는(은) Additional Projectiles Type이 아닙니다.");
            }
        }

        /// <summary>
        /// Init-SkillActive-Type
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        ARROWSKILL_ACTIVETYPE InitArrowSkillActiveType(int num)
        {
            switch (num)
            {
                #region CYCLE 1. ATTACK_TYPE_CHECKING
                case 0:
                    if (hitSkill != null)
                        return InitArrowSkillActiveType(1);
                    else
                        return InitArrowSkillActiveType(2);
                #endregion
                #region CYCLE 2. AIR_TYPE_CHECKING
                case 1:
                    if (airSkill != null)
                        return InitArrowSkillActiveType(3);
                    else
                        return InitArrowSkillActiveType(4);
                case 2:
                    if (airSkill != null)
                        return InitArrowSkillActiveType(5);
                    else
                        return InitArrowSkillActiveType(6);
                #endregion
                #region CYCLE 3. ADDPROJ_TYPE_CHECKING
                case 3:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(7);
                    else
                        return InitArrowSkillActiveType(8);
                case 4:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(9);
                    else
                        return InitArrowSkillActiveType(10);
                case 5:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(11);
                    else
                        return InitArrowSkillActiveType(12);
                case 6:
                    if (addProjSkill != null)
                        return InitArrowSkillActiveType(13);
                    else
                        return InitArrowSkillActiveType(14);
                #endregion
                #region CYCLE 4. FINALLY
                case 7: return ARROWSKILL_ACTIVETYPE.FULL;
                case 8: return ARROWSKILL_ACTIVETYPE.ATTACK_AIR;
                case 9: return ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ;
                case 10: return ARROWSKILL_ACTIVETYPE.ATTACK;
                case 11: return ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ;
                case 12: return ARROWSKILL_ACTIVETYPE.AIR;
                case 13: return ARROWSKILL_ACTIVETYPE.ADDPROJ;
                case 14: return ARROWSKILL_ACTIVETYPE.EMPTY;
                default: return ARROWSKILL_ACTIVETYPE.EMPTY;
                #endregion
            }
        }

        /// <summary>
        /// Init-Arrow Skill Sets
        /// </summary>
        /// <param name="arrowTr"></param>
        /// <param name="rigidBody"></param>
        /// <param name="arrow"></param>
        public void Init(Transform arrowTr, Rigidbody2D rigidBody, IArrowObject arrow)
        {
            if (addProjSkill != null)
                addProjSkill.Init(arrowTr, rigidBody, arrow);

            if (hitSkill != null)
                hitSkill.Init(arrowTr, rigidBody, arrow);

            if (airSkill != null)
                airSkill.Init(arrowTr, rigidBody, arrow);
        }

        #endregion

        public bool OnHit(Collider2D collider)
        {
            switch (activeType)
            {
                case ARROWSKILL_ACTIVETYPE.FULL:           return ActiveFull(collider); 
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     return ActiveAtkAir(collider);        
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: return ActiveAtkAddProj(collider);
                case ARROWSKILL_ACTIVETYPE.ATTACK:         return ActiveAtk(collider);
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ:    return ActiveAddProj(); // <- 공격 판정 없음
                case ARROWSKILL_ACTIVETYPE.AIR:            return DefaultHit(collider);
                case ARROWSKILL_ACTIVETYPE.ADDPROJ:        return ActiveAddProj(); // <- 공격 판정 없음
                case ARROWSKILL_ACTIVETYPE.EMPTY:          return true; //Empty인 경우는 SkillSets 자체가 성립할 수 없다
                default:                                   return true;
            }
        }

        public void OnUpdate()
        {
            switch (activeType)
            {
                case ARROWSKILL_ACTIVETYPE.FULL:        UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:  UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ: UpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR:         UpdateOnAir(); break;
                default:                                               break;
            }
        }

        public void OnFixedUpdate()
        {
            switch (activeType)
            {
                case ARROWSKILL_ACTIVETYPE.FULL:        FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:  FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ: FixedUpdateOnAir(); break;
                case ARROWSKILL_ACTIVETYPE.AIR:         FixedUpdateOnAir(); break;
                default:                                                    break;
            }
        }

        public void OnExit(Collider2D target)
        {
            switch (activeType) {
                case ARROWSKILL_ACTIVETYPE.FULL:           hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: hitSkill.OnExit(target); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK:         hitSkill.OnExit(target); break;
                default: break;
            }
        }

        /// <summary>
        /// Call When Disable Arrow. if the Init SkillSets
        /// </summary>
        public void Clear()
        {
            switch (activeType)
            {
                case ARROWSKILL_ACTIVETYPE.FULL:           ClearHit(); ClearAir(); ClearAddProj(); break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     ClearHit(); ClearAir();                 break;
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: ClearHit(); ClearAddProj();             break;
                case ARROWSKILL_ACTIVETYPE.ATTACK:         ClearHit();                             break;
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ:    ClearAir(); ClearAddProj();             break;
                case ARROWSKILL_ACTIVETYPE.AIR:            ClearAir();                             break;
                case ARROWSKILL_ACTIVETYPE.ADDPROJ:        ClearAddProj();                         break;
            }
        }

        #region ON-HIT-CALLBACK

        bool ActiveFull(Collider2D collider)
        {
            addProjSkill.OnHit();
            bool isDisable = hitSkill.OnHit(collider, out tempTr);
            if (isDisable == false)
                airSkill.CallbackOnHit(tempTr);
            return isDisable;
        }

        bool ActiveAtkAddProj(Collider2D collider)
        {
            addProjSkill.OnHit();
            return hitSkill.OnHit(collider);
        }

        bool ActiveAtk(Collider2D collider)
        {
            return hitSkill.OnHit(collider);
        }

        bool ActiveAddProj()
        {
            addProjSkill.OnHit();
            return true;
        }

        bool ActiveAtkAir(Collider2D collider)
        {
            bool isDisable = hitSkill.OnHit(collider, out tempTr);
            if (isDisable == false) //Disable되는 상황이 아닐 경우만 Transform 보내줌
                airSkill.CallbackOnHit(tempTr);
            return isDisable;

            ///tempTr을 Hitskill의 OnHit함수로 매개변수로써 보내고, 
            ///OnHit함수에서 적절한 타겟을 찾게되어 tempTr에 null이 아닌 값이 할당되면
            ///OnAir에서 업데이트로 목표를 찾는 로직을 실행하지 않고 바로 
            ///targetTransform을 사용하여 효과를 실행시킬 수 있게된다.
        }

        #endregion

        #region ON-AIR-UPDATE

        void UpdateOnAir() => airSkill.OnUpdate();

        void FixedUpdateOnAir() => airSkill.OnFixedUpdate();

        #endregion

        #region CLEAR



        void ClearHit() => hitSkill.Clear();

        void ClearAir() => airSkill.Clear();

        void ClearAddProj() => addProjSkill.Clear();

        #endregion

        #region DEFAULT

        bool DefaultHit(Collider2D collider)
        {
            //Air Skill만 존재하는 경우, Monster가 대미지를 받는 로직만 적용. 추후 대미지 관련 코드가 구현되면 적용할 것.
            collider.SendMessage("OnHitObject", Random.Range(30f, 50f), SendMessageOptions.DontRequireReceiver);
            return true;
        }

        void DefaultAir()
        {

        }

        #endregion
    }
}
