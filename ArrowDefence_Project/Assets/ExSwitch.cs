namespace ActionCat {
    using UnityEngine;
    using UnityEngine.EventSystems;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    public class ExSwitch : UIBehaviour, IPointerClickHandler {
        [Header("REQUIREMENT")]
        [SerializeField] RectTransform enableRectTr  = null;
        [SerializeField] RectTransform disableRectTr = null;

        [Header("SWITCH")]
        [SerializeField] private bool isOn = false;

        //ACTION
        System.Action<bool> switchAction = null;

        /// <summary>
        /// Change Switch isOn Value, this Value Changing is Not Called Switch Action.
        /// </summary>
        public bool IsOn {
            get => isOn;
            set {
                isOn = value;
                enableRectTr.gameObject.SetActive(value);
                disableRectTr.gameObject.SetActive((!value));
            }
        }

        #region BUTTON_METHOD

        protected override void Awake() {
            base.Awake();
        }

        protected override void OnDisable() {
            DeleteAllListner();
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            IsOn = (isOn) ? false : true;
            switchAction?.Invoke(isOn);
        }

        public void AddListnerSwitch(System.Action<bool> action) {
            switchAction += action;
        }

        /// <summary>
        /// 테스트중인 기능
        /// </summary>
        /// <param name="targetAction"></param>
        /// <returns></returns>
        public bool DelListnerSwitch(System.Action<bool> targetAction) {
            if (switchAction == null) {
                return false;
            }

            switchAction -= targetAction;
            return true;
        }

        public void DeleteAllListner() {
            switchAction = null;
        }

        #endregion

#if UNITY_EDITOR
        [CustomEditor(typeof(ExSwitch))]
        public class ExSwitchEditor : UnityEditor.Editor {
            ExSwitch origin = null;
            SerializedProperty isOnProp = null;
            SerializedProperty enableRectTrProp = null;
            SerializedProperty disableRectTrProp = null;

            GUIStyle titleStyle = null;
            Color lineColor = new Color(0.6f, 0.6f, 0.6f, 0.6f);

            public void OnEnable() {
                origin = target as ExSwitch;
                enableRectTrProp  = serializedObject.FindProperty(nameof(ExSwitch.enableRectTr));
                disableRectTrProp = serializedObject.FindProperty(nameof(ExSwitch.disableRectTr));
                isOnProp = serializedObject.FindProperty(nameof(ExSwitch.isOn));

                titleStyle = new GUIStyle();
                titleStyle.fontStyle = FontStyle.BoldAndItalic;
                titleStyle.fontSize = 18;
                titleStyle.normal.textColor = new Color(1f, 1f, 1f, 1f);
            }

            public override void OnInspectorGUI() {
                serializedObject.Update();

                EditorGUILayout.LabelField("Switch Extended (UI Behaviour)", titleStyle);
                DrawUILine(lineColor);
                EditorGUILayout.PropertyField(enableRectTrProp);
                EditorGUILayout.PropertyField(disableRectTrProp);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(isOnProp);
                if (EditorGUI.EndChangeCheck()) {
                    origin.enableRectTr.gameObject.SetActive(isOnProp.boolValue);
                    origin.disableRectTr.gameObject.SetActive((isOnProp.boolValue) ? false : true);
                }

                serializedObject.ApplyModifiedProperties();
            }

            private void DrawUILine(Color color, int thickness = 2, int padding = 10) {
                Rect rect = EditorGUILayout.GetControlRect(GUILayout.Height(thickness + padding));
                rect.height = thickness;
                rect.y += padding / 2;
                rect.x -= 4;
                //rect.width += 6;
                EditorGUI.DrawRect(rect, color);
            }
        }
#endif
    }
}
