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
        [SerializeField] RectTransform rotateTargetTr = null;
        [SerializeField] RectTransform dotRect = null;
        float rotateSpeed = 6f;
        WaitUntil waitResume = null;

        [Header("PRESSED SCALE")]
        [SerializeField] Vector3 pressedScale = new Vector3(0.85f, 0.85f, 1f);
        [SerializeField] [ReadOnly] Vector3 initScale;
        [SerializeField] [RangeEx(0.1f, 1.0f, 0.1f)]
        float scalingTime = 0.3f;

        [Header("SOUND EFFECT")]
        [SerializeField] Audio.ACSound soundEffect = null;

        public void Init(System.Action<bool> action, bool isDebug = false) {
            //Initialize Scale
            initScale = buttonRect.localScale;

            //Add Event Data to EventTrigger
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((eventData) => action(isDebug));
            eventTrigger.triggers.Add(entry);

            //Hide Dot Rect GameObject
            dotRect.gameObject.SetActive(false);
            waitResume = new WaitUntil(() => GameManager.Instance.GameState != GAMESTATE.STATE_PAUSE);

            //Active Auto Button GameObject
            if(gameObject.activeSelf == false) {
                gameObject.SetActive(true);
            }
        }

        public void Disable() {
            gameObject.SetActive(false);
        }

        public void ForceStopAuto() {
            if (isOnAuto) {
                isOnAuto = false;
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            if(isTouch == false) {
                currTouchId = eventData.pointerId;
                buttonRect.DOScale(pressedScale, scalingTime); //Button Pressed Scale
                isTouch     = true;
                //Play SoundEffect
                soundEffect.PlayOneShot(0);
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if(currTouchId == eventData.pointerId) {
                isOnAuto = (isOnAuto) ? false : true;
                if (isOnAuto) {
                    StartCoroutine(DotRotateCo());
                }
                //Play SoundEffect
                soundEffect.PlayOneShot(1);
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

        System.Collections.IEnumerator DotRotateCo() {
            //Show DotRect GameObject
            dotRect.gameObject.SetActive(true);

            while (isOnAuto) { //Running Dot
                dotRect.RotateAround(rotateTargetTr.position, Vector3.back, rotateSpeed);
                yield return waitResume;
            }

            //Hide DotRect GameObject
            dotRect.gameObject.SetActive(false);
        }
    }
}
