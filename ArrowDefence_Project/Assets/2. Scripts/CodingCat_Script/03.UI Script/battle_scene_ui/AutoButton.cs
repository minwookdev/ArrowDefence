namespace ActionCat.UI.Auto {
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class AutoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
        [Header("AUTO BUTTON")]
        [SerializeField] EventTrigger eventTrigger = null;

        public void Init(bool isEnable, System.Action<bool> action = null, bool isDebug = false) {
            if(isEnable == false) {
                gameObject.SetActive(false);
            }

            //Add Event Data to EventTrigger
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => action(isDebug));
            eventTrigger.triggers.Add(entry);

            //Add EventEntry Bow Pulling Prevention
            EventTrigger.Entry downEntry = new EventTrigger.Entry(); //Event Entry Down
            downEntry.eventID = EventTriggerType.PointerDown;
            downEntry.callback.AddListener((eventdata) => GameManager.Instance.SetBowPullingStop(true));
            eventTrigger.triggers.Add(downEntry);
            EventTrigger.Entry upEntry = new EventTrigger.Entry();   //Event Entry Up
            upEntry.eventID = EventTriggerType.PointerUp;
            upEntry.callback.AddListener((eventdata) => GameManager.Instance.SetBowPullingStop(false));
            eventTrigger.triggers.Add(upEntry);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            throw new System.NotImplementedException();

            //Button Scaler with DoTween. Try
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            throw new System.NotImplementedException();
        }
    }
}
