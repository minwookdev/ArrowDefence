namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.Audio;
    using DG.Tweening;

    public class SettingsPanel : MonoBehaviour {
        [Header("REQUIREMENT")]
        [SerializeField] CanvasGroup fadeCanvasGroup = null;
        [SerializeField] ExSwitch switchControlType  = null;
        [SerializeField] ExSwitch switchEmpty        = null;
        [SerializeField] Slider bgmSlider            = null;
        [SerializeField] Slider seSlider             = null;
        ActionCat.Data.GameSettings settings         = null;

        [Header("AUDIO")]
        [SerializeField] AudioMixer mixer    = null;
        [SerializeField] string seParameter  = null;
        [SerializeField] string bgmParameter = null;

        public void OpenPanel() => this.gameObject.SetActive(true);

        public void ClosePanel() => this.gameObject.SetActive(false);

        private void Awake() {
            settings = ActionCat.Data.CCPlayerData.settings;
            if (settings == null) {
                throw new System.Exception("Player Settings class is Null !");
            }

            bgmParameter = GOSO.Inst.BgmVolumeParameter;
            seParameter  = GOSO.Inst.SeVolumeParameter;
        }

        private void OnEnable() {
            //SWITCH
            switchControlType.IsOn = settings.GetPullTypeToBoolean;

            //SLIDER: Settings의 변수 가져와서 Slider 세팅
            if (mixer.GetFloat(bgmParameter, out float bgmsoundfitch)) {
                bgmSlider.value = bgmsoundfitch;
            }
            else {
                CatLog.WLog("Failed to BgmFitch Value Global AudioMixer.");
            }

            if (mixer.GetFloat(seParameter, out float sesoundfitch)) {
                seSlider.value = sesoundfitch;
            }
            else {
                CatLog.WLog("Failed to SeFitch Value Global AudioMixer.");
            }
        }

        private void Start() {
            switchControlType.AddListnerSwitch(settings.SetPullType);
        }

        private void OnDisable() => Data.CCPlayerData.SaveSettingsJson(); //변경된 GameSettings 변수 변경에 따른 저장

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

        #endregion

        #region SLIDER_ONVALUECHANGED

        /// <summary>
        /// AudioMixer의 BGM Fitch 변경. (볼륨 조절)
        /// </summary>
        public void SLIDER_BGM() => mixer.SetFloat(bgmParameter, bgmSlider.value <= bgmSlider.minValue ? -80f : bgmSlider.value);

        /// <summary>
        /// AudioMixer의 SE Fitch 변경. (볼륨 조절)
        /// </summary>
        public void SLIDER_SE()  => mixer.SetFloat(seParameter, seSlider.value <= seSlider.minValue ? -80f : seSlider.value);

        /// <summary>
        /// GameSettings에 최종적으로 변경된 BgmSoundVolume 변경. (저장되는 변수 변경)
        /// </summary>
        public void SLIDER_RELEASE_BGM() {
            if (mixer.GetFloat(bgmParameter, out float bgmsoundfitch)) {
                settings.BgmVolumeParamsValue = bgmsoundfitch;
            }
        }

        /// <summary>
        /// GameSettings에 최종적으로 변경된 SeSoundVolume 변경. (저장되는 변수 변경)
        /// </summary>
        public void SLIDER_RELEASE_SE() {
            if (mixer.GetFloat(seParameter, out float sesoundfitch)) {
                settings.SeVolumeParamsValue = sesoundfitch;
            }
        }
        #endregion
    }
}
