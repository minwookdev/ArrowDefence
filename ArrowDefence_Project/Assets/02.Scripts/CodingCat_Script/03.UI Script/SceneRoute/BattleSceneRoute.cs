namespace ActionCat.UI {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;

    public class BattleSceneRoute : MonoBehaviour {
        //Screen Limit Variable
        [Header("DEV OPTIONS")]
        public bool IsVisible;
        [Range(0f, 1.0f)] 
        public float LineWidth = 0.1f;
        public Material DefaultLineMat;
        public bool isOnFPS = false;
        public bool isActiveScreenHit = true;

        [Header("MODULE : PANEL")]
        [SerializeField] PanelClear panelClear   = null;
        [SerializeField] PanelDeath panelDeath   = null;
        [SerializeField] ComboCounter panelCombo = null;

        [Header("MODULE : SLOTS")]
        [SerializeField] SwapSlots arrSwapSlot = null;
        [SerializeField] AcspSlots acspSlots   = null;

        [Header("MODULE : BUTTON")]
        [SerializeField] Auto.AutoButton autoButton = null;

        [Header("MODULE : SLIDERS")]
        [SerializeField] ClearSlider stageClearSlider  = null;
        [SerializeField] SliderCtrl playerHealthSlider = null;
        [SerializeField] SliderCtrl bossHealthSlider   = null;

        [Header("ENTERING SCENE FADE OPTION")]
        public CanvasGroup ImgFade;
        public float FadeTime = 1.0f;

        [Header("PAUSE PANEL VARIABLES")]
        public GameObject PausePanel;
        public float PanelOpenFadeTime = 0.5f;

        [Header("RESULT PANEL VARIABLES")]
        public GameObject ResultPanel;
        public Transform SlotParentTr;
        public GameObject DropItemSlotPref;
        public List<UI_ItemDataSlot> DropItemSlots;

        [Header("GAMEOVER PANEL VARIABLES")]
        [SerializeField] GameObject overPanel = null;
        [SerializeField] GameObject overFrontPanel = null;
        [SerializeField] Transform overLogo = null;
        [SerializeField] Image overBackPanel = null;
        [SerializeField] CanvasGroup canvasGroupOverButton = null;
        [SerializeField] TextMeshProUGUI tmp_tips = null;
        [TextArea(3, 5)] public string StageTips = "";
        Sequence overSeq = null;

        [Header("HIT SCREEN")]
        [SerializeField] Material hitScreenMaterial = null;
        public float ScreenHitFadeTime = .5f;
        public float ScreenHitAlpha = 0.4f;
        float hitFadeTimer = 0f;
        string generalAlpha = "_Alpha";

        private float screenZpos = 0f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 2f);

        [Header("ITEM SLOT TOOLTIP")]
        public Canvas BattleSceneUICanvas;
        public Camera UICamera;

        #region SLIDER
        public ClearSlider StageClearSlider {
            get {
                if(stageClearSlider == null) {
                    throw new System.Exception("Clear Slider is Not Cached.");
                }
                return stageClearSlider;
            }
        }
        public SliderCtrl PlayerHealthSlider {
            get {
                if(playerHealthSlider == null) {
                    throw new System.Exception("Player Health Slider is Null.");
                }
                return playerHealthSlider;
            }
        }
        #endregion

        void Start() {
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

            //Item Data Slot Initializing
            //for (int i = 0; i < SlotParentTr.childCount; i++) {
            //    DropItemSlots.Add(SlotParentTr.GetChild(i).GetComponent<UI_ItemDataSlot>());
            //}

            //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
            this.OnSceneEnteringFadeOut();

            //FPS Checker Initialize
            if (isOnFPS) {
                gameObject.AddComponent<FrameRateCheck>();
            }
                

            #endregion
        }

        void Update() {
            HitScreenUpdate();
        }

        void OnDisable() {
            //Hit Screen Fade Color Check
            if(hitScreenMaterial.GetFloat(generalAlpha) != 0f) {
                hitScreenMaterial.SetFloat(generalAlpha, 0f);
            }
        }

        private void OnSceneEnteringFadeOut() {
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

        #region BUTTON_EVENT

        public void Btn_OpenPausePanel()
        {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(1f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => { PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false;
                                       PausePanel.SetActive(true);
                                       GameManager.Instance.PauseBattle();})
                      .OnComplete(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = true);
        }

        public void Btn_ContinueGame() {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(0f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false)
                      .OnComplete(() => { PausePanel.SetActive(false);
                                          GameManager.Instance.ResumeBattle();
                      });
        }

        public void Btn_LoadMainScene() {
            ImgFade.DOFade(1f, FadeTime)
                   .SetUpdate(true)
                   .OnStart(() => {    // 1. Phase Start
                       ImgFade.blocksRaycasts = false;
                       ImgFade.gameObject.SetActive(true);
                   }) 
                   .OnComplete(() => { // 2. Phase Ended
                       TooltipRelease();
                       SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN);
                   });
        }

        public void Btn_ReloadScene() {
            ImgFade.DOFade(1f, FadeTime)
                   .SetUpdate(true)
                   .OnStart(() => {    // 1. Phase Start.
                       ImgFade.blocksRaycasts = false;
                       ImgFade.gameObject.SetActive(true);
                   })
                   .OnComplete(() => { // 2. Phase Ended.
                       TooltipRelease();
                       SceneLoader.Instance.ReloadScene();
                   });
        }

        public void Btn_Resurrection() {
            //1. Kill All Monster's
            //2. Return the GameManager.GameState is In-Game State.
        }

        /// <summary>
        /// Item Tooltip Release
        /// </summary>
        void TooltipRelease() {
            //Release Item Info Tooltip
            Games.UI.ItemTooltip.Inst.ReleaseParent();
        }

        #endregion

        #region PANEL
        public void OnEnableResultPanel(List<DropItem> items) {
            if (ResultPanel.activeSelf) return;

            ResultPanel.SetActive(true);

            CatLog.Log("Result Panel Updated !");

            //Scene에 미리 깔려있는 Slot들은 Awake때 캐싱됨 
            //slotCount Number Object Disable된 상황에서도 잘 잡히는거 확인

            int slotCount = SlotParentTr.childCount;

            if(items.Count > slotCount)
            {
                int moreSlotCount = items.Count - slotCount;

                for (int i = 0; i < moreSlotCount; i++)
                {
                    var newSlot = Instantiate(DropItemSlotPref, SlotParentTr).GetComponent<UI_ItemDataSlot>();
                    newSlot.gameObject.SetActive(false);
                    DropItemSlots.Add(newSlot);
                }
            }

            for (int i = 0; i < items.Count; i++)
            {
                DropItemSlots[i].gameObject.SetActive(true);
                //DropItemSlots[i].SetSlot(items[i].ItemAsset, items[i].Quantity);
            }
        }

        public void OnEnableGameOverPanel() {
            //GameOver Back Panel Alpha Set.
            var tempBackPanelColor = overBackPanel.color;
            var backPanelOriginAlpha = tempBackPanelColor.a;
            tempBackPanelColor.a = 0f;
            overBackPanel.color = tempBackPanelColor;
            //Prepare Logo Position Set. (Save Origin Position.)
            var tempPos = overLogo.position;
            var originPos = tempPos;
            tempPos.y += 10f;
            overLogo.position = tempPos;
            //Button Group Fade Out
            canvasGroupOverButton.alpha = 0f;
            //Clear Tips String
            tmp_tips.text = "";

            overPanel.SetActive(true);
            overSeq = DOTween.Sequence()
                             .Append(overLogo.DOMove(originPos, 1f))
                             .Prepend(overBackPanel.DOFade(backPanelOriginAlpha, .5f))
                             .Append(canvasGroupOverButton.DOFade(1f, 0.3f))
                             .Insert(1f, tmp_tips.DOText(StageTips, 2f))
                             .OnComplete(() => overFrontPanel.SetActive(false));
            //Don't set SetAutoKill because no re-run is required.
        }

        public void OpenClearPanel(DropItem[] drops) {
            panelClear.OpenPanel(drops);
        }

        public void OpenGameOverPanel() {
            panelDeath.OpenPanel();
        }

        public void ComboCounterInit(float maxTime, bool inUnscaledTime) {
            if(panelCombo == null) {
                throw new System.Exception("ComboCounter Component Not Cached.");
            }

            panelCombo.InitComboCounter(maxTime, inUnscaledTime);
        }

        public void ComboCounterUpdate(short comboCount) {
            panelCombo.UpdateComboCounter(comboCount);
        }

        public void ComboCounterClear() {
            panelCombo.ComboClear();
        }

        #endregion

        #region UI_SLOTS

        public SwapSlots GetArrSwapSlots() {
            return (arrSwapSlot == null) ? throw new System.Exception("SwapSlot Component Not Cached.") : arrSwapSlot;
        }

        public AcspSlots GetAcspSlots() {
            return (acspSlots == null) ? throw new System.Exception("Accessory SkillSlot Component Not Cached.") : acspSlots;
        }

        #endregion

        #region SCREEN_HIT

        public void OnHitScreen() {
            //hitScreenMaterial.SetFloat(generalAlpha, ScreenHitAlpha);
            hitFadeTimer = ScreenHitFadeTime;
        }

        void HitScreenUpdate() {
            if(hitFadeTimer > 0f) {
                hitFadeTimer -= Time.unscaledDeltaTime;
                hitScreenMaterial.SetFloat(generalAlpha, Mathf.Lerp(ScreenHitAlpha, 0f, (1 - (hitFadeTimer / ScreenHitFadeTime))));
                if(hitFadeTimer <= 0f) {
                    hitScreenMaterial.SetFloat(generalAlpha, 0f);
                }
            }
        }

        #endregion

        #region AUTO_BUTTON

        public Auto.AutoButton GetAutoButton() {
            return (autoButton != null) ? autoButton : throw new System.Exception("AutoButton Component is Not Cached.");
        }

        #endregion
    }
}
