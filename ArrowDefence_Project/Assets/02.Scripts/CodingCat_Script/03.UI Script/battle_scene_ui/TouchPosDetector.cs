namespace ActionCat {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public interface ITouchPosReceiver {
        void SendWorldPos(Vector2 position);
    }

    public class TouchPosDetector : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler {
        [Header("RECT TRANSFORM")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] RectTransform detectorRectTr = null;
        [SerializeField] RectTransform worldPosDetectorRectTr = null; // <--- 나중에 실제 월드 좌표에서의 콜라이더 탐색용

        [Header("CAMERA")]
        [SerializeField] Camera uiCam = null;
        [SerializeField] [ReadOnly] Camera mainCam = null;

        [Header("VARIABLES")]
        [SerializeField] [ReadOnly] float radius;
        [SerializeField] [ReadOnly] float detectorScaleTime = 0.3f;

        [Header("CANVASGROUP")]
        [SerializeField] CanvasGroup canvasGroup = null;

        Vector2 touchPosition = Vector2.zero;
        Vector2 detectorWorldPos = Vector2.zero;
        ITouchPosReceiver receiver = null; // <-- Detect result 수신자
        Sequence quitSequence = null;

        public bool IsTouching {
            get; protected set;
        }

        public void Start() {
            mainCam = Camera.main;
            if (mainCam == null) {
                CatLog.WLog("Not Found Main Camera in this Scene");
            }

            //Init New Quit Sequence 
            quitSequence = DOTween.Sequence()
                                  .Prepend(detectorRectTr.DOScale(1.3f, 0.25f).SetEase(Ease.OutBack))
                                  .Append(detectorRectTr.DOScale(Vector2.zero, 0.15f))
                                  .Append(canvasGroup.DOFade(StNum.floatZero, 0.25f))
                                  .OnComplete(() => this.gameObject.SetActive(false))
                                  .SetUpdate(false)
                                  .SetAutoKill(false)
                                  .Pause();
        }

        private void OnEnable() {
            detectorRectTr.localScale = Vector3.zero;
        }

        public void OpenDetector(float radius, ITouchPosReceiver receiver) {
            this.radius   = radius;
            this.receiver = receiver;
            detectorRectTr.sizeDelta = new Vector2(this.radius * 2f, this.radius * 2f);
            gameObject.SetActive(true);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            //Touch Start
            touchPosition = PointerDataToRelativePosition(eventData);
            detectorRectTr.DOScale(Vector3.one, detectorScaleTime);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            //Touch Release
            touchPosition = PointerDataToRelativePosition(eventData);
            //detectorRectTr.DOScale(Vector3.zero, detectorScaleTime); // <-- Release되면 좌표 보내고, 비활성화 처리

            //콜라이더 뿌려서 가져올 월드 포인트 장소 지정
            var worldPos = Camera.main.ScreenToWorldPoint(eventData.position);
            CatLog.Log($"World Point X: {worldPos.x}, Y: {worldPos.y}");

            //좌표보내기
            if (receiver != null) {
                receiver.SendWorldPos(worldPos);
                receiver = null;
            }

            //GameObject 비활성화
            //gameObject.SetActive(false); 
            quitSequence.Restart();
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            //Touch Move
            touchPosition = PointerDataToRelativePosition(eventData);
            detectorWorldPos = eventData.position;
        }

        private void Update() {
            detectorRectTr.anchoredPosition = touchPosition;
            worldPosDetectorRectTr.position = ToWorldPos(detectorWorldPos); // <--- Update World Position Detector
        }

        Vector2 PointerDataToRelativePosition(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTr, eventData.position, uiCam, out Vector2 result);
            return result;
        }

        Vector2 ToWorldPos(Vector3 eventDataPosition) {
            return mainCam.ScreenToWorldPoint(eventDataPosition);
        }
    }
}
