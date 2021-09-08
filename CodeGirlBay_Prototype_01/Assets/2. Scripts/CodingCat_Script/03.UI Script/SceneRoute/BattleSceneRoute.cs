namespace CodingCat_Games
{
    using UnityEngine;
    using UnityEngine.UI;
    using CodingCat_Scripts;
    using DG.Tweening;
    using System.Collections;

    public class BattleSceneRoute : MonoBehaviour
    {
        //Screen Limit Variable
        [Header("Visible Disable Limit Line")]
        public bool IsVisible;
        [Range(0f, 1.0f)] 
        public float LineWidth = 0.1f;
        public Material DefaultLineMat;

        [Header("START FADE OPTION")]
        public CanvasGroup ImgFade;
        public float FadeTime = 1.0f;

        [Header("PANEL's")]
        public GameObject PausePanel;
        public GameObject ResultPanel;
        public GameObject GameOverPanel;
        public float PanelOpenFadeTime = 0.5f;

        [Header("PLAYER's INIT")]
        public Transform ParentCanvas;
        public Transform InitPos;

        private float screenZpos = 90f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 2f);

        void Start()
        {
            #region LIMIT_LINE_MAKER
            if (IsVisible)
            {
                topLeftPoint = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height));
                bottomRightPoint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0f));

                limitPoints[0] = new Vector3(topLeftPoint.x - offset.x, topLeftPoint.y + offset.y, screenZpos);
                limitPoints[1] = new Vector3(bottomRightPoint.x + offset.x, topLeftPoint.y + offset.y, screenZpos);
                limitPoints[2] = new Vector3(bottomRightPoint.x + offset.x, bottomRightPoint.y - offset.y, screenZpos);
                limitPoints[3] = new Vector3(topLeftPoint.x - offset.x, bottomRightPoint.y - offset.y, screenZpos);
                limitPoints[4] = new Vector3(topLeftPoint.x - offset.x, (topLeftPoint.y + offset.y) +
                                             (LineWidth * 0.5f), screenZpos);

                arrowLimitLine = gameObject.AddComponent<LineRenderer>();
                arrowLimitLine.positionCount = 5;
                arrowLimitLine.SetPosition(0, limitPoints[0]);
                arrowLimitLine.SetPosition(1, limitPoints[1]);
                arrowLimitLine.SetPosition(2, limitPoints[2]);
                arrowLimitLine.SetPosition(3, limitPoints[3]);
                arrowLimitLine.SetPosition(4, limitPoints[4]);
                arrowLimitLine.startWidth = LineWidth;

                if (DefaultLineMat != null) arrowLimitLine.material = DefaultLineMat;

                arrowLimitLine.hideFlags = HideFlags.HideInInspector;
            }
            #endregion

            #region BATTLE_SCENE_INITIALIZING

            PausePanel.GetComponent<CanvasGroup>().alpha = 0f; //이거 무엇?

            //Battle Initializing
            GameManager.Instance.SetPooler();
            GameManager.Instance.SetPlayerBow(ParentCanvas, InitPos);

            //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
            this.OnSceneEnteringFadeOut();

            #endregion
        }


        #region Region_Button_Methods

        public void Btn_OpenPausePanel()
        {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(1f, PanelOpenFadeTime)
                      .OnStart(() => { PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                                       PausePanel.SetActive(true);})
                      .OnComplete(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = true);
        }

        public void Btn_ContinueGame()
        {
            PausePanel.GetComponent<CanvasGroup>().DOFade(0f, PanelOpenFadeTime)
                      .OnStart(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false)
                      .OnComplete(() => PausePanel.SetActive(false));
        }

        public void Btn_LoadMainScene()
        {
            ImgFade.DOFade(1f, FadeTime)
                   .OnStart(() => { ImgFade.blocksRaycasts = false;
                                    ImgFade.gameObject.SetActive(true);
                                    //CatPoolManager.Instance.ReleaseInstance();
                                    CCPooler.DestroyCCPooler();})
                   .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.Scene_Main));
        }

        #endregion

        private void OnSceneEnteringFadeOut()
        {
            if (GameManager.Instance.IsDevMode) return;

            //씬 진입 시 alpha 값 바꾸기 전 상황이 나오는 지 체크, alpha 값 바꿔주기 전 상황이 나오면
            //Build 시 alpha 값을 살려놓은 상태로 빌드..?
            ImgFade.alpha = 1f;

            ImgFade.DOFade(0f, FadeTime)
                   .OnStart(() =>
                   {
                       ImgFade.blocksRaycasts = false;
                   })
                   .OnComplete(() =>
                   {
                       ImgFade.blocksRaycasts = true;
                       ImgFade.gameObject.SetActive(false);
                   });
        }

        public void OnResultPanel()
        {
            if (ResultPanel.activeSelf) return;

            ResultPanel.SetActive(true);
        }
    }
}
