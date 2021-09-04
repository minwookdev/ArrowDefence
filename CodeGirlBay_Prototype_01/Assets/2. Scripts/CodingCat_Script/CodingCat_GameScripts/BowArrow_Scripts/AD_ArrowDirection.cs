namespace CodingCat_Games
{
    using UnityEngine;

    public class AD_ArrowDirection : MonoBehaviour
    {
        //Screen Limit Variable
        private Vector2 topLeftScreenPoint;
        private Vector2 bottomRightScreenPoint;
        private Vector2 offset = new Vector2(2, 2);
        private bool xIn, yIn;  //The x-Position inside Screen, y-Position inside Screen Flags.

        //Arrow Angle Calculate Variable
        private Vector2 arrowPosition;
        private Vector2 velocity;
        private Rigidbody2D arrowRigidBody;
        private float arrowAngle = 0f;
        private AD_Arrow adArrow;

        private void Start()
        {
            //Screen top-left, bottom-right 계산
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));
            arrowRigidBody         = gameObject.GetComponent<Rigidbody2D>();
            adArrow                = gameObject.GetComponent<AD_Arrow>();
        }

        private void Update()
        {
            //Get Velocity of the Arrow
            velocity = arrowRigidBody.velocity;

            if(velocity.magnitude != 0 && !arrowRigidBody.isKinematic)
            {
                //Calculate the angle if the arrow
                this.arrowAngle = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
                //Set Rotation of the Arrow
                transform.rotation = Quaternion.AngleAxis(arrowAngle, transform.forward);
                //Check the Arrow Bounds
                this.CheckArrowBounds();
            }
        }

        private void OnDisable()
        {
            //CCPooler.ReturnToPool(this.gameObject, AD_Data.ARROW_MAIN);
        }

        //arrow Position is Out of Screen, then Destroy the Arrow (When Arrow Firing).
        private void CheckArrowBounds()
        {
            //Get the Position of the Arrow
            arrowPosition = transform.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            if(!(xIn && yIn))
            {
                //화면 밖으로 나가면 Disable
                this.DisableArrow();
                return;
            }
        }

        /// <summary>
        /// 화면밖으로 나가거나 충돌되어 Disable 처리되어야할 상황에서 각종 변수들 초기화 처리
        /// </summary>
        private void DisableArrow()
        {
            this.arrowRigidBody.isKinematic = true;
            adArrow.arrowTrail.gameObject.SetActive(false);
            CCPooler.ReturnToPool(this.gameObject, 0);

            //gameObject.SetActive(false);
            //Disable 하기전에 SerParent 하면 스케일이랑 좌표 난리난다. 항상 SetParent할 경우 Disable 후에 부모바꿔줄것.
            //CCPooler.ReturnToPool 실행으로 비활성화 요청
        }
    }
}
