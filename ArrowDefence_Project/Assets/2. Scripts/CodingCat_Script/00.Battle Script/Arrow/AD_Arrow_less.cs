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

        [SerializeField]
        private GameObject trailObject;

        [SerializeField]
        private Rigidbody2D rBody;

        //Arrow Skill Data
        ArrowSkillSet arrowSkillSets  = null;
        bool isInitSkill              = false;


        private void Awake()
        {
            //Init-Component
            rBody       = gameObject.GetComponent<Rigidbody2D>();
            tr          = gameObject.GetComponent<Transform>();
            trailObject = transform.GetChild(2).GetChild(0).gameObject;
        }

        private void Start()
        {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            //Init-Arrow Skill
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
                //Calculate the angle if the arrow
                //arrowAngle = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
                //Set Rotation of the Arrow
                //transform.rotation = Quaternion.AngleAxis(arrowAngle, transform.forward);

                //if (isInitSkill)
                //    arrowSkill.OnAir();
                //else
                //    OnAir();

                OnAir();
                CheckArrowBounds();
            }
        }

        private void OnDisable()
        {
            this.rBody.velocity = Vector2.zero;
            this.trailObject.SetActive(false);
            this.isLaunched = false;
        }

        private void OnDestroy()
        {
            arrowSkillSets = null;
        }

        private void CheckArrowBounds()
        {
            arrowPosition = transform.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            if (!(xIn && yIn))
            {
                //DisableArrow();
                DisableObject_Req(this.gameObject);
                return;
            }
        }

        void OnHit(GameObject target)
        {
            target.SendMessage("OnHitObject", Random.Range(10f, 30f), SendMessageOptions.DontRequireReceiver);
            DisableObject_Req(gameObject);
        }

        void OnAir()
        {
            //Calc Arrow Angle
            arrowAngle  = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180); //transform.up?
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);
        }

        public void DisableObject_Req(GameObject target) => CCPooler.ReturnToPool(target, 0);

        /// <summary>
        /// Shot Arrow
        /// </summary>
        /// <param name="force"></param>
        public void ShotArrow(Vector2 force)
        {
            isLaunched = true;
            //Force to Arrow RigidBody
            rBody.velocity = force;
            //or [Used AddForce]
            //rBody.AddForce(force, ForceMode2D.Impulse); //-> recommend
            //rBody.AddForce(force, ForceMode2D.Force);

            trailObject.SetActive(true);
            trailObject.GetComponent<TrailRenderer>().Clear();
        }

        /// <summary>
        /// Shot Arrow With Rotate
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="force"></param>
        public void ShotArrow(Vector3 targetPos, Vector2 force)
        {
            //isLaunched  = false;
            //tr.rotation = rotation;
            //transform.rotation = rotation;

            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, targetPos - tr.position).eulerAngles.z);
            
            isLaunched     = true;
            //rBody.velocity = force;
            rBody.velocity = tr.up * 18f;

            //force 받고있던게 문제네..입력 받을당시의 force는 Vector2인 Struct이고 값 복사 일어나니까 무조건 정면이라는걸 생각못했다
            
            trailObject.SetActive(true);
            trailObject.GetComponent<TrailRenderer>().Clear();
            //Trail 미리 캐싱해놓기 -> trailObject를 TrailRenderer 컴포넌트로 가지고 있음 안되나?

            //Start Coroutine
            //StartCoroutine(ShotArrowWithTarget(targetPos, force));
        }

        IEnumerator ShotArrowWithTarget(Vector3 target, Vector2 force)
        {
            //방향을 잡아줘버리기도 전에 들어가버려서 그렇다
            tr.rotation = Quaternion.Euler(0f, 0f, Quaternion.FromToRotation(Vector3.up, target - tr.position).eulerAngles.z);

            //1 Frame Wait
            yield return null;

            isLaunched = true;
            rBody.velocity = force;

            trailObject.SetActive(true);
            trailObject.GetComponent<TrailRenderer>().Clear();
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            //Change Logic -> Trigger Check
            //if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER))
            //{
            //    //if (isInitSkill)
            //    //    arrowSkill.OnHit();
            //    //else
            //    //    CatLog.Log("Arrow Skill is NULL !");
            //    OnHit(coll.gameObject);
            //}
        }

        private void OnTriggerEnter2D(Collider2D coll)
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER))
            {
                if (isInitSkill)
                {
                    //isLaunched = false; //Handling islaunched False for Arrow object Rotate.
                    bool isDisableArrow = arrowSkillSets.OnHit(coll, this);
                    if (isDisableArrow)
                        DisableObject_Req(gameObject);
                }
                else
                    OnHit(coll.gameObject);
            }
        }
    }
}
