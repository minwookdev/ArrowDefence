namespace ActionCat
{
    using UnityEngine;

    public class AD_Arrow_less : MonoBehaviour, IPoolObject
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

        //Arrow Skill Action
        System.Action onHit;
        System.Action onAir;

        private void Awake()
        {
            //Init-Component
            rBody       = gameObject.GetComponent<Rigidbody2D>();
            tr          = gameObject.GetComponent<Transform>();
            trailObject = transform.GetChild(2).GetChild(0).gameObject;

            //Init-Arrow Skill
            onHit += OnHit;
            onAir += OnAir;
        }

        private void Start()
        {
            //Init Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));
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

                onAir();
                CheckArrowBounds();
            }
        }

        //private void OnDisable() => this.isLaunched = false;
        private void OnDisable()
        {
            this.rBody.velocity = Vector2.zero;
            this.trailObject.SetActive(false);
            this.isLaunched = false;
        }

        private void OnDestroy()
        {
            onAir -= OnAir;
            onHit -= OnHit;
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

        void OnHit() => DisableObject_Req(this.gameObject);

        void OnAir()
        {
            //Calc Arrow Angle
            arrowAngle  = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180); //transform.up?
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);
        }

        public void DisableObject_Req(GameObject target) => CCPooler.ReturnToPool(target, 0);

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

        private void OnCollisionEnter2D(Collision2D coll)
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER))
            {
                //DisableObject_Req(this.gameObject);
                onHit();
            }
        }
    }
}
