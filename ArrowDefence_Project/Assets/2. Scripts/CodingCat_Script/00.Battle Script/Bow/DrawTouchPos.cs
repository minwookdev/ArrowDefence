namespace ActionCat
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class DrawTouchPos : MonoBehaviour
    {
        public static DrawTouchPos Instance;

        [Header("DRAW TOUCH POSITION")]
        public Sprite CenterPivotSprite;
        public Sprite CurrentTouchPosSprite;
        public Material LineRenderMaterial;

        [Header("DRAW LINE GAMEOBJECT")]
        public Image ObjectCenterPivotImage;
        public Image ObjectTouchPosImage;
        public LineRenderer ObjectLineRender;

        [Header("LINE VARIABLES")]
        [Range(0.1f, 0.001f)] public float LineWidth = 0.025f;

        [Header("COLOR")]
        public float ColorChangeDuration = 1f;
        public Color EnableColor;
        public Color DisableColor;

        private bool isInitialize  = false;
        private bool isDrawLine    = false;
        private bool isColored     = false;
        private float colorTime = 0f;
        Color tempColor;
        Color colorAlphaZero;
        Vector3 tempPivotPos;
        Vector3 tempTouchPos;

        private void Awake() => Instance = this;

        private void Start()
        {
            isInitialize = (CenterPivotSprite     == null ||
                            CurrentTouchPosSprite == null ||
                            LineRenderMaterial    == null) ? false : true;

            if (!isInitialize)
            {
                CatLog.WLog("Draw Touch Pos is Not Initialized");
                return;
            }
            else //Initialize Objects
            {
                if (ObjectCenterPivotImage == null) ObjectCenterPivotImage = transform.GetChild(0).GetComponent<Image>();
                if (ObjectTouchPosImage == null)    ObjectTouchPosImage = transform.GetChild(1).GetComponent<Image>();
                if (ObjectLineRender == null)       ObjectLineRender = transform.GetChild(2).GetComponent<LineRenderer>();
            }

            //Initialize Sprite
            ObjectCenterPivotImage.sprite = CenterPivotSprite;
            ObjectTouchPosImage.sprite    = CurrentTouchPosSprite;

            //Initialize LineRender
            ObjectLineRender.startWidth       = LineWidth;
            ObjectLineRender.endWidth         = LineWidth;
            ObjectLineRender.material         = LineRenderMaterial;
            ObjectLineRender.sortingLayerName = "Object:Bow";
            ObjectLineRender.sortingOrder     = 0;

            //Initialize Color
            if (ObjectCenterPivotImage.color.a != 0f)
                ObjectCenterPivotImage.color = new Color(ObjectCenterPivotImage.color.r, ObjectCenterPivotImage.color.g, ObjectCenterPivotImage.color.b, 0f);
            if (ObjectTouchPosImage.color.a != 0f)
                ObjectTouchPosImage.color = new Color(ObjectTouchPosImage.color.r, ObjectTouchPosImage.color.g, ObjectTouchPosImage.color.b, 0f);
            if (ObjectLineRender.startColor.a != 0f)
                ObjectLineRender.startColor = new Color(ObjectLineRender.startColor.r, ObjectLineRender.startColor.g, ObjectLineRender.startColor.b, 0f);
            if (ObjectLineRender.endColor.a != 0f)
                ObjectLineRender.endColor = new Color(ObjectLineRender.endColor.r, ObjectLineRender.endColor.g, ObjectLineRender.endColor.b, 0f);

            colorAlphaZero = DisableColor;
            colorAlphaZero.a = 0f;
        }

        private void FixedUpdate() => DrawLine();

        #region CONTROLLER_USED

        /// <summary>
        /// Draw LineRenderer between StartPos and EndPos on Screen
        /// </summary>
        /// <param name="startPos">Start Position is Moveable Vector3</param>
        /// <param name="endPos">End Position is Static Vector</param>
        public void DrawTouchLine(Vector3 startPos, Vector3 endPos)
        {
            if (isInitialize == false) return;

            //Update Temp Position
            FixPos(startPos, endPos);

            ObjectCenterPivotImage.transform.position = tempPivotPos;
            ObjectTouchPosImage.transform.position    = tempTouchPos;
            isDrawLine = true;
        }

        /// <summary>
        /// Erase LineRenderer on Screen
        /// </summary>
        public void ReleaseTouchLine()
        {
            if (isInitialize == false) return;
            isDrawLine = false;
        }

        /// <summary>
        /// (Apply Color) Draw LineRenderer between StartPos and EndPos on Screen
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <param name="targetColor"></param>
        public void DrawTouchLine(Vector3 startPos, Vector3 endPos, bool isEnable)
        {
            if (isInitialize == false) return;

            //Update Temp Position
            FixPos(startPos, endPos);

            ObjectCenterPivotImage.transform.position = tempPivotPos;
            ObjectTouchPosImage.transform.position    = tempTouchPos;
            isDrawLine = true;

            //Lerp Color
            LerpColor(isEnable);
        }

        #endregion

        private void DrawLine()
        {
            if(isDrawLine)
            {
                ObjectLineRender.SetPosition(0, ObjectTouchPosImage.transform.position);
                ObjectLineRender.SetPosition(1, ObjectCenterPivotImage.transform.position);

                EnableObjectsAlpha();
            }
            else
            {
                DisableObjectsAlpha();
            }
        }

        private void EnableObjectsAlpha()
        {
            if (ObjectCenterPivotImage.color.a <= 0)
            {
                tempColor = ObjectCenterPivotImage.color;
                tempColor.a = 1f;
                ObjectCenterPivotImage.color = tempColor;
            }

            if (ObjectTouchPosImage.color.a <= 0)
            {
                tempColor = ObjectTouchPosImage.color;
                tempColor.a = 1f;
                ObjectTouchPosImage.color = tempColor;
            }

            if (ObjectLineRender.startColor.a <= 0)
            {
                tempColor = ObjectLineRender.startColor;
                tempColor.a = 1f;
                ObjectLineRender.startColor = tempColor;
            }

            if (ObjectLineRender.endColor.a <= 0)
            {
                tempColor = ObjectLineRender.endColor;
                tempColor.a = 1f;
                ObjectLineRender.endColor = tempColor;
            }
        }

        private void DisableObjectsAlpha()
        {
            ObjectCenterPivotImage.color = colorAlphaZero;
            ObjectTouchPosImage.color    = colorAlphaZero;
            ObjectLineRender.startColor  = colorAlphaZero;
            ObjectLineRender.endColor    = colorAlphaZero;
        }

        private void ChangeColor(bool isEnabled)
        {
            ObjectCenterPivotImage.color = (isEnabled) ? Color.Lerp(ObjectCenterPivotImage.color, EnableColor, colorTime) : 
                                                         Color.Lerp(ObjectCenterPivotImage.color, DisableColor, colorTime);
            ObjectTouchPosImage.color    = (isEnabled) ? Color.Lerp(ObjectTouchPosImage.color, EnableColor, colorTime) :
                                                         Color.Lerp(ObjectTouchPosImage.color, DisableColor, colorTime);
            ObjectLineRender.startColor  = (isEnabled) ? Color.Lerp(ObjectLineRender.startColor, EnableColor, colorTime) :
                                                         Color.Lerp(ObjectLineRender.startColor, DisableColor, colorTime);
            ObjectLineRender.endColor    = (isEnabled) ? Color.Lerp(ObjectLineRender.endColor, EnableColor, colorTime) :
                                                         Color.Lerp(ObjectLineRender.endColor, DisableColor, colorTime);

            if (colorTime < ColorChangeDuration)
            {
                colorTime += Time.deltaTime / ColorChangeDuration;
            }
            else
            {
                if(isEnabled)
                {
                    if      (ObjectCenterPivotImage.color != EnableColor) colorTime = 0f;
                    else if (ObjectTouchPosImage.color != EnableColor)    colorTime = 0f;
                    else if (ObjectLineRender.startColor != EnableColor)  colorTime = 0f;
                    else if (ObjectLineRender.endColor != EnableColor)    colorTime = 0f;
                }
                else
                {
                    if      (ObjectCenterPivotImage.color != DisableColor) colorTime = 0f;
                    else if (ObjectTouchPosImage.color != DisableColor)    colorTime = 0f;
                    else if (ObjectLineRender.startColor != DisableColor)  colorTime = 0f;
                    else if (ObjectLineRender.endColor != DisableColor)    colorTime = 0f;
                }
            }
        }

        private void LerpColor(bool isEnable) {
            if (isEnable) {
                if (isColored == true) {
                    ObjectCenterPivotImage.DOColor(EnableColor, ColorChangeDuration).SetUpdate(true);
                    ObjectTouchPosImage.DOColor(EnableColor, ColorChangeDuration).SetUpdate(true);
                    ObjectLineRender.DOColor(new Color2(ObjectLineRender.startColor, ObjectLineRender.endColor), 
                                             new Color2(EnableColor, EnableColor), ColorChangeDuration)
                                    .OnStart(() => isColored = false)
                                    .SetUpdate(true);
                }
            }
            else {
                if(isColored == false) {
                    ObjectCenterPivotImage.DOColor(DisableColor, ColorChangeDuration).SetUpdate(true);
                    ObjectTouchPosImage.DOColor(DisableColor, ColorChangeDuration).SetUpdate(true);
                    ObjectLineRender.DOColor(new Color2(ObjectLineRender.startColor, ObjectLineRender.endColor), 
                                             new Color2(DisableColor, DisableColor), ColorChangeDuration)
                                    .OnStart(() => isColored = true)
                                    .SetUpdate(true);
                }
            }
        }

        void FixPos(Vector3 pivotPos, Vector3 touchPos) {
            tempPivotPos = new Vector3(pivotPos.x, pivotPos.y, 0f);
            tempTouchPos = new Vector3(touchPos.x, touchPos.y, 0f);
        }
    }
}
