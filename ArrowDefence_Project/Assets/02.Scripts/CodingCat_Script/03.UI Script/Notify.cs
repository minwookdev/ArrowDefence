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
        [SerializeField] RectTransform notifyBackRectTr = null;
        [SerializeField] UnityEngine.UI.Image imageNotifyBack = null;
        [SerializeField] CanvasGroup notifyCanvasGroup = null;
        [SerializeField] TextMeshProUGUI notifyText = null;
        Sequence notifySeq = null;

        [Header("NOTIFY OPTIONS")]
        [SerializeField] [RangeEx(1f, 5f)] float messageDuration = 1f;
        [SerializeField] Color backGroundDefaultColor;
        [SerializeField] Color backGroundHideColor;

        [Header("NOTIFY LIST")]
        [SerializeField] GameObject prefMessage;

        bool isInit = false;
        string endColor = "</color>";
        float defaultBackGroundSpacing = 70f;
        float defaultMargins = 15f;

        public float GetBackRectHeight {
            get {
                return notifyText.rectTransform.rect.height + (defaultMargins * 2f);
            }
        }

        public void Init(RectTransform parentCanvas) {
            if(isInit == true) {
                CatLog.WLog("Notify Manager is Already Initialized this Scene."); return;
            }

            //Get CavnasGroup Component
            if (notifyCanvasGroup == null) {
                notifyCanvasGroup = GetComponent<CanvasGroup>();
            } 

            //Set UI Canvas Parent
            notifyRectTr.SetParent(parentCanvas);
            notifyRectTr.RectResizer(Vector2.zero, Vector2.zero, Vector3.one);

            //assignment new Sequence
            notifySeq = DOTween.Sequence()
                               .Pause()
                               .Append(notifyCanvasGroup.DOFade(StNum.floatZero, messageDuration).From(StNum.floatOne).SetEase(Ease.InExpo))
                               .Join(notifyText.rectTransform.DOShakePosition(1f, 5f, 15, 90, false, true))
                               .SetAutoKill(false)
                               .SetUpdate(false);

            notifyCanvasGroup.alpha = StNum.floatZero;
            isInit = true;
        }

        public void Message(string text, StringColor color = StringColor.WHITE, bool isDrawBG = true) {
            if(!isInit) {   // Check Initialized. 
                throw new System.Exception("Notify is Not Initialized or missing component");
            }

            //Set Notify Message Strings
            notifyText.AlphaOne();
            notifyText.text = string.Format("{1}{0}{2}", text, GetColor(color), endColor);
            notifyText.ForceMeshUpdate(true, true);   //변경된 TextMeshPro에 대해서 바로 적용되지 않아서 강제로 업데이트 호출해줌.
            var lines = notifyText.textInfo.lineCount;
            float calcHeight = (defaultBackGroundSpacing * lines) + (defaultMargins * 2f);

            //notifyText.CalculateLayoutInputHorizontal();
            //notifyText.CalculateLayoutInputVertical();
            //notifyText.rectTransform.ForceUpdateRectTransforms();

            //Set BackGround SizeDelta
            notifyBackRectTr.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, calcHeight);
            imageNotifyBack.color = (isDrawBG) ? backGroundDefaultColor : backGroundHideColor;

            //Fin. Play Notify Sequence. 
            notifySeq.Restart();
        }

        public void ForceHide() {
            
        }

        private void Awake() {
            if (notifyRectTr == null || notifyText == null) {
                throw new System.Exception("Notify Component is Not Assignment !");
            }
            SceneLoader.SceneChangeCallback += OnReset;
        }

        /// <summary>
        /// ActionCat.SceneManager 통합예정
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="loadSceneMode"></param>
        public void OnReset() {
            if (notifyRectTr.parent != null) {
                notifyRectTr.SetParent(null);
            }

            DontDestroyOnLoad(gameObject);
            isInit = false;
        }

        string GetColor(StringColor color) {
            switch (color) {
                case StringColor.YELLOW: return "<color=red>";
                case StringColor.BLUE:   return "<color=blue>";
                case StringColor.RED:    return "<color=red>";
                case StringColor.GREEN:  return "<color=green>";
                case StringColor.WHITE:  return "<color=white>";
                case StringColor.BLACK:  return "<color=black>";
                default: throw new System.NotImplementedException();
            }
        }

        private void OnApplicationQuit() {
            _shuttingDown = true;
        }

        private void OnDestroy() {
            _shuttingDown = true;
            //SceneManager.sceneLoaded -= OnReset;

            if (SceneLoader.IsExist) {
                SceneLoader.SceneChangeCallback -= OnReset;
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            throw new System.Exception("Notify gameobject is clicked, should not recieve this Event.");
        }
    }
}
