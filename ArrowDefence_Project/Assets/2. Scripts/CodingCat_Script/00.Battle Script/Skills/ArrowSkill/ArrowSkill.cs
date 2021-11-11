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

        //void -> bool형태로 바꿔서 TriggerEnter2D에서 반환받을 때, Disable할지 유지할지 결정내리면 되겠다.
        //shooter도 저장해서 Collider2D만 매개변수로 받아도 괜찮을것같다
        public override bool OnHit(Collider2D target)
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

            //arrowTr.rotation = Quaternion.Euler(0f, 0f,
            //                   Quaternion.FromToRotation(Vector3.up, monsterPos - arrowTr.position).eulerAngles.z);
            //arrow.ShotArrow(arrowTr.up * 18f);

            //var rot = Quaternion.Euler(0f, 0f,
            //          Quaternion.FromToRotation(Vector3.up, monsterPos - arrowTr.position).eulerAngles.z);
            //arrow.ShotArrow(rot, arrowTr.up * 18f); 

            arrow.ShotArrow(monsterPos, arrowTr.up * 18f);
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
        }

        public ReboundArrow()
        {
            maxChainCount = 1;
        }
    }

    public class GuidanceArrow : AirActiveTypeAS
    {
        public override void OnAir()
        {
            throw new System.NotImplementedException();
        }

        //public override bool OnHit(Collider2D target, IArrowObject shooter)
        //{
        //    throw new System.NotImplementedException();
        //}

        public override void Clear()
        {
            throw new System.NotImplementedException();
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
