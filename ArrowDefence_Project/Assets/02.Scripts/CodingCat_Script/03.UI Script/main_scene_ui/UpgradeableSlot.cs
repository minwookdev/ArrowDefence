namespace ActionCat {
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class UpgradeableSlot : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler {
        [Header("COMPONENT")]
        [SerializeField] RectTransform selectedImageRectTr = null;
        private Action<UpgradeableSlot, AD_item> selectedAction = null;
        private Action<UpgradeableSlot> deSelectedAction = null;
        private Action<AD_item> openInfoAction  = null;
        private AD_item itemRef = null;

        private bool isSelected = false;
        private bool isTouched  = false;
        private float currentTouchTime = 0f;
        private float maxTouchTime = 1.5f;

        public bool IsActive {
            get {
                return gameObject.activeSelf;
            }
        }

        private void Update() {
            if(isTouched == true) {
                currentTouchTime += Time.unscaledDeltaTime;

                if(currentTouchTime >= maxTouchTime) {
                    if(!isSelected) {
                        selectedAction(this, itemRef);
                    }
                    else {
                        deSelectedAction(this);
                    }
                }
            }
        }

        public void SetAction(Action<AD_item> clickAction, Action<UpgradeableSlot, AD_item> selectedAction, Action<UpgradeableSlot> deSelectedAction) {
            this.openInfoAction = clickAction;
            this.selectedAction = selectedAction;
            this.deSelectedAction = deSelectedAction;
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
        }

        public void EnableSlot(AD_item item) {
            gameObject.SetActive(true);
        }

        public void Selected() {
            isSelected = true;
            isTouched  = false;
            selectedImageRectTr.gameObject.SetActive(true);
        }

        public void DeSelected() {
            isSelected = false;
            isTouched  = false;
            selectedImageRectTr.gameObject.SetActive(false);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (!isSelected) {
                if(currentTouchTime < maxTouchTime) {
                    openInfoAction(itemRef);
                }
            }
            else {
                if(currentTouchTime < maxTouchTime) {
                    openInfoAction(itemRef);
                }
            }

            CatLog.Log("Called OnPointerClick");
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            currentTouchTime = 0f;
            isTouched = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            isTouched = false;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            isTouched = false;
            CatLog.Log("Called PointerUp !");
        }
    }
}
