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

        [Header("SHOOTING")]
        public float ArrowPower;
        [ReadOnly] public bool isLaunched;

        //Controll Arrow Position (Before Launched)
        private Vector3 arrowPosition;

        //Launch Power for the Arrow
        //private float powerFactor = 2000;
        private Rigidbody2D rBody;
        private PolygonCollider2D polyCollider;

        private void Start()
        {
            //if (ReferenceEquals(rBody, null)) rBody = gameObject.GetComponent<Rigidbody2D
            //Initial Arrow Childs
            if (arrowChatchPoint == null) arrowChatchPoint = transform.GetChild(2);
            if (arrowTrail == null) arrowTrail = transform.GetChild(2).GetChild(0).GetComponent<TrailRenderer>();
            rBody = gameObject.GetComponent<Rigidbody2D>();
            rBody.gravityScale = 0f;

            if (polyCollider == null) polyCollider = transform.GetChild(0).GetComponent<PolygonCollider2D>();
            polyCollider.enabled = false;

            //Init-Arrow Skill
            
        }

        private void Update()
        {
            if (!isLaunched)
            {
                ClampPosition();
                //CalculatePower();
            }
        }

        private void OnDisable() => this.isLaunched = false;

        private void ClampPosition()
        {
            //Get the Current Position of the Arrow
            arrowPosition = transform.position;
            //Clamp the X Y position Between min and Max Points
            arrowPosition.x = Mathf.Clamp(arrowPosition.x, Mathf.Min(rightClampPoint.position.x, leftClampPoint.position.x),
                                                           Mathf.Max(rightClampPoint.position.x, leftClampPoint.position.x));
            arrowPosition.y = Mathf.Clamp(arrowPosition.y, Mathf.Min(rightClampPoint.position.y, leftClampPoint.position.y),
                                                           Mathf.Max(rightClampPoint.position.y, leftClampPoint.position.y));

            //Set new Position for the Arrow
            transform.position = arrowPosition;
        }

        private void CalculatePower()
        {
            //this.power = Vector2.Distance(transform.position, rightClampPoint.position) * powerFactor;
        }

        public void ShotArrow(Vector2 force, Transform parent)
        {
            //부모바꿔준 상태에서 발사
            //발사되고 난 뒤에 SetParent로 Canvas의 Child로 바꿔주지 않으면 활 각도 돌릴때마다 자식으로 취급되서 날아가면서 화살각도가 휘어버린다
            //발사할 때는 보정 필요함 뒤에 false 붙이면 이상한 곳에서 날아감;
            transform.SetParent(parent);

            this.rBody.isKinematic = false;
            //this.rBody.gravityScale = 0;
            this.isLaunched = true;
            //Force to Arrow RigidBody 
            rBody.velocity = force;
            //or [Used AddForce]
            //this.rBody.AddForce(force, ForceMode2D.Force);
            //rBody.AddForce(force, ForceMode2D.Impulse); // -> Recommend

            //발사할 때 Clear 해주지 않으면 전에 있던 잔상이 남는다
            arrowTrail.gameObject.SetActive(true);
            arrowTrail.Clear();

            //Poly Collider가 활성되는 순간 충돌 가능
            polyCollider.enabled = true;
        }

        #region PROPERTIES

        public void OnDisableCollider() => this.polyCollider.enabled = false;

        public void DisableRequest(GameObject target)
        {
            rBody.isKinematic = true;
            arrowTrail.gameObject.SetActive(false);
            polyCollider.enabled = false;

            CCPooler.ReturnToPool(target, 0);

            //객체가 Disable되기전에 SetParent메서드로 부모객체가 바뀌어버리면 
            //보통은 스케일과 좌표가 난리가 난다. 어떤 객체던 SetParent하기전에 Disable후 부모를 바꿔줄 것.
            //(현재는 CCPooler에 비활성화를 요청하도록 로직 변경)
        }

        #endregion

        void OnTriggerEnter2D(Collider2D coll)
        {
            if(coll.gameObject.layer == LayerMask.NameToLayer(AD_Data.LAYER_MONSTER))
            {

            }
        }

        void OnHit(GameObject target)
        {
            target.SendMessage("OnHitObject", Random.Range(30f, 50f), SendMessageOptions.DontRequireReceiver);
            DisableRequest(gameObject);
        }

        public void ShotArrow(Vector2 force)
        {

        }

        public void ShotArrow(Vector3 target)
        {

        }

        public void ForceArrow(Vector3 target)
        {

        }
    }
}
