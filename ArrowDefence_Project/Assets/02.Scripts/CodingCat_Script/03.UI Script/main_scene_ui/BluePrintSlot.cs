namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class BluePrintSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler {
        [Header("COMPONENT")]
        [SerializeField] Image imageIcon  = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] TextMeshProUGUI textAmount = null;

        [Header("FRAMES")]
        [SerializeField] Sprite[] frameSprites = null;

        public bool IsActive {
            get {
                return gameObject.activeSelf;
            }
        }

        public void EnableSlot(AD_item item) {
            imageIcon.sprite  = item.GetSprite;
            textAmount.text   = (item.GetAmount > 1) ? item.GetAmount.ToString() : ""; 
            imageFrame.sprite = GetFrameSptite(item.GetGrade);

            if(gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
        }

        public void DisableSlot() {
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
            throw new System.NotImplementedException();
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            throw new System.NotImplementedException();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            throw new System.NotImplementedException();
        }
    }
}
