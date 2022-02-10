namespace ActionCat
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
        private Transform tr;

        //Main Arrow Component
        private AD_Arrow adArrow;

        private void Start() {
            //Init-Component
            tr = GetComponent<Transform>();

            //Init-Screen top-left, bottom-right
            topLeftScreenPoint     = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));
            arrowRigidBody         = gameObject.GetComponent<Rigidbody2D>();
            adArrow                = gameObject.GetComponent<AD_Arrow>();
        }

        private void Update() {
            //Get Velocity of the Arrow //CalcAngle Verriables
            velocity = arrowRigidBody.velocity;

            //When Arrow Fired
            if(velocity.magnitude != 0 && arrowRigidBody.isKinematic == false) {
                CheckArrowBounds(); //Check the Arrow Bounds
                //CalcAngle();      //Arrow Direction Update
            }
        }

        void CalcAngle() {
            //Calculate the angle if the arrow
            arrowAngle = (Mathf.Atan2(velocity.x, -velocity.y) * Mathf.Rad2Deg + 180);
            //Set Rotation of the Arrow
            tr.rotation = Quaternion.AngleAxis(arrowAngle, tr.forward);

            ///Description
            ///이게 아마 예전 버전의 프로젝트에서 현재 rigidbody의 velocity direction에 따라서
            ///화살이 사출되고, 중력에 따라 화살이 포물선을 그리도록 헤드 파트부터 떨어지도록 하는 코드 일 것이다
            ///현재는 사용되지 않는다.
        }

        /// <summary>
        /// arrow Position is Out of Screen, then Destroy the Arrow (When Arrow Firing).
        /// </summary>
        private void CheckArrowBounds()
        {
            //Get the Position of the Arrow
            arrowPosition = tr.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            //Arrow Out of Screen
            if(!(xIn && yIn)) {
                adArrow.DisableRequest();
                return;
            }
        }

    }

}
