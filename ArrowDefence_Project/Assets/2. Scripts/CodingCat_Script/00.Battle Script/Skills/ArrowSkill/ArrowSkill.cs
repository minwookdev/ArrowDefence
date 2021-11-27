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

        public abstract void OnExit(Collider2D target);
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
                
                //마지막으로 hit된 대상 거르고, 주변에 다른 타겟이 없다면 Hit처리 후 비활성화
                if(tempCollList.Count <= 0) {
                    target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
                    targetTr = null;
                    return true;
                }

                //Scan범위에 다른 대상이 존재하는 경우 [랜덤 목표 산출] [거리를 비교해서 찾지 않음]
                targetTr = tempCollList[Random.Range(0, tempCollList.Count)].transform;
                return false;

                ///Description
                ///A. 현재 로직상에서 중복 대상과 Hit된 경우는 호밍화살과 연계된 상황에서 
                ///   첫 대상과 hit되고 두번째 Target을 찾아가던 중 Target이 다른화살에 의해 비활성화
                ///   처리되고, 호밍화살에서 다시 첫 대상을 찾아가는 상황에서 자주 발생한다.
                ///B. 이 경우 원래 로직에서는 중복 대상이기 때문에 hit처리하지 않고, 계속 대상에 무의미한 hit를
                ///   수행하는 모습을 보였다. 이 때문에 Air Skill (Homing)과 연계된 경우 중복 대상일 경우
                ///   주변의 다른 타겟이 있는지 확인한 이후, 다른 타겟이 있다면 hit처리를 무시하고 대상
                ///   Target의 Transform을 할당하여 내보내는 처리를 하고, 대상외 다른 적절한 타겟이 없다면
                ///   hit처리 후 비활성화를 요청한다.
                ///C. 야기되는 문제는 첫 대상에 hit된 후, 화살의 회전 로직으로 인해 다시금 충돌되는 상황에서의
                ///   처리 문제이지만, 이 경우는 첫 충돌 이후, 다른 대상 타겟을 찾았고 연계횟수 중첩이후
                ///   대미지 처리가 들어간 후 다른 Target Transform을 찾았을 때 라는 것을 의미한다.
                ///   결국에 첫 충돌이후 비활성화되지 않았으면 주변의 다른 대상이 분명히 있고
                ///   다시 충돌한다고 해도 hit처리 및 비활성화 처리가 들어가지 않게되고, 다른 Target Transform을
                ///   전달한다는 말이다.
                ///D. 논리상 문제는 없을것으로 판단.
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

            //■■■■■■■■■■■■■■■ IV. Force to Target Position ■■■■■■■■■■■■■■■
            targetTr = bestTargetTr; 
            arrow.ForceToTarget(monsterPos);
            return false;
        }

        public override void OnExit(Collider2D target)
        {
            throw new System.NotImplementedException();
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

        /// <summary>
        /// Create Skill Class Data in Skill Scripable Object
        /// </summary>
        /// <param name="item"></param>
        public ReboundArrow(DataRebound item)
        {
            scanRange     = item.ScanRadius;
            maxChainCount = item.MaxChainCount;
        }

        /// <summary>
        /// Public Empty Constructor for ES3
        /// </summary>
        public ReboundArrow() {

        }
    }

    public class HomingArrow : AirActiveTypeAS
    {
        Transform targetTr      = null;    //temp Target Transform
        float currentSearchTime = 0f;   //Current Target Search Time
        bool isFindTarget       = false;
                                       
        //Target Colliders
        Collider2D[] colliders = null;
        bool isFixDirection    = false;

        //Saving Variables
        float searchInterval = .1f;  //Find Target Update Interval
        float scanRadius     = 3f;   //Detection Range
        float speed          = 6f;   //Target Chasing Speed Value
        float rotateSpeed    = 800f; //Target Chasing Rotate Speed Value

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
            if (targetTr == null) return; //Call Safety
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
        public HomingArrow(HomingArrow origin)
        {
            searchInterval = origin.searchInterval;
            scanRadius     = origin.scanRadius;
            speed          = origin.speed;
            rotateSpeed    = origin.rotateSpeed;
        }

        /// <summary>
        /// Create Skill Data in Skill Scriptable Object
        /// </summary>
        /// <param name="data"></param>
        public HomingArrow(DataHoming data)
        {
            searchInterval = data.TargetSearchInterval;
            scanRadius     = data.ScanRadius;
            speed          = data.HomingSpeed;
            rotateSpeed    = data.HomingRotateSpeed;
        }

        /// <summary>
        /// TEMP Constructor
        /// </summary>
        public HomingArrow() {

        }
    }

    public class SplitArrow : AddProjTypeAS
    {
        public override void Clear()
        {
            throw new System.NotImplementedException();
        }

        public override void OnHit()
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
        public byte currentChainCount = 0;
        public byte maxChainCount;

        public GameObject lastHitTarget = null;

        float tempRadius = 5f;

        ///관통 횟수에 따른 데미지 감소효과 구현필요.

        /// <summary>
        /// return true == DisableArrow || false == aliveArrow
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target)
        {
            if (lastHitTarget == target.gameObject) {
                //Ignore Duplicate Target
                return false;
            }
            else {
                if (currentChainCount >= maxChainCount) {
                    //Hit 처리 후 화살객체 Disable
                    target.SendMessage("OnHitObject", Random.Range(30f, 5f), SendMessageOptions.DontRequireReceiver);
                    return true;
                }

                //연쇄횟수 중첩 및 타겟 저장
                currentChainCount++;
                lastHitTarget = target.gameObject;

                //Monster Hit 처리
                target.SendMessage("OnHitObject", Random.Range(30f, 50f), SendMessageOptions.DontRequireReceiver);
            } return false;
        }

        /// <summary>
        /// Linked Air Type Skill
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetTr"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, out Transform targetTr)
        {
            if(lastHitTarget == target.gameObject) {
                //Ignore Duplicate Target
                targetTr = null; return false;
            }
            else {
                if(currentChainCount >= maxChainCount) {
                    //Hit처리 후 Arrow Object Disable 요청
                    target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
                    targetTr = null; return true;
                }

                //연쇄 횟수 중첩 및 타겟 저장
                currentChainCount++;
                lastHitTarget = target.gameObject;

                target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            }

            //Air Skill과 연계된 경우, 주변의 Random Monster Target을 넘겨줌
            var collList = new List<Collider2D>(
            Physics2D.OverlapCircleAll(arrowTr.position, tempRadius,
                                       1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)));
            for (int i = collList.Count - 1; i >= 0 ; i--) {
                if (collList[i].gameObject == lastHitTarget)
                    collList.Remove(collList[i]);
            }

            if(collList.Count <= 0) {
                targetTr = null;
                return false;
            }
            else {
                targetTr = collList[Random.Range(0, collList.Count)].transform;
                return false;
            }
        }

        public override void OnExit(Collider2D target)
        {
            //저장 객체 초기화
            if (lastHitTarget == target.gameObject)
                lastHitTarget = null;
        }

        public override void Clear()
        {
            currentChainCount = 0;
        }

        public PiercingArrow(PiercingArrow origin)
        {
            maxChainCount = origin.maxChainCount;
        }

        public PiercingArrow(DataPiercing data)
        {
            maxChainCount = data.MaxChainCount;
        }

        /// <summary>
        /// Public Empty Constructor for ES3
        /// </summary>
        public PiercingArrow() { }
    }

}
