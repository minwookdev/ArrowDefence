namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;

    public class AD_Arrow_less : MonoBehaviour, IPoolObject, IArrowObject
    {
        //Screen Limit Variable
        private Vector2 topLeftScreenPoint;
        private Vector2 bottomRightScreenPoint;
        private Vector2 offset = GameGlobal.ScreenOffset;
        private bool xIn, yIn;  //The x-Position inside Screen, y-Position inside Screen Flags.

        //Arrow Angle Calculate Variable
        private Vector2 arrowPosition;
        private Vector2 velocity;
        private float arrowAngle = 0f;

        private bool isLaunched = false;
        private Transform tr;
        
        //TrailRenderer
        [SerializeField]
        TrailRenderer trailRender;

        [SerializeField]
        private Rigidbody2D rBody;

        [SerializeField]
        private float forceMagnitude = 18f; //Default Force : 18f

        [Header("EXPERIMENTAL")]
        [SerializeField] Transform centerTr = null;

        //Arrow Skill Data
        DamageStruct damageStruct;
        ArrowSkillSet arrowSkillSets = null;
        bool isInitSkill = false;

        private void Awake() {
            //Init-Component
            rBody = gameObject.GetComponent<Rigidbody2D>();
            tr    = gameObject.GetComponent<Transform>();
        }

        private void Start() {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            //Init-Arrow Skill with Pool-tag params
            arrowSkillSets = GameManager.Instance.GetArrowSkillSets(gameObject.name);
            if (arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(tr, rBody, this);
            }

            //set rigid body gravity scale to zero
            if (rBody.gravityScale != 0f) rBody.gravityScale = 0f;
        }

        private void Update()
        {
            //velocity = rBody.velocity;

            //Arrow Launched
            //if(velocity.magnitude != 0 && isLaunched == true)
            //{
            //    //CalcAngle(); //아나..증말
            //    CheckArrowBounds();
            //}
            //
            //if (isLaunched)
            //    arrowSkillSets.OnUpdate();

            if(isLaunched == true) {
                UpdateOutOfScreen();
                if (isInitSkill == true) {
                    arrowSkillSets.OnUpdate();
                }
            }
        }

        private void FixedUpdate() {
            if (isInitSkill)
                arrowSkillSets.OnFixedUpdate();
        }

        private void OnDisable()
        {
            rBody.velocity = Vector2.zero;
            trailRender.gameObject.SetActive(false);
            isLaunched = false;

            //SkillSets가 init되어있을때, 비활성화 시 Clear처리.
            if (isInitSkill == false)
                return;
            arrowSkillSets.Clear();
        }

        private void OnDestroy() => arrowSkillSets = null;

        private void UpdateOutOfScreen() {
            arrowPosition = tr.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            //Out of Screen
            if (!(xIn && yIn)) {
                DisableRequest();
                return;
            }
        }

        void CalcAngle() {
            //Calc Arrow Angle
            arrowAngle  = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);

            ///Description
            ///AD_Arrow에 작성함.
        }

        #region Interface_ArrowObject

        public void ShotByBow(Vector2 direction, Transform parent, DamageStruct damage) {
            throw new System.NotImplementedException("CAUTION ! : Less Arrow is Not Used ShotByBow interface method.");
        }

        /// <summary>
        /// Shot to Direction.
        /// </summary>
        /// <param name="force">applied as normalized</param>
        public void ShotToDirection(Vector2 direction, DamageStruct damage) {
            damageStruct = damage;                                  // Copy Damage Struct

            isLaunched = true;                                      // trigger on launched
            rBody.velocity = direction.normalized * forceMagnitude; // force to direction.
            ///or [Used AddForce] (this action is need modify force value)
            ///rBody.AddForce(direction * force, ForceMode2D.Impulse); //-> recommend or use ForceMode2D.Force

            trailRender.gameObject.SetActive(true); //Enable TrailRenderer and Clear Line
            trailRender.Clear();                    //prevention afterimage
        }

        /// <summary>
        /// Force to directly direction
        /// </summary>
        public void ForceToDirectly() {
            //Force to transform.up direction
            rBody.velocity = tr.up * forceMagnitude;
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ForceToTarget(Vector3 targetPosition) {
            //Rotate Direction to target position
            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPosition - tr.position).eulerAngles.z);

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            rBody.velocity = tr.up * forceMagnitude;
        }

        #endregion

        #region Interface_Pool

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject, 0);

        #endregion

        private void OnTriggerStay2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                Vector3 point = collision.ClosestPoint(tr.position);
                if(isInitSkill == true) {
                    //Active-Skill
                    if (arrowSkillSets.OnHit(collision, ref damageStruct, point, GameGlobal.RotateToVector2(tr.eulerAngles.z))) {
                        DisableRequest();
                    }
                }
                else {
                    //Non-Skill
                    if(collision.GetComponent<IDamageable>().OnHitWithResult(ref damageStruct, point, GameGlobal.RotateToVector2(tr.eulerAngles.z))) {
                        DisableRequest();
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if(isInitSkill == true) {
                if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                    arrowSkillSets.OnExit(collision);
                }
            }
        }


    }
}
