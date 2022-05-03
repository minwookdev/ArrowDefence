namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class SwapSlots : MonoBehaviour, IPointerDownHandler {
        [Header("COMPONENT")]
        //[SerializeField] RectTransform slotBodyTr  = null;
        [SerializeField] RectTransform slotGroupTr = null;

        [Header("SLOT OPTIONS")]
        [SerializeField] [RangeEx(1f, 5f, 1f)] float slotOpenDuration = 1f;
        [SerializeField] [RangeEx(0.1f, 2f, 0.1f)] float slotMovingTime = 1f;
        [SerializeField] bool isUseUnscaledTime = false; // <-- 일시정지 로직이랑 논리 충돌하니까 지금은 켜지말 것
        float currOpenedTime = 0f;
        bool isOpen = true;
        float openPosX;
        float closePosX;

        [Header("SLOTS")]
        [SerializeField] EventTrigger[] slots = null;

        [Header("SP SLOT ADDS")]
        [SerializeField] Image[] stackImages = null;
        [SerializeField] Image costImage     = null;

        [Header("Notify")]
        [SerializeField] SlotNotify notify = null;

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
                    isOpen = false;
                    TweenClose();
                }
            }
            else {
                if (isUseUnscaledTime) currOpenedTime -= Time.unscaledDeltaTime;
                else                   currOpenedTime -= Time.deltaTime;

                if(isOpen == false) {
                    isOpen = true;
                    TweenOpen();
                }
            }
        }

        private void OnDisable() {
            slotGroupTr.DOKill(complete:true);
            if (notify.IsPlaying()) {
                notify.Stop();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            TimerReset();
        }

        void TimerReset() {
            currOpenedTime = slotOpenDuration;
            if (notify.IsPlaying()) {
                notify.Stop();
            }
        }

        void TweenOpen() {
            slotGroupTr.DOLocalMoveX(openPosX, slotMovingTime);
        }

        void TweenClose() {
            slotGroupTr.DOLocalMoveX(closePosX, slotMovingTime);
        }

        #region INITALIZE

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
                    case 0: type = ARROWTYPE.ARROW_MAIN;                   break;
                    case 1: type = ARROWTYPE.ARROW_SUB;                    break;
                    case 2: type = ARROWTYPE.ARROW_SPECIAL; notify.Init(); break;
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

        #region SPECIAL

        public void SSlotUpdateCost(float value) {
            costImage.fillAmount = value;   
        }

        /// <summary>
        /// Enable Stack Images
        /// </summary>
        /// <param name="count"> 0 ~ 2 </param>
        public void SSSlotUpdateStack(int count) {
            var ImageLenght = stackImages.Length;
            if (count > ImageLenght || count < 0) {
                return;
            }

            //Enable Count of Image
            for (int i = 0; i < count; i++) {
                stackImages[i].gameObject.SetActive(true);
                ImageLenght--;
            }

            //Disable unuse Image
            if (ImageLenght > 0) {
                for (int i = stackImages.Length - 1; i >= stackImages.Length - ImageLenght; i--) {
                    stackImages[i].gameObject.SetActive(false);
                }
            }
        }

        public void PlayNotify() {
            if (isOpen) {
                return;
            }

            notify.Play();
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

    [System.Serializable]
    internal sealed class SlotNotify {
        [Header("SLOT NOTIFY")]
        [SerializeField] RectTransform notifyRectTr = null;
        [SerializeField] CanvasGroup notifyCanvasGroup = null;

        byte repeatCount;
        float interval;
        Vector3 fadeInScale  = new Vector3(1f, 1f, 1f);
        Vector3 fadeOutScale = new Vector3(1.2f, 1.2f, 1f);
        Sequence notifySequence = null;

        public void Init(byte repeat = 2, float intervalTime = 0.3f, bool isDefaultTimeScale = true) {
            repeatCount = repeat;
            interval    = intervalTime;
            notifySequence = DOTween.Sequence();
            notifySequence.SetAutoKill(false).SetUpdate(isDefaultTimeScale).Pause();
            for (int i = 0; i < repeatCount; i++) {
                notifySequence.Append(notifyCanvasGroup.DOFade(0.7f, interval))
                              .Join(notifyRectTr.DOScale(fadeOutScale, interval))
                              .Append(notifyCanvasGroup.DOFade(StNum.floatZero, interval))
                              .Join(notifyRectTr.DOScale(fadeInScale, interval));
            }
        }

        public void Play() {
            if(notifySequence == null) {
                throw new System.Exception("Slot Notify Sequence is Null.");
            }

            notifySequence.Restart();
        }

        public void Stop() {
            notifySequence.Complete();
        }

        public bool IsPlaying() {
            if(notifySequence == null) {
                return false;
            }

            return notifySequence.IsPlaying();
        }
    }
}
