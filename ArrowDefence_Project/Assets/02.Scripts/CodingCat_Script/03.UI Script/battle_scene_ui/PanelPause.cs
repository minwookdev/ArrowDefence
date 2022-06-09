namespace ActionCat.UI {
    using UnityEngine;
    using DG.Tweening;

    public class PanelPause : MonoBehaviour {
        [Header("COMPONENT")]
        //[SerializeField] RectTransform pausePanelTr   = null;
        [SerializeField] CanvasGroup panelCanvasGroup = null;
        [SerializeField] BattleSceneRoute route = null;

        [Header("PAUSE PANEL")]
        [SerializeField] [RangeEx(0.5f, 2f, 0.5f)]
        float openFadeTime = 0.5f;

        [Header("BUTTON EVENT")]
        [SerializeField] BattleSceneButton battleButtons = null;

        public void OpenPanel() {
            panelCanvasGroup.alpha = StNum.floatZero;
            gameObject.SetActive(true);
            panelCanvasGroup.DOFade(StNum.floatOne, openFadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    panelCanvasGroup.blocksRaycasts = false;
                    GameManager.Instance.PauseBattle();
                })
                .OnComplete(() => {
                    panelCanvasGroup.blocksRaycasts = true;
                });
        }

        #region Ref_Btn

        public void ButtonResume() {
            if (GameManager.Instance.IsControlTypeChanged()) {  //Settings Panel에서 Control Type를 바꿧으면 Controller에 업데이트를 요청
                GameManager.Instance.GetControllerInstOrNull().ReloadControlType();
            }
            battleButtons.Resume(this, panelCanvasGroup, openFadeTime, route.ExitPause);
        }

        public void ButtonRestart() {
            battleButtons.Restart();
        }

        public void ButtonLoadMain() {
            battleButtons.LoadMain();
        }

        public void ButtonSettings() {
            battleButtons.Settings();
        }

        #endregion
    }
}
