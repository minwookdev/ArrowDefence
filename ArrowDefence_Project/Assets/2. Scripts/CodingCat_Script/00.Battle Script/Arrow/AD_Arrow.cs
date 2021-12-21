namespace ActionCat
{
    using UnityEngine;

    public class AD_Arrow : MonoBehaviour, IPoolObject, IArrowObject
    {
        //The Left, Right Clamp Point for the Arrow.
        [Header("COMPONENT")]
        public Transform arrowChatchPoint;
        public TrailRenderer arrowTrail;
        public Transform leftClampPoint, rightClampPoint;
        [SerializeField] SpriteRenderer arrowSprite = null;

        [Header("SHOOTING")]
        public float ArrowPower;
        [ReadOnly] public bool isLaunched;

        //Controll Arrow Position (Before Launched)
        private Vector3 arrowPosition;

        //Launch Power for the Arrow
        //private float powerFactor = 2000;
        private Rigidbody2D rBody;
        private PolygonCollider2D polyCollider;
        Transform arrowTr;

        //Skill Variables
        bool isInitSkill    = false;
        bool isCollision    = false;
        ArrowSkillSet arrowSkillSets = null;

        //Damage Veriables
        DamageStruct damageStruct;

        private void Start() {
            //if (ReferenceEquals(rBody, null)) rBody = gameObject.GetComponent<Rigidbody2D
            //Initial Arrow Childs
            if (arrowChatchPoint == null) arrowChatchPoint = transform.GetChild(2);
            if (arrowTrail == null) arrowTrail = transform.GetChild(2).GetChild(0).GetComponent<TrailRenderer>();
            arrowTr = GetComponent<Transform>();
            rBody = gameObject.GetComponent<Rigidbody2D>();
            rBody.gravityScale = 0f;

            if (polyCollider == null) polyCollider = arrowTr.GetChild(0).GetComponent<PolygonCollider2D>();
            polyCollider.enabled = false;

            //Init-Arrow Skill
            arrowSkillSets = GameManager.Instance.GetArrowSkillSets(gameObject.name);
            if(arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(arrowTr, rBody, this);
            }
        }

        private void Update()
        {
            if (isLaunched == false) {
                ClampPosition(); //CalculatePower();
            }
            else {
                if (isInitSkill == true)
                    arrowSkillSets.OnUpdate();
            }
        }

        void FixedUpdate()
        {
            if(isLaunched == true) {
                if (isInitSkill == true)
                    arrowSkillSets.OnFixedUpdate();
            }
        }

        void OnDisable() {
            isLaunched  = false;
            isCollision = false;

            if (isInitSkill == true){
                arrowSkillSets.Clear();
            }
        }

        void OnEnable() {
            isCollision = false;
        }

        void OnDestroy() => arrowSkillSets = null;

        private void ClampPosition()
        {
            //Get the Current Position of the Arrow
            arrowPosition = arrowTr.position;
            //Clamp the X Y position Between min and Max Points
            arrowPosition.x = Mathf.Clamp(arrowPosition.x, Mathf.Min(rightClampPoint.position.x, leftClampPoint.position.x),
                                                           Mathf.Max(rightClampPoint.position.x, leftClampPoint.position.x));
            arrowPosition.y = Mathf.Clamp(arrowPosition.y, Mathf.Min(rightClampPoint.position.y, leftClampPoint.position.y),
                                                           Mathf.Max(rightClampPoint.position.y, leftClampPoint.position.y));

            //Set new Position for the Arrow
            arrowTr.position = arrowPosition;
        }

        public void OnDisableCollider() => this.polyCollider.enabled = false;

        public void DisableRequest(GameObject target)
        {
            rBody.isKinematic = true;
            arrowTrail.gameObject.SetActive(false);
            polyCollider.enabled = false;   //Disable Collider -> Block Collision

            CCPooler.ReturnToPool(target, 0);

            //객체가 Disable되기전에 SetParent메서드로 부모객체가 바뀌어버리면 
            //보통은 스케일과 좌표가 난리가 난다. 어떤 객체던 SetParent하기전에 Disable후 부모를 바꿔줄 것.
            //(현재는 CCPooler에 비활성화를 요청하도록 로직 변경)
        }

        public void DisableRequest() {
            rBody.isKinematic = true;
            arrowTrail.gameObject.SetActive(false);
            polyCollider.enabled = false;

            CCPooler.ReturnToPool(gameObject, 0);
        }

        void OnTriggerEnter2D(Collider2D coll) {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                if (isInitSkill == true) {
                    if(arrowSkillSets.OnHit(coll, ref damageStruct) == true) {
                        DisableRequest();
                    }
                }
                else { //중복 피격 방지
                    if(isCollision == false) {
                        OnHit(coll.gameObject);
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

        void OnHit(GameObject target) {
            isCollision = true;
            //target.SendMessage(nameof(IDamageable.OnHitObject), damageStruct, SendMessageOptions.DontRequireReceiver);
            target.GetComponent<IDamageable>().OnHitObject(ref damageStruct);
            DisableRequest();
        }

        /// <summary>
        /// Shot Used Bow Object
        /// </summary>
        /// <param name="force"></param>
        /// <param name="parent"></param>
        public void ShotArrow(Vector2 force, DamageStruct damage, Transform parent)
        {
            //Battle Scene Main Canvas의 Transform을 매개변수로 받음
            //발사 처리 후, 부모객체를 바꿔주지 않으면 활의 자식으로 남아있게 되어, 활의 회전을 따라가버리는 문제가 생긴다
            //부모 변경처리 함수에는 보정처리는 true이다. 보정이 들어가지 않으면, 좌표와 스케일이 난리남 [기본값 : true]
            damageStruct = damage;
            arrowTr.SetParent(parent);

            ShotToDirectly(force);
        }

        /// <summary>
        /// Shot Directly to Direction
        /// </summary>
        /// <param name="force"></param>
        public void ShotToDirectly(Vector2 force)
        {
            rBody.isKinematic = false;
            isLaunched = true;
            //this.rBody.gravityScale = 0; //-> Modify in Start Method

            rBody.velocity = force.normalized * ArrowPower;
            //or [Used AddForce]
            //this.rBody.AddForce(force, ForceMode2D.Force);
            //rBody.AddForce(force, ForceMode2D.Impulse); // -> Recommend

            //Trail Clear  
            arrowTrail.gameObject.SetActive(true);
            arrowTrail.Clear();

            //Enable Collider -> Allow Collision
            polyCollider.enabled = true;
        }

        /// <summary>
        /// Shot with Target Position
        /// </summary>
        /// <param name="target"></param>
        public void ShotToTarget(Vector3 target)
        {
            rBody.isKinematic = false;

            //Change Rotation to Target Position
            arrowTr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, target - arrowTr.position).eulerAngles.z);
            isLaunched = true;

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            rBody.velocity = arrowTr.up * ArrowPower;

            //Clear TrailRender
            arrowTrail.gameObject.SetActive(true);
            arrowTrail.Clear();
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="target"></param>
        public void ForceToTarget(Vector3 target)
        {
            arrowTr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, target - arrowTr.position).eulerAngles.z);
            rBody.velocity = arrowTr.up * ArrowPower;

            //Clear Arrow Trail
            arrowTrail.Clear();
        }

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

        public void ShotToDirectly(Vector2 force, DamageStruct damage) {
            throw new System.NotImplementedException();
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
