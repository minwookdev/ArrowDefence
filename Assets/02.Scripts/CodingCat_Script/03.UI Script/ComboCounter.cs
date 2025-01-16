namespace ActionCat.UI {
    using UnityEngine;
    using DG.Tweening;
    using TMPro;

    public class ComboCounter : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform textRectTr = null;
        [SerializeField] TextMeshProUGUI textCounter = null;
        [SerializeField] CanvasGroup canvasGroup = null;

        [Header("COMBO")]
        [SerializeField] float comboHoldTime = 0f;
        [SerializeField] bool isUnscaledTime = false;
        [Range(0f, -20f)]
        [SerializeField] float tweenStartPositionY = -10f;
        float tweenTextMoveTime = .3f;
        Vector3 initPosition;
        Vector3 tweenStartPosition;

        private void Start() {
            canvasGroup.alpha = 0f;
        }

        private void OnDisable() {
            TweenKill();
        }

        public void InitComboCounter(float maxTime, bool isUnscaledTime) {
            this.isUnscaledTime = isUnscaledTime;
            comboHoldTime = maxTime;
            initPosition  = textRectTr.anchoredPosition;
            tweenStartPosition.y = initPosition.y + tweenStartPositionY;
        }

        public void UpdateComboCounter(short comboCount) {
            textCounter.text = comboCount.ToString();
            TweenKill();

            //Set Tween Start Position, Canvas Alpha
            canvasGroup.alpha = 1f;
            textRectTr.anchoredPosition = tweenStartPosition;

            //Combo Counter CanvasGorup Fade in
            TweenStart();
        }

        public void ComboClear() {
            textCounter.text = "";
        }

        void TweenKill() {
            canvasGroup.DOKill();
            textRectTr.DOKill();
        }

        void TweenStart() {
            canvasGroup.DOFade(StNum.floatZero, comboHoldTime).SetUpdate(isUnscaledTime);
            textRectTr.DOLocalMove(initPosition, tweenTextMoveTime);
        }
    }
}
