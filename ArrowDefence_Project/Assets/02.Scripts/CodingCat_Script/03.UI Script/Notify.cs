namespace ActionCat {
    using UnityEngine;
    using TMPro;

    /// Apply Function
    /// a. Singleton
    /// b. Safety Application Quit
    /// (not) thread safe 
    public class Notify : MonoBehaviour {
        static Notify _inst;
        static bool _shuttingDown = false;
        public static Notify Instance {
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

                //이거만 가지고 생성 안될수도 있음 테스트 요망
                _inst = Resources.Load<Notify>("ArrowDefence_UI/panel_notify");
                return _inst;
            }
        }

        [Header("NOFITY")]
        [SerializeField] RectTransform notifyRectTr = null;
        [SerializeField] TextMeshProUGUI notifyText = null;

        public void Init(RectTransform parentCanvas) {
            if(notifyRectTr.parent == parentCanvas) {
                CatLog.WLog("Notify Manager is Already Initialized this Scene.");
                return;
            }

            notifyRectTr.SetParent(parentCanvas);
            notifyRectTr.RectResizer(Vector2.zero, Vector2.zero, Vector3.one);

            //요놈 메인씬 루트에서 테스트하자
        }

        public void Text(string text, float time = 3f) {

        }

        private void OnApplicationQuit() {
            _shuttingDown = true;
        }

        private void OnDestroy() {
            _shuttingDown = true;
        }
    }
}
