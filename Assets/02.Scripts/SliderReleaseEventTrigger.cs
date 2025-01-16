namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.Events;

    public class SliderReleaseEventTrigger : MonoBehaviour, IPointerUpHandler {
        [Space(10f)] // 인스펙터를 사용하여 이벤트를 등록할 수 있도록 UnityEvent 형으로 선언
        [SerializeField] UnityEvent releaseEvent = null; 

        public UnityEvent ReleaseEvent {
            get => releaseEvent;
            set => releaseEvent = value;
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            releaseEvent?.Invoke();
        }

        public void AddSliderReleaseListner(UnityAction releaseCallback) {
            releaseEvent.AddListener(releaseCallback);
        }
    }
}
