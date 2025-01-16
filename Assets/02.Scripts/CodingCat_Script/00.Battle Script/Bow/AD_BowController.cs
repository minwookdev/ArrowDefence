namespace ActionCat
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using ActionCat.Audio;
    using Unity.Collections.LowLevel.Unsafe;
    using EasyMobile;

    public partial class AD_BowController : MonoBehaviour
    {
        public static AD_BowController instance;

        [Header("COMPONENT")]
        [SerializeField] BowSprite bowEffect;
        [SerializeField] AD_BowAbility ability;
        [SerializeField] Transform bowTr = null;
        private Touch tempTouch;
        private Touch currentTouch;
        public Camera MainCam = null;
        [SerializeField] ControllerSound bowSound = null;

        [Header("CONTROLLER")]
        public float TouchRadius = 1f;
        public Image BowCenterPointImg;
        public Transform ClampPointTop, ClampPointBottom;
        [Range(1f, 20f)] public float SmoothRotateSpeed = 12f;
        [Range(1f, 3f)] public float ArrowPullingSpeed = 1f;

        private int touchIndex = 0;
        private bool isBowDragging = false;     //활이 일정거리 이상 당겨져서 회전할 수 있는 상태
        private bool isBowPullBegan = false;     //Bow Pull State Variables
        private bool isTouched = false;
        private bool isChargeShotReady = false;
        private bool isPlayedDrawSound = false;
        private float maxBowAngle, minBowAngle;  //Min, Max BowAngle Variables
        private float bowAngle;                  //The Angle Variable (angle between Click point and Bow).
        private float maxChargingTime = 0f;
        private float chargingTime = 0f;
        private Vector3 initialTouchPos;         //처음 터치한 곳을 저장할 벡터
        private Vector3 direction;
        private Vector3 currentClickPosition;
        private Vector3 tempEulerAngle;
        //private AutoMachine autoMachine = null;
        public bool IsStopManualControl { get; set; }  //Pulling Stop boolean For Pause, Clear Battle Scene

        [Header("ARROW VARIABLES")] //Arrow Relation Variables
        [SerializeField][ReadOnly] Transform ArrowParentTr;
        [SerializeField][ReadOnly] Transform arrowTr = null;
        [SerializeField][ReadOnly] GameObject loadedArrow; // <- Not Used Now
        [SerializeField][ReadOnly] AD_Arrow arrowComponent;

        private Vector3 arrowPos;
        private Vector2 arrowForce;
        private Vector3 initArrowScale = new Vector3(1.5f, 1.5f, 1f);
        private Vector3 initArrowRot = new Vector3(0f, 0f, -90f);

        [Header("SLOTS")]
        [SerializeField] private Transform mainSlotTr = null;
        #region SLOTS
        public Transform GetSlotTrOrNull
        {
            get
            {
                if (mainSlotTr != null)
                {
                    if (mainSlotTr.childCount == 0)
                    {
                        return mainSlotTr;
                    }
                }
                else
                {
                    CatLog.WLog("main slot transform is null.");
                }

                CatLog.Log("no slot transforms available.");
                return null;
            }
        }
        public Transform effectTr
        {
            get
            {
                if (mainSlotTr == null)
                {
                    throw new System.Exception("MainSlot Transform is Null");
                }

                return mainSlotTr;
            }
        }
        #endregion

        //Enums
        private ARROWTYPE currentArrowType;
        private ARROWTYPE previousType;
        private PULLINGTYPE currentPullType;
        public PULLINGTYPE ControlType
        {
            get
            {
                return currentPullType;
            }
        }

        //Struct
        private DamageStruct damageStruct;

        //Delegate
        public delegate void BowSkillsDel(Transform bowTr, AD_BowController controller, ref DamageStruct damage, Vector3 arrowPos, ARROWTYPE type);
        private BowSkillsDel BowSkillSet;

        #region NOT_USED
        //The Distance between first click and Bowlope
        //private float radius;
        //The Point on the Circumference based on radius and angle.
        //private Vector3 cPoint = Vector3.zero;
        //private Vector2 distance;
        //private bool isPullingStartPosSet = false;    //When pulling is started, it is used to forcibly hold the position.
        //Enabled later, when using with -> PullingStartPosition();
        #endregion

        public bool IsStatePulling() => isBowDragging;

        private void Awake() => instance = this;

        private IEnumerator Start()
        {
            //Init Controller Transform
            if (bowTr == null)
            {
                bowTr = gameObject.GetComponent<Transform>();
            }

            if (ability == null)
            {
                ability = GetComponent<AD_BowAbility>();
            }

            //Initialize Main Camera Object
            MainCam = Camera.main;
            currentPullType = GameManager.Instance.GetPlayerPullType();

            //Initialize variables For Arrow to be Loaded
            if (ClampPointTop == null || ClampPointBottom == null)
            {
                CatLog.ELog("Bow Controller : Clamp Point is Not Cached.", true);
            }

            //Init-parent Canvas
            ArrowParentTr = bowTr.parent;

            //Initialize Bow Center Pivot Image Object
            if (BowCenterPointImg)
                BowCenterPointImg.transform.position = bowTr.position;

            //Init Start Bow Angle : Start 이후 처음 조준할 때 Bounce 현상 방지
            bowAngle = bowTr.eulerAngles.z;
            minBowAngle = 0f; maxBowAngle = 180f;

            yield return new WaitUntil(() => ability.IsInitEquipments);

            //Load Arr, Get Fisrt Loaded Arrow Type
            Reload(GameManager.Instance.GetFirstArrType());

            //============================================================== << CALLBACK GAMEOVER >> ==============================================================
            //게임오버 이벤트에 Burn Effect 추가. Resurrection 구현 시 추가적인 로직 구현 필요.
            GameManager.Instance.AddListnerGameOver(() =>
            {
                //stop auto-mode if the Running, Stop Touch State
                OnAutoModeStop();
                IsStopManualControl = true;   //Bow Pulling Stop
                //Active Burn Fade Effect.
                bowEffect.Effect(BOWEFFECTYPE.FADE, false);
                //Bow Rope Material Alpha Change.
                AD_BowRope.instance.RopeAlpha(false);
                //if Loaded Arrow, Change Sprite color Alpha.
                if (arrowComponent != null)
                {
                    arrowComponent.SpriteAlpha(false);
                }
            });
            //================================================================ << CALLBACK CLEAR >> ===============================================================
            GameManager.Instance.AddListnerEndBattle(() =>
            {
                OnAutoModeStop();
                IsStopManualControl = true;
            });
            //================================================================ << CALLBACK PAUSE >> ===============================================================
            GameManager.Instance.AddListnerPause(() =>
            {
                IsStopManualControl = true;
                if (isAutoRunning)
                {
                    if (autoState == AUTOSTATE.FIND || autoState == AUTOSTATE.TRAC || autoState == AUTOSTATE.SHOT)
                    {
                        AutoStateChange(AUTOSTATE.WAIT);
                    }
                }
            });
            //=====================================================================================================================================================

            //Init-Bow Skill and Current Slot Damage Struct.
            CatLog.Log(StringColor.YELLOW, $"Damage Struct SizeOf : {System.Runtime.InteropServices.Marshal.SizeOf(typeof(DamageStruct))}");
            CatLog.Log(StringColor.YELLOW, $"Damage Struct SizeOf : {System.Runtime.InteropServices.Marshal.SizeOf(damageStruct)}");
            ability.AddListnerToSkillDel(ref BowSkillSet, this.bowSound.AudioSource);

            //Get Max Charging Timeif(
            maxChargingTime = GameGlobal.CHARGINGTIME;

            //AutoMode Variables Init
            arrSwapWait = new WaitUntil(() => autoState == AUTOSTATE.WAIT || autoState == AUTOSTATE.FIND || autoState == AUTOSTATE.TRAC);

            //Hiding Warning Log ---
            if (touchIndex == 0 || isTouched == false)
            {

            }
        }

        private void Update()
        {
            //===============================================<< AUTO MODE UPDATE >>==============================================
            if (isAutoRunning)
            { //Enabled AutoMode : AutoMode Update
                AutoModeUpdate();
                return;
            }
            //===================================================================================================================

            //==============================================<< MANUAL MODE UPDATE >>=============================================
#if UNITY_EDITOR
            //==============================================<< EDITOR CONTROLLER >>==============================================
            if (Input.GetMouseButtonDown(0))
            {    //Click Began
                if (EventSystem.current.IsPointerOverGameObject() == false)
                { // if the mouse Click UI GameObject
                    BowBegan(Input.mousePosition);
                }
                else
                { //Block click Peedback
                    CatLog.Log("Click Blocked.");
                }
            }
            else if (Input.GetMouseButtonUp(0))
            { //Click Ended
                BowReleased(Input.mousePosition);
            }
            if (isBowPullBegan == true)
            {         //Click Moved
                Dragging(Input.mousePosition);
            }
#elif UNITY_ANDROID
            //==============================================<< MOBILE CONTROLLER >>==============================================
            // Touch Update : Only Mobile
            //if (!isTouched) {
            //    //if (Input.touchCount > 0 && GameManager.Instance.IsBattleState) {
            //    //    for (int i = 0; i < Input.touchCount; i++) {
            //    //        if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId) == false) {
            //    //            touchIndex = i;    // Finded Correct Touch Index
            //    //            isTouched  = true;  
            //    //            this.BowBegan(Input.GetTouch(touchIndex).position); //Touch ID를 찾음과 동시에 Bow Pull Bega 들어감
            //    //            break;
            //    //        }
            //    //    }
            //    //}
            //
            //    // UI를 터치하지 않은 ID값 찾기
            //    //for (int i = 0; i < Input.touchCount; i++) {
            //    //    currentTouch = Input.GetTouch(i);
            //    //    if (EventSystem.current.IsPointerOverGameObject(currentTouch.fingerId) == false) { // i번 터치가 UI와 겹치치 않은 경우
            //    //        touchIndex = i;
            //    //        this.BowBegan(currentTouch.position); // 올바른 ID를 찾음과 동시에 Draw Began 진입
            //    //        break;                                // 올바른 값을 찾은 경우에 루프 탈출
            //    //    }
            //    //}
            //
            //    // GetTouch를 0번으로 제한하는 경우
            //    if (Input.touchCount > 0) {
            //        currentTouch = Input.GetTouch(0);                                                  // 0번으로 제한
            //        if (EventSystem.current.IsPointerOverGameObject(currentTouch.fingerId) == false) { // 0번 터치가 UI와 겹치치 않은 경우
            //            this.BowBegan(currentTouch.position);   //
            //            isTouched  = true;
            //            touchIndex = 0;                         //
            //        }
            //    }
            //}
            //else {
            //    // Input.GetTouch 계속 받아오지 않으면 움직이지 않음.
            //    currentTouch = Input.GetTouch(touchIndex);
            //
            //    // Touched State
            //    //switch (currentTouch.phase) {
            //    //    case TouchPhase.Began:      this.BowBegan(currentTouch.position);                            break;
            //    //    case TouchPhase.Ended:      this.BowReleased(currentTouch.position); this.isTouched = false; break;
            //    //    case TouchPhase.Moved:      break;
            //    //    case TouchPhase.Stationary: break;
            //    //    case TouchPhase.Canceled:   break;
            //    //    default: break;
            //    //}
            //
            //    if (isBowPullBegan) {
            //        this.BowMoved(currentTouch.position);
            //    }
            //    if (currentTouch.phase == TouchPhase.Ended) {
            //        this.BowReleased(currentTouch.position);
            //        this.isTouched = false;
            //    }
            //}

            // :::::::::::::::::::::::: NEW MOBILE CONTROLLER ::::::::::::::::::::::::
            // Not Touched 
            if (!isTouched) { // Get Touch State
                if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
                    if (!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) {
                        BowBegan(Input.GetTouch(0).position);
                        isTouched = true;
                    }
                }
            }
            else { // Touching
                currentTouch = Input.GetTouch(0);

                if (isBowPullBegan) {
                    BowMoved(currentTouch.position);
                }

                if (currentTouch.phase == TouchPhase.Ended || currentTouch.phase == TouchPhase.Canceled) {
                    BowReleased(currentTouch.position);
                    isTouched = false;
                }
            }
#endif
            //================================================<< ARROW POSITION UPDATE >>============================================
            UpdateArrPos();
            //=======================================================================================================================
        }

        private void OnDestroy() => instance = null;

        private void BowBegan(Vector2 pos)
        {
            //Stop Pulling or Arrow Component is null return Pull Began.
            if (IsStopManualControl == true || arrowComponent == null) return;

            switch (currentPullType)
            {
                case PULLINGTYPE.AROUND_BOW_TOUCH: if (PullTypeTouchAround(pos) == false) return; break; //Type 0.-활 주변의 일정거리 터치 조준
                case PULLINGTYPE.FREE_TOUCH: PullTypeTouchFree(pos); break; //Type 1.-터치한 곳 기준 활 조준
            }

            //Rope Catch Point Set.
            AD_BowRope.instance.SetCatchPoint(arrowComponent.CatchTr);

            //Clear Charging Data
            ChargeClear();
            bowSound.PlaySelectedSound();
            isBowPullBegan = true;
        }

        Vector3 tempPosition = Vector3.zero;

        /// <summary>
        /// 활 시위 당김 (조작에 따른 활 오브젝트의 회전 처리)
        /// </summary>
        /// <param name="touchPosition"></param>
        private void Dragging(Vector2 touchPosition)
        {
            // 현재 터치 위치
            currentClickPosition = MainCam.ScreenToWorldPoint(touchPosition);

            // 활이 일정거리 이상 당겨졌는지 체크
            isBowDragging = (DistanceOfPoint() > 1f) ? true : false;
            if (isBowDragging)
            { // 일정 거리 이상 당겨짐
                // 터치 위치로 부터 조준 방향
                direction = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ? currentClickPosition - bowTr.position : currentClickPosition - initialTouchPos;
                ///change bow angle depending on touch or click position. *Mathf.Clamp, *Mathf.LerpAngle.
                ///bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.unscaledDeltaTime * SmoothRotateSpeed),
                ///            minBowAngle, maxBowAngle);

                float targetAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90f; // 현재 활 오브젝트가 터치 방향을 바라보는 각도 구하고 To Degree
                float lerpAngle = Mathf.LerpAngle(bowAngle, targetAngle, Time.unscaledDeltaTime * SmoothRotateSpeed);
                bowAngle = Mathf.Clamp(lerpAngle, minBowAngle, maxBowAngle); // 활이 일정 이상 각도로 회전하지 않도록 제한

                // 조준 방향으로 활 오브젝트 회전
                tempEulerAngle = bowTr.eulerAngles;
                tempEulerAngle.z = bowAngle;
                bowTr.eulerAngles = tempEulerAngle;
            }

            // 조작에 따른 LineRenderer의 업데이트와 카메라 효과
            DrawLineRendererWithCameraEffects();

            // Check the Pulling Stop Trigger is true
            CheckStopPulling();

            float DistanceOfPoint()
            {
                return (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ? Vector2.Distance(bowTr.position, currentClickPosition) : Vector2.Distance(initialTouchPos, currentClickPosition);
            }

            void DrawLineRendererWithCameraEffects()
            {
                if (isBowDragging)
                {
                    // 줌 아웃 효과 및 조준 방향으로 카메라 이동
                    CineCam.Inst.ZoomOutAsync();
                    CineCam.Inst.CamMove2Direction(direction);
                }
                else
                {
                    // 줌 인 효과 및 카메라 원위치
                    CineCam.Inst.ZoomRestoreAsync();
                    CineCam.Inst.CamPosRestore();
                }

                // 처음 터치위치 <-> 현재 터치위치 간 LineRenderer Drawing 및 거리에 따른 색상 변경 함수 호출
                tempPosition = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ? bowTr.position : initialTouchPos;
                DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, tempPosition, isBowDragging);
            }
        }

        private void BowReleased(Vector2 pos)
        {
            currentClickPosition = MainCam.ScreenToWorldPoint(pos);

            if (isBowPullBegan == true)
            {    //1. Check the Pull Began.
                if (isBowDragging == true)
                {  //2. Check the Pulling
                    Launch();               //3. if the Completely Pulled, Launched.
                    isBowDragging = false;   // Release Pulling State.
                }                           // Release Pull Began State.
                isBowPullBegan = false;
            }

            // Erase Touch Line
            DrawTouchPos.Instance.ReleaseTouchLine();
            CineCam.Inst.ZoomRestoreAsync();
            CineCam.Inst.CamPositionRestoreAsync();

            // Rope Clear
            AD_BowRope.instance.CatchPointClear();
        }

        /// <summary>
        /// 화살 발사의 처리
        /// </summary>
        private void Shot()
        {
            // 현재 장전되어있는 화살의 타입과 차징 여부에 따른 데미지 수치 업데이트
            damageStruct = ability.GetDamage(currentArrowType, isChargeShotReady);

            // 화살 발사처리 및 활의 스킬 발동
            arrowComponent.ShotByBow(arrowForce, ArrowParentTr, damageStruct);
            if (currentArrowType != ARROWTYPE.ARROW_SPECIAL) // 특수화살의 경우 활 스킬 발동 예외
                BowSkillSet?.Invoke(bowTr, this, ref damageStruct, arrowComponent.CatchTr.position, currentArrowType);

            // 장전되어있던 화살 컴포넌트 해제
            arrowTr = null;
            arrowComponent = null;

            // 각종 효과의 처리 (화살 발사 이펙트 및 카메라 쉐이크와 사운드 효과)
            bowEffect.Effect(BOWEFFECTYPE.IMPACT);
            bowEffect.EffectMuzzleFlash(mainSlotTr.position, bowTr.eulerAngles.z - angleOffset);
            CineCam.Inst.ShakeCamera(6f, 0.2f);
            bowSound.PlayReleasedSound();

            // 화살 재장전 처리
            var reloadArrowType = currentArrowType == ARROWTYPE.ARROW_SPECIAL ? previousType : currentArrowType;
            Reload(reloadArrowType);
        }

        private void Launch()
        {
            //Pull Stop while reloading Arrow.
            IsStopManualControl = true;

            //Update Damage Struct
            damageStruct = ability.GetDamage(currentArrowType, isChargeShotReady);

            //Shot Arrow & Active Skill.
            arrowComponent.ShotByBow(arrowForce, ArrowParentTr, damageStruct);
            if (currentArrowType != ARROWTYPE.ARROW_SPECIAL)
            { //active skills, only not special type arrow
                BowSkillSet?.Invoke(bowTr, this, ref damageStruct, arrowComponent.CatchTr.position, currentArrowType);
            }

            //Release GameObject and Component Arrow.
            arrowTr = null;
            arrowComponent = null;

            //Active Shot Impact Effect
            bowEffect.Effect(BOWEFFECTYPE.IMPACT);
            bowEffect.EffectMuzzleFlash(mainSlotTr.position, bowTr.eulerAngles.z - angleOffset);

            //Active Camera Shake & Release Sound Play
            CineCam.Inst.ShakeCamera(6f, .2f);
            bowSound.PlayReleasedSound();

            //Reload
            var reloadType = (currentArrowType == ARROWTYPE.ARROW_SPECIAL) ? previousType : currentArrowType;
            Reload(reloadType);

            //Release Pulling Stop State
            IsStopManualControl = false;
        }

        bool PullTypeTouchAround(Vector2 touchPos)
        {
            Vector2 currentTouchPos = MainCam.ScreenToWorldPoint(touchPos);
            float distFromBow = (currentTouchPos - (Vector2)bowTr.position).magnitude;
            return (distFromBow <= TouchRadius) ? true : false;
        }

        void PullTypeTouchFree(Vector2 touchPos)
        {
            initialTouchPos = MainCam.ScreenToWorldPoint(touchPos);
        }

        void Reload(ARROWTYPE type)
        {
            if (type == ARROWTYPE.ARROW_SPECIAL)
            {
                previousType = currentArrowType;
            }

            currentArrowType = type;
            switch (currentArrowType)
            { //Reload Arrow by Current Equipped Arrow Type.
                case ARROWTYPE.ARROW_MAIN: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_SUB: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_SPECIAL: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SPECIAL_ARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                default: throw new System.Exception("Load Arrow Type is None.");
            }

            //Get Arrow Component with init Clamp Points.
            arrowComponent = arrowTr.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop, initArrowRot);
        }

        /// <summary>
        /// 화살이 당겨진 상황에서 클리어, 일시정지 들어오면 당기고있는 상태 해제
        /// </summary>
        void CheckStopPulling()
        {
            if (IsStopManualControl == true)
            {
                if (arrowTr != null)
                {
                    arrowTr.position = ClampPointTop.position;
                }

                isTouched = false;
                isBowPullBegan = false;
                isBowDragging = false;
                AD_BowRope.instance.CatchPointClear();
                DrawTouchPos.Instance.ReleaseTouchLine();
                CineCam.Inst.CamPositionRestoreAsync();
                CineCam.Inst.ZoomRestoreAsync();
            }
        }

        void CorrectionArrPos(Vector3 touchPos)
        {
            //Before use, the 'isCorrectionArrPos' declaration is required.
            bool isCorrectionArrPos = true;
            if (isCorrectionArrPos == false)
            {
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) direction = touchPos - bowTr.position;
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH) direction = touchPos - initialTouchPos;

                bowAngle = Mathf.Clamp(Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, minBowAngle, maxBowAngle);

                tempEulerAngle = bowTr.eulerAngles;
                tempEulerAngle.z = bowAngle;
                bowTr.eulerAngles = tempEulerAngle;

                isCorrectionArrPos = true;
            }
        }

        void UpdateArrPos()
        {
            if (arrowTr != null)
            {
                if (isBowDragging)
                {
                    //Arrow Direction * Force
                    arrowForce = arrowTr.up * arrowComponent.PowerFactor;

                    if (currentArrowType != ARROWTYPE.ARROW_SPECIAL)
                    {
                        arrowPos = arrowTr.position;
                        arrowPos = Vector3.MoveTowards(arrowPos, ClampPointBottom.position, Time.unscaledDeltaTime * ArrowPullingSpeed);
                        arrowTr.position = arrowPos;
                    }
                    else
                    {
                        arrowComponent.Pull(ArrowPullingSpeed);
                    }

                    //Increase Charged Power
                    ChargeIncrease();

                    //Draw Sound Check
                    DrawSoundCheck();
                }
                else
                {
                    if (currentArrowType != ARROWTYPE.ARROW_SPECIAL)
                    {
                        arrowTr.position = ClampPointTop.position;
                    }
                    else
                    {
                        arrowComponent.Move(ClampPointTop.position);
                    }

                    //Clear Charged Power <Once>
                    ChargeCancel();

                    //Clear Draw Sound Checker
                    DrawSoundClear();
                }
            }
        }

        public void Swap(ARROWTYPE type)
        {
            //return when Pulling or attempting to replace with the same arrowtype.
            if (isBowPullBegan == true || currentArrowType == type)
            {
                CatLog.WLog("Bow State is Pulling or Same Type of arrow currently loaded"); return;
            }

            if (type == ARROWTYPE.ARROW_SPECIAL)
            {
                if (ability.Condition.IsReadyToLoad == false)
                {
                    Notify.Inst.Message("Special Arrow Not Prepared.");
                    return;
                }
            }

            //Swap Arrow
            if (isAutoRunning == false)
            { // Menual Control
                //Disable Loaded Arrow, Cleanup variables and reload Arrow.
                if (arrowComponent != null)
                {
                    arrowComponent.ReturnToPoolRequest();
                }
                AD_BowRope.instance.CatchPointClear();
                arrowTr = null; arrowComponent = null;
                Reload(type);
            }
            else
            { //Auto Control
                AutoModeArrSwap(type);
            }
        }

        #region CHARGED

        void ChargeClear()
        {
            isChargeShotReady = false;
            chargingTime = 0f;
        }

        void ChargeIncrease()
        {
            chargingTime += Time.unscaledDeltaTime;
            if (chargingTime > maxChargingTime)
            {
                if (isChargeShotReady == false)
                {
                    bowEffect.Effect(BOWEFFECTYPE.CHARGED);
                    bowSound.PlayFullChargedSound();
                    isChargeShotReady = true;
                }
            }
        }

        void ChargeCancel()
        {
            if (isChargeShotReady == true || chargingTime > 0)
            {
                isChargeShotReady = false;
                chargingTime = 0f;
                CatLog.Log("Charge Cancel !");
            }
        }

        #endregion

        public void ReloadControlType()
        {
            currentPullType = GameManager.Instance.GetPlayerPullType();
        }

        #region SOUND
        public void DrawSoundCheck()
        {
            if (!isPlayedDrawSound)
            {
                bowSound.PlayPullingSound();
                isPlayedDrawSound = true;
            }
        }

        public void DrawSoundClear()
        {
            if (isPlayedDrawSound)
            {
                isPlayedDrawSound = false;
            }
        }

        public void PlayOneShot(AudioClip clip) => bowSound.AudioSource.PlayOneShot(clip);
        #endregion

        #region NOT_USED

        private IEnumerator ArrowReload()
        {
            #region ORIGIN_RELAOD
            //var arrow = CatPoolManager.Instance.LoadNormalArrow(this);
            //
            //currentLoadedArrow = arrow;
            //arrowComponent     = arrow.gameObject.GetComponent<AD_Arrow>();
            //loadedArrowRbody   = arrow.gameObject.GetComponent<Rigidbody2D>();
            //
            //arrow.transform.SetParent(this.transform, false);
            //arrow.transform.localScale                     = this.initialArrowScale;
            //arrow.transform.localEulerAngles               = this.initialArrowRotation;
            //arrow.transform.position   = ReturnInitArrowPos(arrow.transform.position);
            //
            ////Right, Left Clamp 한번만 잡아주면 다음 Active때 잡아주지 않아도 가능한지?
            //// -> 추후 게임이 시작되기 전에 미리 Clamp 한번에 Initial해주면 어떨지?
            //arrowComponent.leftClampPoint  = this.leftClampPoint;
            //arrowComponent.rightClampPoint = this.rightClampPoint;
            #endregion

            #region POOL_RELOAD
            yield return null;

            if (currentArrowType == ARROWTYPE.ARROW_MAIN)
                arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, transform, initArrowScale, ClampPointTop.position, Quaternion.Euler(initArrowRot));
            else if (currentArrowType == ARROWTYPE.ARROW_SUB)
                arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW, transform, initArrowScale, ClampPointTop.position, Quaternion.Euler(initArrowRot));

            //origin code 
            //LoadedArrow.transform.localEulerAngles = initArrowRot;

            arrowComponent = arrowTr.GetComponent<AD_Arrow>();
            arrowComponent.bottomClampTr = this.ClampPointBottom;
            arrowComponent.topClampTr = this.ClampPointTop;

            //Get Arrow Component
            //ArrowComponent = LoadedArrow.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop);
            #endregion
        }

        //Origin Fixed Update
        //private void FixedUpdate() {
        //    //if (isLaunch == true) { //bool isLaunched
        //    //    Launch();
        //    //    isLaunch = false;
        //    //}
        //}

        //void Update() {
        //    //else if (screenTouch.phase == TouchPhase.Moved && isBowPullBegan)
        //    //{
        //    //    //Touch Moved
        //    //    this.BowMoved(screenTouch.position);
        //    //}
        //}

        #endregion
    }
}
