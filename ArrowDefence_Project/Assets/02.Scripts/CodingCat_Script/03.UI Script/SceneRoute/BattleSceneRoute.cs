namespace ActionCat.UI {
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using TMPro;

    public class BattleSceneRoute : MonoBehaviour {
        [Header("MODULE : PANEL")]
        [SerializeField] PanelClear panelClear   = null;
        [SerializeField] PanelDeath panelDeath   = null;
        [SerializeField] PanelPause panelPause   = null;
        [SerializeField] ComboCounter panelCombo = null;

        [Header("MODULE : SLOTS")]
        [SerializeField] SwapSlots slotArrSwap = null;
        [SerializeField] AcspSlots slotAcsp    = null;

        [Header("MODULE : BUTTON")]
        [SerializeField] Auto.AutoButton buttonAuto = null;

        [Header("MODULE : SLIDERS")]
        [SerializeField] ClearSlider sliderStageClear  = null;
        [SerializeField] SliderCtrl sliderPlayerHealth = null;
        [SerializeField] SliderCtrl sliderBossHealth   = null;

        [Header("HIT SCREEN")]
        [SerializeField] Material hitScreenMaterial = null;
        [SerializeField] float ScreenHitFadeTime = .5f;
        [SerializeField] float ScreenHitAlpha    = 0.4f;
        float currentHitFadeTime = 0f;
        string shaderAlphaString = "_Alpha";

        [Header("FADE")]
        [SerializeField] CanvasGroup fadeCanvasGroup = null;
        [SerializeField] [RangeEx(0.5f, 2f, .5f)] 
        float fadeOutTime = 1.0f;

        [Header("OPTIONS")]
        [SerializeField] bool isOnDisableLine;
        [SerializeField] bool isOnFPS = false;
        [SerializeField] Material DefaultLineMat;
        float LineWidth = 0.1f;

        [Header("COMPONENT")]
        [SerializeField] RectTransform canvasRectTr = null;

        private float screenZpos = 0f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 3f);

        #region PROPERTY
        public SwapSlots SlotArrSwap {
            get {
                if(slotArrSwap == null) {
                    throw new System.Exception("Route: ArrSwap Slot Component is Null.");
                }
                return slotArrSwap;
            }
        }
        public AcspSlots SlotAcSkill {
            get {
                if(slotAcsp == null) {
                    throw new System.Exception("Route: Accessory Special Skill Slot Component is Null.");
                }
                return slotAcsp;
            }
        }
        public Auto.AutoButton ButtonAuto {
            get {
                if(buttonAuto == null) {
                    throw new System.Exception("Route: AutoButton Component is Null.");
                }
                return buttonAuto;
            }
        }
        #endregion

        void Awake() {
            FadeOut();
        }

        void Start() {
            #region LIMIT_LINE_MAKER
            if (isOnDisableLine == true) {
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

                if (DefaultLineMat != null) {
                    arrowLimitLine.material = DefaultLineMat;
                }

                arrowLimitLine.hideFlags = HideFlags.HideInInspector;
            }
            #endregion

            //FPS Checker Initialize
            if (isOnFPS == true) {
                gameObject.AddComponent<FrameRateCheck>();
            }

            //Initialize Notify
            Notify.Inst.Init(canvasRectTr);
        }

        void Update() {
            HitScreenUpdate();
        }

        void OnDisable() {
            //Claen up Screen Hit Material
            if(hitScreenMaterial.GetFloat(shaderAlphaString) != 0f) {
                hitScreenMaterial.SetFloat(shaderAlphaString, 0f);
            }
        }

        void FadeOut() {
            fadeCanvasGroup.alpha = StNum.floatOne;
            fadeCanvasGroup.DOFade(StNum.floatZero, fadeOutTime)
                .OnStart(() => fadeCanvasGroup.blocksRaycasts = false)
                .OnComplete(() => {
                    fadeCanvasGroup.blocksRaycasts = true;
                    fadeCanvasGroup.gameObject.SetActive(false);
                });
        }

        #region Ref_Btn
        public void ButtonPause() => panelPause.OpenPanel();
        #endregion

        #region MODULE:PANEL
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

        #region MODULE:SLIDER
        public void PlayerSliderDec(float dest) {
            sliderPlayerHealth.Decrease(dest);
        }

        public void ClearSliderInit(bool isBossExist, float maxTime) {
            sliderStageClear.InitSlider(isBossExist, maxTime);
        }

        public void ClearSliderUpdate(float time) {
            sliderStageClear.UpdateSlider(time);
        }

        public void SliderDecBoss(float dest) {
            sliderBossHealth.Decrease(dest);
        }
        #endregion

        #region SCREEN_HIT

        public void OnHitScreen() {
            //hitScreenMaterial.SetFloat(generalAlpha, ScreenHitAlpha);
            currentHitFadeTime = ScreenHitFadeTime;
        }

        void HitScreenUpdate() {
            if(currentHitFadeTime > 0f) {
                currentHitFadeTime -= Time.unscaledDeltaTime;
                hitScreenMaterial.SetFloat(shaderAlphaString, Mathf.Lerp(ScreenHitAlpha, 0f, (1 - (currentHitFadeTime / ScreenHitFadeTime))));
                if(currentHitFadeTime <= 0f) {
                    hitScreenMaterial.SetFloat(shaderAlphaString, 0f);
                }
            }
        }

        #endregion
    }
}
