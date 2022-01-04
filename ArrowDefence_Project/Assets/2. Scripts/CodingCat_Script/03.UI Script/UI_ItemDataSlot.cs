namespace ActionCat
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UI_ItemDataSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("ITEM DATA SLOT")]
        [SerializeField]
        RectTransform slotRect = null;
        public Image ItemImg;
        public Image ItemFrame;
        public TextMeshProUGUI ItemStackTmp;
        public Sprite[] Frames;
        [SerializeField] [Tooltip("is Show Item Grade Frame")]
        bool isShowGrade = true;

        [Header("EVENT")]
        [SerializeField]
        Vector3 touchedScale = new Vector3(0.9f, 0.9f, 1f);
        Vector3 normalScale  = new Vector3(1f, 1f, 1f);
        [SerializeField] [Range(1f, 3f, order = 1)]
        float scaleSpeed = 1f;

        private ItemData itemDataAddress = null;

        [Header("SHOW TOOLTIP")]
        public float TooltipOpenPressedTime = .3f;

        //※ Build하고 두개의 DataSlot을 동시에 터치했을때 어떻게되는지 확인하고 Tooltip 띄워주는 방식 최적화※
        //Tooltip Variables
        private Vector2 tooltipPoint;
        private RectTransform tooltipParent;
        private Camera uiCamera;
        private float pressedTime;
        private bool isTimeStart   = false;
        private bool isToolTipOpen = false;

        //is Data Slot Pressed
        bool isPressed = false;

        private void Start() {
            pressedTime = TooltipOpenPressedTime;
        }

        private void Update() {
            if(isTimeStart) {
                if (pressedTime >= 0) pressedTime -= Time.deltaTime;
                else {
                    ActionCat.Games.UI.ItemTooltip.Inst.Expose(tooltipPoint, tooltipParent, 
                                                                   itemDataAddress.Item_Name, itemDataAddress.Item_Desc, 
                                                                   this.gameObject, uiCamera);
                    isToolTipOpen = true;
                    isTimeStart   = false;
                }
            }

            //====================================[ PRESSED SLOT ]========================================
            if(isPressed == true) {
                if(slotRect.localScale != touchedScale) {
                    slotRect.localScale = Vector3.MoveTowards(slotRect.localScale, touchedScale, Time.unscaledDeltaTime * scaleSpeed);
                }
            }
            //====================================[ RELEASE SLOT ]========================================
            else {
                if(slotRect.localScale != normalScale) {
                    slotRect.localScale = Vector3.MoveTowards(slotRect.localScale, normalScale, Time.unscaledDeltaTime * scaleSpeed);
                }
            }
        }

        public void Setup(ItemData address, int visibleStack, Canvas tooltipTargetCanvas, Camera uiCamera) {
            itemDataAddress = address;

            ItemImg.sprite = address.Item_Sprite;

            if (address.Item_Type != ITEMTYPE.ITEM_EQUIPMENT)
            {
                if (ItemStackTmp.gameObject.activeSelf == false)
                    ItemStackTmp.gameObject.SetActive(true);
                ItemStackTmp.text = visibleStack.ToString();
            }
            else ItemStackTmp.gameObject.SetActive(false);

            //Set Item Frame according to Item Grade
            ItemFrame.sprite = Frames[(int)address.Item_Grade];

            //Init Tooltip Variables
            this.tooltipParent = tooltipTargetCanvas.GetComponent<RectTransform>();
            this.uiCamera          = uiCamera;
        }

        public void SlotEnable(ItemData data, int visibleStack, Canvas targetcanvas, Camera targetCam) {
            itemDataAddress = data;
            ItemImg.sprite  = data.Item_Sprite;

            if(data.Item_Type != ITEMTYPE.ITEM_EQUIPMENT) {
                if (ItemStackTmp.gameObject.activeSelf == false)
                    ItemStackTmp.gameObject.SetActive(true);
                ItemStackTmp.text = visibleStack.ToString();
            }
            else { //Equipment Type Item Data. No visible item Stack.
                ItemStackTmp.gameObject.SetActive(false);
            }

            //Set Item Frame [Item Grade]
            if(isShowGrade == true) {
                ItemFrame.sprite = Frames[(int)data.Item_Grade];
            }

            //Init Tooltip variables
            tooltipParent = targetcanvas.GetComponent<RectTransform>();
            uiCamera      = targetCam;
        }

        public void Clear() {
            ItemStackTmp.text = "";
            if(isShowGrade) {
                ItemFrame.sprite = Frames[0];
            }
            
            this.gameObject.SetActive(false);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData data) {
            //Data Slot is Pressed.
            isPressed = true;

            //slotRect.localScale = touchedScale;
            //slotRect.localScale = Vector3.MoveTowards(slotRect.localScale, touchedScale, Time.unscaledDeltaTime * scaleSpeed);

            //Origin
            //if (isToolTipOpen) return;
            //
            //if (isTimeStart == false) {
            //    RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipCanvasRect, data.position, uiCamera, out tooltipPoint);
            //    pressedTime   = TooltipOpenPressedTime;
            //    isTimeStart   = true;
            //}
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData data) {
            //Release Pointer
            isPressed = false;

            //slotRect.localScale = normalScale;

            //Origin
            //if(isToolTipOpen)
            //{
            //    ActionCat.Games.UI.ItemTooltip.Instance.Hide(this.gameObject);
            //    isToolTipOpen = false;
            //}
            //
            //isTimeStart   = false;
            //pressedTime   = TooltipOpenPressedTime;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(tooltipParent, slotRect.anchoredPosition, uiCamera, out Vector2 point);

            //Expose ItemData Type Tooltip
            Games.UI.ItemTooltip.Inst.ItemTooltipExpose(slotRect.position, tooltipParent, itemDataAddress, ItemFrame.sprite);
            CatLog.Log($"Sending Position X : {slotRect.position.x}, Y : {slotRect.position.y}");
            CatLog.Log($"Sending LocalPos X : {slotRect.localPosition.x}, Y : {slotRect.localPosition.y}");
            CatLog.Log($"Sending Anchored X : {slotRect.anchoredPosition.x}, Y : {slotRect.anchoredPosition.y}");
            //CatLog.Log($"{slotRect.SetSizeWithCurrentAnchors}");
            //slotRect.anchore
        }
    }
}
