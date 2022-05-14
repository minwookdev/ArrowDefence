namespace ActionCat {
    using UnityEngine;
    using System.Collections.Generic;
    using ActionCat.Interface;

    public abstract class AirType : ArrowSkill {
        public virtual void OnHitCallback(Transform tr) { }
        public virtual void OnFixedUpdate() { }
        public abstract void OnUpdate();
    }

    public class HomingArrow : AirType {
        // [ Saved-Variables ]
        float scanRadius = 3f;    //Detection Range
        float speed = 6f;         //Target Chasing Speed Value
        float rotateSpeed = 800f; //Target Chasing Rotate Speed Value

        // [ Non-Saved-Variables ]
        List<Collider2D> colliderList = null;
        Transform targetTr = null;    //temp Target Transform
        bool isFindTarget = false;    //is Find a Target?
        bool isFixDirection = false;  //Non-Target Direction
        float currentSearchTime = 0f; //Current Target Search Time

        //Update Interval [Static] [Recomended = 0.1f]
        float searchInterval = .1f;  //Find Target Update Interval

        public override string GetDesc(string localizedString) {
            return localizedString;
        }

        //Call Every Frames
        public override void OnUpdate() {
            //================================================[ FIND A NEW TARGET ]=======================================================
            if (isFindTarget == false) {
                //Update Target Finder
                currentSearchTime -= Time.deltaTime;
                if (currentSearchTime <= 0) {
                    //Target Search Interval
                    targetTr = SearchTarget(out isFindTarget);
                    currentSearchTime = searchInterval;
                }
            }
            //==================================================[ TARGET FOUND ]==========================================================
            else {
                //Update Target alive Check
                currentSearchTime -= Time.deltaTime;
                if (currentSearchTime <= 0) {
                    //Target alive Check Interval
                    if (targetTr.gameObject.activeSelf == false || targetTr.GetComponent<IDamageable>().IsAlive() == false) {
                        targetTr = null;
                        isFindTarget = false;
                    }
                    currentSearchTime = searchInterval;
                }
            }
            //============================================================================================================================
        }

        public override void OnFixedUpdate() {
            if (isFindTarget == true) {
                Homing(targetTr);
            }
            else {
                DirectionFix();
            }
        }

        /// <summary>
        /// Link with Hit Type Skill
        /// </summary>
        /// <param name="tr">target transform</param>
        public override void OnHitCallback(Transform tr) {
            if (tr != null) {
                targetTr = tr;
            }
        }

        public override void Clear() {
            currentSearchTime = 0f;
            targetTr = null;
            isFindTarget = false;
        }

        Transform SearchTarget(out bool isTargetFind) {
            //================================================[ START TARGET FIND ]==========================================================
            colliderList = new List<Collider2D>(Physics2D.OverlapCircleAll(arrowTr.position, scanRadius, 1 << LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)));
            colliderList.RemoveAll(collider => collider.GetComponent<IDamageable>().IsAlive() == false); //Remove Element at already Death.
            //==================================================[ NO TARGET FIND ]==========================================================
            if (colliderList.Count <= 0) {
                isTargetFind = false;
                return null;
            }
            //=================================================[ ONE TARGET FIND ]==========================================================
            else if (colliderList.Count == 1) {
                isTargetFind = true;
                return colliderList[0].transform;
            }
            //================================================[ MORE TARGET FIND ]==========================================================
            else {
                float closestDistSqr = Mathf.Infinity;
                Transform optimalTargetTr = null;
                //Check Disatance Comparison.
                for (int i = 0; i < colliderList.Count; i++) {
                    //Distance Check
                    float distSqr = (colliderList[i].transform.position - arrowTr.position).sqrMagnitude;
                    if (distSqr < closestDistSqr) {
                        //Catch Best Monster Target Transform
                        optimalTargetTr = colliderList[i].transform;
                        closestDistSqr = distSqr;
                    }
                }

                isTargetFind = true;
                return optimalTargetTr;
            }
            //==============================================================================================================================
        }

        void Homing(Transform targetTr) {
            if (targetTr == null) { //Target Transform Check (call safety)
                return;
            }

            Vector2 direction = (Vector2)targetTr.position - rBody.position;
            direction.Normalize(); //Only Direction

            //Only Used Z angle : 2D
            float rotateAmount = Vector3.Cross(direction, arrowTr.up).z;

            rBody.angularVelocity = -rotateAmount * rotateSpeed;

            //Force To Arrow Forward [Delete this]
            rBody.velocity = arrowTr.up * speed;

            //Set Fix Direction is false, for Non-Target.
            isFixDirection = false;
        }

        void DirectionFix() {
            //if Not Found the Target Object, Force to Directly Direction.
            if (isFixDirection == false) { //Stop Arrow Homing.
                if (rBody.angularVelocity > 0f || rBody.velocity.magnitude < 10f) {
                    rBody.angularVelocity = 0f;
                    arrow.ForceToDirectly();
                }
                isFixDirection = true;
            }
        }

        public override void Init(Transform tr, Rigidbody2D rigid, IArrowObject arrowInter) {
            base.Init(tr, rigid, arrowInter);
            currentSearchTime = searchInterval;
        }

        /// <summary>
        /// Copy Class Constructor
        /// </summary>
        /// <param name="guidanceArrow"></param>
        public HomingArrow(HomingArrow origin) {
            scanRadius = origin.scanRadius;
            speed = origin.speed;
            rotateSpeed = origin.rotateSpeed;
        }

        /// <summary>
        /// Create Skill Data in Skill Scriptable Object
        /// </summary>
        /// <param name="data"></param>
        public HomingArrow(DataHoming data) {
            scanRadius = data.ScanRadius;
            speed = data.HomingSpeed;
            rotateSpeed = data.HomingRotateSpeed;
        }

        #region ES3
        public HomingArrow() { }
        ~HomingArrow() { }
        #endregion
    }
}
