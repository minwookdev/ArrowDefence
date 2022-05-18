namespace ActionCat {
    using UnityEngine;
    using TMPro;

    public class TextWritter : MonoBehaviour {
        [Header("TEXT")]
        [SerializeField] TextMeshProUGUI tmpText = null;
        [SerializeField] float textRotateTime = 0f;
        [SerializeField] int currentIndex = 0;
        [SerializeField] bool isRealTime = false;
        [SerializeField] [ReadOnly] bool isBreak = false;
        float currentRotateTime = 0f;

        [Header("String")]
        [SerializeField] string[] messages = null;

        public void Switch(bool isEnable) {
            gameObject.SetActive(isEnable);
        }

        private void Start() {
            isBreak = (tmpText == null || messages == null || messages.Length <= 0);
            if (isBreak) {
                CatLog.WLog("Warning: Unable to Start TextWritter.");
            }
        }

        private void OnDisable() {
            currentIndex = 0;
            currentRotateTime = 0f;
        }

        private void Update() {
            if(isBreak) {
                return;
            }

            currentRotateTime += (isRealTime) ? Time.unscaledDeltaTime : Time.deltaTime;
            if (currentRotateTime > textRotateTime) {
                tmpText.text = messages[currentIndex];
                currentIndex = (currentIndex >= messages.Length - 1) ? 0 : currentIndex + 1;
                currentRotateTime = 0f;
            }
        }
    }
}
