namespace CodingCat_Scripts
{
    using CodingCat_Games;
    using DG.Tweening;
    using UnityEngine;

    public class AD_Arrow : MonoBehaviour
    {
        //The Left, Right Clamp Point for the Arrow.
        public Transform leftClampPoint, rightClampPoint;
        public Transform arrowChatchPoint;
        public GameObject arrowTrail;

        [HideInInspector]
        public bool isLaunched;
        [HideInInspector]
        public float power;

        //Controll Arrow Position (Before Launched)
        private Vector3 arrowPosition;

        //Launch Power for the Arrow
        private float powerFactor = 2000;
        private TrailRenderer trail;

        //Arrow Attributes
        //private AD_GameScripts.ArrowAttrubute arrowAttribute;

        private void Start()
        {
            //Set Normal Arrow (TEST)
            //arrowAttribute = AD_GameScripts.ArrowAttrubute.Arrow_Normal;


            //Initial Arrow Childs
            if (arrowChatchPoint == null) arrowChatchPoint = transform.GetChild(2);
            if (arrowTrail == null)       arrowTrail = transform.GetChild(2).GetChild(0).gameObject;
        }

        private void Update()
        {
            if (!isLaunched)
            {
                ClampPosition();
                CalculatePower();
            }
        }

        private void OnDisable()
        {
            if(this.isLaunched) isLaunched = false;
        }

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
            this.power = Vector2.Distance(transform.position, rightClampPoint.position) * powerFactor;
        }

        public void DestroyArrow()
        {
            Destroy(this.gameObject);
        }
    }
}
