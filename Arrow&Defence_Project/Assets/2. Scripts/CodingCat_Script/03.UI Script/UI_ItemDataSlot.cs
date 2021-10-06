namespace CodingCat_Games
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;
    using CodingCat_Scripts;

    public class UI_ItemDataSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDeselectHandler
    {
        [Header("ITEM DATA SLOT")]
        public Image ItemImg;
        public Image ItemFrame;
        public TextMeshProUGUI ItemStackTmp;
        public Sprite[] Frames;

        private ItemData itemDataAddress = null;

        [Header("SHOW TOOLTIP")]
        public float TooltipOpenPressedTime = 1f;

        //※ Build하고 두개의 DataSlot을 동시에 터치했을때 어떻게되는지 확인하고 Tooltip 띄워주는 방식 최적화※
        private Vector2 tooltipPoint;
        private Canvas tooltipParent;
        private float pressedTime;
        private bool isTimeStart   = false;
        private bool isToolTipOpen = false;

        private void Start()
        {
            pressedTime = TooltipOpenPressedTime;
        }

        private void Update()
        {
            if(isTimeStart)
            {
                if (pressedTime >= 0) pressedTime -= Time.deltaTime;
                else
                {
                    ActionCat.Games.UI.ItemTooltip.Instance.Expose(tooltipPoint, tooltipParent, itemDataAddress.Item_Name, itemDataAddress.Item_Desc);
                    isToolTipOpen = true;
                    isTimeStart   = false;
                }
            }
        }

        public void Setup(ItemData address, int visibleStack, Canvas tooltipTargetCanvas)
        {
            itemDataAddress = address;

            ItemImg.sprite = address.Item_Sprite;
            tooltipParent  = tooltipTargetCanvas;

            if (address.Item_Type != ITEMTYPE.ITEM_EQUIPMENT)
            {
                if (ItemStackTmp.gameObject.activeSelf == false)
                    ItemStackTmp.gameObject.SetActive(true);
                ItemStackTmp.text = visibleStack.ToString();
            }
            else ItemStackTmp.gameObject.SetActive(false);

            //Set Item Frame according to Item Grade
            ItemFrame.sprite = Frames[(int)address.Item_Grade];
        }

        public void Clear()
        {
            ItemStackTmp.text = "";
            ItemFrame.sprite = Frames[0];

            this.gameObject.SetActive(false);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            if (isTimeStart == false)
            {
                tooltipPoint  = eventData.position;
                pressedTime   = TooltipOpenPressedTime;
                isTimeStart   = true;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            if(isToolTipOpen)
            {
                ActionCat.Games.UI.ItemTooltip.Instance.Hide();
                isToolTipOpen = false;
            }

            isTimeStart   = false;
            pressedTime   = TooltipOpenPressedTime;
        }

        void IDeselectHandler.OnDeselect(BaseEventData eventData)
        {
            CatLog.Log("DeSeleted Called !");
        }
    }
}
