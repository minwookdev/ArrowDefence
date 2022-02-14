namespace ActionCat {
    using ActionCat.Interface;
    using UnityEngine;

    public class AD_Arrow_less : MonoBehaviour, IPoolObject, IArrowObject {
        [Header("COMPONENT")]
        [SerializeField] Rigidbody2D rBody;
        [SerializeField] Transform arrowTr;
        [SerializeField] TrailRenderer trailRender;

        [Header("VARIAVLES")]
        [SerializeField] [ReadOnly]
        private float forceMagnitude;//Default : 18f

        //Screen Limit Variable
        private Vector2 topLeftScreenPoint;
        private Vector2 bottomRightScreenPoint;
        private Vector2 offset = GameGlobal.ScreenOffset;
        private bool xIn, yIn;  //The x-Position inside Screen, y-Position inside Screen Flags.

        //Arrow Angle Calculate Variable
        private Vector2 arrowPosition;
        private Vector2 velocity;
        [SerializeField]
        private string[] effectPoolTags;
        private float arrowAngle = 0f;
        private bool isLaunched  = false;
        private bool isInitSkill = false;

        //STURCT
        DamageStruct damageStruct;

        //CLASS
        ArrowSkillSet arrowSkillSets = null;
        System.Collections.Generic.Queue<CollisionData> collisionQueue = null;

        //PROPERTY
        public float PowerFactor {
            get {
                return forceMagnitude;
            }
            set {
                if(value < 18 || value > 30) {
                    CatLog.ELog("Speed Value OverRange");
                }
                else {
                    forceMagnitude = value;
                }
            }
        }

        void ComponentChecker() {
            if (rBody == null) {
                CatLog.ELog("Less Arrow : RigidBody2D is Null.");
            }
            if (arrowTr == null) {
                CatLog.ELog("Less Arrow : Transform is Null");
            }
            if (trailRender == null) {
                CatLog.ELog("Less Arrow : TrailRenderer is Null.");
            }
        }

        private void Awake() {
            ComponentChecker();
        }

        private void Start() {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            //Init-Arrow Skill with Pool-tag params
            arrowSkillSets = GameManager.Instance.GetArrSkillSetsOrNull(gameObject.name);
            if (arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(arrowTr, rBody, this);
            }

            //set rigid body gravity scale to zero
            if (rBody.gravityScale != 0f) rBody.gravityScale = 0f;

            //Init Collision Data Queue
            collisionQueue = new System.Collections.Generic.Queue<CollisionData>();

            CatLog.Log($"Less Arrow Speed: {forceMagnitude}");
        }

        private void Update() {
            //velocity = rBody.velocity;

            //Arrow Launched
            //if(velocity.magnitude != 0 && isLaunched == true)
            //{
            //    //CalcAngle();
            //    CheckArrowBounds();
            //}
            //
            //if (isLaunched)
            //    arrowSkillSets.OnUpdate();

            //When Arrow Fired
            if(isLaunched == true) {
                ScreenOutCheck();       //Position OutOf Screen
                UpdateCollisionQueue(); //Update Collision Data

                if (isInitSkill == true) {
                    arrowSkillSets.OnUpdate();
                }
            }
        }

        Quaternion RotateToTarget2D(Vector2 startPos, Vector2 targetPos, float offset) {
            Vector2 direction = (targetPos - startPos).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return Quaternion.AngleAxis(angle + offset, Vector3.forward);
        }

        private void FixedUpdate() {
            if (isInitSkill)
                arrowSkillSets.OnFixedUpdate();
        }

        private void OnDisable() {
            rBody.velocity = Vector2.zero;
            trailRender.gameObject.SetActive(false);
            isLaunched        = false;

            //Clear Collision Data Queue
            if(collisionQueue != null && collisionQueue.Count > 0) {
                collisionQueue.Clear();
            }

            //SkillSets가 init되어있을때, 비활성화 시 Clear처리.
            if (isInitSkill == false)
                return;
            arrowSkillSets.Clear();
        }

        private void OnDestroy() => arrowSkillSets = null;

        private void ScreenOutCheck() {
            arrowPosition = arrowTr.position;

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
            arrowTr.rotation = Quaternion.AngleAxis(arrowAngle, arrowTr.forward);

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
            rBody.velocity = arrowTr.up * forceMagnitude;
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void ForceToTarget(Vector3 targetPosition) {
            //Rotate Direction to target position
            arrowTr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPosition - arrowTr.position).eulerAngles.z);

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            rBody.velocity = arrowTr.up * forceMagnitude;
        }

        #endregion

        #region Interface_Pool

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject, 0);

        #endregion
        
        /// <summary>
        /// Update COllision Queue, After Launched
        /// </summary>
        private void UpdateCollisionQueue() {
            while (collisionQueue.Count > 0) {
                var collData = collisionQueue.Dequeue();
                if(isInitSkill == true) { //Try OnHit with ArrSkill
                    if (arrowSkillSets.OnHit(collData.Collider, ref damageStruct, collData.CollisionPoint, collData.CollisionDirection)) {
                        //Hit Result True - Disable GameObject
                        PlayEffect(collData.CollisionPoint);
                        collisionQueue.Clear();
                        DisableRequest();
                    }
                    else { //Hit Result False - Ignore Collision
                        break;
                    }
                }
                else {  //Try OnHit with Non-ArrSkill
                    if(collData.Collider.GetComponent<IDamageable>().TryOnHit(ref damageStruct, collData.CollisionPoint, collData.CollisionDirection)) {
                        //Hit Result True - Disable GameObject
                        PlayEffect(collData.CollisionPoint);
                        collisionQueue.Clear();
                        DisableRequest();
                    }
                    else { //Hit Result False - Ignore Collision
                        break;
                    }
                }
                break;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                Vector3 contactPoint = collision.ClosestPoint(arrowTr.position);
                Vector2 hitDirection = GameGlobal.RotateToVector2(arrowTr.eulerAngles.z);
                collisionQueue.Enqueue(new CollisionData(collision, hitDirection, contactPoint));
            }
        }

        private void OnTriggerExit2D(Collider2D collision) {
            if(isInitSkill == true) {
                if(collision.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                    arrowSkillSets.OnExit(collision);
                }
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
        //        if (isIgnoreCollision == true) return; //ignore duplicate collision
        //
        //        isIgnoreCollision = true;
        //        Vector3 point     = collision.ClosestPoint(arrowTr.position);
        //        if(isInitSkill == true) {
        //            //Active-Skill
        //            if (arrowSkillSets.OnHit(collision, ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
        //                DisableRequest();
        //            }
        //            else { //Not Disable Arrow: Re-Collision
        //                isIgnoreCollision = false;
        //            }
        //        }
        //        else {
        //            //Non-Skill
        //            if(collision.GetComponent<IDamageable>().OnHitWithResult(ref damageStruct, point, GameGlobal.RotateToVector2(arrowTr.eulerAngles.z))) {
        //                DisableRequest();
        //            }
        //            else { //Not Disable Arrow: Re-Collision
        //                isIgnoreCollision = false;
        //            }
        //        }
        //    }
        //}

        //ROTATE TESTED CODE
        //if (Input.GetMouseButtonDown(0)) {  //Type. Rotate EulerAngles
        //    var mousePos = Input.mousePosition;
        //    var dir = arrowTr.position - mousePos;
        //    //var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    //arrowTr.RotateAround(tempTr.position, Vector3.forward, angle);
        //
        //    //arrowTr.rotation = Quaternion.AngleAxis()
        //
        //    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    var targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        //    arrowTr.rotation = RotateToTarget2D(arrowTr.position, targetPos, -90f);
        //}
        //if (Input.GetMouseButtonDown(1)) {  //Type. RotateAround Failed
        //    var mousePos = Input.mousePosition;
        //    var targetPos = Camera.main.ScreenToWorldPoint(mousePos);
        //    var dir = targetPos - arrowTr.position;
        //    dir.Normalize();
        //    var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //    arrowTr.RotateAround(tempTr.position, arrowTr.forward, angle);
        //}

        #endregion
    }
}
