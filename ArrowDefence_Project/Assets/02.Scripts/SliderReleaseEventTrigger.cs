namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;

    public class SliderReleaseEventTrigger : MonoBehaviour, IPointerUpHandler {
        [Space(10f)]
        [SerializeField] UnityEvent releaseEvent = null;
        // 코드를 사용하여 Event를 등록하기 위한 변수
        public UnityEvent ReleaseEvent {
            get => releaseEvent;
            set => releaseEvent = value;
        }
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            releaseEvent?.Invoke();
        }

        public void TempEvent() {
            CatLog.Log("PointerUp OnSlider !");
        }
    }
}
