namespace ActionCat
{
    using System.Collections;
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

        //Arrow Skill Data
        DamageStruct damageStruct;
        ArrowSkillSet arrowSkillSets = null;
        bool isInitSkill = false;
        bool isCollision = false;


        private void Awake()
        {
            //Init-Component
            rBody = gameObject.GetComponent<Rigidbody2D>();
            tr    = gameObject.GetComponent<Transform>();
        }

        private void Start()
        {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            //Init-Arrow Skill with Pool-tag params
            arrowSkillSets = GameManager.Instance.GetArrowSkillSets(gameObject.name);
            if (arrowSkillSets != null) {
                isInitSkill = true;
                arrowSkillSets.Init(tr, rBody, this);
            }
        }

        void OnEnable() => isCollision = false;

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
                CheckArrowBounds();
                if (isInitSkill == true)
                    arrowSkillSets.OnUpdate();
            }
        }

        private void FixedUpdate()
        {
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

        private void CheckArrowBounds()
        {
            arrowPosition = tr.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            //Out of Screen
            if (!(xIn && yIn)) {
                DisableRequest();
                return;
            }
        }

        void OnHit(GameObject target) {
            isCollision = true;
            //target.SendMessage(nameof(IDamageable.OnHitObject), Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            target.GetComponent<IDamageable>().OnHitObject(ref damageStruct);
            DisableRequest();
        }

        void CalcAngle()
        {
            //Calc Arrow Angle
            arrowAngle  = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);

            ///Description
            ///AD_Arrow에 작성함.
        }

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target, 0);

        public void DisableRequest() => CCPooler.ReturnToPool(gameObject, 0);

        /// <summary>
        /// Shot Directly to Direction
        /// </summary>
        /// <param name="force">applied as normalized</param>
        public void ShotToDirectly(Vector2 direction, DamageStruct damage) {
            isLaunched = true;
            //Force to Arrow RigidBody
            rBody.velocity = direction.normalized * forceMagnitude;
            //or [Used AddForce]
            //rBody.AddForce(force, ForceMode2D.Impulse); //-> recommend
            //rBody.AddForce(force, ForceMode2D.Force);
            //Save Force.magnitude

            //Get Damage Struct
            damageStruct = damage;

            //Clear TrailRender
            trailRender.gameObject.SetActive(true);
            trailRender.Clear();
        }

        public void ShotToDirectly(Vector2 direction) {
            isLaunched = true;
            //Force to Arrow RigidBody
            rBody.velocity = direction.normalized * forceMagnitude;
            //or [Used AddForce]
            //rBody.AddForce(force, ForceMode2D.Impulse); //-> recommend
            //rBody.AddForce(force, ForceMode2D.Force);
            //Save Force.magnitude

            //Clear TrailRender
            trailRender.gameObject.SetActive(true);
            trailRender.Clear();
        }

        /// <summary>
        /// Shot with Target Position
        /// </summary>
        /// <param name="targetPosition">Shot to Target Position</param>
        public void ShotToTarget(Vector3 targetPosition)
        {
            //Rotate the Arrow Angle
            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPosition - tr.position).eulerAngles.z);

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            isLaunched     = true;
            rBody.velocity = tr.up * forceMagnitude;
            
            //Clear TrailRender
            trailRender.gameObject.SetActive(true); 
            trailRender.Clear();

            ///Shot Arrow 메서드 호출받을 당시에 arrowTransform.up * force 로 받고있었기 때문에,
            ///Arrow Object가 회전되기 전 force를 받고있었고, 
            ///force를 받아서 OnAir 메서드가 호출되어버렸기 때문에 
            ///tr.rotation에 할당한 Quaternion 값이 제대로 적용되지 못했다
            ///라고 봐야할것 같다. -> struct의 문제라 판단중 본체 Arrow의 Speed값을 따로 저장해놓고 있어야하겠다.
        }

        /// <summary>
        /// The Shot method used by the Skill Class.
        /// </summary>
        /// <param name="target"></param>
        public void ForceToTarget(Vector3 target)
        {
            //Rotate Direction to target position
            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, target - tr.position).eulerAngles.z);

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            rBody.velocity = tr.up * forceMagnitude;
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                if (isInitSkill) {
                    if(arrowSkillSets.OnHit(coll, ref damageStruct) == true) {
                        DisableRequest();
                    }
                }
                else {
                    if(isCollision == false) {
                        OnHit(coll.gameObject);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D coll) {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER)) {
                if(isInitSkill) {
                    arrowSkillSets.OnExit(coll);
                }
            }
        }


    }
}
