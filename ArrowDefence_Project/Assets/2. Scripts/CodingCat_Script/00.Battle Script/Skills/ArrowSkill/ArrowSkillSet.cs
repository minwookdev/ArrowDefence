namespace ActionCat
{
    using UnityEngine;

    public class ArrowSkillSet
    {
        ARROWSKILL_ACTIVETYPE activeType;
        AttackActiveTypeAS hitSkill = null;
        AirActiveTypeAS airSkill    = null;
        AddProjTypeAS addProjSkill  = null;

        #region CONSTRUCTOR

        /// <summary>
        /// 생성자 I. Arrow Item에서 '원본 Skill Set Class' 생성.
        /// Current Max ArrowSkill Count : 2
        /// </summary>
        /// <param name="arrowskills"></param>
        public ArrowSkillSet(ArrowSkill arrowSkillFst, ArrowSkill arrowSkillSec)
        {
            //First Arrow Skill Init
            if(arrowSkillFst != null)
            {
                switch (arrowSkillFst)
                {
                    case AttackActiveTypeAS attackType: InitAttackTypeSkill(attackType);             break;
                    case AirActiveTypeAS       airType: InitAirTypeSkill(airType);                   break;
                    case AddProjTypeAS     addProjType: InitAdditionalProjectilesSkill(addProjType); break;
                    default: break;
                }
            }

            //Seconds Arrow Skill Init
            if(arrowSkillSec != null)
            {
                if (arrowSkillSec is AttackActiveTypeAS atkType)     InitAttackTypeSkill(atkType);
                else if (arrowSkillSec is AirActiveTypeAS airType)   InitAirTypeSkill(airType);
                else if (arrowSkillSec is AddProjTypeAS addProjType) InitAdditionalProjectilesSkill(addProjType);
            }

            //Start Active Type Init
            activeType = InitArrowSkillActiveType(0);
            CatLog.Log($"Arrow SkillSets Active Type : {activeType.ToString()}");
        }

        /// <summary>
        /// 생성자 II. GameManager에서 각각의 Arrow Prefab으로 원본 Skill Set Class를 복사하여 뿌려지는 생성자.
        /// </summary>
        /// <param name="skillsets"></param>
        public ArrowSkillSet(ArrowSkillSet skillsets)
        {
            if (skillsets == null)
                return;

            //Clone-(struct)Active-Type
            activeType = skillsets.activeType;

            //Clone-Arrow Skill Classes
            if(skillsets.hitSkill != null)
            {
                switch (skillsets.hitSkill)
                {
                    case ReboundArrow reboundArrow: hitSkill = new ReboundArrow(reboundArrow); break;
                    case SplitArrow     splitArrow: hitSkill = new SplitArrow(splitArrow);     break;
                    default: hitSkill = null; break; //else
                }
            }

            //Clone-Air Active Type Skill
            if(skillsets.airSkill != null)
            {
                switch (skillsets.airSkill)
                {
                    case GuidanceArrow guidanceArrow: airSkill = new GuidanceArrow(guidanceArrow); break;
                    default: airSkill = null; break; //else
                }
            }

            //Clone-Additional Projectile Type Skill
            if(skillsets.addProjSkill != null)
            {
                switch (skillsets.addProjSkill)
                {
                    default: break;
                }
            }
        }

        #endregion

        #region INIT

        void InitAttackTypeSkill(AttackActiveTypeAS skillData)
        {
            if (hitSkill == null)
                hitSkill = skillData;
            else
                CatLog.WLog($"중복 Type Arrow Skill: {skillData}가 Init되었습니다.");
        }

        void InitAirTypeSkill(AirActiveTypeAS skillData)
        {
            if (airSkill == null)
                airSkill = skillData;
            else
                CatLog.WLog($"중복 Type Arrow Skill: {skillData}가 Init되었습니다.");
        }

        void InitAdditionalProjectilesSkill(AddProjTypeAS skillData)
        {
            if (addProjSkill == null)
                addProjSkill = skillData;
            else
                CatLog.WLog($"중복 Type Arrow Skill: {skillData}가 Init되었습니다.");
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
        /// 각각의 ArrowObject에서 ArrowSkillSet Class를 할당받고 실행.
        /// </summary>
        /// <param name="arrowTr"></param>
        /// <param name="rigidBody"></param>
        /// <param name="arrow"></param>
        public void Init(Transform arrowTr, Rigidbody2D rigidBody, IArrowObject arrow)
        {
            if (addProjSkill != null)
                addProjSkill.Init(arrowTr, rigidBody);

            if (hitSkill != null)
                hitSkill.Init(arrowTr, rigidBody, arrow);

            if (airSkill != null)
                airSkill.Init(arrowTr, rigidBody);
        }

        #endregion

        public bool OnHit(Collider2D collider)
        {
            switch (activeType)
            {
                case ARROWSKILL_ACTIVETYPE.FULL:           return ActiveAtkAddProj(collider); 
                case ARROWSKILL_ACTIVETYPE.ATTACK_AIR:     return ActiveAtk(collider);        
                case ARROWSKILL_ACTIVETYPE.ATTACK_ADDPROJ: return ActiveAtkAddProj(collider);
                case ARROWSKILL_ACTIVETYPE.ATTACK:         return ActiveAtk(collider);
                case ARROWSKILL_ACTIVETYPE.AIR_ADDPROJ:    return ActiveAddProj();
                case ARROWSKILL_ACTIVETYPE.AIR:            return true;
                case ARROWSKILL_ACTIVETYPE.ADDPROJ:        return ActiveAddProj();
                case ARROWSKILL_ACTIVETYPE.EMPTY:          return true;
                default:                                   return true;
            }
        }

        public void OnAir()
        {

        }

        #region Functions by Active Type
        ///AIR TYPE은 아직 OnHit쪽에서 해줄게 없어서 따로 구현안함, 추후에 Air Type Skill 어떻게 구현되냐에 따라
        ///함수 따로 만들어서 사용할 것.

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

        #endregion
    }
}
