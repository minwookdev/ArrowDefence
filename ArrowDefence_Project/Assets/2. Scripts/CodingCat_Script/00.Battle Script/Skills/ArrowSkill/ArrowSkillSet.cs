namespace ActionCat
{
    using UnityEngine;

    public class ArrowSkillSet
    {
        ARROWSKILL_ACTIVETYPE activeType;
        AttackActiveTypeAS attackActiveSkill;
        AirActiveTypeAS airActiveSkill;
        AddProjTypeAS additionalProjectilesSkill;

        #region CONSTRCTOR

        /// <summary>
        /// Create New Arrow Skill Sets
        /// </summary>
        /// <param name="arrowskills"></param>
        public ArrowSkillSet(ArrowSkill[] arrowskills)
        {
            //현재 화살이 가질 수 있는 최대 스킬 가짓 수 : 2
            if(arrowskills.Length > 2)
            {
                CatLog.WLog("Arrow Skill Array Size Over 2");
                return;
            }

            //Skill Class Init
            for (int i = 0; i < arrowskills.Length; i++)
            {
                switch (arrowskills[i])
                {
                    case AttackActiveTypeAS  attackType: InitAttackTypeSkill(attackType);                  break;
                    case AirActiveTypeAS        airType: InitAirTypeSkill(airType);                        break;
                    case AddProjTypeAS addProjTypeSkill: InitAdditionalProjectilesSkill(addProjTypeSkill); break;
                    default: break; //null
                }
            }

            //Start Active Type Init
            activeType = InitArrowSkillActiveType(0);
            CatLog.Log($"Arrow SkillSets Active Type : {activeType.ToString()}");
        }

        /// <summary>
        /// Constrctor For Copy Origin Arrow Skill Sets Class.
        /// </summary>
        /// <param name="skillsets"></param>
        public ArrowSkillSet(ArrowSkillSet skillsets)
        {
            if (skillsets == null)
                return;

            //이렇게 하면 결국엔 똑같은Skill Class의 주소값을 참조하게 되는 꼴이네..
            //여기서 또 새로운 Skill Class로 할당을 해줘야 제대로 각각의 Skill을 가지고있게 되는 꼴이네 결국엔
            activeType                 = skillsets.activeType;
            //타입은 상관없는데 만약에 이러한 방식으로 Arrow Skill을 불러온다고 하면 Skill Class들은 깊은 복사를 해야만 한다.

            //attackActiveSkill          = skillsets.attackActiveSkill;
            //airActiveSkill             = skillsets.airActiveSkill;
            //additionalProjectilesSkill = skillsets.additionalProjectilesSkill;

            //대충 요런식으로 작성해야지 않을까
            //Attack Active Type Skill Copy
            //너무 무겁지는 않을까 가비지 엄청 생기지않을지..조금 더 가벼운 방법을 생각해보자
            if(skillsets.attackActiveSkill != null)
            {
                switch (skillsets.attackActiveSkill)
                {
                    case ReboundArrow reboundArrow: attackActiveSkill = new ReboundArrow(); break;
                    case SplitArrow splitArrow:     attackActiveSkill = new SplitArrow(); break;
                    default: attackActiveSkill = null; break; //else
                }
            }

            //Air Active Type Skill Copy
            if(skillsets.airActiveSkill != null)
            {
                switch (skillsets.airActiveSkill)
                {
                    case GuidanceArrow guidanceArrow:
                        airActiveSkill = new GuidanceArrow(); break;
                    default: airActiveSkill = null; break; //else
                }
            }

            //Additional Projectile Type Skill Copy
            if(skillsets.additionalProjectilesSkill != null)
            {
                switch (skillsets.additionalProjectilesSkill)
                {
                    default: break;
                }
            }
        }

        #endregion

        #region INIT

        void InitAttackTypeSkill(AttackActiveTypeAS skillData)
        {
            if (attackActiveSkill == null)
                attackActiveSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");
        }

        void InitAirTypeSkill(AirActiveTypeAS skillData)
        {
            if (airActiveSkill == null)
                airActiveSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");
        }

        void InitAdditionalProjectilesSkill(AddProjTypeAS skillData)
        {
            if (additionalProjectilesSkill == null)
                additionalProjectilesSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");
        }

        /// <summary>
        /// Return Activate Type [재귀적 호출 형태]
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        ARROWSKILL_ACTIVETYPE InitArrowSkillActiveType(int num)
        {
            switch (num)
            {
                #region CYCLE 1. ATTACK_TYPE_CHECKING
                case 0:
                    if (attackActiveSkill != null)
                        return InitArrowSkillActiveType(1);
                    else
                        return InitArrowSkillActiveType(2);
                #endregion
                #region CYCLE 2. AIR_TYPE_CHECKING
                case 1:
                    if (airActiveSkill != null)
                        return InitArrowSkillActiveType(3);
                    else
                        return InitArrowSkillActiveType(4);
                case 2:
                    if (airActiveSkill != null)
                        return InitArrowSkillActiveType(5);
                    else
                        return InitArrowSkillActiveType(6);
                #endregion
                #region CYCLE 3. ADDPROJ_TYPE_CHECKING
                case 3:
                    if (additionalProjectilesSkill != null)
                        return InitArrowSkillActiveType(7);
                    else
                        return InitArrowSkillActiveType(8);
                case 4:
                    if (additionalProjectilesSkill != null)
                        return InitArrowSkillActiveType(9);
                    else
                        return InitArrowSkillActiveType(10);
                case 5:
                    if (additionalProjectilesSkill != null)
                        return InitArrowSkillActiveType(11);
                    else
                        return InitArrowSkillActiveType(12);
                case 6:
                    if (additionalProjectilesSkill != null)
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

        public void Init(Transform arrowTr, Rigidbody2D rigidBody, IArrowObject arrow)
        {
            if (additionalProjectilesSkill != null)
                additionalProjectilesSkill.Init(arrowTr, rigidBody);

            if (attackActiveSkill != null)
                attackActiveSkill.Init(arrowTr, rigidBody, arrow);

            if (airActiveSkill != null)
                airActiveSkill.Init(arrowTr, rigidBody);
        }

        #endregion

        public bool OnHit(Collider2D collider, IArrowObject arrow)
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
            additionalProjectilesSkill.OnHit();
            return attackActiveSkill.OnHit(collider);
        }

        bool ActiveAtk(Collider2D collider)
        {
            return attackActiveSkill.OnHit(collider);
        }

        bool ActiveAddProj()
        {
            additionalProjectilesSkill.OnHit();
            return true;
        }

        #endregion
    }
}
