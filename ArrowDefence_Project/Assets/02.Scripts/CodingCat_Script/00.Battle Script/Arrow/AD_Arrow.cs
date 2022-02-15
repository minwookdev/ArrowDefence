namespace ActionCat {
    using System.Collections.Generic;
    using ActionCat.Interface;
    using UnityEngine;

    public class AD_Arrow : MonoBehaviour, IPoolObject, IArrowObject {
        //The Left, Right Clamp Point for the Arrow.
        [Header("COMPONENT")]
        [SerializeField] Transform arrowTr = null;
        [SerializeField] Rigidbody2D rBody = null;
        [Tooltip("This is Arrow Head Polygon Collider")]
        [SerializeField] PolygonCollider2D arrowColl   = null;
        [SerializeField] TrailRenderer arrowTrail      = null;
        [SerializeField] SpriteRenderer arrowSprite    = null;
        [SerializeField] Transform arrowCatchTransform = null;

        [Header("COMPONENT OTHERS")]
        [ReadOnly] public Transform bottomClampTr = null;
        [ReadOnly] public Transform topClampTr    = null;

        [Header("SHOOTING")]
        [SerializeField] [ReadOnly]
        private float powerFactor; // Default : 18f
        [SerializeField] [ReadOnly] 
        private bool isLaunched;

        //FIELDS
        [SerializeField]
        private string[] effectPoolTags;
        private Vector3 arrowPosition;
        private DamageStruct damageStruct;
        private Queue<CollisionData> collisionQueue = null;

        //SKILLS
        ArrowSkillSet arrowSkillSets = null;
        bool isInitSkill = false;

        //PROPERTY
        public Transform CatchTr { get => arrowCatchTransform; }
        public float PowerFactor { get => powerFactor; 
            set {
                if(value < 18 || value > 30) {
                    CatLog.ELog("Speed Value OverRange");
                }
                else {
                    powerFactor = value;
                }
            }
        }

        void InitComponent() {
            if (arrowTr == null) arrowTr = GetComponent<Transform>();
            if (rBody   == null) rBody   = GetComponent<Rigidbody2D>();
            if (arrowColl == null) CatLog.ELog("Arrow Main : Collider Not Cached.", true);
            if (arrowTrail   == null) CatLog.ELog("Arrow Main : TrailRenderer Not Cached.", true);
            if (arrowSprite  == null) CatLog.ELog("Arrow Main : Sprite Renderer Not Cached.", true);
        }

        private void Start() {
            InitComponent();
            rBody.gravityScale = 0f;
            arrowColl.enabled  = false;

            //Init-Arrow Skill
            arrowSkillSets = GameManager.Instance.GetArrSkillSetsOrNull(gameObject.name);
            if(arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(arrowTr, rBody, this);
            }

            //Init Collision Data Queue
            collisionQueue = new Queue<CollisionData>();

            //Check Speed Value
            if(powerFactor <= 0f) {
                CatLog.ELog("Is Not Set Arrow Speed Value.");
            }
        }

        private void Update() {
            if (isLaunched == false) {
                ClampPosition(); //CalculatePower();
            }
            else {
                if (isInitSkill == true)
                    arrowSkillSets.OnUpdate();

                //Only Queue Update On Launched
                UpdateCollisionQueue();
            }
        }

        void FixedUpdate() {
            if(isLaunched == true) {
                if (isInitSkill == true)
                    arrowSkillSets.OnFixedUpdate();
            }
        }

        void OnDisable() {
            isLaunched        = false;        // Change Launched State
            arrowColl.enabled = false;
            rBody.velocity    = Vector2.zero; // Reset RigidBody Velocity
            rBody.isKinematic = true;         // Change Body Type

            //Disable TrailRenderer
            arrowTrail.gameObject.SetActive(false);

            //Clear Skills Data
            if (isInitSkill == true){
                arrowSkillSets.Clear();
            }

            //Clear Collision Queue
            if(collisionQueue != null && collisionQueue.Count > 0) {
                collisionQueue.Clear();
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

        public void OnDisableCollider() => this.arrowColl.enabled = false;
        
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

        private void OnTriggerEnter2D(Collider2D collider) {
            if(collider.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                Vector3 collisionPoint     = collider.ClosestPoint(arrowTr.position);
                Vector2 collisionDirection = GameGlobal.RotateToVector2(arrowTr.eulerAngles.z);
                collisionQueue.Enqueue(new CollisionData(collider, collisionDirection, collisionPoint));
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

            rBody.velocity = direction.normalized * powerFactor; //Force to direction.
            ///or Use AddForce, (this action is need to Modity Arrow Power)
            ///rBody.AddForce(direction * ArrowPower, ForceMode2D.Impluse); //or ForceMode2D.Force

            arrowTrail.gameObject.SetActive(true);  //Enable TrailRenderer and Line Clear 
            arrowTrail.Clear();                     //prevention afterimage

            // Start Collision Start.
            arrowColl.enabled = true; 
        }

        public void ShotToDirection(Vector2 direction, DamageStruct damage) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Force to directly direction.
        /// </summary>
        /// <param name="force"></param>
        public void ForceToDirectly() {
            rBody.velocity = arrowTr.up * powerFactor;
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ForceToTarget(Vector3 targetPosition) {
            //Rotate Direction to target Position
            arrowTr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPosition - arrowTr.position).eulerAngles.z);

            //force to straight Direction.
            rBody.velocity = arrowTr.up * powerFactor;
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

        /// <summary>
        /// Update Collision Queue [Atfer launced]
        /// </summary>
        void UpdateCollisionQueue() {
            while (collisionQueue.Count > 0) {
                var collData = collisionQueue.Dequeue();
                if (isInitSkill == true) { //Try OnHit with ArrowSkill
                    //Hit Success
                    if (arrowSkillSets.OnHit(collData.Collider, ref damageStruct, collData.CollisionPoint, collData.CollisionDirection)) {
                        PlayEffect(collData.CollisionPoint);
                        collisionQueue.Clear();
                        DisableRequest();
                    }
                    else { //Hit Failed - Get Next Collision Queue
                        break;
                    }
                }
                else { //Try OnHit with Non-Skill
                    //Hit Success
                    if (collData.Collider.GetComponent<IDamageable>().TryOnHit(ref damageStruct, collData.CollisionPoint, collData.CollisionDirection)) {
                        PlayEffect(collData.CollisionPoint);
                        collisionQueue.Clear();
                        DisableRequest();
                    }
                    else {  //Hit Failed - Get Next Collision Queue
                        break;
                    }
                }
                break;
            }
        }

        #region EFFECT

        public void SetEffectInfo(string[] tagArray) {
            effectPoolTags = tagArray;
        }

        public void RemoveEffectInfo() {
            effectPoolTags = null;
        }

        public void PlayEffect(Vector3 position) {
            if(effectPoolTags.Length == 0) {
                CatLog.WLog("Effect PoolTag Array is Empty."); return;
            }

            CCPooler.SpawnFromPool<ACEffector2D>(effectPoolTags.RandIndex<string>(), position, Quaternion.identity).PlayOnce(true);
        }

        string IArrowObject.GetMainTag() {
            return string.Format("{0}{1}", gameObject.name, AD_Data.POOLTAG_HITEFFECT);
        }

        #endregion

        #region LEGACY

        //private void OnTriggerStay2D(Collider2D collision) {
        //    if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
        //        if (isIgnoreCollision == false) {
        //            isIgnoreCollision = true;
        //            //arrowColl.enabled = false; //ignore multi collision
        //            Vector3 point = collision.ClosestPoint(arrowTr.position);
        //            if (isInitSkill == true) {
        //                //Try OnHit with Arrow Skill
        //                if (arrowSkillSets.OnHit(collision, ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
        //                    DisableRequest();
        //                }
        //                else { //Hit Failed, Restart Collision Check
        //                    isIgnoreCollision = false;
        //                    //arrowColl.enabled = true;
        //                }
        //            }
        //            else {
        //                //Try OnHit with Non-Skill
        //                if (collision.GetComponent<IDamageable>().OnHitWithResult(ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
        //                    DisableRequest();
        //                }
        //                else { //Hit Failed, Restart Collision Check
        //                    isIgnoreCollision = false;
        //                    //arrowColl.enabled = true;
        //                }
        //            }
        //        }
        //    }
        //}

        //private void CalculatePower()
        //{
        //    this.power = Vector2.Distance(transform.position, rightClampPoint.position) * powerFactor;
        //}

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
    }

    internal sealed class CollisionData {
        public Collider2D Collider { get; private set; } = null;
        public Vector2 CollisionDirection { get; private set; }
        public Vector3 CollisionPoint { get; private set; }

        internal CollisionData(Collider2D coll, Vector2 collisionDirection, Vector3 collisionPoint) {
            Collider           = coll;
            CollisionPoint     = collisionPoint;
            CollisionDirection = collisionDirection;
        }
    }
}
