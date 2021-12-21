namespace ActionCat
{
    using UnityEngine;
    using UnityEngine.UI;
    using System.Collections;

    public class AD_BowController : MonoBehaviour
    {
        public static AD_BowController instance;

        [Header("GENERAL")]
        public Camera MainCam = null;
        private Touch screenTouch;
        [SerializeField] BowSprite bowSprite;
        [SerializeField] AD_BowAbility bowAbility;
        DamageStruct damageStruct;
        
        [Header("BOW CONTROL VARIABLES")]
        public float TouchRadius    = 1f;
        public float PullableRadius = 1f;
        public Image BowCenterPointImg;
        public Transform ClampPointTop, ClampPointBottom;
        [Range(1f, 20f)] public float SmoothRotateSpeed = 12f;
        [Range(1f, 3f)]  public float ArrowPullingSpeed = 1f;

        private PULLINGTYPE currentPullType;     //조준 타입 변경 -> Player Settings로 부터 받아오는 Enum Data
        private bool isBowPulling   = false;     //활이 일정거리 이상 당겨져서 회전할 수 있는 상태
        private bool isBowPullBegan = false;     //Bow Pull State Variables
        private float maxBowAngle, minBowAngle;  //Min, Max BowAngle Variables
        private float bowAngle;                  //The Angle Variable (angle between Click point and Bow).
        private Vector2 limitTouchPosVec;        //Bow GameObject와 거리를 비교할 벡터
        private Vector3 initialTouchPos;         //처음 터치한 곳을 저장할 벡터
        private Vector3 direction;
        private Vector3 currentClickPosition;
        private Vector3 tempEulerAngle;
        //private bool isPullingStartPosSet = false;    //When pulling is started, it is used to forcibly hold the position.
                                                        //Enabled later, when using with -> PullingStartPosition();
        public bool IsBowPulling { get { return isBowPulling; } }   //Bow Pulling Condition property (Used ACSP)
        public bool IsPullingStop { get; set; }                     //Pulling Stop boolean For Pause, Clear Battle Scene

        [Header("ARROW VARIABLES")] //Arrow Relation Variables
        [ReadOnly] public AD_Arrow ArrowComponent;
        [ReadOnly] public Transform ArrowParentTr;
        [ReadOnly] public GameObject LoadedArrow;

        //private float requiredLaunchForce = 250f;
        private bool launchArrow = false;
        private Vector2 arrowForce;
        private Vector3 arrowPosition;
        private Vector3 initArrowScale = new Vector3(1.5f, 1.5f, 0f);
        private Vector3 initArrowRot   = new Vector3(0f, 0f, -90f);
        private LOAD_ARROW_TYPE loadArrowType;

        /// <summary>
        /// Bow Skills Delegate
        /// </summary>
        public delegate void BowSkillsDel(float anglez, Transform parent, MonoBehaviour mono, ref DamageStruct damage,
                                          Vector3 initscale, Vector3 initpos, Vector2 force, LOAD_ARROW_TYPE type);
        public BowSkillsDel BowSkillSet;

        #region NOT_USED_VARIABLES
        //The Distance between first click and Bowlope
        //private float radius;
        //The Point on the Circumference based on radius and angle.
        //private Vector3 cPoint = Vector3.zero;
        //private Vector2 distance;
        #endregion

        private void Awake() => instance = this;

        private void Start()
        {
            //Initialize Main Camera Object
            MainCam = Camera.main;
            currentPullType = Data.CCPlayerData.settings.PullingType;

            //Initialize variables For Arrow to be Loaded
            if(ClampPointTop == null || ClampPointBottom == null) {
                CatLog.ELog("Bow Controller : Clamp Point is Not Cached."); CatLog.Break();
            }

            ArrowParentTr = transform.parent;

            //Initialize Bow Center Pivot Image Object
            if (BowCenterPointImg)
                BowCenterPointImg.transform.position = transform.position;

            //Init Start Bow Angle : Start 이후 처음 조준할 때 Bounce 현상 방지
            bowAngle = transform.eulerAngles.z;
            minBowAngle = 0f; maxBowAngle = 180f;

            //Init Load Arrow Type : 장전될 화살 타입 정의
            loadArrowType = GameManager.Instance.LoadArrowType();

            StartCoroutine(this.ArrowReload());

            //게임오버 이벤트에 Burn Effect 추가. Resurrection 구현 시 추가적인 로직 구현 필요.
            GameManager.Instance.AddListnerGameOver(() => { 
                //Active Burn Fade Effect.
                bowSprite.ActiveBurn(false);
                //Bow Rope Material Alpha Change.
                AD_BowRope.instance.RopeAlpha(false);
                //if Loaded Arrow, Change Sprite color Alpha.
                if (ArrowComponent != null) {
                    ArrowComponent.SpriteAlpha(false);
                }
            });

            if(bowAbility == null) {
                bowAbility = GetComponent<AD_BowAbility>();
            }
            //Init-Bow Skill and Current Slot Damage Struct.
            damageStruct = bowAbility.GetDamage(loadArrowType);
            bowAbility.AddListnerToSkillDel(ref BowSkillSet);
        }

        private void Update()
        {
#if UNITY_ANDROID
            if (Input.touchCount != 0)
            {
                //Get Value On Screen Touch -> Area Designation Func Add
                screenTouch = Input.GetTouch(0);

                if (screenTouch.phase == TouchPhase.Began)        
                    BowBegan(screenTouch.position);        //Touch Began
                else if (screenTouch.phase == TouchPhase.Ended)
                    BowReleased(screenTouch.position);     //Touch Ended

                if (isBowPullBegan)
                    BowMoved(screenTouch.position);        //Touch Moved

                #region OLD
                //else if (screenTouch.phase == TouchPhase.Moved && isBowPullBegan)
                //{
                //    //Touch Moved
                //    this.BowMoved(screenTouch.position);
                //}
                #endregion
            }
#endif
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                //Click Began
                this.BowBegan(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                //Click Ended
                this.BowReleased(Input.mousePosition);
            }

            if (isBowPullBegan)
            {
                //Click Moved
                this.BowMoved(Input.mousePosition);
            }
#endif
            ArrowPosUpdate();
        }

        private void FixedUpdate()
        {
            if (launchArrow)
            {
                this.LaunchTheArrow();
                launchArrow = false;
            }
        }

        private void OnDestroy() => instance = null;

        private void BowBegan(Vector2 pos)
        {
            //Pause 상태거나, Battle Clear된 상태라면 활을 당길 수 없음
            if (IsPullingStop == true) return;

            switch (currentPullType)
            {
                case PULLINGTYPE.AROUND_BOW_TOUCH: if (CheckTouchRaius(pos) == false) return;       break;  //Type1. 활 주변의 일정거리 터치 조준
                case PULLINGTYPE.FREE_TOUCH:           SetInitialTouchPos(pos);                     break;  //Type2. 터치한 곳 기준 활 조준
                case PULLINGTYPE.AUTOMATIC:            CatLog.Log("Not Support This Pulling Type"); break;  //Type3. 자동 사격 (미구현)
                default: break;
            }

            if(AD_BowRope.instance.arrowCatchPoint == null && ArrowComponent != null) {
                AD_BowRope.instance.arrowCatchPoint = ArrowComponent.arrowChatchPoint;
            }

            isBowPullBegan = true;
        }

        private void BowMoved(Vector2 pos)
        {
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

            #region NEW_CONTROLLER

            float distOfPoint = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ? 
                Vector2.Distance(transform.position, currentClickPosition) : 
                Vector2.Distance(initialTouchPos, currentClickPosition);

            isBowPulling = (distOfPoint > 1f) ? true : false;

            if(isBowPulling)
            {
                //when pulling starts, correct the position once. (부자연스러워 보여서 비활성화.)
                //PullingStartPosition(currentClickPosition);

                this.direction = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ?
                    currentClickPosition - transform.position :
                    currentClickPosition - initialTouchPos;

                //change bow angle depending on touch or click position. *Mathf.Clamp, *Mathf.LerpAngle.
                this.bowAngle = Mathf.Clamp(Mathf.LerpAngle(bowAngle, Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, Time.unscaledDeltaTime * SmoothRotateSpeed),
                            minBowAngle, maxBowAngle);

                //Set Direction of the Bow.
                tempEulerAngle        = transform.eulerAngles;
                tempEulerAngle.z      = bowAngle;
                transform.eulerAngles = tempEulerAngle;

                //Draw Touch Line : Color Green.
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, transform.position, true);
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH)  DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, initialTouchPos, true);
            }
            else
            {
                //Draw Touch Line : Color Red.
                if (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, transform.position, false);
                else if (currentPullType == PULLINGTYPE.FREE_TOUCH)  DrawTouchPos.Instance.DrawTouchLine(currentClickPosition, initialTouchPos, false);
            }

            CheckPauseBattle();

            #endregion

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

        private void BowReleased(Vector2 pos)
        {
            if (isBowPullBegan)
            {
                currentClickPosition = MainCam.ScreenToWorldPoint(pos);
                launchArrow    = true;  //Check LaunchArrow The FixedUpdate.
                isBowPullBegan = false;
            }

            DrawTouchPos.Instance.ReleaseTouchLine();
        }

        private void LaunchTheArrow()
        {
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
            #endregion
            //장전되어 있는 화살이 없거나 isPulling 않들어왔을때 Return
            if (LoadedArrow == null || isBowPulling == false) {
                CatLog.WLog("Can't Launch the Arrow"); return;
            } 
            
            //로프 해제
            AD_BowRope.instance.arrowCatchPoint = null;

            isBowPulling = false;
            ArrowComponent.ShotArrow(arrowForce, damageStruct, ArrowParentTr);

            //Active Bow Skill
            BowSkillSet?.Invoke(transform.eulerAngles.z, ArrowParentTr, this, ref damageStruct, initArrowScale,
                                ArrowComponent.arrowChatchPoint.transform.position, arrowForce, loadArrowType);

            LoadedArrow    = null;
            ArrowComponent = null;

            //Active Shot Impact
            bowSprite.ActiveImpact();

            //Active Camera Shake
            CineCam.Inst.ShakeCamera(5f, .1f);

            //ReLoad Logic Start
            StartCoroutine(this.ArrowReload());
        }

        private bool CheckTouchRaius(Vector2 pos)
        {
            this.limitTouchPosVec = MainCam.ScreenToWorldPoint(pos);
            
            float touchDistanceFromBow = (limitTouchPosVec - (Vector2)transform.position).magnitude;

            if (touchDistanceFromBow <= TouchRadius) return true;
            else                                     return false;
        }

        private void SetInitialTouchPos(Vector2 pos)
        {
            initialTouchPos = MainCam.ScreenToWorldPoint(pos);

            CatLog.Log("Save First Touch Position");
        }

        private IEnumerator ArrowReload() {
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

            if (loadArrowType == LOAD_ARROW_TYPE.ARROW_MAIN)
                LoadedArrow = CCPooler.SpawnFromPool(AD_Data.POOLTAG_MAINARROW, transform, initArrowScale, ClampPointTop.position, Quaternion.identity);
            else if (loadArrowType == LOAD_ARROW_TYPE.ARROW_SUB)
                LoadedArrow = CCPooler.SpawnFromPool(AD_Data.POOLTAG_SUBARROW, transform, initArrowScale, ClampPointTop.position, Quaternion.identity);

            LoadedArrow.transform.localEulerAngles = initArrowRot;

            ArrowComponent = LoadedArrow.GetComponent<AD_Arrow>();
            ArrowComponent.leftClampPoint  = this.ClampPointBottom;
            ArrowComponent.rightClampPoint = this.ClampPointTop;
#endregion
        }

        /// <summary>
        /// 화살이 당겨진 상황에서 클리어, 일시정지 들어오면 당기고있는 상태 해제
        /// </summary>
        private void CheckPauseBattle()
        {
            if (IsPullingStop)
            {
                if (LoadedArrow != null)
                    LoadedArrow.transform.position = ClampPointTop.transform.position;
                DrawTouchPos.Instance.ReleaseTouchLine();
                isBowPullBegan = false; isBowPulling = false;
            }
        }

        private void PullingStartPosition(Vector3 touchPos)
        {
            //if(isPullingStartPosSet == false)
            //{
            //    //Pulling Start Action
            //    this.direction = (currentPullType == PULLINGTYPE.AROUND_BOW_TOUCH) ?
            //        touchPos - transform.position :
            //        touchPos - initialTouchPos;
            //
            //    this.bowAngle = Mathf.Clamp(Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90, minBowAngle, maxBowAngle);
            //
            //    tempEulerAngle        = transform.eulerAngles;
            //    tempEulerAngle.z      = bowAngle;
            //    transform.eulerAngles = tempEulerAngle;
            //
            //    isPullingStartPosSet = true;
            //}
        }

        private void ArrowPosUpdate()
        {
            if(LoadedArrow != null)
            {
                if(isBowPulling)
                {
                    arrowPosition = LoadedArrow.transform.position;
                    arrowPosition = Vector3.MoveTowards(arrowPosition, ClampPointBottom.position, Time.unscaledDeltaTime * ArrowPullingSpeed);
                    LoadedArrow.transform.position = arrowPosition;
                    
                    //Arrow Direction * Force
                    arrowForce = LoadedArrow.transform.up * ArrowComponent.ArrowPower;
                }
                else
                {
                    LoadedArrow.transform.position = ClampPointTop.position;
                }
            }
        }

        public void ArrowSwap(LOAD_ARROW_TYPE type)
        {
            //조준중 또는 스왑하려는 화살과 동일된 타입의 화살의 경우 스왑 불가
            if (isBowPullBegan || loadArrowType == type) {
                CatLog.WLog("Bow State is Pulling or Same Type of arrow currently loaded");
                return;
            }

            //장전된 화살 Disable 처리하고 Arrow 관련 변수정리, Pool에서 화살을 꺼내서 장전
            if (LoadedArrow != null)
                ArrowComponent.DisableRequest();
            AD_BowRope.instance.arrowCatchPoint  = null;
            LoadedArrow   = null; ArrowComponent = null;
            loadArrowType = type;
            damageStruct = bowAbility.GetDamage(loadArrowType);
            StartCoroutine(ArrowReload());
        }


    }
}
