namespace ActionCat.UI.Auto {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class AutoButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerClickHandler {
        [Header("AUTO BUTTON")]
        [SerializeField] RectTransform buttonRect  = null;
        [SerializeField] EventTrigger eventTrigger = null;
        [SerializeField] [ReadOnly]
        bool isOnAuto = false;
        [SerializeField] [ReadOnly]
        bool isTouch = false;
        int currTouchId = 0;

        [Header("PRESSED SCALE")]
        [SerializeField] Vector3 pressedScale = new Vector3(0.85f, 0.85f, 1f);
        [SerializeField] [ReadOnly] 
        Vector3 initScale;
        [SerializeField] [RangeEx(0.1f, 1.0f, 0.1f)]
        float scalingTime = 0.3f;

        public void Init(bool isEnable, System.Action<bool> action = null, bool isDebug = false) {
            if(isEnable == false) {
                gameObject.SetActive(false); return;
            }

            //Initialize Scale
            initScale = buttonRect.localScale;

            //Add Event Data to EventTrigger
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => action(isDebug));
            eventTrigger.triggers.Add(entry);

            //Add EventEntry Bow Pulling Prevention
            //EventTrigger.Entry downEntry = new EventTrigger.Entry(); //Event Entry Down
            //downEntry.eventID = EventTriggerType.PointerDown;
            //downEntry.callback.AddListener((eventdata) => GameManager.Instance.SetBowPullingStop(true));
            //eventTrigger.triggers.Add(downEntry);
            //EventTrigger.Entry upEntry = new EventTrigger.Entry();   //Event Entry Up
            //upEntry.eventID = EventTriggerType.PointerUp;
            //upEntry.callback.AddListener((eventdata) => GameManager.Instance.SetBowPullingStop(false));
            //eventTrigger.triggers.Add(upEntry);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(isTouch == false) {
                currTouchId = eventData.pointerId;
                buttonRect.DOScale(pressedScale, scalingTime); //Button Pressed Scale
                isTouch     = true;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if(currTouchId == eventData.pointerId) {
                isOnAuto = (isOnAuto) ? false : true;
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            if(currTouchId == eventData.pointerId) {
                buttonRect.DOScale(initScale, scalingTime); //Button up Scale
                isTouch = false;
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            if(currTouchId == eventData.pointerId) {
                buttonRect.DOScale(initScale, scalingTime); //Button Up Scale
                isTouch = false;
            }
        }
    }
}
