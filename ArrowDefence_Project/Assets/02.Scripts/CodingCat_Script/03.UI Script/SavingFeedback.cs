namespace ActionCat {
    using UnityEngine;
    using DG.Tweening;

    public class SavingFeedback : MonoBehaviour {
        [Header("REQUIREMENT")]
        [SerializeField] Animation animRotateGear = null;
        [SerializeField] RectTransform rectTr = null;
        public bool IsPlaying {
            get; private set;
        } = false;

        Sequence panelSequence = null;

        private float openTime    = 1f;
        private float holdingTime = 1.5f;
        private float closingTime = 1f;

        /// <summary>
        /// 피드백 애니메이션이 재생중이 아닐 때, 재생.
        /// </summary>
        public void Play() {
            if (IsPlaying) {
                return;
            }

            panelSequence = this.PanelSequence();
        }

        /// <summary>
        /// 현재 피드백 애니메이션 재생을 초기화하고 강제 재생.
        /// </summary>
        public void PlayForce() {
            if (IsPlaying) {
                panelSequence.Kill(complete:true);
            }

            panelSequence = this.PanelSequence();
        }

        private void OnDestroy() {
            if (IsPlaying) {
                panelSequence.Kill();
            }
        }

        private void Start() {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
            }
        }

        Sequence PanelSequence() {
            return DOTween.Sequence()
                          .Append(rectTr.DOAnchorPosX(StNum.floatZero, openTime))
                          .Insert(openTime + holdingTime, rectTr.DOAnchorPosX(rectTr.sizeDelta.x, closingTime))
                          .OnStart(() => { animRotateGear.Play(); IsPlaying = true; gameObject.SetActive(true); })
                          .OnComplete(() => { animRotateGear.Stop(); IsPlaying = false; gameObject.SetActive(false); });
        }

        public static bool operator true(SavingFeedback feedback) => (feedback != null) ? true : false;

        public static bool operator false(SavingFeedback feedback) => (feedback == null) ? true : false;
    }
}
