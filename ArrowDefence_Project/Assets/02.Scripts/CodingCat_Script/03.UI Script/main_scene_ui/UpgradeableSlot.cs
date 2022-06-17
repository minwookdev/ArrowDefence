namespace ActionCat {
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class UpgradeableSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler, IEndDragHandler, IBeginDragHandler, IDragHandler {
        [Header("COMPONENT")]
        [SerializeField] ScrollRect scrollView = null;
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] RectTransform selectedImageRectTr = null;
        [SerializeField] Image imageItemIcon  = null;
        [SerializeField] Image imageItemFrame = null;
        [SerializeField] Sprite[] frames = null;
        private Action<UpgradeableSlot, bool, AD_item> actionSlotSelect = null;
        private Action<AD_item> actionOpenInfoPanel  = null;
        private AD_item itemRef = null;

        [Header("STATE")]
        [SerializeField] [ReadOnly] private bool isSelected = false;
        [SerializeField] [ReadOnly] private bool isPressed  = false;
        [SerializeField] [ReadOnly] private float currentTouchTime = 0f;
        private float maxTouchTime = 1.5f;

        [Header("SOUND COMPATIBILITY")]
        [SerializeField] bool isSoundCompatibility = false;
        [SerializeField] CHANNELTYPE channelKey    = CHANNELTYPE.NONE;
        [SerializeField] Audio.ACSound channel     = null;

        //DOSCALE
        float scalingTime = 0.3f;
        Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f);
        Vector3 normalScale  = new Vector3(1f, 1f, 1f);
        Vector3 tempScale;

        public bool IsActive {
            get {
                return gameObject.activeSelf;
            }
        }

        public void SetAction(Action<AD_item> openpanelaction, Action<UpgradeableSlot, bool, AD_item> slotaction) {
            this.actionOpenInfoPanel = openpanelaction;
            this.actionSlotSelect    = slotaction;
        }

        private void Update() {
            if(isPressed == true) {
                currentTouchTime += Time.unscaledDeltaTime;

                //MaxTime이상 Touching하고 있으면 Select 판별
                if(currentTouchTime >= maxTouchTime) {
                    actionSlotSelect(this, isSelected, itemRef);
                }
            }
        }

        private void Start() {
            channel = isSoundCompatibility && SoundManager.Instance.TryGetChannel(channelKey, out Audio.ACSound result) ? result : channel;
            if (isSoundCompatibility && channel == null) {
                CatLog.ELog("Channel Not Found !");
            }
        }

        #region ENABLE / DISABLE

        public void DisableSlot() {
            itemRef = null;
            gameObject.SetActive(false);
        }

        public void EnableSlot(AD_item itemref) {
            itemRef = itemref;
            imageItemIcon.sprite  = itemRef.GetSprite;
            imageItemFrame.sprite = frames[(int)itemRef.GetGrade];

            gameObject.SetActive(true);
        }

        #endregion

        #region SELECT / DESELECT

        public void Selected() {
            isSelected = true;
            isPressed  = false;
            selectedImageRectTr.gameObject.SetActive(true);
        }

        public void DeSelected() {
            isSelected = false;
            isPressed  = false;
            selectedImageRectTr.gameObject.SetActive(false);
        }

        #endregion

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (isPressed) {
                actionOpenInfoPanel(itemRef);
            }
            ScaleTrigger();

            if (isSoundCompatibility) {
                channel.PlayOneShot();
            }
        }
        
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            currentTouchTime = 0f;
            isPressed = true;
            ScaleTrigger();
        }
        
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            isPressed = false;
            ScaleTrigger();
        }
        

        #region DRAGHANDLER

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            scrollView.OnEndDrag(eventData);
            if (isPressed) {
                actionOpenInfoPanel(itemRef);
            }
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            scrollView.OnBeginDrag(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            scrollView.OnDrag(eventData);
        }

        void ScaleTrigger() {
            tempScale = (isPressed) ? pressedScale : normalScale;
            rectTr.DOScale(tempScale, scalingTime);
        }

        #endregion
    }
}
