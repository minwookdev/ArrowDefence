namespace ActionCat {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public interface ITouchPosReceiver {
        void SendWorldPos(Vector2 position);
    }

    public class TouchPosDetector : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler {
        [Header("DETECTOR")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] RectTransform detectorRectTr = null;
        [SerializeField] Camera uiCam = null;
        [SerializeField] [ReadOnly] float radius;
        [SerializeField] [ReadOnly] float detectorScaleTime = 0.3f;
        Vector2 touchPosition = Vector2.zero;
        ITouchPosReceiver receiver = null; // <-- 디텍터를 다른곳에서도 사용해주면 Interface OR delegate 사용 고려/ 적용 완료

        public bool IsDetectorOpen {
            get {
                return (gameObject.activeSelf);
            }
        }
        public bool IsTouching {
            get; protected set;
        }

        public void Start() {
            //OpenDetector(50f); <-- 테스트 호출
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
            gameObject.SetActive(false); 
        }

        void IDragHandler.OnDrag(PointerEventData eventData) {
            //Touch Move
            touchPosition = PointerDataToRelativePosition(eventData);
        }

        private void Update() {
            detectorRectTr.anchoredPosition = touchPosition;
        }

        Vector2 PointerDataToRelativePosition(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTr, eventData.position, uiCam, out Vector2 result);
            return result;
        }
    }
}
