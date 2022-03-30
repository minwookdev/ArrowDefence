namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ArrowSkill {
        protected Transform arrowTr;
        protected Rigidbody2D rBody;
        protected IArrowObject arrow;

        /// <summary>
        /// Arrow classes that use skill must use init.
        /// </summary>
        /// <param name="tr">Arrow Transform</param>
        /// <param name="rigid">Arrow Rigid Body 2D</param>
        /// <param name="arrowInter">Interface Arrow Object</param>
        public virtual void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter)
        {
            arrowTr = tr;
            rBody   = rigid;
            arrow   = arrowInter;
        }
        public virtual void Release() { }

        /// <summary>
        /// Call When Arrow GameObject Disable
        /// </summary>
        public abstract void Clear();

        public abstract string GetDescription(string localizedString);
    }

    public abstract class AttackActiveTypeAS : ArrowSkill {
        protected GameObject lastHitTarget = null;
        protected ACEffector2D[] effects   = null;
        #region PROPERTY
        public int EffectsLength {
            get {
                if(effects == null) {
                    throw new System.Exception("the effects Array is Not assignment.");
                }

                return effects.Length;
            }
        }
        public bool IsEffectUser {
            get {
                if(effects == null) {
                    return false;
                }

                if(effects.Length == 0) {
                    CatLog.WLog($"hit skill is assignment, but effects Array is Empty.");
                    return false;
                }

                return true;
            }
        }
        public ACEffector2D[] Effects {
            get {
                if(effects == null) {
                    throw new System.Exception("the EffectArray is Not Assignment.");
                }

                return effects;
            }
        }
        #endregion

        public abstract bool OnHit(Collider2D target, ref DamageStruct damage, Vector3 contact, Vector2 direction);

        public virtual bool OnHit(Collider2D target, out Transform targetTr, ref DamageStruct damage, Vector3 contact, Vector2 direction) {
            targetTr = null; return true;
        }

        public virtual void OnExit(Collider2D target) {
            //Hit처리 되면서 대상 객체가 비활성화 처리됨과 동시에 Exit함수가 들어오면 NULL잡음.
            //if (target == null) return;

            //저장된 마지막 타격 객체 제거
            //if (target != null && target.gameObject == lastHitTarget) {
            //    lastHitTarget = null;
            //}

            //개선식
            if (target != null) {
                if (target.gameObject == lastHitTarget) {
                    lastHitTarget = null;
                }
                else return;
            }
            else return;
        }

        public abstract void EffectPlay();
    }

    public class ReboundArrow : AttackActiveTypeAS {
        //Save Variables
        int maxChainCount = 2;  // Max Chain Count
        float scanRange   = 5f; // Monster Detect Range

        //Temp Variables
        List<Collider2D> tempCollList = null;
        int currentChainCount         = 0;  // Current Chain Count
        string[] effectPoolTags;

        public override string GetDescription(string localizedString) {
            return string.Format(localizedString, maxChainCount);
        }

        public override bool OnHit(Collider2D target, ref DamageStruct damage, Vector3 contact, Vector2 direction) {
            //=============================================[ PHASE I. ACTIVATING & TARGET CHECKER ]=========================================================
            //최근에 Hit처리한 객체와 동일한 객체와 다시 충돌될 경우, return 처리
            //해당 Monster Object를 무시함 [같은 객체에게 스킬 효과를 터트릴 수 없음]
            if (lastHitTarget == target.gameObject) {    // Same Target Check
                return false;                            // Ignore
            }
            else {
                // Max Chain : Try On Hit and Disable
                if (currentChainCount >= maxChainCount) { //Try OnHit
                    return target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction);
                }

                // Not Max Chain : Try Activate Skill
                if (target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction)) { //Try OnHit
                    currentChainCount++;
                    lastHitTarget = target.gameObject;
                    arrow.PlayEffect(contact);
                }
                else { //if Failed OnHit, Ignore
                    return false;
                }
            }
            //==============================================================================================================================================

            //================================================[ PHASE II. REBOUND TARGET FINDER ]===========================================================
            tempCollList = new List<Collider2D>(Physics2D.OverlapCircleAll(arrowTr.position, scanRange, 1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)));
            var duplicateTarget = tempCollList.Find(element => element == target);
            if (duplicateTarget != null) tempCollList.Remove(duplicateTarget);  // Remove Duplicate Target
            if (tempCollList.Count <= 0) return true;                           // Rebound Target Not Found -> Disable.
            //==============================================================================================================================================

            //==============================================[ PHASE III. CALCULATE TARGET'S DISTANCE ]======================================================
            //Transform bestTargetTr = null;       // Used Transform is Instability
            Vector3 monsterPos     = Vector3.zero; // Transform이 아닌 Vector Type 사용.
            float closestDistSqr   = Mathf.Infinity;
            for (int i = 0; i < tempCollList.Count; i++) {
                Vector2 directionToTarget = tempCollList[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if(distSqr < closestDistSqr) {
                    closestDistSqr = distSqr;
                    monsterPos     = tempCollList[i].transform.position;
                }
            }
            //==============================================================================================================================================

            //================================================[ PHASE IV. FORCE TO TARGET POSITION ]========================================================
            arrow.ForceToTarget(monsterPos);
            return false;
            //==============================================================================================================================================
        }

        /// <summary>
        /// linked to air skills
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetTr"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, out Transform targetTr, ref DamageStruct damage, Vector3 contact, Vector2 direction) {
            //=============================================[ PHASE I. ACTIVATING & TARGET CHECKER ]=========================================================
            if (lastHitTarget == target.gameObject) {
                //Ignore Same Target for Collision Stay
                targetTr = null; return false;
            }
            else {
                // Max Chain : Try On Hit
                if (currentChainCount >= maxChainCount){
                    targetTr = null; 
                    return target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction);
                }

                // Not Max Chain : Try OnHit and Activating Skill
                if (target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction)) {
                    currentChainCount++;
                    lastHitTarget = target.gameObject;

                    arrow.PlayEffect(contact);
                }
                else{ //if Failed OnHit, Ignore
                    targetTr = null;
                    return false;
                }
            }
            //==============================================================================================================================================

            //================================================[ PHASE II. REBOUND TARGET FINDER ]===========================================================
            tempCollList = new List<Collider2D>(Physics2D.OverlapCircleAll(arrowTr.position, scanRange, 1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)));
            var dupTarget = tempCollList.Find(element => element == target);
            if (dupTarget != null) tempCollList.Remove(dupTarget);  //Remove Duplicate Target Monster
            if (tempCollList.Count <= 0) {                          //Not Found Target -> Disable
                targetTr = null;
                return true;
            }
            //==============================================================================================================================================

            //==============================================[ PHASE III. CALCULATE TARGET'S DISTANCE ]======================================================
            Transform tempTransform = null;
            Vector3 monsterPos      = Vector3.zero; //Rebound Target Position Save.
            float closestDistSqr    = Mathf.Infinity;
            for (int i = 0; i < tempCollList.Count; i++) {
                Vector2 directionToTarget = tempCollList[i].transform.position - arrowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if (distSqr < closestDistSqr) {
                    closestDistSqr = distSqr;
                    monsterPos     = tempCollList[i].transform.position;
                    tempTransform  = tempCollList[i].transform; //Sending Target Transform.
                }
            }
            //==============================================================================================================================================

            //================================================[ PHASE IV. FORCE TO TARGET POSITION ]========================================================
            arrow.ForceToTarget(monsterPos);
            targetTr = tempTransform;
            return false;
            //==============================================================================================================================================
        }

        /// <summary>
        /// call when arrow disable
        /// </summary>
        public override void Clear() {
            lastHitTarget     = null;
            currentChainCount = 0;
        }

        public override void EffectPlay() {
            CCPooler.SpawnFromPool<ACEffector2D>(effectPoolTags.RandIndex<string>(), Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="origin"></param>
        public ReboundArrow(ReboundArrow origin, string tag= "") {
            maxChainCount = origin.maxChainCount;
            scanRange     = origin.scanRange;
            effects       = origin.effects;

            if(tag != null || tag != "") {
                List<string> effectPoolTagList = new List<string>();
                for (int i = 0; i < effects.Length; i++) {
                    effectPoolTagList.Add($"{tag}{AD_Data.POOLTAG_HITEFFECT}{i}");
                }
                effectPoolTags = effectPoolTagList.ToArray();
            }
        }

        /// <summary>
        /// Create Skill Class Data in Skill Scripable Object
        /// </summary>
        /// <param name="item"></param>
        public ReboundArrow(DataRebound item) {
            scanRange     = item.ScanRadius;
            maxChainCount = item.MaxChainCount;
            effects       = item.effects;
        }

        /// <summary>
        /// Public Empty Constructor for ES3
        /// </summary>
        public ReboundArrow() { }
    }

    public class PiercingArrow : AttackActiveTypeAS {
        //SAVE VARIABLES
        public byte maxChainCount;

        //TEMP VARIABLES
        byte currentChainCount = 0;
        bool isResult = false;
        float tempRadius = 5f;
        Collider2D[] tempArray = null;

        public override string GetDescription(string localizedString) {
            throw new System.NotImplementedException();
        }

        ///관통 횟수에 따른 데미지 감소효과 구현

        /// <summary>
        /// return true == DisableArrow || false == aliveArrow
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            //================================================[ ON HIT TARGET & INC CHAIN ]=======================================================
            if (lastHitTarget == target.gameObject) {    //Ignore Duplicate Target
                return false;
            }
            else {
                if(currentChainCount >= maxChainCount) { // Max Chain Count : Try OnHit
                    return target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
                }

                //Try On Hit
                isResult = target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
                if(isResult) {
                    //Success OnHit
                    currentChainCount++;
                    lastHitTarget = target.gameObject;
                }

                return isResult;
            } 
            //====================================================================================================================================
        }

        /// <summary>
        /// Linked Air Type Skill
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetTr"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, out Transform targetTr, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            //================================================[ IGNORE DUPLICATE TARGET ]=========================================================
            if (lastHitTarget == target.gameObject) {
                targetTr = null; 
                return false;
            }
            else {
            //=================================================[ ARRIVAL MAX CHAINCOUNT ]=========================================================
                if(currentChainCount >= maxChainCount) {
                    targetTr = null;
                    return target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
                }

            //========================================================[ TRY ON HIT ]==============================================================
                isResult = target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
                if(isResult == false) { //Failed Target OnHit
                    targetTr = null;
                    return isResult;
                }

                //Success Target OnHit
                currentChainCount++;
                lastHitTarget = target.gameObject;
            //====================================================================================================================================
            }

            //==================================================[ FIND TARGET TRANSFORM ]=========================================================
            tempArray = GameGlobal.OverlapCircleAll2D(arrowTr, tempRadius, AD_Data.LAYER_MONSTER, collider => collider.gameObject == target);
            if(tempArray.Length <= 0) {
                //Not Found a Target.
                targetTr = null;
            }
            else {
                //Find the Target. (Random Target Sending)
                targetTr = tempArray[Random.Range(0, tempArray.Length)].transform;
            }
            //====================================================================================================================================

            return isResult;
        }

        public override void Clear() {
            currentChainCount = 0;
        }

        public override void EffectPlay() {
            throw new System.NotImplementedException();
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
