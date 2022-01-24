namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;

    public partial class AD_BowController : MonoBehaviour {
        public static AD_BowController instance;

        [Header("COMPONENT")]
        [SerializeField] BowSprite bowSprite;
        [SerializeField] AD_BowAbility bowAbility;
        [SerializeField] Transform bowTr = null;
        private Touch screenTouch;
        public Camera MainCam = null;

        [Header("CONTROLLER")]
        public float TouchRadius = 1f;
        public Image BowCenterPointImg;
        public Transform ClampPointTop, ClampPointBottom;
        [Range(1f, 20f)] public float SmoothRotateSpeed = 12f;
        [Range(1f, 3f)]  public float ArrowPullingSpeed = 1f;

        private int pointerId = 0;
        private bool isBowPulling   = false;     //활이 일정거리 이상 당겨져서 회전할 수 있는 상태
        private bool isBowPullBegan = false;     //Bow Pull State Variables
        private bool isTouched      = false;
        private bool isChargeShotReady  = false;
        private float maxBowAngle, minBowAngle;  //Min, Max BowAngle Variables
        private float bowAngle;                  //The Angle Variable (angle between Click point and Bow).
        private float maxChargingTime = 0f;
        private float chargingTime    = 0f;
        private Vector3 initialTouchPos;         //처음 터치한 곳을 저장할 벡터
        private Vector3 direction;
        private Vector3 currentClickPosition;
        private Vector3 tempEulerAngle;
        //private AutoMachine autoMachine = null;
        public bool IsPullingStop { get; set; }  //Pulling Stop boolean For Pause, Clear Battle Scene

        [Header("ARROW VARIABLES")] //Arrow Relation Variables
        [SerializeField] Transform ArrowParentTr;
        [SerializeField] Transform arrowTr = null;
        [SerializeField] GameObject loadedArrow; // <- Not Used
        [SerializeField] AD_Arrow arrowComponent;

        private Vector3 arrowPos;
        private Vector2 arrowForce;
        private Vector3 initArrowScale = new Vector3(1.5f, 1.5f, 1f);
        private Vector3 initArrowRot   = new Vector3(0f, 0f, -90f);


        //Enums
        private ARROWTYPE arrowType;
        private PULLINGTYPE currentPullType;

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

        public bool IsStatePulling() => isBowPulling;

        private void Awake() => instance = this;

        private void Start() {
            //Init Controller Transform
            if (bowTr == null) {
                bowTr = gameObject.GetComponent<Transform>();
            }

            //Initialize Main Camera Object
            MainCam = Camera.main;
            currentPullType = Data.CCPlayerData.settings.PullingType;

            //Initialize variables For Arrow to be Loaded
            if(ClampPointTop == null || ClampPointBottom == null) {
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

            //Init Load Arrow Type : 장전될 화살 타입 정의
            arrowType = GameManager.Instance.LoadArrowType();

            //Load Arrow From CCPooler.
            Reload();

            //============================================================== << CALLBACK GAMEOVER >> ==============================================================
            //게임오버 이벤트에 Burn Effect 추가. Resurrection 구현 시 추가적인 로직 구현 필요.
            GameManager.Instance.AddListnerGameOver(() => {
                //Active Burn Fade Effect.
                bowSprite.Effect(BOWEFFECTYPE.FADE, false);
                //Bow Rope Material Alpha Change.
                AD_BowRope.instance.RopeAlpha(false);
                //if Loaded Arrow, Change Sprite color Alpha.
                if (arrowComponent != null) {
                    arrowComponent.SpriteAlpha(false);
                }
            });
            //=====================================================================================================================================================
            //================================================================ << CALLBACK CLEAR >> ===============================================================
            GameManager.Instance.AddListnerEndBattle(() => ClearAutoStop());
            //=====================================================================================================================================================

            if(bowAbility == null) {
                bowAbility = GetComponent<AD_BowAbility>();
            }

            //Init-Bow Skill and Current Slot Damage Struct.
            CatLog.Log(StringColor.YELLOW, $"Damage Struct SizeOf : {System.Runtime.InteropServices.Marshal.SizeOf(typeof(DamageStruct))}");
            CatLog.Log(StringColor.YELLOW, $"Damage Struct SizeOf : {System.Runtime.InteropServices.Marshal.SizeOf(damageStruct)}");
            bowAbility.AddListnerToSkillDel(ref BowSkillSet);

            //Get Max Charging Timeif(
            maxChargingTime = GameGlobal.CHARGINGTIME;

            //AutoMode Variables Init
            arrSwapWait = new WaitUntil(() => autoState == AUTOSTATE.WAIT || autoState == AUTOSTATE.FIND || autoState == AUTOSTATE.TRAC);
        }

        private void Update() {
            //===============================================<< AUTO MODE UPDATE >>==============================================
            if(isAutoRunning) { //Enabled AutoMode : AutoMode Update
                AutoModeUpdate(); 
                return;
            }
            //===================================================================================================================

            //==============================================<< MANUAL MODE UPDATE >>=============================================
#if UNITY_EDITOR
            //==============================================<< EDITOR CONTROLLER >>==============================================
            if (Input.GetMouseButtonDown(0)) {    //Click Began
                if(EventSystem.current.IsPointerOverGameObject() == false) { // if the mouse Click UI GameObject
                    BowBegan(Input.mousePosition);
                }
                else { //Block click Peedback
                    CatLog.Log("Click Blocked.");
                }
            }
            else if (Input.GetMouseButtonUp(0)) { //Click Ended
                BowReleased(Input.mousePosition);
            }
            if (isBowPullBegan == true) {         //Click Moved
                BowMoved(Input.mousePosition);
            }
            //===================================================================================================================
#elif UNITY_ANDROID
            //==============================================<< MOBILE CONTROLLER >>==============================================
            //Touch Update : Only Mobile
            if (Input.touchCount != 0) {
                //Get Value On Screen Touch
                //screenTouch = Input.GetTouch(0);

                //New Touch Logic 
                if(isTouched == false) { //Find Touch
                    for (int i = 0; i < Input.touchCount; i++) {
                        if(EventSystem.current.IsPointerOverGameObject(i) == false) {
                            screenTouch = Input.GetTouch(i);
                            isTouched   = true; break;
                        }
                    }
                }

                if(screenTouch.phase == TouchPhase.Began) {       //Touch Began       
                    BowBegan(screenTouch.position); 
                }
                else if (screenTouch.phase == TouchPhase.Ended) { //Touch Ended
                    BowReleased(screenTouch.position); isTouched = false;
                }
                if(isBowPullBegan == true) {                      //Touch Moved
                    BowMoved(screenTouch.position);
                }
            }
            //===================================================================================================================
#endif

/**#if UNITY_ANDROID //Original Controller Update
            if (Input.touchCount != 0) {
                //Get Value On Screen Touch -> Area Designation Func Add
                screenTouch = Input.GetTouch(0);

                if (screenTouch.phase == TouchPhase.Began)        
                    BowBegan(screenTouch.position);        //Touch Began
                else if (screenTouch.phase == TouchPhase.Ended)
                    BowReleased(screenTouch.position);     //Touch Ended

                if (isBowPullBegan)
                    BowMoved(screenTouch.position);        //Touch Moved
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0)) {      //Click Began
                this.BowBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0)) {   //Click Ended
                this.BowReleased(Input.mousePosition);
            }

            if (isBowPullBegan) {                   //Click Moved
                this.BowMoved(Input.mousePosition);
            }
#endif **/

            //================================================<< ARROW POSITION UPDATE >>============================================
            UpdateArrPos();
            //=======================================================================================================================
            //=======================================================================================================================
        }

        private void OnDestroy() => instance = null;

        private void BowBegan(Vector2 pos) {
            //Stop Pulling or Arrow Component is null return Pull Began.
            if (IsPullingStop == true || arrowComponent == null) return;

            switch (currentPullType) {
                case PULLINGTYPE.AROUND_BOW_TOUCH: if (PullTypeTouchAround(pos) == false) return; break; //Type 0.-활 주변의 일정거리 터치 조준
                case PULLINGTYPE.FREE_TOUCH:           PullTypeTouchFree(pos);                    break; //Type 1.-터치한 곳 기준 활 조준
            }

            //Rope Catch Point Set.
            AD_BowRope.instance.SetCatchPoint(arrowComponent.CatchTr);

            //Clear Charging Data
            ChargeClear();

            isBowPullBegan = true;
        }

        private void BowMoved(Vector2 pos) {
            //Get CurrentClick Position
            currentClickPosition = MainCam.ScreenToWorldPoint(pos);

#region OLD_BOW_ROTATION_LOGIC
            //Pull Type 추가에 따른 스크립트 구분
            //if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH)
            //{
            //    #region ORIGIN_CODES
            //    //this.direction = currentClickPosition - transform.position;
            //    //
            //    ////클릭 위치에 따른 활 자체의 각도를 변경할 변수 저장
            //    //this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90;
            //    //
            //    ////Set Direction of the Bow
            //    //tempEulerAngle = transform.eulerAngles;
            //    //tempEulerAngle.z = bowAngle;
            //    //transform.eulerAngles = tempEulerAngle;
            //    //
            //    ////Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
            //    //cPoint.x = AD_BowRope.instance.transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
            //    //cPoint.y = AD_BowRope.instance.transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
            //    //
            //    ////Pull or Drag the arrow ralative to Click Position
            //    //distance = (AD_BowRope.instance.transform.position - currentClickPosition) -
            //    //           (AD_BowRope.instance.transform.position - cPoint);
            //    //Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
            //    //cPoint.x = transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
            //    //cPoint.y = transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
            //    //
            //    //Pull or Drag the arrow ralative to Click Position
            //    //distance = (transform.position - currentClickPosition) -
            //    //           (transform.position - cPoint);
            //    //
            //    //this.bowAngle = Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed);
            //    #endregion
            //
            //    this.direction = currentClickPosition - transform.position;
            //
            //    this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed),
            //                                minBowAngle, maxBowAngle);
            //
            //    //Set Direction of the Bow
            //    tempEulerAngle = transform.eulerAngles;
            //    tempEulerAngle.z = bowAngle;
            //    transform.eulerAngles = tempEulerAngle;
            //
            //    //CatLog.Log($"Temp Euler Angle Z Pos : {bowAngle.ToString()}"); //Bow Angle Debugging
            //    DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, transform.position);
            //}
            //else if (currentPullType == PULLINGTYPE.FREE_TOUCH)
            //{
            //    //Touch Correction Vector Setting (보정하지 않으면 활이 바로 아래로 회전해버린다)
            //    correctionTouchPosition = new Vector2(currentClickPosition.x, currentClickPosition.y - 0.1f);
            //
            //    this.direction = correctionTouchPosition - initialTouchPos;
            //
            //    //클릭 위치에 따른 활 자체의 각도를 변경할 변수
            //    this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.deltaTime * SmoothRotateSpeed),
            //                                minBowAngle, maxBowAngle);
            //
            //    //Lerp to Set Direction of the Bow
            //    tempEulerAngle = transform.eulerAngles;
            //    tempEulerAngle.z = bowAngle;
            //    transform.eulerAngles = tempEulerAngle;
            //
            //    #region ORIGIN_CODES
            //    //BowRope Controller
            //    //cPoint.x = initialTouchPos.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
            //    //cPoint.y = initialTouchPos.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
            //    //
            //    //Pull or Drag the arrow ralative to Click Position (distance 변수는 )
            //    //distance = (initialTouchPos - (Vector2)currentClickPosition) -
            //    //           (initialTouchPos - (Vector2)cPoint);
            //    #endregion
            //
            //    DrawTouchPos.Instance.DrawTouchLine(correctionTouchPosition, initialTouchPos);
            //}
#endregion

            float distOfPoint = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ? 
                Vector2.Distance(bowTr.position, currentClickPosition) : 
                Vector2.Distance(initialTouchPos, currentClickPosition);

            isBowPulling = (distOfPoint > 1f) ? true : false;

            if(isBowPulling) {
                //when pulling starts, correct the position once. [DISABLE]
                //CorrectionArrPos(currentClickPosition);

                this.direction = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ?
                    currentClickPosition - bowTr.position :
                    currentClickPosition - initialTouchPos;

                //change bow angle depending on touch or click position. *Mathf.Clamp, *Mathf.LerpAngle.
                this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.unscaledDeltaTime * SmoothRotateSpeed),
                            minBowAngle, maxBowAngle);

                //Set Direction of the Bow.
                tempEulerAngle    = bowTr.eulerAngles;
                tempEulerAngle.z  = bowAngle;
                bowTr.eulerAngles = tempEulerAngle;

                //Draw Touch Line : Color Green.
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, bowTr.position, true);
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH)  DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, initialTouchPos, true);
            }
            else {
                //Draw Touch Line : Color Red.
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, bowTr.position, false);
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH)  DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, initialTouchPos, false);
            }

            //Check the Pulling Stop Trigger is true
            UpdateStopPulling();

#region OLD_ARROW_LOGIC

            //if (LoadedArrow != null)
            //{
            //    #region ORIGIN_CODES
            //    //Before Visualizing
            //    //arrowPosition = currentLoadedArrow.transform.position;
            //    //arrowPosition.x = arrowComponent.rightClampPoint.position.x - distance.x;
            //    //arrowPosition.y = arrowComponent.rightClampPoint.position.y - distance.y;
            //    //currentLoadedArrow.transform.position = arrowPosition;
            //
            //    //After Visualizing
            //    //arrowPosition = currentLoadedArrow.transform.position;
            //    //arrowPosition = leftClampPoint.position;
            //    //currentLoadedArrow.transform.position = arrowPosition;
            //
            //    //if (Vector2.Distance(currentLoadedArrow.transform.position, leftClampPoint.position) > .4f) CatLog.Log("0.1f 보다 멀다");
            //    //else CatLog.Log("가깝다");
            //    //CatLog.Log($"Distance of Arrow Position : {Vector2.Distance(currentLoadedArrow.transform.position, leftClampPoint.position).ToString()}");
            //    #endregion
            //
            //    //Bow Pulling Over Time
            //    arrowPosition = LoadedArrow.transform.position;
            //    arrowPosition = Vector3.MoveTowards(arrowPosition, LeftClampPoint.position, Time.deltaTime * ArrowPullingSpeed); //deltaTime * speed 변수해주면 되겠다
            //    LoadedArrow.transform.position = arrowPosition;
            //
            //    arrowForce = LoadedArrow.transform.up * ArrowComponent.power;
            //
            //    this.CheckPauseBattle();
            //
            //    if (arrowForce.magnitude > requiredLaunchForce)
            //    {
            //        //초기 스크립트에 Path Drawer가 있었던 조건문
            //        //
            //        //
            //    }
            //    else
            //    {
            //
            //    }
            //}
#endregion
        }

        private void BowReleased(Vector2 pos) {
            currentClickPosition = MainCam.ScreenToWorldPoint(pos);

            if(isBowPullBegan == true) {    //1. Check the Pull Began.
                if(isBowPulling == true) {  //2. Check the Pulling
                    Launch();               //3. if the Completely Pulled, Launched.
                    isBowPulling = false;   // Release Pulling State.
                }                           // Release Pull Began State.
                isBowPullBegan = false;
            }

            //Erase Touch Line
            DrawTouchPos.Instance.ReleaseTouchLine();
        }

        private void Launch() {
#region OLD
            //일정 이상 당겨져야 발사되도록 할 조건
            //if (arrowForce.magnitude < requiredLaunchForce)
            //{
            //    //Reset Position
            //    LoadedArrow.transform.position = RightClampPoint.transform.position;
            //
            //    CatLog.Log("Not Enough Require Force, More Pulling the Bow !");
            //    return;
            //}

            //Check Bow Pulling [distOfPoint]
            //if(isBowPulling == false)
            //{
            //    CatLog.Log("Dist of Point was short, returned.");
            //    LoadedArrow.transform.position = RightClampPoint.transform.position;
            //    return;
            //}

            //장전되어 있는 화살이 없거나 isPulling 않들어왔을때 Return
            //if (LoadedArrow == null || isBowPulling == false) {
            //    CatLog.WLog("Can't Launch the Arrow"); return;
            //}
#endregion
            //Pull Stop while reloading Arrow.
            IsPullingStop = true;

            //Release Bow Rope
            AD_BowRope.instance.CatchPointClear();

            //Update Damage Struct
            damageStruct = bowAbility.GetDamage(arrowType, isChargeShotReady);

            //Shot Arrow & Active Skill.
            arrowComponent.ShotByBow(arrowForce, ArrowParentTr, damageStruct);
            BowSkillSet?.Invoke(bowTr, this, ref damageStruct, arrowComponent.CatchTr.position, arrowType);
            //BowSkillSet?.Invoke(transform.eulerAngles.z, ArrowParentTr, this, ref damageStruct, initArrowScale,
            //                    ArrowComponent.CatchTr.position, arrowForce, loadArrowType);

            //Release GameObject and Component Arrow.
            //loadedArrow    = null;
            arrowTr        = null;
            arrowComponent = null;

            //Active Shot Impact Effect
            bowSprite.Effect(BOWEFFECTYPE.IMPACT);

            //Active Camera Shake
            CineCam.Inst.ShakeCamera(5f, .1f);

            //Arrow Reload
            Reload();

            //Release Pulling Stop State
            IsPullingStop = false;
        }

        bool PullTypeTouchAround(Vector2 touchPos) {
            Vector2 currentTouchPos = MainCam.ScreenToWorldPoint(touchPos);
            float distFromBow = (currentTouchPos - (Vector2)bowTr.position).magnitude;
            return (distFromBow <= TouchRadius) ? true : false;
        }

        void PullTypeTouchFree(Vector2 touchPos) {
            initialTouchPos = MainCam.ScreenToWorldPoint(touchPos);
        }

        void Reload() {
            switch (arrowType) { //Reload Arrow by Current Equipped Arrow Type.
                //case ARROWTYPE.ARROW_MAIN: loadedArrow = CCPooler.SpawnFromPool(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                //case ARROWTYPE.ARROW_SUB:  loadedArrow = CCPooler.SpawnFromPool(AD_Data.POOLTAG_SUBARROW,  bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_MAIN: arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
                case ARROWTYPE.ARROW_SUB:  arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_SUBARROW,  bowTr, initArrowScale, ClampPointTop.position, Quaternion.identity); break;
            }

            //Get Arrow Component with init Clamp Points.
            arrowComponent = arrowTr.GetComponent<AD_Arrow>().Reload(ClampPointBottom, ClampPointTop, initArrowRot);
        }

        /// <summary>
        /// 화살이 당겨진 상황에서 클리어, 일시정지 들어오면 당기고있는 상태 해제
        /// </summary>
        void UpdateStopPulling() {
            if (IsPullingStop == true) {
                //if (loadedArrow != null)
                //    loadedArrow.transform.position = ClampPointTop.position;

                if (arrowTr != null)
                    arrowTr.position = ClampPointTop.position;
                isBowPullBegan = false; isBowPulling = false;
                DrawTouchPos.Instance.ReleaseTouchLine();
            }
        }

        void CorrectionArrPos(Vector3 touchPos) {
            //Before use, the 'isCorrectionArrPos' declaration is required.
            bool isCorrectionArrPos = true;
            if(isCorrectionArrPos == false) {
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) direction = touchPos - bowTr.position;
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH)  direction = touchPos - initialTouchPos;
            
                bowAngle = Mathf.Clamp(Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, minBowAngle, maxBowAngle);
            
                tempEulerAngle    = bowTr.eulerAngles;
                tempEulerAngle.z  = bowAngle;
                bowTr.eulerAngles = tempEulerAngle;
            
                isCorrectionArrPos = true;
            }
        }

        void UpdateArrPos() {
            if(arrowTr != null) {
                if(isBowPulling) {
                    arrowPos = arrowTr.position;
                    arrowPos = Vector3.MoveTowards(arrowPos, ClampPointBottom.position, Time.unscaledDeltaTime * ArrowPullingSpeed);
                    arrowTr.position = arrowPos;
                    
                    //Arrow Direction * Force
                    arrowForce = arrowTr.up * arrowComponent.PowerFactor;

                    //Increase Charged Power
                    ChargeIncrease();
                }
                else {
                    arrowTr.position = ClampPointTop.position;

                    //Clear Charged Power <Once>
                    ChargeCancel();
                }
            }
        }

        public void Swap(ARROWTYPE type) {
            //return when Pulling or attempting to replace with the same arrowtype.
            if (isBowPullBegan == true || arrowType == type) {
                CatLog.WLog("Bow State is Pulling or Same Type of arrow currently loaded");
                return;
            }

            //Swap Arrow
            if(isAutoRunning == false) { 
                //Manual Control
                //Disable Loaded Arrow, Cleanup variables and reload Arrow.
                if (arrowComponent != null)
                    arrowComponent.DisableRequest();
                AD_BowRope.instance.CatchPointClear();
                arrowTr = null; arrowComponent = null;
                arrowType = type;

                Reload();
            }
            else { 
                //Auto Control
                AutoModeArrSwap(type);
            }
        }

#region CHARGED

        void ChargeClear() {
            isChargeShotReady = false;
            chargingTime      = 0f;
            CatLog.Log("Charge Clear !");
        }

        void ChargeIncrease() {
            chargingTime += Time.unscaledDeltaTime;
            if(chargingTime > maxChargingTime) {
                if(isChargeShotReady == false) {
                    bowSprite.Effect(BOWEFFECTYPE.CHARGED);
                    isChargeShotReady = true;
                    CatLog.Log("Charge Complete !");
                }
            }
        }

        void ChargeCancel() {
            if (isChargeShotReady == true || chargingTime > 0) {
                isChargeShotReady = false;
                chargingTime      = 0f;
                CatLog.Log("Charge Cancel !");
            }
        }

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

            if (arrowType == ARROWTYPE.ARROW_MAIN)
                arrowTr = CCPooler.SpawnFromPool<Transform>(AD_Data.POOLTAG_MAINARROW, transform, initArrowScale, ClampPointTop.position, Quaternion.Euler(initArrowRot));
            else if (arrowType == ARROWTYPE.ARROW_SUB)
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
