namespace ActionCat.UI {
    using UnityEngine;
    using ActionCat.Interface;
    using UI.MainMenu;

    public class UI_BattleSelect : MonoBehaviour, IMainMenu {
        MainMenuTween tween = new MainMenuTween(.5f, .3f);

        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] CanvasGroup canvasGroup = null;

        void IMainMenu.MenuOpen() {
            tween.MenuOpenTween(rectTr, canvasGroup);
        }

        void IMainMenu.MenuClose() {
            tween.MenuCloseTween(rectTr, canvasGroup);
        }

        bool IMainMenu.IsTweenPlaying() {
            return tween.IsTweenPlaying;
        }
    }
}
