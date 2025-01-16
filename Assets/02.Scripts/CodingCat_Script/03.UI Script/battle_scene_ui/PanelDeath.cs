namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using DG.Tweening;

    public class PanelDeath : MonoBehaviour {
        [Header("PANEL GAMEOVER")]
        [SerializeField] RectTransform panelTr = null;
        [SerializeField] RectTransform frontPanelTr = null;
        [SerializeField] RectTransform logoTr = null;
        [SerializeField] Image imagePanelBack = null;
        [SerializeField] CanvasGroup buttonCanvasGroup = null;
        [SerializeField] TextMeshProUGUI tmptips = null;
        [SerializeField] [TextArea(3, 5)]
        string stageTips = "";
        Sequence panelSeq = null;
        [SerializeField] [I2.Loc.TermsPopup] string[] tipsTerms = null;

        [Header("BUTTON EVENT")]
        [SerializeField] BattleSceneButton battleButtons = null;

        [Header("GAMEOVER")]
        [SerializeField] Audio.ACSound soundEffect = null;

        private void OnEnable() {
            soundEffect.PlayOneShot();
        }

        public void OpenPanel() {
            //Save BackPanel Image Color
            var tempBackPanelColor = imagePanelBack.color;
            var saveBackPanelColor = imagePanelBack.color;
            tempBackPanelColor.a = 0f;
            imagePanelBack.color = tempBackPanelColor;

            //Save First Logo Position
            var tempLogoPos = logoTr.anchoredPosition;
            var saveLogoPos = tempLogoPos;
            tempLogoPos.y += 300f;
            logoTr.anchoredPosition = tempLogoPos;

            //Clear Text and Alpha
            buttonCanvasGroup.alpha = 0f;
            tmptips.text = "";

            //Get Random Tips String
            I2.Loc.LocalizedString localTipsString = tipsTerms.RandIndex<string>();

            panelTr.gameObject.SetActive(true);
            panelSeq = DOTween.Sequence()
                              .Append(logoTr.DOAnchorPos(saveLogoPos, 1f))
                              .Prepend(imagePanelBack.DOFade(saveBackPanelColor.a, .5f))
                              .Append(buttonCanvasGroup.DOFade(1f, 0.3f))
                              .Insert(1f, tmptips.DOText(localTipsString, 2f))
                              .OnComplete(() => frontPanelTr.gameObject.SetActive(false));
            //Don't set SetAutoKill because no re-run is required.
        }

        #region BUTTON
        public void ButtonFrontPanel() {
            if(panelSeq.IsPlaying()) {
                panelSeq.Complete(); //Skipping GameOver Panel Sequence.
            }

            //Disable Front Block Panel
            frontPanelTr.gameObject.SetActive(false);
        }

        public void ButtonLoadMain() => battleButtons.LoadMain();

        public void ButtonRestart() => battleButtons.Restart();

        public void ButtonResurrect() => battleButtons.Resurrect();
        #endregion
    }
}
