namespace ActionCat {
    using UnityEngine;

    public class SPEffect_AimSight : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] private LineRenderer lineRender = null;
        [SerializeField] private Material lineMaterial   = null;
        [SerializeField] private Color matColor;
        [SerializeField] [ReadOnly]
        private Transform lineStartPoint = null;
        [SerializeField] [ReadOnly] 
        private AD_BowController controller = null;

        [Header("SIGHT")]
        [SerializeField] [RangeEx(10f, 30f, 1f)]
        private float rayDist = 10f;
        [SerializeField] [ReadOnly]
        private bool isOn = false;
        private float timeElapsed  = 0f;
        private float lerpDuration = 1f;
        private float valueToLerp;


        float lineEndAlpha   = 0.3f;
        float lineStartAlpha = 0.8f;
        float lineRenderWidth;
        float alphaChangeSpeed   = 0.8f;
        string shaderAlphaString = "_Alpha";
        string shaderColorString = "_Color";
        Color currentStartColor;
        Color currentEndColor;

        /// <summary>
        /// Executed before the Start method is called. Setting Linerenderer Related Variables
        /// </summary>
        /// <param name="lineMat">Material LineRenderer</param>
        /// <param name="lineWidth">LineRenderer Width (Recommen [0.010 ~ 0.5])</param>
        public void Initialize(AD_BowController bow) {
            //Material Set
            //lineMaterial    = lineMat;
            //lineRenderWidth = lineWidth;

            controller     = bow;
            lineStartPoint = controller.ClampPointBottom;
        }

        private void Start() {
            //Initial LineRender
            //gameObject.AddComponent<LineRenderer>();
            //lineRender = GetComponent<LineRenderer>();
            //lineRender.startColor = new Color(255f, 0f, 0f, 0f);
            //lineRender.endColor   = new Color(255f, 0f, 0f, 0f);
            //lineRender.startWidth = lineRenderWidth; lineRender.endWidth = lineRenderWidth;
            //lineRender.material   = lineMaterial;

            //LineRenderer Sorting
            lineRender.sortingLayerName = "Object:Bow";
            lineRender.sortingOrder     = 0;

            //LineRenderer Alpha Clear
            lineMaterial.SetFloat(shaderAlphaString, 0f);

            //Initial Bow Controller
            //controller = GetComponent<AD_BowController>();

            //Initial Line Start Position
            //lineStartPoint = controller.ClampPointBottom;
        }

        private void Update() {
            if (controller.IsStatePulling() == true) {
                if(isOn == false) {
                    timeElapsed = 0f;
                    isOn = true;
                }

                RaycastHit2D rayhit = Physics2D.Raycast(lineStartPoint.position, lineStartPoint.right, rayDist);
                //Debug.DrawRay(LineStartPoint.position, LineStartPoint.right * rayDistance, Color.green); //Debugging

                lineRender.SetPosition(0, lineStartPoint.position);

                if (rayhit) { // raycast collision issue ** Read Note
                    if (rayhit.collider.CompareTag(AD_Data.OBJECT_TAG_MONSTER))
                        lineRender.SetPosition(1, GameGlobal.FixedVectorOnScreen(rayhit.point));
                    else lineRender.SetPosition(1, lineStartPoint.position + (lineStartPoint.right * rayDist));
                }
                else lineRender.SetPosition(1, lineStartPoint.position + (lineStartPoint.right * rayDist));

                //Line Alpha to 1
                DrawLineAlpha();
            }
            else {
                if(isOn == true) {
                    timeElapsed = 0f;
                    isOn = false;
                }

                //Line Alpha to 0
                EraseLineAlpha();
            }
        }

        /// <summary>
        /// Decrease Alpha Value in Laser Liner
        /// </summary>
        private void DecreaseLineAlpha() {
            if(lineRender.startColor.a > 0f) {
                //Pulling 상태가 끝나면 바로 사라지도록 수정
                currentStartColor    = lineRender.startColor;
                currentStartColor.a  = 0f;
                lineRender.startColor = currentStartColor;
            }
            
            if(lineRender.endColor.a > 0f) {
                currentEndColor    = lineRender.endColor;
                currentEndColor.a  = 0f;
                lineRender.endColor = currentEndColor;
            }
        }

        /// <summary>
        /// Increase Alpha Value in Laser Liner
        /// </summary>
        private void IncreaseLineAlpha() {
            if (lineRender.startColor.a < lineStartAlpha) {
                currentStartColor = lineRender.startColor;
                currentStartColor.a += Time.unscaledDeltaTime * alphaChangeSpeed;
                lineRender.startColor = currentStartColor;
            }

            if (lineRender.endColor.a < lineEndAlpha) {
                currentEndColor = lineRender.endColor;
                currentEndColor.a += Time.unscaledDeltaTime * alphaChangeSpeed;
                lineRender.endColor = currentEndColor;
            }
        }

        void DrawLineAlpha() {
            if(timeElapsed < lerpDuration) {
                lineMaterial.SetFloat(shaderAlphaString, Mathf.Lerp(lineMaterial.GetFloat(shaderAlphaString), 1f, timeElapsed / lerpDuration));
                timeElapsed += Time.unscaledDeltaTime;
            }
            else {
                lineMaterial.SetFloat(shaderAlphaString, 1f);
            }
        }

        void EraseLineAlpha() {
            if(timeElapsed < lerpDuration) {
                lineMaterial.SetFloat(shaderAlphaString, Mathf.Lerp(lineMaterial.GetFloat(shaderAlphaString), 0f, timeElapsed / lerpDuration));
                timeElapsed += Time.unscaledDeltaTime;
            }
            else {
                lineMaterial.SetFloat(shaderAlphaString, 0f);
            }
        }
    }
}
