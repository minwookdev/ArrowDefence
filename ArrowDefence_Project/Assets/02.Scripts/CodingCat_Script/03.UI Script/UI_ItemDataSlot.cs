namespace ActionCat
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UI_ItemDataSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [Header("ITEM DATA SLOT")]
        [SerializeField] RectTransform slotRectTr = null;
        [SerializeField] Image ItemImg;
        [SerializeField] Image ItemFrame;
        [SerializeField] TextMeshProUGUI ItemStackTmp;
        [SerializeField] Sprite[] Frames;
        [Tooltip("is Show Item Grade Frame")]
        [SerializeField] bool isShowGrade = true;

        [Header("TOUCH SCALE")]
        [SerializeField] Vector3 touchedScale = new Vector3(0.9f, 0.9f, 1f);
        [SerializeField] [Range(1f, 3f, order = 1)]
        float scaleSpeed = 1f;

        [Header("TOOLTIP")]
        [SerializeField] private RectTransform tooltipParent;

        //Fields
        bool isPressed = false;
        Vector3 normalScale;
        ItemData itemDataAddress = null;

        private void Start() {
            //Save RectTransform Scale
            normalScale = slotRectTr.localScale;
        }

        private void Update() {
            //====================================[ PRESSED SLOT ]========================================
            if(isPressed == true) {
                if(slotRectTr.localScale != touchedScale) {
                    slotRectTr.localScale = Vector3.MoveTowards(slotRectTr.localScale, touchedScale, Time.unscaledDeltaTime * scaleSpeed);
                }
            }
            //====================================[ RELEASE SLOT ]========================================
            else {
                if(slotRectTr.localScale != normalScale) {
                    slotRectTr.localScale = Vector3.MoveTowards(slotRectTr.localScale, normalScale, Time.unscaledDeltaTime * scaleSpeed);
                }
            }
        }

        /// <summary>
        /// Enable Item Data Slot
        /// </summary>
        /// <param name="data"></param>
        /// <param name="stack"></param>
        /// <param name="rootParent">Root Parent Transform for Tooltip</param>
        public void SetSlot(ItemData data, int stack, RectTransform rootParent) {
            itemDataAddress = data;
            ItemImg.sprite  = data.Item_Sprite;

            //Set Visible Item Stack <Not Disable Item Stack Text>
            ItemStackTmp.text = (data.Item_Type == ITEMTYPE.ITEM_EQUIPMENT) ? GameGlobal.EMPTYSTR : stack.ToString();

            if(isShowGrade == true) {
                ItemFrame.sprite = Frames[(int)data.Item_Grade];
            }

            //Get Parent Canvas (to use tooltip)
            tooltipParent = rootParent;

            if(gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Clear Not Uesd Data Slot
        /// </summary>
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
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData data) {
            //Release 
            isPressed = false;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            //Expose ItemData Type Tooltip
            Games.UI.ItemTooltip.Inst.Expose(slotRectTr.position, tooltipParent, itemDataAddress, ItemFrame.sprite, ItemStackTmp.text);
        }

        #region v1

        //public void Setup(ItemData address, int visibleStack, Canvas tooltipTargetCanvas, Camera uiCamera) {
        //    itemDataAddress = address;
        //
        //    ItemImg.sprite = address.Item_Sprite;
        //
        //    if (address.Item_Type != ITEMTYPE.ITEM_EQUIPMENT)
        //    {
        //        ItemStackTmp.text = visibleStack.ToString();
        //    }
        //    else
        //    {
        //        ItemStackTmp.text = GameGlobal.EMPTYSTR;
        //    }
        //
        //    //Set Item Frame according to Item Grade
        //    ItemFrame.sprite = Frames[(int)address.Item_Grade];
        //
        //    //Init Tooltip Variables
        //    this.tooltipParent = tooltipTargetCanvas.GetComponent<RectTransform>();
        //    this.uiCamera = uiCamera;
        //}
        //
        //public void SlotEnable(ItemData data, int visibleStack, Canvas targetcanvas, Camera targetCam) {
        //    itemDataAddress = data;
        //    ItemImg.sprite = data.Item_Sprite;
        //
        //    if (data.Item_Type != ITEMTYPE.ITEM_EQUIPMENT)
        //    {
        //        if (ItemStackTmp.gameObject.activeSelf == false)
        //            ItemStackTmp.gameObject.SetActive(true);
        //        ItemStackTmp.text = visibleStack.ToString();
        //    }
        //    else
        //    { //Equipment Type Item Data. No visible item Stack.
        //        ItemStackTmp.gameObject.SetActive(false);
        //    }
        //
        //    //Set Item Frame [Item Grade]
        //    if (isShowGrade == true)
        //    {
        //        ItemFrame.sprite = Frames[(int)data.Item_Grade];
        //    }
        //
        //    //Init Tooltip variables
        //    tooltipParent = transform.parent.GetComponent<RectTransform>();
        //    uiCamera = targetCam;
        //}

        #endregion
    }
}
