namespace CodingCat_Games
{
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;
    using CodingCat_Scripts;

    public class AD_BowController : MonoBehaviour
    {
        public static AD_BowController instance;

        private enum CurrentPlatform
        {
            Platform_PC,
            Platform_Mobile
        }

        public enum BowPullType
        {
            Around_Bow,
            FirstTouch_Position,
            Automatic_ShotMode
        }

        [Header("Game Settings Variable")]
        public Camera mainCam;
        private Touch screenTouch;
        private CurrentPlatform currentPlatform;

        //The Angle Variable (angle between Click point and Bow).
        private float bowAngle;
        private Vector3 direction;
        private Vector3 currentClickPosition;
        private Vector3 tempEulerAngle;

        //The Distance between first click and Bowlope
        private float radius;

        //The Point on the Circumference based on radius and angle.
        private Vector3 cPoint = Vector3.zero;

        private Vector2 distance;

        [Header("Bow Pulling Variable")]
        //0 == 활 주변, 1 == 마지막 터치한 곳 기준
        public BowPullType pullType = BowPullType.Around_Bow;
        //Bow Pull State variable
        private bool bowPullBegan;
        public Image bowPivotImg;
        //Pull Type 1 Save Position Variable
        private Vector2 initialTouchPos; //InitialTouchPos 현재 두가지 용도로 사용중, 추후 수정.
        private Vector2 limitTouchPosVec;
        public float touchRadius = 1f;
        public Transform rightClampPoint, leftClampPoint;

        [Header("Arrow Variable")]
        //Arrow Relation Variables
        [ReadOnly] public AD_Arrow arrowComponent;
        [ReadOnly] public GameObject currentLoadedArrow;
        private Vector3 arrowPosition;
        private Vector2 arrowForce;
        private float requiredLaunchForce = 250f;
        private bool launchArrow = false;
        public Transform arrowParent;

        [Header("ArrowInitial Variables")]
        private Vector3 initialArrowScale    = new Vector3(1.5f, 1.5f, 0f);
        private Vector3 initialArrowRotation = new Vector3(0f, 0f, -90f);

        [Header("Test Object")]
        private Action TestFunction;

        /// <summary>
        /// Bow Skill Sets Delegate
        /// </summary>
        public delegate void BowSkillsDel(float rot, float angle, byte arrownum, Transform arrowparent,
                                          AD_BowController bowcontroller, Vector3 initScale, Vector3 initpos, Vector2 force);
        public BowSkillsDel bowSkillSet;
        //굳이 Event 쓸 필요 없을거같아서 그냥 delegate로 일단 만듦

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
        }

        private void Start()
        {
            //Play Platform Check -> Move Manager Script.
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                currentPlatform = CurrentPlatform.Platform_Mobile;
            }
            else
            {
                currentPlatform = CurrentPlatform.Platform_PC;
            }

            if (mainCam == null)
            {
                Action act1 = () => { mainCam = Camera.main; };
                act1();
            }

            //장전될 화살들에게 부여할 변수 설정
            if(rightClampPoint == null || leftClampPoint == null)
            {
                leftClampPoint  = this.transform.GetChild(3).GetChild(0);
                rightClampPoint = this.transform.GetChild(3).GetChild(1);

                if(transform.GetChild(3).name != "Bow_ClampPoints")
                {
                    CatLog.WLog("Bow Clamp Point is in the wrong Location. Check The Bow");
                }
            }

            if (pullType == BowPullType.Around_Bow) bowPivotImg.rectTransform.position = transform.position;

            //TestFunction Set
            TestFunction = () => {
                //Arrow Position Init Method
                //var arrowPos = currentLoadedArrow.transform.position;
                //arrowPos.x = rightClampPoint.position.x;
                //arrowPos.y = rightClampPoint.position.y;

                // ↑ Improvement / fix
                //currentLoadedArrow.transform.position = ReturnInitArrowPos(currentLoadedArrow.transform.position);

            }; TestFunction();

            //arrowParent = GameObject.FindWithTag(AD_Data.Tag_BattleScene_MainCanvas).GetComponent<RectTransform>();
            arrowParent = transform.parent;

            //yield return new WaitUntil(() => CCPooler.IsInitialized);
            StartCoroutine(this.ArrowReload());
        }

        private void Update()
        {
            #region Mobile_Platform
            if (currentPlatform == CurrentPlatform.Platform_Mobile)
            {
                if (Input.touchCount != 0)
                {
                    //Get Value On Screen Touch -> Area Designation Func Add
                    screenTouch = Input.GetTouch(0);

                    if (screenTouch.phase == TouchPhase.Began)
                    {
                        //Touch Begin
                        this.BowBegan(screenTouch.position);
                    }
                    else if (screenTouch.phase == TouchPhase.Moved)
                    {
                        //Touch Moved
                        this.BowMoved(screenTouch.position);
                    }
                    else if (screenTouch.phase == TouchPhase.Ended)
                    {
                        //Touch Ended
                        this.BowReleased(screenTouch.position);
                    }
                }
            }
            #endregion
            else //PC
            {
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

                if (bowPullBegan)
                {
                    //Click Moved
                    this.BowMoved(Input.mousePosition);
                }

            }

            if(Input.GetKeyDown(KeyCode.N))
            {
                //this.TestFunction();

                Time.timeScale = 0.1f;
            }
        }

        private void FixedUpdate()
        {
            if (launchArrow)
            {
                this.LaunchTheArrow();
                launchArrow = false;
            }
        }

        private void BowBegan(Vector2 pos)
        {
            //처음 터치 포인트 저장 Vec2
            //Vector2 touchBeganPos = mainCam.ScreenToWorldPoint(pos);

            switch (pullType)
            {
                //조건 1. 활 주변의 일정거리 주변을 클릭 | 터치했을때만 조준 가능
                case BowPullType.Around_Bow:
                    if (!this.CheckTouchRaius(pos)) return;
                    #region ORIGIN_SCRIPTS
                    //radius = Vector2.Distance(AD_BowRope.instance.transform.position, mainCam.ScreenToWorldPoint(pos));
                    #endregion
                    radius = Vector2.Distance(transform.position, mainCam.ScreenToWorldPoint(pos));
                    break;

                //조건 2. 처음 클릭한 곳 기준으로 활의 기준점 지정
                case BowPullType.FirstTouch_Position:
                    this.SetInitialTouchPos(pos);
                    radius = Vector2.Distance(initialTouchPos, mainCam.ScreenToWorldPoint(pos));
                    break;

                case BowPullType.Automatic_ShotMode:
                    CatLog.Log("Not Support this Pull Type, Return Function");
                    return;
            }

            if(AD_BowRope.instance.arrowCatchPoint == null && arrowComponent != null)
            {
                AD_BowRope.instance.arrowCatchPoint = arrowComponent.arrowChatchPoint;
            }

            bowPullBegan = true;
        }

        private void BowMoved(Vector2 pos)
        {
            //Get CurrentClick Position
            currentClickPosition = mainCam.ScreenToWorldPoint(pos);

            //Pull Type 추가에 따른 스크립트 구분
            if (pullType == BowPullType.Around_Bow)
            {
                #region ORIGIN_SCRIPTS
                //this.direction = currentClickPosition - transform.position;
                //
                ////클릭 위치에 따른 활 자체의 각도를 변경할 변수 저장
                //this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90;
                //
                ////Set Direction of the Bow
                //tempEulerAngle = transform.eulerAngles;
                //tempEulerAngle.z = bowAngle;
                //transform.eulerAngles = tempEulerAngle;
                //
                ////Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
                //cPoint.x = AD_BowRope.instance.transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                //cPoint.y = AD_BowRope.instance.transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
                //
                ////Pull or Drag the arrow ralative to Click Position
                //distance = (AD_BowRope.instance.transform.position - currentClickPosition) -
                //           (AD_BowRope.instance.transform.position - cPoint);
                #endregion 

                this.direction = currentClickPosition - transform.position;

                //클릭 위치에 따른 활 자체의 각도를 변경할 변수 저장
                this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90;
                //this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg;
                
                //Set Direction of the Bow
                tempEulerAngle = transform.eulerAngles;
                tempEulerAngle.z = bowAngle;
                transform.eulerAngles = tempEulerAngle;
                
                //Calculate current cPoint based on angle and radius (center.x - r * cos(theta), center.y - r * sin(theta))
                cPoint.x = transform.position.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                cPoint.y = transform.position.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);
                
                //Pull or Drag the arrow ralative to Click Position
                distance = (transform.position - currentClickPosition) -
                           (transform.position - cPoint);
                
                //위 공식과 거리 차이가 있는지 체크 

            }
            else if (pullType == BowPullType.FirstTouch_Position)
            {
                this.direction = (Vector2)currentClickPosition - initialTouchPos;

                //클릭 위치에 따른 활 자체의 각도를 변경할 변수 저장
                this.bowAngle = Mathf.Atan2(direction.x, -direction.y) * Mathf.Rad2Deg + 90;

                //Set Direction of the Bow
                tempEulerAngle = transform.eulerAngles;
                tempEulerAngle.z = bowAngle;
                transform.eulerAngles = tempEulerAngle;

                //BowRope Controller
                cPoint.x = initialTouchPos.x - radius * Mathf.Cos(bowAngle * Mathf.Deg2Rad);
                cPoint.y = initialTouchPos.y - radius * Mathf.Sin(bowAngle * Mathf.Deg2Rad);

                //Pull or Drag the arrow ralative to Click Position (distance 변수는 )
                distance = (initialTouchPos - (Vector2)currentClickPosition) -
                           (initialTouchPos - (Vector2)cPoint);
            }

            if(currentLoadedArrow != null)
            {
                arrowPosition = currentLoadedArrow.transform.position;
                arrowPosition.x = arrowComponent.rightClampPoint.position.x - distance.x;
                arrowPosition.y = arrowComponent.rightClampPoint.position.y - distance.y;
                currentLoadedArrow.transform.position = arrowPosition;

                arrowForce = currentLoadedArrow.transform.up * arrowComponent.power;

                if (arrowForce.magnitude > requiredLaunchForce)
                {
                    //Path Drawer
                    //추후 Path Drawer 작업 후 활 당김 시, 조건 사용
                    //조준경 아이템을 사용했을 때 나타나는 효과로서 작업
                }
                else
                {

                }
            }
        }

        private void BowReleased(Vector2 pos)
        {
            if (bowPullBegan)
            {
                bowPullBegan = false;
                currentClickPosition = mainCam.ScreenToWorldPoint(pos);

                launchArrow = true;

                //CatLog.Log("Bow Released !");
            }
        }

        private void LaunchTheArrow()
        {
            if (currentLoadedArrow == null)
            {
                //장전할 수 있는 화살이 없음 -> CatPoolManager 체크 또는 Pool Arrow Object 갯수 체크
                CatLog.WLog("Current Loaded Arrow is Null, Can't Launch the Arrow");
                return;
            }

            //일정 이상 당겨져야 발사되도록 할 조건 :: 다시 점검 
            if (arrowForce.magnitude < requiredLaunchForce)
            {
                //Reset Position
                currentLoadedArrow.transform.position = rightClampPoint.transform.position;

                CatLog.Log("Not Enough Require Force, More Pulling the Bow !");
                return;
            }

            AD_BowRope.instance.arrowCatchPoint = null;

            arrowComponent.ShotArrow(arrowForce, arrowParent);

            //Active Bow Skill
            bowSkillSet?.Invoke(transform.eulerAngles.z, 30f, 2, arrowParent, this, initialArrowScale,
                                arrowComponent.arrowChatchPoint.transform.position, arrowForce);

            currentLoadedArrow = null;
            arrowComponent     = null;

            //ReLoad Logic Start
            StartCoroutine(this.ArrowReload());
        }

        private bool CheckTouchRaius(Vector2 pos)
        {
            this.limitTouchPosVec = mainCam.ScreenToWorldPoint(pos);
            
            float touchDistanceFromBow = (limitTouchPosVec - (Vector2)transform.position).magnitude;

            if (touchDistanceFromBow <= touchRadius) return true;
            else CatLog.WLog("Touch Aroud Bow !");   return false;
        }

        private void SetInitialTouchPos(Vector2 pos)
        {
            this.initialTouchPos = mainCam.ScreenToWorldPoint(pos);
            bowPivotImg.rectTransform.position = new Vector3(initialTouchPos.x, initialTouchPos.y, 0);
            CatLog.Log("Save First Touch Position");
        }

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
            
            currentLoadedArrow = CCPooler.SpawnFromPool(AD_Data.TAG_MAINARROW, this.transform, initialArrowScale,
                                     rightClampPoint.position, Quaternion.identity);

            currentLoadedArrow.transform.localEulerAngles = initialArrowRotation;

            arrowComponent = currentLoadedArrow.GetComponent<AD_Arrow>();
            arrowComponent.leftClampPoint = this.leftClampPoint;
            arrowComponent.rightClampPoint = this.rightClampPoint;

            #endregion

        }

        /// <summary>
        /// this a Function To Adjust Current Loaded Arrow to the Position of Right Clamp Point.
        /// </summary>
        /// <param name="pos">Currnet Loaded Arrow Position Vector</param>
        /// <returns></returns>
        private Vector3 ReturnInitArrowPos(Vector3 pos)
        {
            var changePos = pos;
            changePos = rightClampPoint.position;

            return changePos;
        }
    }
}
