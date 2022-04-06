namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class MonsterSlot : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTrSlot = null;
        [SerializeField] Image imageMonsterIcon = null;
        [SerializeField] ItemData_Mat monsterEntity = null;

        [Header("SCALING")]
        [SerializeField] [ReadOnly] bool isTouched = false;

        Vector3 touchedScale = new Vector3(1.2f, 1.2f, 1f);
        Vector3 normalScale  = Vector3.one;
        float scalingTime    = 0.2f;

        public void EnableSlot(ItemData_Mat entity) {
            monsterEntity = entity;
            imageMonsterIcon.sprite = monsterEntity.Item_Sprite;
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
        }

        public void PointerDown() {
            if (!isTouched) {
                rectTrSlot.DOScale(touchedScale, scalingTime).OnStart(() => isTouched = true);
            }
        }

        public void PointerClick() {
            if (isTouched) {
                rectTrSlot.DOScale(normalScale, scalingTime).OnStart(() => isTouched = false);
                //Open Monster Information Tooltip !
            }
        }

        public void PointerExit() {
            if (isTouched) {
                rectTrSlot.DOScale(normalScale, scalingTime).OnStart(() => isTouched = false);
            }
        }
    }
}
