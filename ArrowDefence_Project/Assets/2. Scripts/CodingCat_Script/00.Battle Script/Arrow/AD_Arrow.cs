namespace ActionCat
{
    using ActionCat.Interface;
    using UnityEngine;

    public class AD_Arrow : MonoBehaviour, IPoolObject, IArrowObject {
        //The Left, Right Clamp Point for the Arrow.
        [Header("COMPONENT")]
        [SerializeField] Transform arrowTr;
        [SerializeField] Rigidbody2D rBody;
        [Tooltip("This is Arrow Head Polygon Collider")]
        [SerializeField] PolygonCollider2D polyCollider;
        [SerializeField] TrailRenderer arrowTrail;
        [SerializeField] SpriteRenderer arrowSprite = null;
        [SerializeField] Transform arrowCatchTransform;

        [Header("COMPONENT OTHERS")]
        public Transform bottomClampTr;
        public Transform topClampTr;

        [Header("SHOOTING")]
        public float ArrowPower;
        [ReadOnly] public bool isLaunched;

        //Controll Arrow Position (Before Launched)
        private Vector3 arrowPosition;

        //Launch Power for the Arrow
        //private float powerFactor = 2000;

        //Skill Variables
        bool isInitSkill    = false;
        ArrowSkillSet arrowSkillSets = null;

        //Damage Veriables
        DamageStruct damageStruct;

        //Catch Point Proeprty
        public Transform CatchTr { get => arrowCatchTransform; }

        void InitComponent() {
            if (arrowTr == null) arrowTr = GetComponent<Transform>();
            if (rBody   == null) rBody   = GetComponent<Rigidbody2D>();
            if (polyCollider == null) CatLog.ELog("Arrow Main : Collider Not Cached.", true);
            if (arrowTrail   == null) CatLog.ELog("Arrow Main : TrailRenderer Not Cached.", true);
            if (arrowSprite  == null) CatLog.ELog("Arrow Main : Sprite Renderer Not Cached.", true);
        }

        private void Start() {
            InitComponent();
            rBody.gravityScale   = 0f;
            polyCollider.enabled = false;

            //Init-Arrow Skill
            arrowSkillSets = GameManager.Instance.GetArrowSkillSets(gameObject.name);
            if(arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(arrowTr, rBody, this);
            }
        }

        private void Update() {
            if (isLaunched == false) {
                ClampPosition(); //CalculatePower();
            }
            else {
                if (isInitSkill == true)
                    arrowSkillSets.OnUpdate();
            }
        }

        void FixedUpdate() {
            if(isLaunched == true) {
                if (isInitSkill == true)
                    arrowSkillSets.OnFixedUpdate();
            }
        }

        void OnDisable() {
            polyCollider.enabled = false;        // Disable Collision
            isLaunched           = false;        // Change Launched State
            rBody.velocity       = Vector2.zero; // Reset RigidBody Velocity
            rBody.isKinematic    = true;         // Change Body Type

            //Disable TrailRenderer
            arrowTrail.gameObject.SetActive(false);

            //Clear Skills Data
            if (isInitSkill == true){
                arrowSkillSets.Clear();
            }
        }

        void OnDestroy() => arrowSkillSets = null;

        private void ClampPosition() {
            //Get the Current Position of the Arrow
            arrowPosition = arrowTr.position;
            //Clamp the X Y position Between min and Max Points
            arrowPosition.x = Mathf.Clamp(arrowPosition.x, Mathf.Min(topClampTr.position.x, bottomClampTr.position.x), 
                                                           Mathf.Max(topClampTr.position.x, bottomClampTr.position.x));
            arrowPosition.y = Mathf.Clamp(arrowPosition.y, Mathf.Min(topClampTr.position.y, bottomClampTr.position.y), 
                                                           Mathf.Max(topClampTr.position.y, bottomClampTr.position.y));

            //Set new Position for the Arrow
            arrowTr.position = arrowPosition;
        }

        public void OnDisableCollider() => this.polyCollider.enabled = false;
        
        /// <summary>
        /// init clamp positions and return arrow component
        /// </summary>
        /// <param name="bottomClampPoint">Transform Bottom Clamp Position</param>
        /// <param name="topClampPoint">Transform Top Clamp position</param>
        /// <returns></returns>
        public AD_Arrow Reload(Transform bottomClampPoint, Transform topClampPoint, Vector3 rotation) {
            bottomClampTr  = bottomClampPoint;   //Init Clamp Point Bottom
            topClampTr     = topClampPoint;      //Init Clamp Point Top
            arrowTr.localEulerAngles = rotation; //Init Arrow Rotation
            return this;
        }

        private void OnTriggerStay2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                Vector3 point = collision.ClosestPoint(arrowTr.position);
                if(isInitSkill == true) {   
                    // Active-Skill
                    if (arrowSkillSets.OnHit(collision, ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
                        DisableRequest();
                    }
                }
                else {  
                    // Non-Skill
                    if(collision.GetComponent<IDamageable>().OnHitWithResult(ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
                        DisableRequest();
                    }
                }
            }
        }

        void OnTriggerExit2D(Collider2D coll) {
            if(isInitSkill == true) {
                if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                    arrowSkillSets.OnExit(coll);
                }
            }
        }

        #region Interface_Pool

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject, 0);

        #endregion

        #region Interface_Arrow

        /// <summary>
        /// Arrow Shot By Bow 
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="parent"></param>
        /// <param name="damage"></param>
        public void ShotByBow(Vector2 direction, Transform parent, DamageStruct damage) {
            damageStruct = damage;      //Copy damageStruct.
            arrowTr.SetParent(parent);  //Parent Change to Canvas.

            rBody.isKinematic = false;  //Change Body Type to Dynamic.
            isLaunched = true;

            rBody.velocity = direction.normalized * ArrowPower; //Force to direction.
            ///or Use AddForce, (this action is need to Modity Arrow Power)
            ///rBody.AddForce(direction * ArrowPower, ForceMode2D.Impluse); //or ForceMode2D.Force

            arrowTrail.gameObject.SetActive(true);  //Enable TrailRenderer and Line Clear 
            arrowTrail.Clear();                     //prevention afterimage

            // Start Collision Start.
            polyCollider.enabled = true; 
        }

        public void ShotToDirection(Vector2 direction, DamageStruct damage) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Force to directly direction.
        /// </summary>
        /// <param name="force"></param>
        public void ForceToDirectly() {
            rBody.velocity = arrowTr.up * ArrowPower;
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ForceToTarget(Vector3 targetPosition) {
            //Rotate Direction to target Position
            arrowTr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPosition - arrowTr.position).eulerAngles.z);

            //force to straight Direction.
            rBody.velocity = arrowTr.up * ArrowPower;
        }

        #endregion

        public void SpriteAlpha(bool isRewind) {
            var tempColor = arrowSprite.color;
            if(isRewind == false) {
                tempColor.a = 0f;
                arrowSprite.color = tempColor;
            }
            else {
                tempColor.a = 1f;
                arrowSprite.color = tempColor;
            }
        }

        #region GIZMOS
        //void OnDrawGizmosSelected() {
        //    float radius = 3f;
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(arrowTr.position, radius);
        //}

        //void OnDrawGizmos() {
        //    float radius = 3f;
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(arrowTr.position, radius);
        //}
        #endregion

        //private void CalculatePower()
        //{
        //    this.power = Vector2.Distance(transform.position, rightClampPoint.position) * powerFactor;
        //}
    }
}
