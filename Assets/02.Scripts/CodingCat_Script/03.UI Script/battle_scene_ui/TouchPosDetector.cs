namespace ActionCat {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public interface ITouchPosReceiver {
        void SendWorldPos(Vector2 position);
        void SendColliders(Collider2D[] colliders);
    }

    // World Position Detector 까지 같이 놔둔 이유는 
    // 화면안에서 터치하고 중인 UI Canvas상의 좌표가 아닌 월드좌표에서 터치하고있는 곳의 좌표를 움직여주고 있는데
    // 이 오브젝트는 콜라이더를 하나 가지고 있다. 이 콜라이더를 사용해 충돌중인 몬스터 객체들의 스프라이트 데이터를
    // 조작해주어, 범위안의 객체들의 강조효과나 설명등을 넣을 수 있게 사용할 수 있도록 하나 만들어 둠

    public class TouchPosDetector : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IDragHandler {
        [Header("RECT TRANSFORM")]
        [SerializeField] TouchWorldPosDetector worldDetector = null;
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
        Coroutine gameStateCheckCo = null;
        bool isTouchEnded = false;

        public bool IsTouching {
            get; protected set;
        }

        public void Start() {
            mainCam = Camera.main;
            if (mainCam == null) {
                CatLog.ELog("Not Found Main Camera in this Scene");
            }

            if(worldPosDetectorRectTr == null) {
                worldPosDetectorRectTr = worldDetector.RectTr;
            }

            //Init New Quit Sequence 
            quitSequence = DOTween.Sequence()
                                  .Prepend(detectorRectTr.DOScale(1.25f, 0.3f).SetEase(Ease.OutBack))
                                  .Append(detectorRectTr.DOScale(Vector2.zero, 0.1f))
                                  .Append(canvasGroup.DOFade(StNum.floatZero, 0.1f))
                                  .OnComplete(() => this.gameObject.SetActive(false))
                                  .SetUpdate(true)
                                  .SetAutoKill(false)
                                  .Pause();
        }

        private void OnEnable() {
            isTouchEnded = false;
            detectorRectTr.anchoredPosition = Vector2.zero;
            detectorRectTr.localScale = Vector3.zero;
            canvasGroup.DOFade(1f, 0.2f).From(0f);

            //Start Coroutine GameState Ended
            CheckStart();
        }

        public void OpenDetector(float radius, ITouchPosReceiver receiver) {
            this.radius   = radius;
            this.receiver = receiver;

            //Set Detector's Size Delta
            detectorRectTr.sizeDelta = new Vector2(this.radius * 2f, this.radius * 2f);
            worldDetector.SetRadius(radius);

            //Active GameObject
            gameObject.SetActive(true);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) { // TOUCH BEGAN
            //Get Current Touch Position
            touchPosition    = PointerDataToRelativePosition(eventData);
            detectorWorldPos = eventData.position;

            //요놈이 문제였음
            if (!isTouchEnded) {
                detectorRectTr.DOScale(Vector3.one, detectorScaleTime);
            }
        }

        void IDragHandler.OnDrag(PointerEventData eventData) { // TOUCH MOVE
            //Get Current Touch Position
            touchPosition    = PointerDataToRelativePosition(eventData);
            detectorWorldPos = eventData.position;
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) { // TOUCH END
            //Get Current Touch Position
            touchPosition    = PointerDataToRelativePosition(eventData);
            detectorWorldPos = eventData.position;

            if (!isTouchEnded) {
                isTouchEnded = true;
                CheckStop();
                //if (receiver != null) { //Send World Position and Release Receiver Address
                //    //receiver.SendWorldPos(ToWorldPos(eventData.position)); // <--- World Position를 보낼 때
                //    receiver.SendColliders(worldDetector.GetColliders());    // <--- World Detector에 잡힌 Collider들을 보낼 때
                //    receiver = null;
                //}

                receiver.TrySendColliders(worldDetector.GetCollidersWithLength(out int length));
                receiver = null;
                quitSequence.Restart();
                CatLog.Log($"Get Colldiers Count: {length}");
            }
        }

        private void Update() {
            //detectorRectTr.anchoredPosition = touchPosition;
            //worldPosDetectorRectTr.position = ToWorldPos(detectorWorldPos); // <--- Update World Position Detector

            if (!isTouchEnded) { //Touch가 종료되기 전까지 RectPosition 업데이트
                detectorRectTr.anchoredPosition = touchPosition;
                worldPosDetectorRectTr.position = ToWorldPos(detectorWorldPos);
            }
        }

        Vector2 PointerDataToRelativePosition(PointerEventData eventData) {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTr, eventData.position, uiCam, out Vector2 result);
            return result;
        }

        Vector2 ToWorldPos(Vector3 eventDataPosition) {
            return mainCam.ScreenToWorldPoint(eventDataPosition);
        }

        void CheckStart() {
            gameStateCheckCo = StartCoroutine(CheckGameStateEnd());
        }

        void CheckStop() {
            //if (gameStateCheckCo != null) {
            //    StopCoroutine(gameStateCheckCo);
            //} --> 체크할 필요 없음. 무조건 Enable에서 시작하니까
            StopCoroutine(gameStateCheckCo);
        }

        /// <summary>
        /// Detector가 활성화 중 Game Clear, Game Over 감지
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator CheckGameStateEnd() {
            yield return new WaitUntil(() => GameManager.Instance.IsGameStateEnd == true);
            isTouchEnded = true;
            receiver     = null;
            quitSequence.Restart();
        }
    }
}
