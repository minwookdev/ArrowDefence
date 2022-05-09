namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;
    using System.Collections.Generic;

    public partial class AD_BowController {
        [Header("AUTOMODE")]
        [SerializeField] [ReadOnly] //Enums
        AUTOSTATE autoState = AUTOSTATE.NONE;

        //Fields
        float findInterval  = 0.8f;
        float currFindTime  = 0f;
        float autoShotTime  = 1.5f;
        float currShotTime  = 0f;
        float FindDist      = 15f;
        float angleOffset   = 90f;
        float autoTimeMulti = 1.0f;
        bool isAutoRunning  = false;
        bool isAutoExitWait = false;

        //Temp
        [SerializeField] [ReadOnly]
        Transform targetTr = null;
        IDamageable target = null;
        List<Transform> tempList = null;

        //Coroutines
        Coroutine arrSwapCo   = null;
        WaitUntil arrSwapWait = null;

        [SerializeField] [ReadOnly]
        bool isDebug = false;

        public float CompareAngle {
            get {
                return (bowTr.eulerAngles.z > 180f) ? bowTr.eulerAngles.z - 360f : bowTr.eulerAngles.z;
            }
        }

        #region SWITCH

        public void AutoSwitch(bool isDebug = false) {
            if (GameManager.Instance.IsGameStateEnd) {
                CatLog.WLog("this gameState Not Enable AutoMode.");
                return;
            }

            if (isAutoRunning == false) {
                //is starting auto mode
                AutoStateChange(AUTOSTATE.WAIT);
                isAutoExitWait = false;
                isAutoRunning  = true;

                if (isDebug) {
                    CatLog.Log(StringColor.GREEN, "Start AutoMode !");
                }
            }
            else {
                //is stoping auto mode
                isAutoExitWait = true;

                if (isDebug) {
                    CatLog.Log(StringColor.YELLOW, "Stop AutoMode !");
                }
            }
        }

        void OnAutoModeStop() {
            if (!isAutoRunning) {
                return;
            }
            isAutoExitWait = true;
        }

        #endregion

        #region AUTO_MACHINE

        void AutoModeUpdate() {
            //if gamestate is paused, stop update automode
            //Clear, GameOver State에서 AutoMode를 None State로 바꿔주지 않기 때문에 주석처리 해둠
            //if (IsPullingStop == true) { 
            //    return;
            //}
            switch (autoState) {
                case AUTOSTATE.NONE: StateNone(STATEFLOW.UPDATE); break;
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.UPDATE); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.UPDATE); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.UPDATE); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.UPDATE); break;
                default: throw new System.NotImplementedException("this AutoState is Not Implemented.");
            }
        }

        void AutoStateChange(AUTOSTATE changeState) {
            switch (autoState) {
                case AUTOSTATE.NONE: StateNone(STATEFLOW.EXIT); break;
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.EXIT); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.EXIT); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.EXIT); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.EXIT); break;
                default: throw new System.NotImplementedException();
            }

            //State Change
            autoState = changeState;

            switch (autoState) {
                case AUTOSTATE.NONE: StateNone(STATEFLOW.ENTER); break;
                case AUTOSTATE.WAIT: StateWait(STATEFLOW.ENTER); break;
                case AUTOSTATE.FIND: StateFind(STATEFLOW.ENTER); break;
                case AUTOSTATE.TRAC: StateTrac(STATEFLOW.ENTER); break;
                case AUTOSTATE.SHOT: StateShot(STATEFLOW.ENTER); break;
                default: throw new System.NotImplementedException();
            }
        }

        void StateNone(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  NoneEnter(); break; //-> is AutoMode Update Exit.
                case STATEFLOW.UPDATE:              break;
                case STATEFLOW.EXIT:                break;
            }
        }

        void StateWait(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:                break;
                case STATEFLOW.UPDATE: WaitUpdate(); break;
                case STATEFLOW.EXIT:                 break;
            }
        }

        void StateFind(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  FindEnter();  break;
                case STATEFLOW.UPDATE: FindUpdate(); break;
                case STATEFLOW.EXIT:   FindExit();   break;
            }
        }

        void StateTrac(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  TracEnter();  break;
                case STATEFLOW.UPDATE: TracUpdate(); break;
                case STATEFLOW.EXIT:   TrackExit();  break;
            }
        }

        void StateShot(STATEFLOW flow) {
            switch (flow) {
                case STATEFLOW.ENTER:  ShotEnter(); break;
                case STATEFLOW.UPDATE:              break;
                case STATEFLOW.EXIT:                break;
            }
        }

        #endregion

        #region STATE

        //################################################################################################################################################
        //############################################################### << STATE NONE >> ###############################################################
        void NoneEnter() {
            //clean up AutoMode variables
            target   = null;
            targetTr = null;
            tempList = null;

            if(isBowPulling == true) {
                isBowPulling = false;
            }

            //Restore Arrow position and Clear Rope
            arrowTr.position = ClampPointTop.position;
            AD_BowRope.instance.CatchPointClear();

            //clear exitwait
            isAutoExitWait = false;

            //Stop AutoMode State Update
            isAutoRunning = false;
            //CatLog.Break();
        }
        //################################################################################################################################################
        //############################################################### << STATE WAIT >> ###############################################################
        void WaitUpdate() {
            //AutoExiter(); // <- Wait State is Exitable AutoMode
            if (isAutoExitWait == true) {
                AutoStateChange(AUTOSTATE.NONE);
                return;
            }

            //Start Auto Mode
            if (isAutoExitWait == false && GameManager.Instance.GameState == GAMESTATE.STATE_INBATTLE) {
                AutoStateChange(AUTOSTATE.FIND);
            }
        }
        //################################################################################################################################################
        //############################################################### << STATE FIND >> ###############################################################
        void FindEnter() {
            //Restore Arrow position and claer Rope
            arrowTr.position = ClampPointTop.position;
        }

        void FindUpdate() {
            //AutoExiter(); // <- Find State is Exitable AutoMode
            if (isAutoExitWait == true) {
                AutoStateChange(AUTOSTATE.NONE);
                return;
            }

            currFindTime += Time.unscaledDeltaTime;
            if(currFindTime > findInterval) {
                if(TryFindTarget(out targetTr)) {
                    //Target Finded <- Not include currentFindTimer zero;
                    if (isDebug) CatLog.Log("Target Found");
                    target = targetTr.GetComponent<IDamageable>();
                    //EnterStateTrac();
                    AutoStateChange(AUTOSTATE.TRAC); //State Change - Tracking Target
                }
                else {
                    //Target Not Found
                    if (isDebug) CatLog.Log("Target Not Found");
                    currFindTime = 0f;
                }
            }

            //Implement Rotate the Bow [visualize]
        }

        void FindExit() {
            //clear current find Timer
            //currFindTime = 0f;
        }
        //################################################################################################################################################
        //############################################################### << STATE TRAC >> ###############################################################
        void TracEnter() {
            if(arrowTr == null || arrowComponent == null) {
                CatLog.ELog("Arrow Component is Null, Stop AutoMode !", true);
            }

            AD_BowRope.instance.SetCatchPoint(arrowComponent.CatchTr);
            isBowPulling = true;    //Pulling State isOn, used accessory
        }

        void TracUpdate() {
            //========================================== << TARGET CHECK & EXIT CHECK >> =========================================
            if (isAutoExitWait == true) {
                AutoStateChange(AUTOSTATE.NONE);
                return;
            }
            //==================================================== << PULLING >> =================================================
            if (arrowTr != null) { //auto Pulling the Bow 
                arrowPos = arrowTr.position;
                arrowPos = Vector3.MoveTowards(arrowPos, ClampPointBottom.position, Time.unscaledDeltaTime * ArrowPullingSpeed);
                arrowTr.position = arrowPos;

                // < arrow force calculating is Not need >
                arrowForce = arrowTr.up * arrowComponent.PowerFactor;

                //AutoMode Not using Charged Power

                //Increase Tracking Time [Only Arrow Loaded (R:ArrowReload)]
                currShotTime += Time.unscaledDeltaTime * autoTimeMulti;
            }
            //==================================================================================================================

//#################################################################### << TARGET NOT FOUND >> ##################################################################
            if (targetTr == null || target.IsAlive() == false) { //Target Monster Disable Check
                //EnterStateFind();
                AutoStateChange(AUTOSTATE.FIND);
            }
            else {
//###################################################################### << TARGET FOUND >> ####################################################################
            //=====================================================<< ROTATE >>=================================================
                //auto rotate to Monster Position
                direction = targetTr.position - bowTr.position;
                bowAngle = Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg - angleOffset, Time.unscaledDeltaTime * SmoothRotateSpeed);
                tempEulerAngle = bowTr.eulerAngles;
                tempEulerAngle.z = bowAngle;
                bowTr.eulerAngles = tempEulerAngle;
            //=====================================================<< SHOT >>===================================================
                if (currShotTime > autoShotTime) {
                    //발사 조건 체크 : (몬스터의 위치와 활 위치간의 각도) - 현재 활 각도 : 몬스터 위치 각도와 현재 활이 조준하고있는 각도의 차이
                    //float angle = GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position) - bowTr.eulerAngles.z; //여기에 이거를 bowTr.rotation.z 로 치환하면 어떨까??
                    float conditionAngle = GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position) - CompareAngle;
                    float accuracy = 3f; //-> Change Global Variables, Accuracy 값이 낮아질 수록 더욱 정확하게 잡음 [1최소] [2~3권장]

                    //bool isAngleFrontMonster = (angle >= -range && angle <= range);        // -3 ~ 3
                    bool isAngleFrontMonster = GameGlobal.IsRange(conditionAngle, accuracy); // -3 ~ 3
                    bool isMaxPullingArrow   = (Vector2.Distance(arrowPos, ClampPointBottom.position) < 0.5f);

                    // Debuggning
                    //CatLog.Log($"IsAngleFrontMonster: {isAngleFrontMonster}, IsMaxPullingArrow: {isMaxPullingArrow}");
                    //CatLog.Log($"target is Alive: {target.IsAlive()}");
                    //CatLog.Log($"Angle Between Vector2: {GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position)}, Bow Transform EulerAngles Z: {bowTr.eulerAngles.z}, Calc Angle: {angle}");
                    //float bowEulerAnglesZ = (bowTr.eulerAngles.z > 180f) ? (bowTr.eulerAngles.z - 360f) : bowTr.eulerAngles.z;
                    //CatLog.Log($"Between Angle: {GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position)}, Bow Angle: {CompareAngle}, Calculate Angle: {conditionAngle}");

                    // 개선안
                    // 여기 안에서 쓰이는 로컬변수들 멤버변수로 따로 관리하면 isAngleFrontMonster, isMaxPullingArrow 조건에 맞지않았을때, currentShotTime을 초기화 시켜주지 않아도
                    // 부담없이 기다릴 수 있도록 바꿔줄 수 있겠다. -> 의미없이 한번 더 돌아야하는 경우 최소화 할 수 있다.

                    if (isAngleFrontMonster == true && isMaxPullingArrow == true) {
                        //Shot Arrow
                        //CatLog.Log(StringColor.GREEN, "Is Ready to Shot !");
                        AutoStateChange(AUTOSTATE.SHOT);
                    }
                    else {
                        //Reset Timer
                        currShotTime = 0f;
                    }
                }
            //====================================================<< DEBUG >>===================================================
                //CatLog.Log(StringColor.YELLOW, $"Angle Debug : {GameGlobal.AngleBetweenVec2(bowTr.position, targetTr.position) - bowTr.eulerAngles.z}"); 
            //==================================================================================================================
//##########################################################################################################################################################
            }
        }

        void TrackExit() {
            currShotTime = 0f;
            isBowPulling = false;
            //CatLog.Log(StringColor.YELLOW, "Exit Tracking State !");
        }
        //################################################################################################################################################
        //############################################################### << STATE SHOT >> ###############################################################
        void ShotEnter() {
            //=================================================<< SHOT ARROW >>=================================================
            AD_BowRope.instance.CatchPointClear(); //Release Bow Rope

            //Get New Damage Struct [AutoMode Damage Struct is always non-charged]
            damageStruct = ability.GetDamage(arrowType, false);

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
            arrowType = (arrowType == ARROWTYPE.ARROW_SPECIAL) ? previousType : arrowType;
            switch (arrowType) {    //Reload Arrow by current equipped Arrow Type.
                case ARROWTYPE.ARROW_MAIN: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_SUB:  arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity);  break;
                default: throw new System.NotImplementedException();
            }

            //Get Arrow Componenet with init Clamp Points.
            arrowComponent = arrowTr.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop, initArrowRot);
            //==================================================<< SET STATE >>=================================================
            //EnterStateTrac(); //Return Tracking State
            AutoStateChange(AUTOSTATE.TRAC);
            //==================================================================================================================
        }
        //################################################################################################################################################
        //################################################################################################################################################

        #endregion

        #region ARROW_SWAP

        void AutoModeArrSwap(ARROWTYPE type) {
            if(arrSwapCo != null) {
                StopCoroutine(arrSwapCo);
            }
            arrSwapCo = StartCoroutine(AutoModeArrSwapCo(type));
        }

        System.Collections.IEnumerator AutoModeArrSwapCo(ARROWTYPE type) {
            yield return arrSwapWait; //Wait Swapable State (WAIT) || (FIND) || (TRAC)

            AD_BowRope.instance.CatchPointClear();
            if (arrowComponent != null)
                arrowComponent.DisableRequest();
            arrowTr = null; arrowComponent = null;
            if (type == ARROWTYPE.ARROW_SPECIAL) {
                previousType = arrowType;
            }
            arrowType = type;

            switch (arrowType) {
                case ARROWTYPE.ARROW_MAIN:    arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity);     break;
                case ARROWTYPE.ARROW_SUB:     arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity);      break;
                case ARROWTYPE.ARROW_SPECIAL: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SPECIAL_ARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                default: throw new System.NotImplementedException();
            }

            arrowComponent = arrowTr.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop, initArrowRot);
        }

        #endregion

        bool TryFindTarget(out Transform tr) {
            //Get Enabled in Scene Monster List
            CCPooler.OutEnableMonsters(out tempList);
            if(tempList.Count <= 0) {
                tr = null; return false;
            }

            //Remove Target List
            tempList.RemoveAll(element => Vector2.Distance(element.position, bowTr.position) > FindDist || element.GetComponent<IDamageable>().IsAlive() == false);
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
