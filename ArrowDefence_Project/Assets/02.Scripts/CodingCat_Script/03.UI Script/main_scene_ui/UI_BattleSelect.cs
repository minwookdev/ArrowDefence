namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI.Extensions;
    using ActionCat.Interface;
    using UI.MainMenu;

    public class UI_BattleSelect : MonoBehaviour, IMainMenu {
        MainMenuTween tween = new MainMenuTween(.5f, .3f);

        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] HorizontalScrollSnap horizontalScrollSnap = null;

        void Start() {
            CatLog.Log($"Total Horizontal Snap Page is: {horizontalScrollSnap.ChildObjects.Length}");
        }

        void IMainMenu.MenuOpen() {
            tween.MenuOpenTween(rectTr, canvasGroup);
        }

        void IMainMenu.MenuClose() {
            tween.MenuCloseTween(rectTr, canvasGroup);
        }

        bool IMainMenu.IsTweenPlaying() {
            return tween.IsTweenPlaying;
        }

        public void GetCurrentPage() {
            CatLog.Log($"Current Horizontal Snap Page is: {horizontalScrollSnap.CurrentPage}");
        }

        public void ChangeEndEvent() {
            CatLog.Log("OnChangeEnd Event Called !");
            //보니까 페이지 이동이 완전히 정지한 상태에서 콜 하는 것을 확인함
        }
    }
}
