namespace ActionCat.UI {
    using UnityEngine;
    using System.Collections.Generic;
    using DG.Tweening;

    public class PanelClear : MonoBehaviour {
        [Header("CLEAR PANEL")]
        [SerializeField] RectTransform panelTr      = null;
        [SerializeField] RectTransform slotParentTr = null;
        [SerializeField] UI_ItemDataSlot slotPref   = null;
        [SerializeField] List<UI_ItemDataSlot> existSlots = null;

        [Header("BUTTON EVENT")]
        [SerializeField] BattleSceneButton battleButtons = null;

        public void OpenPanel(DropItem[] drops) {
            if (gameObject.activeSelf == true) return; //prevention duplicate active.
            CatLog.Log("Clear Game, Result Panel Udpated !");

            var currentExistSlotsCount = existSlots.Count;
            if(drops.Length > currentExistSlotsCount) {
                var needSlotCount = drops.Length - existSlots.Count;
                for (int i = 0; i < needSlotCount; i++) {
                    var newSlot = Instantiate<UI_ItemDataSlot>(slotPref, slotParentTr);
                    existSlots.Add(newSlot);
                }
            }

            RectTransform rootCanvas = panelTr.root.GetComponent<RectTransform>();
            for (int i = 0; i < drops.Length; i++) {
                existSlots[i].SetSlot(drops[i].ItemAsset, drops[i].Quantity, rootCanvas);
            }

            gameObject.SetActive(true);
        }

        #region BUTTON
        public void ButtonRestart() => battleButtons.Restart();

        public void ButtonLoadMain() => battleButtons.LoadMain();

        public void ButtonResurrect() => battleButtons.Resurrect();
        #endregion
    }

    [System.Serializable]
    internal sealed class BattleSceneButton {
        [Header("FADE")]
        [SerializeField] CanvasGroup fadePanel = null;
        [SerializeField] [ReadOnly]
        float defaultFadeTime = 1.0f;

        /// <summary>
        /// Editor로 Button 또는 EventTrigger로 캐싱사용 하려면 래핑해서 사용.
        /// </summary>
        public void Restart() {
            fadePanel.DOFade(StNum.floatOne, defaultFadeTime)
                     .SetUpdate(true)
                     .OnStart(() => {
                         fadePanel.blocksRaycasts = false;
                         fadePanel.gameObject.SetActive(true);
                     })
                     .OnComplete(() => {
                         Games.UI.ItemTooltip.Inst.ReleaseParent();
                         SceneLoader.Instance.ReloadScene(1f);
                     });
        }

        /// <summary>
        /// Editor로 Button 또는 EventTrigger로 캐싱사용 하려면 래핑해서 사용.
        /// </summary>
        public void LoadMain() {
            fadePanel.DOFade(StNum.floatOne, defaultFadeTime)
                .SetUpdate(true)
                .OnStart(() => {
                    fadePanel.blocksRaycasts = false;
                    fadePanel.gameObject.SetActive(true);
                })
                .OnComplete(() => {
                    Games.UI.ItemTooltip.Inst.ReleaseParent();
                    SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN, 1f);
                    
                });
        }

        /// <summary>
        /// Only Use PausePanel Scirpts
        /// </summary>
        /// <param name="panel"></param>
        public void Resume(PanelPause panel, CanvasGroup canvasGroup, float fadeTime) {
            canvasGroup.DOFade(StNum.floatZero, fadeTime)
                .SetUpdate(true)
                .OnStart(() => canvasGroup.blocksRaycasts = false)
                .OnComplete(() => {
                    GameManager.Instance.ResumeBattle();
                    panel.gameObject.SetActive(false);
                });
        }

        /// <summary>
        /// Editor로 Button 또는 EventTrigger로 캐싱사용 하려면 래핑해서 사용.
        /// </summary>
        public void Resurrect() {
            throw new System.NotImplementedException();
        }

        public void Settings() {
            throw new System.NotImplementedException();
        }
    }
}
