namespace ActionCat
{
    using UnityEngine;

    public class SPEffect_AimSight : MonoBehaviour
    {
        //Ray Casting Variables
        private AD_BowController bowController;
        private LineRenderer laserLine;
        private Transform LineStartPoint;
        private float rayDistance = 10f;

        //Line Render Variables
        public Material lineRenderMat;
        private float lineEndAlpha = 0.3f;
        private float lineStartAlpha = 0.8f;
        private float lineRenderWidth;
        private float alphaChangeSpeed = 0.8f;
        Color currentStartColor;
        Color currentEndColor;

        /// <summary>
        /// Executed before the Start method is called. Setting Linerenderer Related Variables
        /// </summary>
        /// <param name="lineMat">Material LineRenderer</param>
        /// <param name="lineWidth">LineRenderer Width (Recommen [0.010 ~ 0.5])</param>
        public void Initialize(Material lineMat, float lineWidth)
        {
            //Initial LineRender Variables
            lineRenderMat   = lineMat;
            lineRenderWidth = lineWidth;
        }

        private void Start()
        {
            //Initial LineRender
            gameObject.AddComponent<LineRenderer>();
            laserLine = GetComponent<LineRenderer>();
            laserLine.startColor = new Color(255f, 0f, 0f, 0f);
            laserLine.endColor   = new Color(255f, 0f, 0f, 0f);
            laserLine.startWidth = lineRenderWidth; laserLine.endWidth = lineRenderWidth;
            laserLine.material   = lineRenderMat;
            laserLine.sortingLayerName = "Object:Bow";
            laserLine.sortingOrder = 0;  

            //Initial Bow Controller
            bowController = GetComponent<AD_BowController>();

            //Initial Line Start Position
            LineStartPoint = bowController.ClampPointBottom;
        }

        private void Update()
        {
            if (bowController.IsBowPulling)
            {
                RaycastHit2D rayhit = Physics2D.Raycast(LineStartPoint.position, LineStartPoint.right, rayDistance);
                //Debug.DrawRay(LineStartPoint.position, LineStartPoint.right * rayDistance, Color.green); //Debugging

                laserLine.SetPosition(0, LineStartPoint.position);

                if (rayhit)  //무언가에 부딫혔을때 **문제되었던 부분 정리하기
                {
                    if (rayhit.collider.CompareTag(AD_Data.OBJECT_TAG_MONSTER))
                        laserLine.SetPosition(1, GameGlobal.FixedVectorOnScreen(rayhit.point));
                    else laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));
                }
                else laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));

                IncreaseLineAlpha();
            }
            else
            {
                DecreaseLineAlpha();
            }
        }

        private void FixedUpdate()
        {
            //if (bowController.IsBowPullBegan)
            //{
            //    RaycastHit2D rayhit = Physics2D.Raycast(LineStartPoint.position, LineStartPoint.right, rayDistance);
            //    //Debug.DrawRay(LineStartPoint.position, LineStartPoint.right * rayDistance, Color.green); //Debugging
            //
            //    laserLine.SetPosition(0, LineStartPoint.position);
            //
            //    if (rayhit)  //무언가에 부딫혔을때 **문제되었던 부분 정리하기
            //    {
            //        if (rayhit.collider.CompareTag(AD_Data.OBJECT_TAG_MONSTER)) laserLine.SetPosition(1, GameGlobal.FixedVectorOnScreen(rayhit.point));
            //        else                                                        laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));
            //    }
            //    else laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));
            //
            //    IncreaseLineAlpha();
            //}
            //else
            //{
            //    DecreaseLineAlpha();
            //}

            //방법은 찾았는데 왜 되고 내꺼는 안되는지 모르겠다
        }

        /// <summary>
        /// Decrease Alpha Value in Laser Liner
        /// </summary>
        private void DecreaseLineAlpha()
        {
            if(laserLine.startColor.a > 0f)
            {
                //currentStartColor    = laserLine.startColor;
                //currentStartColor.a -= Time.deltaTime * alphaChangeSpeed;
                //laserLine.startColor = currentStartColor;

                //Pulling 상태가 끝나면 바로 사라지도록 수정
                currentStartColor    = laserLine.startColor;
                currentStartColor.a  = 0f;
                laserLine.startColor = currentStartColor;
            }
            
            if(laserLine.endColor.a > 0f)
            {
                //currentEndColor    = laserLine.endColor;
                //currentEndColor.a -= Time.deltaTime * alphaChangeSpeed;
                //laserLine.endColor = currentEndColor;

                currentEndColor    = laserLine.endColor;
                currentEndColor.a  = 0f;
                laserLine.endColor = currentEndColor;
            }
        }

        /// <summary>
        /// Increase Alpha Value in Laser Liner
        /// </summary>
        private void IncreaseLineAlpha()
        {
            if (laserLine.startColor.a < lineStartAlpha)
            {
                currentStartColor = laserLine.startColor;
                currentStartColor.a += Time.unscaledDeltaTime * alphaChangeSpeed;
                laserLine.startColor = currentStartColor;
            }

            if (laserLine.endColor.a < lineEndAlpha)
            {
                currentEndColor = laserLine.endColor;
                currentEndColor.a += Time.unscaledDeltaTime * alphaChangeSpeed;
                laserLine.endColor = currentEndColor;
            }
        }
    }
}
