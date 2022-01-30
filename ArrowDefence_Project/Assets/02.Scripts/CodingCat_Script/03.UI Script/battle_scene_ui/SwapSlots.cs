namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class SwapSlots : MonoBehaviour, IPointerDownHandler {
        [Header("COMPONENT")]
        [SerializeField] RectTransform slotBodyTr  = null;
        [SerializeField] RectTransform slotGroupTr = null;

        [Header("SLOT OPTIONS")]
        [SerializeField] [RangeEx(1f, 5f, 1f)] float slotOpenDuration = 1f;
        [SerializeField] [RangeEx(0.1f, 2f, 0.1f)] float slotMovingTime = 1f;
        [SerializeField] bool isUseUnscaledTime = false;
        float currOpenedTime = 0f;
        bool isOpen = true;
        float openPosX;
        float closePosX;

        [Header("SLOTS")]
        [SerializeField] EventTrigger[] slots = null;

        void Start() {
            currOpenedTime = slotOpenDuration;
            openPosX  = slotGroupTr.anchoredPosition.x;
            closePosX = slotGroupTr.anchoredPosition.x - slotGroupTr.rect.width;

            //Create OpenEntry
            var openEntry = new EventTrigger.Entry();
            openEntry.eventID = EventTriggerType.PointerDown;
            openEntry.callback.AddListener(eventData => TimerReset());

            //Add OpenEntry in Slot Triggers
            foreach (var slot in slots) {
                slot.triggers.Add(openEntry);
            }
        }

        void Update() {
            if(currOpenedTime <= 0) {
                if(isOpen == true) {
                    TweenClose();
                    isOpen = false;
                }
            }
            else {
                if (isUseUnscaledTime) currOpenedTime -= Time.unscaledDeltaTime;
                else                   currOpenedTime -= Time.deltaTime;

                if(isOpen == false) {
                    TweenOpen();
                    isOpen = true;
                }
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            TimerReset();
        }

        void TimerReset() {
            currOpenedTime = slotOpenDuration;
        }

        void TweenOpen() {
            slotGroupTr.DOLocalMoveX(openPosX, slotMovingTime);
        }

        void TweenClose() {
            slotGroupTr.DOLocalMoveX(closePosX, slotMovingTime);
        }

        #region SLOTS

        public void InitSlots(ArrSSData[] array) {
            for (int i = 0; i < array.Length; i++) {
                if(array[i].IsActive == false) {
                    slots[i].gameObject.SetActive(false);
                    continue;
                }

                //Set Arr Icon Image
                var iconimage = slots[i].transform.GetChild(0).GetComponent<Image>();
                iconimage.enabled        = true;
                iconimage.preserveAspect = true;
                iconimage.sprite         = array[i].ArrIconSprite;

                //Set Click Listner
                int captureNum = i;
                var type = ARROWTYPE.NONE;
                switch (captureNum) {   //Get Arrow Type
                    case 0: type = ARROWTYPE.ARROW_MAIN;    break;
                    case 1: type = ARROWTYPE.ARROW_SUB;     break;
                    case 2: type = ARROWTYPE.ARROW_SPECIAL; break;
                }
                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener(evnetData => array[captureNum].slotCallback(type));
                slots[i].triggers.Add(entry);

                if (slots[i].gameObject.activeSelf == false)
                    slots[i].gameObject.SetActive(true);
            }
        }

        #endregion
    }

    public sealed class ArrSSData {
        public bool IsActive { get; private set; } = false;
        public Sprite ArrIconSprite { get; private set; } = null;
        public System.Action<ARROWTYPE> slotCallback { get; private set; } = null;

        public ArrSSData(bool isactive, Sprite iconsprite = null, System.Action<ARROWTYPE> callback = null) {
            IsActive      = isactive;
            ArrIconSprite = iconsprite;
            slotCallback   = callback;
        }
    }

}
