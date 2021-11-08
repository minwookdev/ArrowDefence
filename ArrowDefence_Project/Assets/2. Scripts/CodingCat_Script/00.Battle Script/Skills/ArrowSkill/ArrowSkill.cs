namespace ActionCat
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ArrowSkill
    {
        protected Transform arrowTr;
        protected Rigidbody2D rBody;

        public virtual void Init(Transform tr, Rigidbody2D rigid)
        {
            arrowTr = tr;
            rBody   = rigid;
        }

        public abstract void OnAir();
        public abstract bool OnHit(Collider2D target, IArrowObject shooter);
        public abstract void Clear();
    }

    public abstract class AttackActiveTypeAS : ArrowSkill
    {
        
    }

    public abstract class AirActiveTypeAS : ArrowSkill
    {

    }

    public abstract class AdditionalProjectilesAS : ArrowSkill
    {
    
    }

    public class ArrowSkillSets
    {
        ARROWSKILL_ACTIVETYPE activeType;
        AttackActiveTypeAS attackActiveSkill;
        AirActiveTypeAS airActiveSkill;
        AdditionalProjectilesAS additionalProjectilesSkill;

        public ArrowSkillSets(ArrowSkill[] arrowskills)
        {
            if(arrowskills.Length > 3)
            {
                CatLog.WLog("Arrow Skills parameter Size Over 3");
                return;
            }

            for (int i = 0; i < arrowskills.Length; i++)
            {
                switch (arrowskills[i])
                {
                    case AttackActiveTypeAS attackType:
                        break;
                    case AirActiveTypeAS airType: 
                        break;
                    case AdditionalProjectilesAS addProjectilesType: 
                        break;
                    default: 
                        break;
                }
            }

            GameGlobal.ArrayForeach<ArrowSkill>(arrowskills, (data) =>
            {
                switch (data)
                {
                    case AttackActiveTypeAS attackType:       InitAttackTypeSkill(attackType);             break;
                    case AirActiveTypeAS airType:             InitAirTypeSkill(airType);                   break;
                    case AdditionalProjectilesAS addProjType: InitAdditionalProjectilesSkill(addProjType); break;
                    default: break;
                }
            });

            
        }

        void InitAttackTypeSkill(AttackActiveTypeAS skillData)
        {
            if (attackActiveSkill == null)
                attackActiveSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");

            //여기서부터 만들어주면 어떨까..? 차례대로 조립해나가는거지 !
            //Attack Type이 없으면 -> TYPE_EMPTY -> Air 타입있으면 -> TYPE_AIR -> Add Proj타입 있으면 ->TYPE_AIR_PROJ
            //시도해보자
        }

        void InitAirTypeSkill(AirActiveTypeAS skillData)
        {
            if (airActiveSkill == null)
                airActiveSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");
        }

        void InitAdditionalProjectilesSkill(AdditionalProjectilesAS skillData)
        {
            if (additionalProjectilesSkill == null)
                additionalProjectilesSkill = skillData;
            else
                CatLog.WLog($"중복 Arrow SkillData {skillData}가 Init되었습니다.");
        }

        void InitActiveType()
        {
            //이딴식으로 하면 안된다 테스트니깐 일단 이렇게 진행하고, 로직개선 하자
            if (attackActiveSkill == null && airActiveSkill == null && additionalProjectilesSkill == null)
            {

            }
            else if(attackActiveSkill != null && airActiveSkill == null && additionalProjectilesSkill == null)
            {

            }
            else if (attackActiveSkill != null && airActiveSkill != null && additionalProjectilesSkill == null)
            {

            }
            else //모든 Type의 Skill이 Init된 경우
            {

            }
        }
    }

    /// <summary>
    /// 보류.
    /// </summary>
    public class ArrowSkillSetGenenerater
    {
        ARROWSKILL_ACTIVETYPE activeType;
        AttackActiveTypeAS attackTypeSkillFst;
        AttackActiveTypeAS attackTypeSkillSec;
        AirActiveTypeAS airTypeSkill;

        /// <summary>
        /// Generate Attack Type Skill Sets
        /// </summary>
        /// <param name="atkType"></param>
        public ArrowSkillSetGenenerater(AttackActiveTypeAS atkType)
        {
            attackTypeSkillFst = atkType;
            attackTypeSkillSec = null;
            airTypeSkill       = null;

            //activeType = ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACK;
        }

        /// <summary>
        /// Generate Air Type Skill Sets
        /// </summary>
        /// <param name="airType"></param>
        public ArrowSkillSetGenenerater(AirActiveTypeAS airType)
        {
            attackTypeSkillFst = null;
            attackTypeSkillSec = null;
            airTypeSkill = airType;

            //activeType = ARROWSKILL_ACTIVETYPE.ACTIVETYPE_AIR;
        }

        /// <summary>
        /// Generate Attack 2 Attack Type Skill Sets
        /// </summary>
        /// <param name="atkTypeFst"></param>
        /// <param name="atkTypeSec"></param>
        public ArrowSkillSetGenenerater(AttackActiveTypeAS atkTypeFst, AttackActiveTypeAS atkTypeSec)
        {
            attackTypeSkillFst = atkTypeFst;
            attackTypeSkillSec = atkTypeSec;
            airTypeSkill       = null;

            //activeType = ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnATTACK;
        }

        /// <summary>
        /// Generate 1 Attack, 1 Air Skill Sets
        /// </summary>
        /// <param name="atkTypeFst"></param>
        /// <param name="atkTypeSec"></param>
        /// <param name="airType"></param>
        public ArrowSkillSetGenenerater(AttackActiveTypeAS atkTypeFst, AirActiveTypeAS airType)
        {
            attackTypeSkillFst = atkTypeFst;
            attackTypeSkillSec = null;
            airTypeSkill       = airType;

            //activeType = ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnAIR;
        }

        /// <summary>
        /// Copy Class Arrow Skill Generater
        /// </summary>
        /// <param name="skillgenerater"></param>
        public ArrowSkillSetGenenerater(ArrowSkillSetGenenerater skillgenerater)
        {
            attackTypeSkillFst = skillgenerater.attackTypeSkillFst;
            attackTypeSkillSec = skillgenerater.attackTypeSkillSec;
            airTypeSkill       = skillgenerater.airTypeSkill;
            activeType         = skillgenerater.activeType;
        }

        //■■■■■■■■■■■■■ TYPE III.ATTACK & ATTACK
        //■■■■■■■■■■■■■ TYPE IV.ATTACK & AIR

        //public bool OnHit()
        //{
        //    switch (activeType)
        //    {
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_EMPTY:         break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACK:        break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_AIR:           break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnATTACK: break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnAIR:    break;
        //        default:                                             break;
        //    }
        //}
        //
        //public void OnAir()
        //{
        //    switch (activeType)
        //    {
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_EMPTY:         break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACK:        break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_AIR:           break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnATTACK: break;
        //        case ARROWSKILL_ACTIVETYPE.ACTIVETYPE_ATTACKnAIR:    break;
        //        default:                                             break;
        //    }
        //}
        //
        ////■■■■■■■■■■■■■ TYPE I.ATTACK
        //void SkillActiveTypeAttack(Collider2D coll, IArrowObject arrow)
        //{
        //    attackTypeSkillFst.OnHit(coll, arrow);
        //}
        //
        ////■■■■■■■■■■■■■ TYPE II.AIR
        //void SkillActiveTypeAir()
        //{
        //
        //}
        //
        //bool SkillActiveTypeAttacknAttack(Collider2D coll, IArrowObject arrow)
        //{
        //    attackTypeSkillFst.OnHit(coll, arrow); attackTypeSkillSec.OnHit(coll, arrow);
        //}
    }

    public class ReboundArrow : ArrowSkill
    {
        GameObject lastHitTarget;
        int currentChainCount = 0;  //현재 연쇄횟수
        int maxChainCount     = 1;  //최대 연쇄횟수

        public override void OnAir()
        {
            //CatLog.Log("");
        }

        //void -> bool형태로 바꿔서 TriggerEnter2D에서 반환받을 때, Disable할지 유지할지 결정내리면 되겠다.
        //shooter도 저장해서 Collider2D만 매개변수로 받아도 괜찮을것같다
        public override bool OnHit(Collider2D target, IArrowObject shooter)
        {
            //if(ReferenceEquals(alreadyHitTarget, target) == false)
            //{
            //
            //}

            //■■■■■■■■■■■■■ I Availablity Arrow Skill : 중복 다겟 및 연쇄 횟수 체크 ■■■■■■■■■■■■■
            //최근에 Hit처리한 객체와 동일한 객체와 다시 충돌될 경우, return 처리
            //해당 Monster Object를 무시함 [같은 객체에게 스킬 효과를 터트릴 수 없음]
            if (lastHitTarget == target.gameObject)
            {
                return false;
            }
            else
            {
                //연쇄횟수 체크
                if(currentChainCount >= maxChainCount)
                {
                    //Monster Hit 처리
                    target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
                    Clear(); return true;
                    //arrow.DisableObject_Req(arrowTr.gameObject); return true;
                }

                //현재 연쇄횟수 중첩 및 마지막 적 저장
                currentChainCount++;
                lastHitTarget = target.gameObject;

                //Monster hit 처리
                target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            }

            //if (currentChainCount >= maxChainCount) //return
            //{
            //    Clear();
            //    arrow.DisableObject_Req(arrowTr.gameObject);
            //    return;
            //}
            //
            //if (alreadyHitTarget != target.gameObject)
            //{
            //    currentChainCount++;
            //    alreadyHitTarget = target.gameObject;
            //}
            //else
            //    return;

            //arrow.DisableObject_Req(arrowTr.gameObject);
            //이거 최적화 안하면 진짜 엄청 무겁겠다..한두번 발동도 아니고 이건 뭐,

            //■■■■■■■■■■■■■ II Rebound Arrow Skill : Active 절차 개시 ■■■■■■■■■■■■■
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(arrowTr.position, 5f);
            if (hitColliders.Length == 0) //return
            {
                //주변에 Rebound할 대상 객체가 없는 경우 제거처리
                Clear(); return true;

                //arrow.DisableObject_Req(arrowTr.gameObject);
                //return true;
            } //->
            else
            {
                //방금 hit한 대상의 Collider가 있는지 검사 순회, target collider list에서 제거.
                for (int i = 0; i < hitColliders.Length; i++)
                {
                    if(hitColliders[i] == target)
                    {
                        List<Collider2D> tempCollList = new List<Collider2D>(hitColliders);
                        tempCollList.RemoveAt(i);
                        hitColliders = tempCollList.ToArray(); break;

                        //for (int j = 0; j < hitColliders.Length; j++)
                        //{
                        //    tempCollList.Add(hitColliders[j]);
                        //}
                    }
                }
            }

            //Monster가 아닌 객체들 걸러내기
            List<Collider2D> monsterColliders = new List<Collider2D>();
            foreach (var coll in hitColliders)
            {
                if (coll.CompareTag(AD_Data.OBJECT_TAG_MONSTER))
                    monsterColliders.Add(coll);
            }

            if (monsterColliders.Count <= 0)
            {
                Clear(); return true;
                //arrow.DisableObject_Req(arrow.gameObject); return true;
            }

            //■■■■■■■■■■■■■■■ II Target Init[New Type] [절차 최적화] ■■■■■■■■■■■■■■■
            //List<Collider2D> collList = new List<Collider2D>(hitColliders);
            //for (int i = collList.Count - 1; i >= 0; i--) //Reverse Loop [Remove Array Element]
            //{
            //    //중복 발동 대상과 Monster가 아닌 Collider는 예외처리.
            //    if (collList[i] == target || collList[i].CompareTag(AD_Data.OBJECT_TAG_MONSTER) == false)
            //        collList.Remove(collList[i]);
            //}
            //if(collList.Count <= 0) //예외처리후 타겟이 없으면 비활성화 처리.
            //{
            //    Clear(); return true;
            //}

            //몬스터 대상이 아닌 객체를 필터링하는 절차와 중복 몬스터 제거순회 과정을 한번에 진행해버리면 되겠다
            //필터링 과정 하나로 통합.

            //Transform bestTargetTr = null; //-> Vector2 Type으로 변경 (참조값이라 중간에 추적중인 Monster Tr 사라지면 에러)
            Vector3 monsterPos     = Vector3.zero; //Position 저장
            float closestDistSqr   = Mathf.Infinity;
            for (int i = 0; i < monsterColliders.Count; i++)
            {
                Vector2 directionToTarget = monsterColliders[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if(distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    monsterPos     = monsterColliders[i].transform.position;
                }
            }

            arrowTr.rotation = Quaternion.Euler(0f, 0f,
                               Quaternion.FromToRotation(Vector3.up, monsterPos - arrowTr.position).eulerAngles.z);
            shooter.ShotArrow(arrowTr.up * 18f);
            return false;
        }

        public override void Clear()
        {
            lastHitTarget     = null;
            currentChainCount = 0;
        }
    }

    public class GuidanceArrow : ArrowSkill
    {
        public override void OnAir()
        {
            throw new System.NotImplementedException();
        }

        public override bool OnHit(Collider2D target, IArrowObject shooter)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }

    public class SplitArrow : ArrowSkill
    {
        public override void OnAir()
        {
            throw new System.NotImplementedException();
        }

        public override bool OnHit(Collider2D target, IArrowObject shooter)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }
}
