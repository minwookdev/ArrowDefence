namespace CodingCat_Games
{
    using CodingCat_Scripts;
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
            LineStartPoint = bowController.leftClampPoint;
        }

        private void FixedUpdate()
        {
            if (bowController.BowPullBegan)
            {
                #region OLD
                //RaycastHit hit;
                //
                //laserLine.SetPosition(0, LineStartPoint.position);
                //
                ////Physics2D.Raycast(LineStartPoint.position, LineStartPoint.right, out hit, rayDistance)
                //
                //if (Physics.Raycast(LineStartPoint.position, LineStartPoint.right, out hit, rayDistance))
                //{
                //    laserLine.SetPosition(1, hit.point);
                //    CatLog.Log("무언가에 부딫히고 있음");
                //}
                //else
                //{
                //    //Vector3 endPos = LineStartPoint.right * lineRenderDist;
                //    //endPos.z = lineRenderZpos;
                //    //laserLine.SetPosition(1, endPos);
                //
                //    //이 방법으로 Line의 Position을 정해주니까 제대로 작동한다 -> 줄어들거나 하는 현상없이 (근데 왜 그러는거지)
                //    laserLine.SetPosition(1, LineStartPoint.position + (transform.right * rayDistance));
                //}
                #endregion

                RaycastHit2D rayhit = Physics2D.Raycast(LineStartPoint.position, LineStartPoint.right, rayDistance);
                //Debug.DrawRay(LineStartPoint.position, LineStartPoint.right * rayDistance, Color.green); //Debugging

                laserLine.SetPosition(0, LineStartPoint.position);

                if (rayhit)  //무언가에 부딫혔을때 **문제되었던 부분 정리하기
                {
                    if (rayhit.collider.CompareTag(AD_Data.OBJECT_TAG_MONSTER)) laserLine.SetPosition(1, GameGlobal.FixedVectorOnScreen(rayhit.point));
                    else                                                        laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));
                }
                else laserLine.SetPosition(1, LineStartPoint.position + (LineStartPoint.right * rayDistance));

                IncreaseLineAlpha();
            }
            else
            {
                DecreaseLineAlpha();
            }

            //방법은 찾았는데 왜 되고 내꺼는 안되는지 모르겠다
        }

        /// <summary>
        /// Decrease Alpha Value in Laser Liner
        /// </summary>
        private void DecreaseLineAlpha()
        {
            //if(laserLine.startColor.a > 0f || laserLine.endColor.a > 0f)
            //{
            //    var lineColor = laserLine.startColor;
            //    lineColor.a -= Time.deltaTime * alphaChangeSpeed;
            //    laserLine.startColor = lineColor;
            //    laserLine.endColor   = lineColor;
            //}

            if(laserLine.startColor.a > 0f)
            {
                var lineColor = laserLine.startColor;
                lineColor.a -= Time.deltaTime * alphaChangeSpeed;
                laserLine.startColor = lineColor;
            }
            
            if(laserLine.endColor.a > 0f)
            {
                var lineColor = laserLine.endColor;
                lineColor.a -= Time.deltaTime * alphaChangeSpeed;
                laserLine.endColor = lineColor;
            }
        }

        /// <summary>
        /// Increase Alpha Value in Laser Liner
        /// </summary>
        private void IncreaseLineAlpha()
        {
            //if(laserLine.startColor.a < 1f || laserLine.endColor.a < 1f)
            //{
            //    var lineColor = laserLine.startColor;
            //    lineColor.a += Time.deltaTime * alphaChangeSpeed;
            //    laserLine.startColor = lineColor;
            //    laserLine.endColor   = lineColor;
            //}

            if (laserLine.startColor.a < lineStartAlpha)
            {
                var lineColor = laserLine.startColor;
                lineColor.a += Time.deltaTime * alphaChangeSpeed;
                laserLine.startColor = lineColor;
            }

            if (laserLine.endColor.a < lineEndAlpha)
            {
                var lineColor = laserLine.endColor;
                lineColor.a += Time.deltaTime * alphaChangeSpeed;
                laserLine.endColor = lineColor;
            }
        }
    }
}
