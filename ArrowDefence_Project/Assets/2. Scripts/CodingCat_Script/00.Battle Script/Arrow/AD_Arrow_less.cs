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
        ArrowSkillSet arrowSkillSets  = null;
        bool isInitSkill              = false;
        bool isDisableArrow           = false;


        private void Awake()
        {
            //Init-Component
            rBody       = gameObject.GetComponent<Rigidbody2D>();
            tr          = gameObject.GetComponent<Transform>();
        }

        private void Start()
        {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            //Init-Arrow Skill with Pool-tag params
            arrowSkillSets = GameManager.Instance.GetArrowSkillSets(gameObject.name);
            if (arrowSkillSets != null)
            {
                isInitSkill = true;
                arrowSkillSets.Init(tr, rBody, this);
            }
        }

        private void Update()
        {
            velocity = rBody.velocity;

            if(velocity.magnitude != 0 && isLaunched == true)
            {
                OnAir();
                CheckArrowBounds();
            }
        }

        private void OnDisable()
        {
            rBody.velocity = Vector2.zero;
            trailRender.gameObject.SetActive(false);
            isLaunched = false;
        }

        private void OnDestroy()
        {
            arrowSkillSets = null;
        }

        private void CheckArrowBounds()
        {
            arrowPosition = tr.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            if (!(xIn && yIn))
            {
                //DisableArrow();
                DisableRequest(this.gameObject);
                return;
            }
        }

        void OnHit(GameObject target)
        {
            target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            DisableRequest(gameObject);
        }

        void OnAir()
        {
            //Calc Arrow Angle
            arrowAngle  = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);
        }

        public void DisableRequest(GameObject target) => CCPooler.ReturnToPool(target, 0);

        /// <summary>
        /// Shot Directly to Direction
        /// </summary>
        /// <param name="force">applied as normalized</param>
        public void ShotArrow(Vector2 force)
        {
            isLaunched = true;
            //Force to Arrow RigidBody
            rBody.velocity = force.normalized * forceMagnitude;
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
        public void ShotArrow(Vector3 targetPosition)
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
        public void ForceArrow(Vector3 target)
        {
            //Rotate Direction to target position
            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, target - tr.position).eulerAngles.z);

            //Fires an arrow rotated in the direction of the target with force in a straight line.
            rBody.velocity = tr.up * forceMagnitude;
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER))
            {
                if (isInitSkill)
                {
                    isDisableArrow = arrowSkillSets.OnHit(coll);
                    if (isDisableArrow)
                        DisableRequest(gameObject);
                }
                else
                    OnHit(coll.gameObject);
            }
        }
    }
}
