namespace ActionCat {
    using UnityEngine;
    using DG.Tweening;
    using UnityEngine.UI;

    public class SettingsPanel : MonoBehaviour {
        [Header("REQUIREMENT")]
        [SerializeField] CanvasGroup fadeCanvasGroup = null;
        [SerializeField] ExSwitch switchControlType = null;
        [SerializeField] ExSwitch switchEmpty = null;
        [SerializeField] Slider bgSlider = null;
        [SerializeField] Slider seSlider = null;
        ActionCat.Data.GameSettings settings = null;

        public void OpenPanel() => this.gameObject.SetActive(true);

        public void ClosePanel() => this.gameObject.SetActive(false);

        private void Awake() {
            settings = ActionCat.Data.CCPlayerData.settings;
            if (settings == null) {
                throw new System.Exception("Player Settings class is Null !");
            }
        }

        private void OnEnable() {
            //SWITCH
            switchControlType.IsOn = settings.GetPullTypeToBoolean;

            //SLIDER
            bgSlider.value = settings.BgmSoundValue;
            seSlider.value = settings.SeSoundValue;
        }

        private void Start() {
            switchControlType.AddListnerSwitch(settings.SetPullType);
        }

        private void OnDisable() => Data.CCPlayerData.SaveSettingsJson();

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

        public void BE_SAVE() {
            var saveResult = GameManager.Instance.SaveUserJsonFile();
            if (!saveResult) {
                Notify.Inst.Message("Save Failed !", StringColor.RED);
            }
        }

        public void SV_BGM() {
            settings.BgmSoundValue = bgSlider.value;
        }

        public void SV_SE() {
            settings.SeSoundValue = seSlider.value;
        }

        #endregion
    }
}
