namespace ActionCat {
    using UnityEngine;
    using DG.Tweening;
    using I2.Loc;

    public class LanguageConfirmPanel : MonoBehaviour {
        [Header("REQUIREMENT")]
        [SerializeField] CanvasGroup fadePanelCG = null;
        [SerializeField] [ReadOnly] string recieveLanguageCode = "";
        SettingsPanel settingsPanel = null;

        public void EnablePanel(string languageCode, SettingsPanel settings) {
            recieveLanguageCode = languageCode;
            this.settingsPanel  = settings;
            gameObject.SetActive(true);

            // Input Language Code
            CatLog.Log($"Input Language Code: {recieveLanguageCode}");
        }

        public void BE_CANCEL() {
            recieveLanguageCode = "";
            gameObject.SetActive(false);

            // Restore Settings panel dropdown list
            this.settingsPanel.RestoreDropDownList();
            this.settingsPanel = null;
        }

        public void BE_CONFIRM() {
            // Save Changed Language Code
            GameManager.Instance.SetLanguageCode(recieveLanguageCode);

            // Change Scene to Title
            fadePanelCG.DOFade(StNum.floatOne, 2.0f)
                       .OnStart(() => {
                           fadePanelCG.blocksRaycasts = true;
                           fadePanelCG.gameObject.SetActive(true);
                       })
                       .OnComplete(() => {
                           LocalizationManager.CurrentLanguageCode = recieveLanguageCode;
                           SceneLoader.Instance.LoadScene(AD_Data.SCENE_TITLE);
                       });
        }
    }
}
