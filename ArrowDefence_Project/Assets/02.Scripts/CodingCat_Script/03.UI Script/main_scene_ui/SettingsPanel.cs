namespace ActionCat {
    using UnityEngine;
    using DG.Tweening;

    public class SettingsPanel : MonoBehaviour {
        [Header("REQUIREMENT")]
        [SerializeField] CanvasGroup fadeCanvasGroup = null;

        public void OpenPanel() => this.gameObject.SetActive(true);

        public void ClosePanel() => this.gameObject.SetActive(false);

        #region BUTTON_EVENT

        public void BE_TITLE() {
            fadeCanvasGroup.DOFade(StNum.floatOne, 2.0f)
                     .OnStart(() => {
                         fadeCanvasGroup.blocksRaycasts = true;
                         fadeCanvasGroup.gameObject.SetActive(true);
                     })
                     .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.SCENE_TITLE));
        }

        public void BE_CLOSE() {
            this.gameObject.SetActive(false);
        }

        public void BE_QUIT() {

        }

        #endregion
    }
}
