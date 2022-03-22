namespace ActionCat.UI {
    using UnityEngine;
    using ActionCat.Interface;

    public class UI_Shop : MonoBehaviour, IMainMenu {
        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr    = null;
        [SerializeField] CanvasGroup canvasGroup = null;

        MainMenu.MainMenuTween tween = new MainMenu.MainMenuTween(0.5f, 0.3f);

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
