namespace ActionCat
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ArrowSkill
    {
        protected Transform arrowTr;
        protected Rigidbody2D rBody;

        //Arrow Skill 자체는 한번만 해놓으면 되지만, Init함수는 각 Arrow별로 한번씩은 잡아줘야한다.
        //각각의 Arrow별로 Transform과 RigidBody를 사용하기 때문.
        public virtual void Init(Transform tr, Rigidbody2D rigid)
        {
            arrowTr = tr;
            rBody   = rigid;
        }

        public abstract void Clear();
    }

    public abstract class AttackActiveTypeAS : ArrowSkill
    {
        protected IArrowObject arrow;

        public virtual void Init(Transform tr, Rigidbody2D rigidbody, IArrowObject arrow)
        {
            base.Init(tr, rigidbody);
            this.arrow = arrow;
        }

        public abstract bool OnHit(Collider2D target);
    }

    public abstract class AirActiveTypeAS : ArrowSkill
    {
        public abstract void OnAir();
    }

    public abstract class AddProjTypeAS : ArrowSkill
    {
        public abstract void OnHit();
    }

    public class ReboundArrow : AttackActiveTypeAS
    {
        GameObject lastHitTarget;
        int currentChainCount = 0;  //현재 연쇄횟수
        int maxChainCount     = 1;  //최대 연쇄횟수
        float scanRange       = 5f; //Monster 인식 범위

        //return true : DisableArrow || false : IgnoreCollision
        public override bool OnHit(Collider2D target)
        {
            //■■■■■■■■■■■■■ I. Availablity Arrow Skill : 중복 다겟 및 연쇄 횟수 체크 ■■■■■■■■■■■■■
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

            //■■■■■■■■■■■■■■■ II Rebound Arrow Skill : Active 절차 개시 ■■■■■■■■■■■■■■■
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(arrowTr.position, scanRange);
            if (hitColliders.Length == 0) //return
            {
                //주변에 Rebound할 대상 객체가 없는 경우 소멸
                Clear(); return true;
            } //->
            #region LEGACY
            //else
            //{
            //    //방금 hit한 대상의 Collider가 있는지 검사 순회, target collider list에서 제거.
            //    for (int i = 0; i < hitColliders.Length; i++)
            //    {
            //        if(hitColliders[i] == target)
            //        {
            //            List<Collider2D> tempCollList = new List<Collider2D>(hitColliders);
            //            tempCollList.RemoveAt(i);
            //            hitColliders = tempCollList.ToArray(); break;
            //
            //            //for (int j = 0; j < hitColliders.Length; j++)
            //            //{
            //            //    tempCollList.Add(hitColliders[j]);
            //            //}
            //        }
            //    }
            //}
            //
            ////Monster가 아닌 객체들 걸러내기
            //List<Collider2D> monsterColliders = new List<Collider2D>();
            //foreach (var coll in hitColliders)
            //{
            //    if (coll.CompareTag(AD_Data.OBJECT_TAG_MONSTER))
            //        monsterColliders.Add(coll);
            //}
            //
            //if (monsterColliders.Count <= 0)
            //{
            //    Clear(); return true;
            //    //arrow.DisableObject_Req(arrow.gameObject); return true;
            //}
            #endregion
            //■■■■■■■■■■■■■■■ III. Optimal target selection [절차 최적화] ■■■■■■■■■■■■■■■
            List<Collider2D> monsterColliders = new List<Collider2D>(hitColliders);
            for (int i = monsterColliders.Count - 1; i >= 0; i--) //Reverse Loop [Remove Array Element]
            {
                //중복 발동 대상과 Monster가 아닌 Collider는 예외처리.
                if (monsterColliders[i] == target || monsterColliders[i].CompareTag(AD_Data.OBJECT_TAG_MONSTER) == false)
                    monsterColliders.Remove(monsterColliders[i]);
            }
            if(monsterColliders.Count <= 0) //예외처리후 타겟이 없으면 비활성화 처리.
            {
                Clear(); return true;
            }

            //Transform bestTargetTr = null; //-> Vector2 Type으로 변경 (참조값이라 중간에 추적중인 Monster Tr 사라지면 에러)
            Vector3 monsterPos     = Vector3.zero; //Transform이 아닌 Position 저장
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

            //■■■■■■■■■■■■■■■ III. Shot to Target Position ■■■■■■■■■■■■■■■
            arrow.ForceArrow(monsterPos);
            return false;
        }

        public override void Clear()
        {
            lastHitTarget     = null;
            currentChainCount = 0;
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="origin"></param>
        public ReboundArrow(ReboundArrow origin)
        {
            maxChainCount = origin.maxChainCount;
            scanRange     = origin.scanRange;
        }

        public ReboundArrow()
        {
            maxChainCount = 1;
            scanRange = 5f;
        }
    }

    public class GuidanceArrow : AirActiveTypeAS
    {
        public override void OnAir()
        {
            //1. Find a Target Monster Object Logic State 
            //1-a. if Find a Monster -> Stop Find Logic Update, Get Next 2
            //1-b. if Not Found a Monster -> Restart Find Monster Logic (Update Every n Seconds)
            //2. Start position change to target position
        }

        public override void Clear()
        {
            
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="guidanceArrow"></param>
        public GuidanceArrow(GuidanceArrow guidanceArrow)
        {

        }
    }

    public class SplitArrow : AttackActiveTypeAS
    {
        public override bool OnHit(Collider2D target)
        {
            throw new System.NotImplementedException();
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="splitArrow"></param>
        public SplitArrow(SplitArrow splitArrow)
        {

        }
    }
}
