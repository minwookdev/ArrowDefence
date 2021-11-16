namespace ActionCat
{
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ArrowSkill
    {
        protected Transform arrowTr;
        protected Rigidbody2D rBody;
        protected IArrowObject arrow;

        //Arrow Skill 자체는 한번만 해놓으면 되지만, Init함수는 각 Arrow별로 한번씩은 잡아줘야한다.
        //각각의 Arrow별로 Transform과 RigidBody를 사용하기 때문.
        public virtual void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter)
        {
            arrowTr = tr;
            rBody   = rigid;
            arrow   = arrowInter;
        }

        public abstract void Clear();
    }

    public abstract class AttackActiveTypeAS : ArrowSkill
    {
        public abstract bool OnHit(Collider2D target);

        public virtual bool OnHit(Collider2D target, out Transform targetTr)
        {
            targetTr = null; return true;
        }
    }

    public abstract class AirActiveTypeAS : ArrowSkill
    {
        public virtual void CallbackOnHit(Transform tr) { }

        public abstract void OnUpdate();

        public virtual void OnFixedUpdate() { }
    }

    public abstract class AddProjTypeAS : ArrowSkill
    {
        public abstract void OnHit();
    }

    public class ReboundArrow : AttackActiveTypeAS
    {
        GameObject lastHitTarget = null;
        int currentChainCount = 0;  //현재 연쇄횟수
        int maxChainCount     = 2;  //최대 연쇄횟수
        float scanRange       = 5f; //Monster 인식 범위

        //temp colliders
        Collider2D[] tempCollArray    = null;
        List<Collider2D> tempCollList = null;

        //return true : DisableArrow || false : IgnoreCollision
        public override bool OnHit(Collider2D target)
        {
            //■■■■■■■■■■■■■ I. Availablity Arrow Skill : 중복 다겟 및 연쇄 횟수 체크 ■■■■■■■■■■■■■
            //최근에 Hit처리한 객체와 동일한 객체와 다시 충돌될 경우, return 처리
            //해당 Monster Object를 무시함 [같은 객체에게 스킬 효과를 터트릴 수 없음]
            if (lastHitTarget == target.gameObject) {
                //동일한 객체에 재-충돌 무시
                return false;
            }
            else {
                //연쇄횟수 체크
                if(currentChainCount >= maxChainCount) {
                    //Monster Hit : 최대 연쇄횟수 도달
                    target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
                    return true;
                }

                //현재 연쇄횟수 중첩 및 마지막 적 저장
                currentChainCount++;
                lastHitTarget = target.gameObject;

                //Monster hit
                target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            }

            //■■■■■■■■■■■■■■■ II Rebound Arrow Skill : Active 절차 개시 ■■■■■■■■■■■■■■■
            tempCollArray = Physics2D.OverlapCircleAll(arrowTr.position, scanRange);
            if (tempCollArray.Length == 0) {
                //주변에 bound 대상 객체가 없는 경우 화살 소멸
                return true;
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

            //■■■■■■■■■■■■■■■ III. Optimal target selection ■■■■■■■■■■■■■■■
            tempCollList = new List<Collider2D>(tempCollArray);
            for (int i = tempCollList.Count - 1; i >= 0; i--) //Reverse Loop [Remove Array Element]
            {
                //중복 발동 대상과 Monster Layer가 아닌 Collider는 예외처리.
                if (tempCollList[i] == target || tempCollList[i].CompareTag(AD_Data.OBJECT_TAG_MONSTER) == false)
                    tempCollList.Remove(tempCollList[i]);
            }
            if(tempCollList.Count <= 0) //예외처리후 타겟이 없으면 비활성화 처리.
            {
                return true;
            }

            //Transform bestTargetTr = null; //-> Vector2 Type으로 변경 (참조값이라 중간에 추적중인 Monster Tr 사라지면 에러)
            Vector3 monsterPos     = Vector3.zero; //Transform이 아닌 Position 저장
            float closestDistSqr   = Mathf.Infinity;
            for (int i = 0; i < tempCollList.Count; i++)
            {
                Vector2 directionToTarget = tempCollList[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if(distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    monsterPos     = tempCollList[i].transform.position;
                }
            }

            //■■■■■■■■■■■■■■■ III. Force to Target Position ■■■■■■■■■■■■■■■
            arrow.ForceToTarget(monsterPos);
            return false;
        }

        /// <summary>
        /// linked to air skills
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetTr"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, out Transform targetTr)
        {
            //■■■■■■■■■■■■■ I. Availablity Arrow Skill : 중복 타겟 및 연쇄 횟수 체크 ■■■■■■■■■■■■■
            if (lastHitTarget == target.gameObject) {
                //Air Skill과 Link된 함수는 중복대상과 다시 충돌 시, 근처의 다른 대상을 탐색후 반환하는 로직을 추가.
                tempCollArray = Physics2D.OverlapCircleAll(arrowTr.position, scanRange, 1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER));
                tempCollList = new List<Collider2D>(tempCollArray);
                for (int i = tempCollList.Count - 1; i >= 0; i--) {
                    if (tempCollList[i].gameObject == lastHitTarget)
                        tempCollList.Remove(tempCollList[i]);
                }
                
                //마지막으로 hit된 대상 거르고, 주변에 다른 타겟이 없다면 null return
                if(tempCollList.Count <= 0) {
                    targetTr = null;
                    return false;
                }

                //Scan범위에 다른 대상이 존재하는 경우 [랜덤 목표 산출] [거리를 비교해서 찾지 않음]
                targetTr = tempCollList[Random.Range(0, tempCollList.Count)].transform;
                return false;
            }
            else {
                //연쇄횟수 체크
                if (currentChainCount >= maxChainCount)
                {
                    //Monster Hit 처리
                    target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
                    targetTr = null; 
                    return true;
                    //arrow.DisableObject_Req(arrowTr.gameObject); return true;
                }

                //현재 연쇄횟수 중첩 및 마지막 적 저장
                currentChainCount++;
                lastHitTarget = target.gameObject;

                //Monster hit 처리
                target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            }
            //■■■■■■■■■■■■■■■ II Rebound Arrow Skill : Active 절차 개시 ■■■■■■■■■■■■■■■
            tempCollArray = Physics2D.OverlapCircleAll(arrowTr.position, scanRange);
            if (tempCollArray.Length == 0) //return
            {
                //주변에 Rebound할 대상 객체가 없는 경우 소멸
                targetTr = null; 
                return true;
            }
            //■■■■■■■■■■■■■■■ III. Optimal target selection ■■■■■■■■■■■■■■■
            tempCollList = new List<Collider2D>(tempCollArray);
            for (int i = tempCollList.Count - 1; i >= 0; i--) //Reverse Loop [Remove Array Element]
            {
                //중복 발동 대상과 Monster가 아닌 Collider는 예외처리.
                if (tempCollList[i] == target || tempCollList[i].CompareTag(AD_Data.OBJECT_TAG_MONSTER) == false)
                    tempCollList.Remove(tempCollList[i]);
            }
            if (tempCollList.Count <= 0) //예외처리후 타겟이 없으면 비활성화 처리.
            {
                targetTr = null; 
                return true;
            }

            Transform bestTargetTr = null;         //-> Target Transform에 넘겨줄 주소
            Vector3 monsterPos     = Vector3.zero; //Transform이 아닌 Position 저장
            float closestDistSqr   = Mathf.Infinity;
            for (int i = 0; i < tempCollList.Count; i++)
            {
                Vector2 directionToTarget = tempCollList[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if (distSqr < closestDistSqr)
                {
                    closestDistSqr = distSqr;
                    monsterPos     = tempCollList[i].transform.position;
                    bestTargetTr   = tempCollList[i].transform;
                }
            }

            //■■■■■■■■■■■■■■■ III. Force to Target Position ■■■■■■■■■■■■■■■
            targetTr = bestTargetTr; 
            arrow.ForceToTarget(monsterPos);
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
            maxChainCount = 2;
            scanRange = 5f;
        }
    }

    public class HomingArrow : AirActiveTypeAS
    {
        Transform targetTr   = null;
        bool isFindTarget    = false;
        float searchInterval = .1f;     //Find Target Update Interval
        float currentSearchTime = 0f;   //Current Search Time
        float scanRadius        = 3f;   //Detection Range
                                       
        //Chasing Speed value
        float speed       = 6f;
        float rotateSpeed = 800f;

        //Target Colliders
        Collider2D[] colliders = null;
        bool isFixDirection    = false;

        //Call Every Frames
        public override void OnUpdate() 
        {
            if (targetTr == null) { //Target Not Found
                isFindTarget = false;

                //Update Target Find Interval
                currentSearchTime -= Time.deltaTime;
                if(currentSearchTime <= 0)
                {
                    //Target Search Interval
                    targetTr = SearchTarget();
                    currentSearchTime = searchInterval;
                }
            }
            else {  //Target Found
                isFindTarget = true;

                //Target GameObject Alive Check
                if (targetTr.gameObject.activeSelf == false)
                    targetTr = null;
            }
        }

        public override void OnFixedUpdate()
        {
            if (isFindTarget)
                Homing(targetTr);
            else
                DirectionFix();
        }

        /// <summary>
        /// Link with Hit Type Skill
        /// </summary>
        /// <param name="tr">target transform</param>
        public override void CallbackOnHit(Transform tr)
        {
            if (tr == null) return;
            targetTr = tr;
        }

        public override void Clear()
        {
            targetTr  = null;
            colliders = null;
            currentSearchTime = 0f;
            isFindTarget      = false;
        }

        Transform SearchTarget()
        {
            colliders = Physics2D.OverlapCircleAll(arrowTr.position, scanRadius, 1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER));
            //CatLog.Log($"Catch Monster Colliders Count : {colliders.Length.ToString()}");
            if (colliders.Length <= 0)      // No Collider Detected.
                return null;
            else if (colliders.Length == 1) // One Collider Detected.
                return colliders[0].transform;
            else                            // Detected 2 or More Colliders
            {
                float closestDistSqr      = Mathf.Infinity;
                Transform optimalTargetTr = null;
                //Check Disatance Comparison.
                for (int i = 0; i < colliders.Length; i++)
                {
                    //Distance Check
                    float distSqr = (colliders[i].transform.position - arrowTr.position).sqrMagnitude;
                    if(distSqr < closestDistSqr)
                    {
                        //Catch Best Monster Target Transform
                        optimalTargetTr = colliders[i].transform;
                        closestDistSqr  = distSqr;
                    }
                }

                return optimalTargetTr;
            }

            //Target을 찾지못하면 DirectionFix들어가서 현재 바라보고있는 방향으로 바로 쏴줘야하는데
            //Homing의 speed로 계속 받고있는건가
            //속도도 조건을 주어서 해결완료.
            //Rebound Arrow와 같이 묶였을때, 중복 대상은 Target으로 잡지 않아야 하는데 잡아버리는 문제 발견.
        }

        void Homing(Transform targetTr)
        {
            //if (targetTr == null) return; //-> 호출부에서 예외처리 되고있기 때문에 따로 안해줌
            //Fix Direction after Non-target
            isFixDirection = false;

            Vector2 direction = (Vector2)targetTr.position - rBody.position;
            direction.Normalize(); //Only Direction

            //Only Used Z angle : 2D
            float rotateAmount = Vector3.Cross(direction, arrowTr.up).z;

            rBody.angularVelocity = -rotateAmount * rotateSpeed;

            //Force To Arrow Forward
            rBody.velocity = arrowTr.up * speed;
        }

        void DirectionFix()
        {
            //if (rBody.angularVelocity > 0f)
            //{
            //    rBody.angularVelocity = 0f;
            //    arrow.ShotToDirectly(arrowTr.up);
            //}

            //Fix Direction Once. (used isFixDirection)
            //if(isFixDirection == false)
            //{
            //    if(rBody.angularVelocity > 0f)
            //    {
            //        rBody.angularVelocity = 0f;
            //        arrow.ShotToDirectly(arrowTr.up);
            //    }
            //
            //    isFixDirection = true;
            //}

            //Fix Direction -> 이대로면 화살이 느려지지는 않지만, 계속 Update들어오게 되서 왠지 좀 싫음 
            //rBody.angularVelocity = 0f;
            //arrow.ShotToDirectly(arrowTr.up);

            //Direction Fix 들어왔을 때, 한번만 잡아주는 방법으로 진행하고 싶음
            if(isFixDirection == false)
            {
                if(rBody.angularVelocity > 0f || rBody.velocity.magnitude < 10f)
                {
                    rBody.angularVelocity = 0f;
                    arrow.ShotToDirectly(arrowTr.up);
                } isFixDirection = true;
            }

            //다섯판정도 테스트 진행 봄
        }

        public override void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter)
        {
            base.Init(tr, rigid, arrowInter);
            currentSearchTime = searchInterval;
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="guidanceArrow"></param>
        public HomingArrow(HomingArrow homingarrow)
        {
            
        }

        /// <summary>
        /// TEMP Constructor
        /// </summary>
        public HomingArrow()
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

    public class PiercingArrow : AttackActiveTypeAS
    {
        public override bool OnHit(Collider2D target)
        {
            throw new System.NotImplementedException();
        }

        public override bool OnHit(Collider2D target, out Transform targetTr)
        {
            return base.OnHit(target, out targetTr);
        }

        public override void Clear()
        {
            throw new System.NotImplementedException();
        }
    }

}
