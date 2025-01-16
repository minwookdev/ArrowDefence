namespace ActionCat {
    using ActionCat.Interface;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ArrowSkill {
        // [ SAVED-VARIABLES ]
        protected AudioClip[] sounds;   //모든 Type의 ArrowSkill에서 Sound를 사용할 수 있도록 최상위 부모에 할당해둠

        // [ Non-Saved-Variables ]
        protected Transform arrowTr;
        protected Rigidbody2D rBody;
        protected IArrowObject arrow;

        /// <summary>
        /// Arrow classes that use skill must use init.
        /// </summary>
        /// <param name="tr">Arrow Transform</param>
        /// <param name="rigid">Arrow Rigid Body 2D</param>
        /// <param name="arrowInter">Interface Arrow Object</param>
        public virtual void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter) {
            arrowTr = tr;
            rBody   = rigid;
            arrow   = arrowInter;
        }

        /// <summary>
        /// Battle Scene종료 시 호출. 원본 ArrowItem의 SkillClass에 정리되어야 할 변수가 있는 경우에 사용.
        /// [각각의 Arrow Prefab 내부에서는 호출하지 않음]
        /// </summary>
        public virtual void ClearOrigin() { }

        /// <summary>
        /// Arrow 오브젝트가 Disable되는 타이밍에 호출됨.
        /// Prefab Disable 시, 정리되어야 할 변수가 있는 경우 사용.
        /// [각각의 Arrow Prefab에서 호출됨]
        /// </summary>
        public abstract void ClearOnDisable();

        public abstract string GetDesc(string localizedString);

        public virtual void PlaySoundToRandom() {
            arrow.PlayOneShot(sounds.RandIndex());
        }

        /// <summary>
        /// 재생하려는 사운드의 인덱스를 확실하게 알고있는 상태에서 사용
        /// </summary>
        /// <param name="soundIndex"></param>
        public virtual void PlaySound2Index(int soundIndex) {
            arrow.PlayOneShot(sounds[soundIndex]);
        }
    }

    //================================================================================================================================================================
    //================================================================ << ATTACK TYPE ARROW SKILL  >> ================================================================

    public abstract class AttackActiveTypeAS : ArrowSkill {
        // [ Saved-Variables ]
        protected ACEffector2D[] effects   = null;

        // [ Non-Saved-Variables ]
        protected GameObject lastHitTarget = null;
        protected string[] effectPoolTags  = null;

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
                    return new ACEffector2D[0] { };
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

        protected virtual void EffectPlay(Vector2 contactPosition, bool isRotateRandom) {
            var effect = CCPooler.SpawnFromPool<ACEffector2D>(effectPoolTags.RandIndex<string>(), contactPosition, Quaternion.identity);
            if (effect) {
                effect.PlayOnce(isRotateRandom);
            }
            else {
                CatLog.WLog("Effect Pool Object Not Exists.");
            }
        }

        public virtual void EffectToPool(string arrowTag) {
            if (this.effects == null || this.effects.Length <= 0) {
                CatLog.WLog("ATK Skill is Not Assignment Hit Effect.");
                return;
            }

            List<string> tempEffectTags = new List<string>();
            string tempEffectPoolTag = string.Empty;
            for (int i = 0; i < effects.Length; i++) {
                tempEffectPoolTag = $"{arrowTag}{AD_Data.POOLTAG_HITSKILL_EFFECT}{i}";
                CCPooler.AddPoolList(tempEffectPoolTag, 10, effects[i].gameObject, false);
                tempEffectTags.Add(tempEffectPoolTag);
            }
            effectPoolTags = tempEffectTags.ToArray();
            CatLog.Log($"EffectPoolTags Length: {effectPoolTags.Length}");
        }

        protected AttackActiveTypeAS(string[] originPoolTags) {
            if(originPoolTags != null) {
                this.effectPoolTags = originPoolTags;
            }
        }
        #region ES3
        public AttackActiveTypeAS() { }
        #endregion
    }

    public class ReboundArrow : AttackActiveTypeAS {
        // [ Saved-Variables ]
        int maxChainCount = 2;  // Max Chain Count
        float scanRange   = 5f; // Monster Detect Range

        // [ Non-Saved-Variables ]
        List<Collider2D> tempCollList = null;
        int currentChainCount         = 0;  // Current Chain Count

        public override string GetDesc(string localizedString) {
            return string.Format(localizedString, maxChainCount.ToString().GetColor(StringColor.YELLOW));
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
                    var result = target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction);
                    if (result) {   //Hit에 성공한 경우만 사운드 출력
                        //PlayRandomSound(); ReboundArrow는 현재 게임 내 구현되지 않는 상태이기 때문에 주석..
                    }
                    return result;
                    //return target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction);
                }

                // Not Max Chain : Try Activate Skill
                if (target.GetComponent<IDamageable>().TryOnHit(ref damage, contact, direction)) { //Try OnHit
                    currentChainCount++;
                    lastHitTarget = target.gameObject;
                    arrow.PlayEffect(contact);
                    //PlayRandomSound(); ReboundArrow는 현재 게임 내 구현되지 않는 상태이기 때문에 주석..
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
        public override void ClearOnDisable() {
            lastHitTarget = null;
            tempCollList  = null;
            currentChainCount = 0;
        }

        public override void EffectToPool(string arrowPoolTag) {
            throw new System.NotImplementedException();
        }

        protected override void EffectPlay(Vector2 contactPosition, bool isRandomRotate) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="origin"></param>
        public ReboundArrow(ReboundArrow origin) : base(origin.effectPoolTags) {
            maxChainCount = origin.maxChainCount;
            scanRange     = origin.scanRange;
            effects       = origin.effects;
            sounds        = origin.sounds;
        }

        /// <summary>
        /// Create Skill Class Data in Skill Scripable Object
        /// </summary>
        /// <param name="item"></param>
        public ReboundArrow(DataRebound item) {
            scanRange     = item.ScanRadius;
            maxChainCount = item.MaxChainCount;
            effects       = item.effects;
            sounds        = item.Sounds;
        }

        /// <summary>
        /// Public Empty Constructor for ES3
        /// </summary>
        public ReboundArrow() { }
    }

    public class PiercingArrow : AttackActiveTypeAS {
        // [ Saved-Variables ]
        public byte maxChainCount;

        // [ Non-Saved-Variables ]
        byte currentChainCount = 0;
        bool isResult = false;
        float tempRadius = 5f;
        Collider2D[] tempArray = null;

        public override string GetDesc(string localizedString) {
            return string.Format(localizedString, maxChainCount.ToString().GetColor(StringColor.YELLOW));
        }

        ///관통 횟수에 따른 데미지 감소효과 구현

        /// <summary>
        /// return true == DisableArrow || false == aliveArrow
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            //================================================[ ON HIT TARGET & INC CHAIN ]=======================================================
            isResult = target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
            if (isResult) { //:몬스터 객체에 충돌
                PlaySoundToRandom(); //Random Hit Sound Play
                if (currentChainCount >= maxChainCount) {
                    return isResult; //:true
                }
                else {
                    currentChainCount++;
                    EffectPlay(contactpoint, true);
                    return false; // 재충돌 가능한 상태이기때문에, false 처리.
                }
            }
            return false; 
            //====================================================================================================================================
        }

        /// <summary>
        /// Linked Air Type Skill
        /// </summary>
        /// <param name="target"></param>
        /// <param name="targetTr"></param>
        /// <returns></returns>
        public override bool OnHit(Collider2D target, out Transform targetTr, ref DamageStruct damage, Vector3 contactpoint, Vector2 direction) {
            isResult = target.GetComponent<IDamageable>().TryOnHit(ref damage, contactpoint, direction);
            if (isResult) {
                PlaySoundToRandom();  //Random Hit Sound Play
                if (currentChainCount >= maxChainCount) {
                    targetTr = null;
                    return isResult; //:true
                }
                else {
                    currentChainCount++;
                    EffectPlay(contactpoint, true);
                }
            }
            tempArray = GameGlobal.OverlapCircleAll2D(arrowTr, tempRadius, AD_Data.LAYER_MONSTER, collider => collider.gameObject == target);
            targetTr  = (tempArray.Length <= 0) ? null : tempArray.RandIndex().transform;
            return false;
        }

        public override void ClearOnDisable() {
            currentChainCount = 0;
            isResult          = false;
            tempArray         = null;
            lastHitTarget     = null;
        }

        public PiercingArrow(PiercingArrow origin) : base(origin.effectPoolTags) {
            maxChainCount = origin.maxChainCount;
            effects       = origin.effects;
            sounds        = origin.sounds;
        }

        public PiercingArrow(DataPiercing data) {
            maxChainCount = data.MaxChainCount;
            effects       = data.effects;
            sounds        = data.Sounds;
        }

        /// <summary>
        /// Public Empty Constructor for ES3
        /// </summary>
        public PiercingArrow() : base() { }
    }

}
