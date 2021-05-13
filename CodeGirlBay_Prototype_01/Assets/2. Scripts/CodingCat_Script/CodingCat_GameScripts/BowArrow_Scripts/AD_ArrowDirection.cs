namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    public class AD_ArrowDirection : MonoBehaviour
    {
        //Arrow Destroy Variable
        private Vector2 topLeftScreenPoint;
        private Vector2 bottomRightScreenPoint;
        private Vector2 offset = new Vector2(8, 8);
        private bool xIn, yIn;  //The x-Position inside Screen, y-Position inside Screen Flags.

        //Arrow Angle Calculate Variable
        private Vector2 arrowPosition;
        private Vector2 velocity;
        private Rigidbody2D arrowRigidBody;
        private float arrowAngle = 0f;

        //Object Pool Parent
        private GameObject arrowPoolObject;

        private void Start()
        {
            //Screen top-left, bottom-right 계산
            topLeftScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height));
            bottomRightScreenPoint = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0));

            if(arrowRigidBody == null)
            {
                arrowRigidBody = gameObject.GetComponent<Rigidbody2D>();  
            }
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

        //arrow Position is Out of Screen, then Destroy the Arrow (When Arrow Firing).
        private void CheckArrowBounds()
        {
            //Get the Position of the Arrow
            arrowPosition = transform.position;

            xIn = (arrowPosition.x >= topLeftScreenPoint.x - offset.x && arrowPosition.x <= bottomRightScreenPoint.x + offset.x);
            yIn = (arrowPosition.y >= bottomRightScreenPoint.y - offset.y && arrowPosition.y <= topLeftScreenPoint.y + offset.y);

            if(!(xIn && yIn))
            {
                //gameObject.SetActive(false);

                //Disable the Current Arrow
                this.DisableArrow();
                return;
            }
        }

        /// <summary>
        /// 화면밖으로 나가거나 충돌되어 Disable 처리되어야할 상황에서 Initialize 처리
        /// </summary>
        private void DisableArrow()
        {
            //RigidBody Kinematic, Trail Disable
            this.arrowRigidBody.isKinematic = true;
            this.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
