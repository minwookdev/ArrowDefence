namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
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

        [Header("LANGUAGE")]
        [SerializeField] LanguageConfirmPanel languageConfirmPanel = null;
        [SerializeField] DropDownList languageDropDown = null;

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

            // 로비 씬의 설정 패널에만 언어선택 드랍 다운 메뉴가 있음
            if (languageDropDown) {
                //var languageDropDownItems = languageDropDown.Items;
                //CatLog.Log($"DropDownItem Length : {languageDropDownItems.Count}");
                //
                //string currentLanguageCode = I2.Loc.LocalizationManager.CurrentLanguageCode;
                //switch (currentLanguageCode) {
                //    case GameGlobal.LangCodeKo:  break;
                //    case GameGlobal.LangCodeEn:  break;
                //    default: break;
                //}

                // 언어 코드 변경 이벤트 등록
                languageDropDown.OnSelectionChanged.AddListener(LanguageDropDownChanged);

                // Apply Localization to First Item
                var firstItem = languageDropDown.Items[0];
                firstItem.Caption = I2.Loc.ScriptLocalization.UI_SettingsPanel.SelectLanguage;
            }
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

        #region LANGUAGE_DROPDOWN

        public void RestoreDropDownList() {
            languageDropDown.OnSelectFirstItem();
        }

        void LanguageDropDownChanged(int selectedIndex) {
            // Selected Index Range is
            // ~ 0 Korean
            // ~ 1 English

            // DropDownList의 ID값으로 할당한 LanguageCode는 차선책.

            if (selectedIndex == 0) {
                return;
            }

            string languageCode = "";
            switch (selectedIndex) {
                case 1: languageCode = GameGlobal.LangCodeKo; break;
                case 2: languageCode = GameGlobal.LangCodeEn; break;
                default: throw new System.NotImplementedException("This Language Index is Not Implemented !");
            }

            if (string.Equals(languageCode, I2.Loc.LocalizationManager.CurrentLanguageCode)) {
                CatLog.Log("현재 게임언어와 동일한 언어가 선택되었습니다.");
                return;
            }

            // Enable Language Select Panel 
            languageConfirmPanel.EnablePanel(languageCode, this);

            //I2.Loc.LocalizationManager.CurrentLanguageCode = languageCode;
        }

        #endregion
    }
}
