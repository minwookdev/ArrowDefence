namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;
    using System.Collections.Generic;

    public partial class AD_BowController {
        [Header("AUTOMODE")]
        [SerializeField] [ReadOnly] //Enums
        AUTOSTATE autoState = AUTOSTATE.WAIT;

        //Fields
        float findInterval    = 0.8f;
        float currentFindTime = 0f;
        float autoShotTime    = 1.5f;
        float currShotTime    = 0f;
        float FindDist        = 10f;
        float angleOffset     = 90f;
        bool isAutoRunning  = false;
        bool isAutoExitWait = false;

        //Temp
        IDamageable target = null;
        [SerializeField] [ReadOnly]
        Transform targetTr = null;
        List<Transform> tempList = null;

        [SerializeField] [ReadOnly]
        bool isDebug = false;

        #region SWITCH

        void AutoStop() {
            if (isAutoRunning == false) {
                CatLog.WLog("is AutoMode already stopped.");
                return;
            }

            isAutoExitWait = true;
        }

        void AutoStart() {
            if (isAutoRunning == true) {
                CatLog.WLog("is AutoMode already started.");
                return;
            }

            autoState = AUTOSTATE.WAIT;
            isAutoExitWait = false;
            isAutoRunning = true;
        }

        public void AutoSwitch(bool isDebug = false) {
            if (isAutoRunning == false) {
                //is starting auto mode
                autoState = AUTOSTATE.WAIT; //set wait automode start state.
                isAutoExitWait = false;
                isAutoRunning  = true;

                if (isDebug) CatLog.Log(StringColor.BLUE, "Start AutoMode !");
            }
            else {
                //is stoping auto mode
                isAutoExitWait = true;

                if (isDebug) CatLog.Log(StringColor.YELLOW, "Stop AutoMode !");
            }
        }

        #endregion


        #region AUTO_STATE

        void AutoModeUpdate() {
            //1. Get Monster List
            //2. Find a Nearest Mosnter
            //3. Add Target GameObject to Monster
            //~~ Target Monster Alive Check Update
            //4. Auto Rotate to Monster Position
            //5. Bow Pulling and Shot with Interval
            //~~ Lose Target Monster (Death)
            //6. Restart Find New Monster Target
            // again ~~

            //Implement Shot and Reload

            //~caution
            //swap arrow
            //special arrow ()
            //boolean isBowPullingStop (clear)

            switch (autoState) {
                case AUTOSTATE.WAIT: AutoWait(); break;
                case AUTOSTATE.FIND: AutoFind(); break;
                case AUTOSTATE.TRAC: AutoTrac(); break;
                case AUTOSTATE.SHOT: AutoShot(); break;
                default: throw new System.NotImplementedException("this AutoState is Not Implemented.");
            }
        }

        void AutoWait() {
            AutoExiter(); //AutoMode Stop Checker

            //Start Auto Mode
            if (isAutoExitWait == false && GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                autoState = AUTOSTATE.FIND;
            }
        }

        void AutoFind() {
            AutoExiter(); //AutoMode Stop Checker

            currentFindTime += Time.deltaTime;
            if(currentFindTime > findInterval) {
                if(TryFindTarget(out targetTr)) {
                    //Target Finded <- Not include currentFindTimer zero;
                    if (isDebug) CatLog.Log("Target Found");
                    target = targetTr.GetComponent<IDamageable>();
                    EnterStateTrac();
                }
                else {
                    //Target Not Found
                    if (isDebug) CatLog.Log("Target Not Found");
                    currentFindTime = 0f;
                }
            }

            //Implement Rotate the Bow [visualize]
        }

        void AutoTrac() {
            //==========================================<< TARGET CHECK & EXIT CHECK >>=========================================
            AutoExiter(); //AutoMode Stop Checker

            //Target Monster Disable Check
            if (targetTr == null || target.IsAlive() == false) {
                currentFindTime = 0f; // <- Reset Timer if Not need a reset, delete this;
                autoState = AUTOSTATE.FIND;
            }
            //==================================================================================================================

            //=================================================<< RUNNING TIMER >>==============================================
            //increase monster tracking timer..if timer is maxed change state is shot
            float autoShotTimeMultiplier = 1.0f; // -> Change value Global Ablility
            currShotTime += Time.deltaTime * autoShotTimeMultiplier;
            //==================================================================================================================

            //=====================================================<< ROTATE >>=================================================
            //auto rotate to Monster Position
            direction = targetTr.position - bowTr.position;
            bowAngle  = Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg - angleOffset, Time.deltaTime * SmoothRotateSpeed);
            tempEulerAngle    = bowTr.eulerAngles;
            tempEulerAngle.z  = bowAngle;
            bowTr.eulerAngles = tempEulerAngle;
            //==================================================================================================================

            //====================================================<< PULLING >>=================================================
            //auto Pulling the Bow
            if(arrowTr != null) {
                arrowPos = arrowTr.position;
                arrowPos = Vector3.MoveTowards(arrowPos, ClampPointBottom.position, Time.deltaTime * ArrowPullingSpeed);
                arrowTr.position = arrowPos;

                // < arrow force calculating is Not need >
                arrowForce = arrowTr.up * arrowComponent.ArrowPower;

                //AutoMode Not using Charged Power
            }
            //==================================================================================================================

            //CatLog.Log($"Angle 1 : {GameGlobal.AngleBetweenVec3(bowTr.position, targetTr.position)}");
            CatLog.Log(StringColor.YELLOW, $"Angle 2 : {GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position) - bowTr.eulerAngles.z}"); //

            //=====================================================<< SHOT >>===================================================
            if(currShotTime > autoShotTime) {
                //발사 조건 체크 : (몬스터의 위치와 활 위치간의 각도) - 현재 활 각도 : 몬스터 위치 각도와 현재 활이 조준하고있는 각도의 차이
                float angle = GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position) - bowTr.eulerAngles.z;
                float range = 3f; //-> Change Global Variables

                //bool isAngleFrontMonster = (angle >= -range && angle <= range); // -3 ~ 3
                bool isAngleFrontMonster = GameGlobal.IsRange(angle, range);      // -3 ~ 3
                bool isMaxPullingArrow   = (Vector2.Distance(arrowPos, ClampPointBottom.position) < 0.5f);

                if(isAngleFrontMonster == true && isMaxPullingArrow == true) {
                    //Shot Arrow
                    CatLog.Log(StringColor.GREEN, "Is Ready to Shot !");
                    autoState = AUTOSTATE.SHOT;
                }
                else {
                    //Reset Timer
                    currShotTime = 0f;
                }
            //==================================================================================================================
            }
        }

        void AutoShot() {
            //=================================================<< SHOT ARROW >>=================================================
            AD_BowRope.instance.CatchPointClear(); //Release Bow Rope

            //Get New Damage Struct [AutoMode Damage Struct is always non-charged]
            damageStruct = bowAbility.GetDamage(arrowType, false);

            //Shot Arrow and active skills
            arrowComponent.ShotByBow(arrowForce, ArrowParentTr, damageStruct);
            BowSkillSet?.Invoke(bowTr, this, ref damageStruct, arrowComponent.CatchTr.position, arrowType);

            //Release Arrow Component and Transform
            arrowTr = null; arrowComponent = null;

            //active effect
            bowSprite.Effect(BOWEFFECTYPE.IMPACT);

            //active camera shake
            CineCam.Inst.ShakeCamera(5f, .1f);
            //================================================<< RELOAD ARROW >>================================================
            switch (arrowType) {    //Reload Arrow by current equipped Arrow Type.
                case ARROWTYPE.ARROW_MAIN:    arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_SUB:     arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity);  break;
                case ARROWTYPE.ARROW_SPECIAL: throw new System.NotImplementedException("this arrow Type is NotImplemented.");
            }

            //Get Arrow Componenet with init Clamp Points.
            arrowComponent = arrowTr.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop, initArrowRot);
            //==================================================<< SET STATE >>=================================================
            EnterStateTrac(); //Return Tracking State
            //==================================================================================================================
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

            // 
            //Auto Exiter 수정 : 어떤 스테이트 에서도 Exiter가 돌아가서 exitWait를 받지만
            //샷 동작에서 바로 바뀌지는 않고, 끝낼 준비가 된 상황까지 대기하고 마무리 처리 다 된 후에 autoState 변경처리 되도록 변경
            //이미 그렇게 되고있네..
        }

        void EnterStateTrac() {
            if(arrowComponent == null || arrowTr == null) {
                CatLog.WLog("Arrow Component is Null, Can't Change Tracking State.");
                return;
            }

            currShotTime = 0f;
            AD_BowRope.instance.SetCatchPoint(arrowComponent.CatchTr);
            autoState = AUTOSTATE.TRAC;
        }

        System.Collections.IEnumerator AutoExitWait() {
            yield return null;
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
