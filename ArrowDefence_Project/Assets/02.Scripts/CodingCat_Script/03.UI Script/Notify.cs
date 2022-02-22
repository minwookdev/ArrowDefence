namespace ActionCat {
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.EventSystems;
    using TMPro;
    using DG.Tweening;

    /// Apply Function
    /// a. Singleton
    /// b. Safety Application Quit
    /// (not) thread safe 
    public class Notify : MonoBehaviour, IPointerClickHandler {
        static Notify _inst;
        static bool _shuttingDown = false;
        public static Notify Inst {
            get {
                if (_shuttingDown) {
                    CatLog.ELog("Notify Instance'" + typeof(Notify) + "' already destroyed. Returnning null.");
                    return null;
                }

                if (_inst != null) {
                    return _inst;
                }

                _inst = FindObjectOfType<Notify>();
                if (_inst != null) {
                    return _inst;
                }

                GameObject instance = GameObject.Instantiate(Resources.Load<GameObject>("ArrowDefence_UI/panel_notify"));
                if (instance == null) throw new System.Exception();
                if (instance.TryGetComponent<Notify>(out Notify notify)) {
                    _inst = notify;
                }
                else {
                    throw new System.Exception("New Instance Not have Notify Component.");
                }

                return _inst;
            }
        }

        [Header("COMPONENT")]
        [SerializeField] RectTransform notifyRectTr = null;
        [SerializeField] TextMeshProUGUI notifyText = null;
        Sequence notifySeq = null;

        [Header("NOTIFY")]
        [SerializeField] [RangeEx(1f, 5f)] float messageDuration = 1f;

        [Header("NOTIFY LIST")]
        [SerializeField] GameObject prefMessage;
        bool isInit = false;

        public void Init(RectTransform parentCanvas) {
            if(isInit == true) {
                CatLog.WLog("Notify Manager is Already Initialized this Scene."); return;
            }

            if(notifyRectTr == null || notifyText == null) {
                throw new System.Exception("Notify Component is Not Assignmnet.");
            }

            //Change Parent
            notifyRectTr.SetParent(parentCanvas);
            notifyRectTr.RectResizer(Vector2.zero, Vector2.zero, Vector3.one);

            notifyText.AlphaZero();

            //assignment new Sequence
            notifySeq = DOTween.Sequence()
                               .Append(notifyText.DOFade(StNum.floatZero, messageDuration))
                               .Join(notifyText.rectTransform.DOShakePosition(1f, 5f, 15, 90, false, true))
                               .SetAutoKill(false)
                               .SetUpdate(false)
                               .Pause();
            isInit = true;
        }

        public void Show(string text) {
            if(!isInit) {
                throw new System.Exception("Notify is Not Initialized or missing component");
            }

            notifyText.AlphaOne();
            notifyText.text = text;
            notifySeq.Restart();
        }

        public void Hide() {
            
        }

        private void Awake() {
            SceneLoader.SceneChangeCallback += OnReset;
            //SceneManager.sceneLoaded += OnReset;
            //SceneManager.activeSceneChanged
        }

        /// <summary>
        /// ActionCat.SceneManager 통합예정
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        public void OnReset() {
            if(notifyRectTr.parent != null) {
                notifyRectTr.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
            isInit = false;
        }

        private void OnApplicationQuit() {
            _shuttingDown = true;
        }

        private void OnDestroy() {
            _shuttingDown = true;
            //SceneManager.sceneLoaded -= OnReset;

            if(SceneLoader.Instance != null) {
                SceneLoader.SceneChangeCallback -= OnReset;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            throw new System.Exception("Notify gameobject is clicked, should not recieve this Event.");
        }
    }
}
