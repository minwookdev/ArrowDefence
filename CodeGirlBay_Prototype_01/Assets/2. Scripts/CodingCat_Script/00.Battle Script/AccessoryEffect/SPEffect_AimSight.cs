namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;

    public class SPEffect_AimSight : MonoBehaviour
    {
        public Material lineRenderMat;

        private LineRenderer laserLine;
        private AD_BowController bowController;
        private Transform LineStartPoint;

        private float rayDistance = 10f;
        private float lineRenderDist = 1000f;
        private float lineRenderZpos = 89f;
        private float lineRenderWidthValue = 0.1f;

        public void Initialize()
        {
            //CatLog.Log("Initialize AimSight Component Success"); //잘 들어오는거 확인됨
        }

        private void Start()
        {
            //Initial LineRender
            gameObject.AddComponent<LineRenderer>();
            laserLine = GetComponent<LineRenderer>();
            laserLine.material = AD_BowRope.instance.ropeMaterial;
            laserLine.startWidth = lineRenderWidthValue;
            laserLine.endWidth   = lineRenderWidthValue;
            laserLine.endColor = new Color(laserLine.endColor.r, laserLine.endColor.g, laserLine.endColor.b, 0.5f);
            laserLine.sortingLayerName = "Object:Bow";
            laserLine.sortingOrder = 0;
            laserLine.useWorldSpace = false;    

            //Initial Bow Controller
            bowController = GetComponent<AD_BowController>();

            //Initial Line Start Position
            LineStartPoint = bowController.leftClampPoint;
        }

        private void FixedUpdate()
        {
            if(bowController.BowPullBegan)
            {
                //CatLog.Log("Check Bow Pull Began !");

                Vector3 startPos = LineStartPoint.position;
                RaycastHit hit;

                laserLine.SetPosition(0, startPos);

                if(Physics.Raycast(startPos, LineStartPoint.right, out hit, rayDistance))
                {
                    laserLine.SetPosition(1, hit.point);
                    CatLog.Log("무언가에 부딫히고 있음");
                }
                else
                {
                    Vector3 endPos = LineStartPoint.right * lineRenderDist;
                    endPos.z = lineRenderZpos;
                    laserLine.SetPosition(1, endPos);
                }
            }
            else
            {
                laserLine.SetPosition(0, LineStartPoint.position);
                laserLine.SetPosition(1, LineStartPoint.position);
            }
        }
    }
}
