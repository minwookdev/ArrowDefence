namespace ActionCat {
    using UnityEngine;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(RectTransform))]
    public class RectSizeChangeChecker : UIBehaviour {
        public delegate void RectSizeChangeEvent();
        public event RectSizeChangeEvent resizeEvent;
        RectTransform rectTr = null;
        protected override void Awake() {
            //base.Awake();
            rectTr = GetComponent<RectTransform>();
        }

        protected override void OnRectTransformDimensionsChange() {
            //resizeEvent?.Invoke();
            if (resizeEvent != null) {
                resizeEvent.Invoke();
                resizeEvent = null;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.CustomEditor(typeof(RectSizeChangeChecker))]
        public class RectSizeChangeCheckerEditor : UnityEditor.Editor {
            string message = "해당 컴포넌트는 본 게임오브젝트의 Rect Size가 변경될 때, \n" +
                             "이벤트를 발생시키며, public 접근자를 가진 해당 컴포넌트의 \n" +
                             "이벤트 [resizeEvent] 를 구독하여 UI관련 처리를 할 수 있습니다.";

            public override void OnInspectorGUI() {
                base.OnInspectorGUI();
                UnityEditor.EditorGUILayout.HelpBox(message, UnityEditor.MessageType.Info);
            }
        }
#endif
    }
}
