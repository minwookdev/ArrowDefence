namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;
    using System.Collections.Generic;

    public partial class AD_BowController {
        [Header("AUTOMODE")]
        [SerializeField] [ReadOnly] //Enums
        AUTOSTATE autoState = AUTOSTATE.WAIT;

        //Fields
        float targetFindInterval = 0.5f;
        float currentFindTime    = 0f;
        float FindDist           = 5f;
        float angleOffset        = 90f;
        bool isAutoRunning  = false;
        bool isAutoExitWait = false;

        //Temp
        IDamageable target = null;
        [SerializeField] [ReadOnly]
        Transform targetTr = null;
        List<Transform> tempList = null;

        [SerializeField] [ReadOnly]
        bool isDebug = false;

        #region AUTO_STATE

        void AutoModeUpdate() {
            //
            //1. Get Monster List
            //2. Find a Nearest Mosnter
            //3. Add Target GameObject to Monster
            //~~ Target Monster Alive Check Update
            //4. Auto Rotate to Monster Position
            //5. Bow Pulling and Shot with Interval
            //~~ Lose Target Monster (Death)
            //6. Restart Find New Monster Target
            // again ~~

            //~caution
            //swap arrow
            //special arrow
            //boolean isBowPullingStop

            switch (autoState) {
                case AUTOSTATE.WAIT: AutoWait(); break;
                case AUTOSTATE.FIND: AutoFind(); break;
                case AUTOSTATE.TRAC: AutoTrac(); break;
                case AUTOSTATE.SHOT: AutoShot(); break;
                default: throw new System.NotImplementedException("this AutoState is Not Implemented.");
            }
        }

        void AutoWait() {
            AutoExiter(); //AutoMode Exit Checker

            //Start Auto Mode
            if (isAutoExitWait == false && GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                autoState = AUTOSTATE.FIND;
            }
        }

        void AutoFind() {
            AutoExiter(); //AutoMode Exit Checker
            
            currentFindTime += Time.deltaTime;
            if(currentFindTime > targetFindInterval) {
                if(TryFindTarget(out targetTr)) {
                    //Target Finded 
                    if (isDebug) CatLog.Log("Target Found");
                    target = targetTr.GetComponent<IDamageable>();
                    autoState = AUTOSTATE.TRAC;
                }
                else {
                    //Target Not Found
                    if (isDebug) CatLog.Log("Target Not Found");
                    currentFindTime = 0f;
                }
            }
        }

        void AutoTrac() {
            AutoExiter(); //AutoMode Exit Checker

            //Target Monster Disable Check
            if (target.IsAlive() == false || targetTr == null) {
                currentFindTime = 0f; // <- Reset Timer if Not need a reset, delete this;
                autoState = AUTOSTATE.FIND;
            }

            //auto rotate to Monster Position
            direction = targetTr.position - bowTr.position;
            bowAngle  = Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + angleOffset, Time.deltaTime * SmoothRotateSpeed);
            tempEulerAngle    = bowTr.eulerAngles;
            tempEulerAngle.z  = bowAngle;
            bowTr.eulerAngles = tempEulerAngle;

            //auto Pulling the Bow

            //Set Shot State When Bow Ready
            bool isReady = false;   //Ready is Tracking Time Maxed
            if(isReady == true) {
                autoState = AUTOSTATE.SHOT;
            }
        }

        void AutoShot() {
            //Shot

            //Reload

            //Return Monster Tracking State
            autoState = AUTOSTATE.TRAC;
        }

        /// <summary>
        /// Clean up Auto variables
        /// </summary>
        void AutoExiter() {
            if(isAutoExitWait == true) {
                target   = null;
                targetTr = null;
                tempList = null;
                isAutoRunning = false;
            }
        }

        public void AutoStop() {
            if (isAutoRunning == false) {
                CatLog.WLog("is AutoMode already stopped.");
                return;
            }
                
            isAutoExitWait = true;
        }

        public void AutoStart() {
            if(isAutoRunning == true) {
                CatLog.WLog("is AutoMode already started.");
                return;
            }

            autoState = AUTOSTATE.WAIT;
            isAutoExitWait = false; 
            isAutoRunning  = true;
        }

        #endregion

        bool TryFindTarget(out Transform tr) {
            //Get Enabled in Scene Monster List
            CCPooler.OutEnableMonsters(out tempList);
            if(tempList.Count <= 0) {
                tr = null; return false;
            }

            //Enable Monsters Distance Between this position
            tempList.RemoveAll(element => Vector2.Distance(element.position, bowTr.position) > FindDist);
            if (tempList.Count <= 0) {
                tr = null; return false;
            }
            else if (tempList.Count == 1) {
                tr = tempList[0]; return true;
            }

            //Find a nearest Target
            Transform targetTr   = null;
            float closestDistSqr = Mathf.Infinity;
            for (int i = 0; i < tempList.Count; i++) {
                //if (tempList[i] == null) continue;  //Check Null -> need test.
                Vector2 directionToTarget = tempList[i].position - bowTr.position;
                float distSqr = directionToTarget.sqrMagnitude;
                if(distSqr < closestDistSqr) {
                    closestDistSqr = distSqr;
                    targetTr       = tempList[i];
                }
            }

            tr = targetTr;
            return true;
        }
    }
}
