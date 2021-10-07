namespace CodingCat_Games
{
    using CodingCat_Scripts;
    using UnityEngine;
    using UnityEngine.UI;

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

        private bool isInitialize  = false;
        private bool isDrawLine    = false;
        private Color tempColor;

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
        }

        private void FixedUpdate() => DrawLine();

        /// <summary>
        /// Draw LineRenderer between StartPos and EndPos on Screen
        /// </summary>
        /// <param name="startPos">Start Position is Moveable Vector3</param>
        /// <param name="endPos">End Position is Static Vector</param>
        public void DrawTouchLine(Vector3 startPos, Vector3 endPos)
        {
            if (isInitialize == false) return;

            ObjectCenterPivotImage.transform.position = GameGlobal.FixedVectorOnScreen(startPos);
            ObjectTouchPosImage.transform.position    = GameGlobal.FixedVectorOnScreen(endPos);
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

        private void DrawLine()
        {
            if(isDrawLine)
            {
                ObjectLineRender.SetPosition(0, GameGlobal.FixedVectorOnScreen(ObjectTouchPosImage.transform.position));
                ObjectLineRender.SetPosition(1, GameGlobal.FixedVectorOnScreen(ObjectCenterPivotImage.transform.position));

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
            if (ObjectCenterPivotImage.color.a > 0)
            {
                tempColor = ObjectCenterPivotImage.color;
                tempColor.a = 0f;
                ObjectCenterPivotImage.color = tempColor;
            }

            if(ObjectTouchPosImage.color.a > 0)
            {
                tempColor = ObjectTouchPosImage.color;
                tempColor.a = 0f;
                ObjectTouchPosImage.color = tempColor;
            }

            if(ObjectLineRender.startColor.a > 0)
            {
                tempColor = ObjectLineRender.startColor;
                tempColor.a = 0f;
                ObjectLineRender.startColor = tempColor;
            }

            if(ObjectLineRender.endColor.a > 0)
            {
                tempColor = ObjectLineRender.endColor;
                tempColor.a = 0f;
                ObjectLineRender.endColor = tempColor;
            }
        }
    }
}
