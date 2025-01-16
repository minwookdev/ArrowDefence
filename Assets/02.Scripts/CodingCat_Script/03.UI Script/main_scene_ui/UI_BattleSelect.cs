namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.UI.Extensions;
    using ActionCat.Interface;
    using UI.MainMenu;

    public class UI_BattleSelect : MonoBehaviour, IMainMenu {
        MainMenuTween tween = new MainMenuTween(.5f, .3f);

        [Header("COMPONENT")]
        [SerializeField] RectTransform rectTr = null;
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] HorizontalScrollSnap horizontalScrollSnap = null;

        [Header("PAGE")]
        [SerializeField] RectTransform pageParentRectTr = null;
        [SerializeField] Image pageImage                = null;
        [SerializeField] Image[] pages                  = null;
        [SerializeField] [ReadOnly] private int currentPageIndex;
        private float disablePageAlpha = 0.4f;
        private float enablePageAlpha  = 1f;

        void Start() {
            CatLog.Log($"Total Horizontal Snap Page is: {horizontalScrollSnap.ChildObjects.Length}");

            //Scene에 미리 배치되어있는 GameObject 비활성화
            var existChildCounts = pageParentRectTr.childCount; 
            for (int i = 0; i < existChildCounts; i++) {
                pageParentRectTr.GetChild(i).gameObject.SetActive(false);
            }

            //페이지 이미지 캐싱 체크, 캐싱안됐으면 여기서 해줌.
            pageImage = (pageImage == null) ? pageParentRectTr.GetChild(0).GetComponent<Image>() : pageImage;

            //필요한 페이지 수 만큼 구성해주고 캐싱
            var totalPageCounts = horizontalScrollSnap.ChildObjects.Length;
            var imageList = new System.Collections.Generic.List<Image>();
            for (int i = 0; i < totalPageCounts; i++) {
                var instantiateImage = GameObject.Instantiate<Image>(pageImage, pageParentRectTr);  //필요 페이지 수 만큼 인스턴스 생성
                instantiateImage.gameObject.SetActive(true);    //끈 상태로 캐싱됐으니까 일단 켜주고, 
                instantiateImage.SetAlpha(disablePageAlpha);    //전부 비활성화 컬러로 변경
                imageList.Add(instantiateImage);                //캐싱 페이지 배열에 넣을거니까 임시 리스트에 담아줌
            }
            pages = imageList.ToArray(); //생성된 페이지들 캐싱

            //현재 활성화된 페이지의 넘버에 따라 페이지 인덱스 활성화 컬러로 변경
            currentPageIndex = horizontalScrollSnap.CurrentPage;
            pages[currentPageIndex].SetAlpha(enablePageAlpha);
            CatLog.Log($"Start Page Index: {currentPageIndex}");
        }

        void OnDisable() {
            Data.CCPlayerData.SaveSettingsJson();
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

        /// <summary>
        /// 페이지 변경 시 콜백
        /// </summary>
        public void OnPageChange() {
            pages[currentPageIndex].SetAlpha(disablePageAlpha);  //이전 페이지를 비활성 컬러로 변경
            currentPageIndex = horizontalScrollSnap.CurrentPage; //현재 페이지 넘버 받아옴 (Update)
            pages[currentPageIndex].SetAlpha(enablePageAlpha);   //현재 페이지를 활성화 컬러로 변경
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
