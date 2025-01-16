namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;
    using TMPro;

    public class BluePrintSlot : MonoBehaviour, IPointerDownHandler, IPointerClickHandler, IPointerExitHandler, 
                                                IBeginDragHandler, IDragHandler, IEndDragHandler {
        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] Image imageIcon  = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] TextMeshProUGUI textAmount = null;
        [SerializeField] ScrollRect scrollView = null;

        [Header("FRAMES")]
        [SerializeField] Sprite[] frameSprites = null;
        [SerializeField] GameObject selectedFrame = null;

        [Header("DEBUG")]
        [SerializeField] bool isPressed  = false;
        [SerializeField] bool isSelected = false;
        ActionCat.UI.CraftingFunc crafting = null;

        [Header("SOUND")]
        [SerializeField] bool isSoundCompatiblity = false;
        [SerializeField] CHANNELTYPE channelKey   = CHANNELTYPE.NONE;
        [SerializeField] Audio.ACSound channel    = null;

        Vector3 normalScale  = new Vector3(1f, 1f, 1f);
        Vector3 pressedScale = new Vector3(1.1f, 1.1f, 1f);
        float scalingTime = 0.3f;
        float currentPressedTime = 0f;
        float maxPressedTime = 1f;

        AD_item itemReference = null;

        public bool IsActive {
            get {
                return gameObject.activeSelf;
            }
        }

        private void Update() {
            if (isPressed) {
                currentPressedTime += Time.deltaTime;
                if (currentPressedTime >= maxPressedTime) {
                    crafting.SelectBluePirnt(this, isSelected, itemReference);
                    isPressed = false;
                    currentPressedTime = 0f;
                }
            }
        }

        public BluePrintSlot InitSlot(UI.CraftingFunc parent) {
            crafting = parent;
            return this;
        }

        public void Selected() {
            isSelected = true;
            selectedFrame.SetActive(true);
        }

        public void DeSelected() {
            isSelected = false;
            selectedFrame.SetActive(false);
        }

        public void EnableSlot(AD_item item) {
            imageIcon.sprite  = item.GetSprite;
            textAmount.text   = item.GetAmount.ToString();
            imageFrame.sprite = GetFrameSptite(item.GetGrade);

            itemReference = item;

            //Check Slot Sound
            channel = (isSoundCompatiblity && SoundManager.Instance.TryGetChannel2Dic(channelKey, out Audio.ACSound result)) ? result : channel;
            if (isSoundCompatiblity && channel == null) {
                CatLog.ELog("Channel Not Found !");
            }

            if(gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
        }

        public void DisableSlot() {
            itemReference = null;
            gameObject.SetActive(false);
        }

        Sprite GetFrameSptite(ITEMGRADE grade) {
            switch (grade) {
                case ITEMGRADE.GRADE_NORMAL:    return frameSprites[0];
                case ITEMGRADE.GRADE_MAGIC:     return frameSprites[1];
                case ITEMGRADE.GRADE_RARE:      return frameSprites[2];
                case ITEMGRADE.GRADE_HERO:      return frameSprites[3];
                case ITEMGRADE.GRADE_EPIC:      return frameSprites[4];
                case ITEMGRADE.GRADE_LEGENDARY: return frameSprites[5];
                case ITEMGRADE.GRADE_UNIQUE:    return frameSprites[6];
                default: throw new System.NotImplementedException();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (isPressed == true) {
                crafting.OpenItemInfo(itemReference);
                isPressed = false;
                currentPressedTime = 0f;
            }
            ScaleTrigger();

            if (isSoundCompatiblity) {
                channel.PlayOneShot();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            isPressed = true;
            ScaleTrigger();
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            isPressed = false;
            currentPressedTime = 0f;
            ScaleTrigger();
        }

        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) {
            scrollView.OnBeginDrag(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            scrollView.OnDrag(eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData) {
            scrollView.OnEndDrag(eventData);
        }

        void ScaleTrigger() {
            Vector3 targetScale = (isPressed) ? pressedScale : normalScale;
            rectTr.DOScale(targetScale, scalingTime);
        }
    }
}
