namespace ActionCat
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;
    using System.Collections.Generic;

    public class BattleSceneRoute : MonoBehaviour
    {
        public class ArrowSwapSlotInitData
        {
            public bool IsActiveSlot { get; private set; }
            public Sprite IconSprite { get; private set; }
            public System.Action SlotCallback { get; private set; }

            public ArrowSwapSlotInitData(bool isactiveslot, Sprite iconsprite, System.Action callback)
            {
                IsActiveSlot = isactiveslot;
                IconSprite   = iconsprite;
                SlotCallback = callback;
            }
        }

        //Screen Limit Variable
        [Header("DEV OPTIONS")]
        public bool IsVisible;
        [Range(0f, 1.0f)] 
        public float LineWidth = 0.1f;
        public Material DefaultLineMat;
        public bool isOnFPS = false;

        [Header("START FADE OPTION")]
        public CanvasGroup ImgFade;
        public float FadeTime = 1.0f;

        [Header("PANEL's")]
        public GameObject PausePanel;
        public GameObject ResultPanel;
        public GameObject GameOverPanel;
        public float PanelOpenFadeTime = 0.5f;

        [Header("RESULT PANEL VARIABLE's")]
        public Transform SlotParentTr;
        public GameObject DropItemSlotPref;
        public List<UI_ItemDataSlot> DropItemSlots;

        [Header("ARROW SLOT")]
        [SerializeField] EventTrigger mainArrowSlot;
        [SerializeField] EventTrigger subArrowSlot;
        [SerializeField] EventTrigger specialArrowSlot;

        [Header("SKILL SLOT")]
        [SerializeField] Transform skillSlotTr;
        [SerializeField] GameObject prefCooldownType;
        [SerializeField] GameObject prefChargingType;
        [SerializeField] GameObject prefHitsType;
        [SerializeField] GameObject prefkillType;

        private float screenZpos = 0f;
        private LineRenderer arrowLimitLine;
        private Vector2 topLeftPoint;
        private Vector2 bottomRightPoint;
        private Vector3[] limitPoints = new Vector3[5];
        private Vector2 offset = new Vector2(2f, 2f);

        [Header("ITEM SLOT TOOLTIP")]
        public Canvas BattleSceneUICanvas;
        public Camera UICamera;

        private void Awake()
        {
            //Camera Resolution Initialize
            GameManager.Instance.ResolutionPortrait(Camera.main);
            GameManager.Instance.ResolutionPortrait(UICamera);

        }

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

            //Item Data Slot Initializing
            for (int i = 0; i < SlotParentTr.childCount; i++)
            {
                DropItemSlots.Add(SlotParentTr.GetChild(i).GetComponent<UI_ItemDataSlot>());
            }

            //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
            this.OnSceneEnteringFadeOut();

            //FPS Checker Initialize
            if (isOnFPS)
                gameObject.AddComponent<FrameRateCheck>();

            #endregion
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

        public void Btn_ContinueGame()
        {
            PausePanel.GetComponent<CanvasGroup>()
                      .DOFade(0f, PanelOpenFadeTime)
                      .SetUpdate(true)
                      .OnStart(() => PausePanel.GetComponent<CanvasGroup>().blocksRaycasts = false)
                      .OnComplete(() => { PausePanel.SetActive(false);
                                          GameManager.Instance.ResumeBattle();
                      });
        }

        public void Btn_LoadMainScene()
        {
            ImgFade.DOFade(1f, FadeTime)
                   .SetUpdate(true)
                   .OnStart(() => { ImgFade.blocksRaycasts = false;
                                    ImgFade.gameObject.SetActive(true);}) 
                   .OnComplete(() => { ReleaseBattleScene();
                                       SceneLoader.Instance.LoadScene(AD_Data.SCENE_MAIN);});
        }

        /// <summary>
        /// Main Scene으로 넘어가기 전, Release되어야 할 로직들의 처리
        /// </summary>
        void ReleaseBattleScene()
        {
            //Bow Pulling Stop = false; 없어도 상관없음
            GameManager.Instance.SetBowPullingStop(false);

            //Release Player Equipments
            GameManager.Instance.ReleaseEquipments();

            //Release CCPooler 
            CCPooler.DestroyCCPooler();

            //Release Item Info Tooltip
            ActionCat.Games.UI.ItemTooltip.Instance.ReleaseParent();
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

        public void OnEnableResultPanel(List<DropItem> items)
        {
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
                DropItemSlots[i].Setup(items[i].ItemAsset, items[i].Quantity, BattleSceneUICanvas, UICamera);
            }
        }

        #region BATTLE_SLOTS

        public void InitArrowSlots(ArrowSwapSlotInitData[] datas)
        {
            for (int i = 0; i < datas.Length; i++)
            {
                if (i == 0)      //Main Arrow Swap Slot
                {
                    if (datas[i].IsActiveSlot == false)
                    {
                        mainArrowSlot.gameObject.SetActive(false);
                        continue;
                    }

                    mainArrowSlot.gameObject.SetActive(true);
                    var arrowIcon = mainArrowSlot.transform.GetChild(0).GetComponent<Image>();
                    arrowIcon.enabled = true;
                    arrowIcon.preserveAspect = true;
                    arrowIcon.sprite = datas[i].IconSprite;

                    //Click Event 처리
                    var tempIndex = i;
                    EventTrigger.Entry mainSlotEntry = new EventTrigger.Entry();
                    mainSlotEntry.eventID = EventTriggerType.PointerClick;
                    mainSlotEntry.callback.AddListener((pointereventdata) => datas[tempIndex].SlotCallback());
                    mainArrowSlot.triggers.Add(mainSlotEntry);

                    //Controller Pulling 예외처리
                    GameManager.Instance.PreventionPulling(mainArrowSlot);
                }
                else if (i == 1) //Sub Arrow Swap Slot
                {
                    if(datas[i].IsActiveSlot == false)
                    {
                        subArrowSlot.gameObject.SetActive(false);
                        continue;
                    }

                    subArrowSlot.gameObject.SetActive(true);
                    var arrowIcon = subArrowSlot.transform.GetChild(0).GetComponent<Image>();
                    arrowIcon.enabled = true;
                    arrowIcon.preserveAspect = true;
                    arrowIcon.sprite = datas[i].IconSprite;

                    var tempIndex = i;
                    EventTrigger.Entry subSlotEntry = new EventTrigger.Entry();
                    subSlotEntry.eventID = EventTriggerType.PointerClick;
                    subSlotEntry.callback.AddListener((pointereventdata) => datas[tempIndex].SlotCallback());
                    subArrowSlot.triggers.Add(subSlotEntry);

                    GameManager.Instance.PreventionPulling(subArrowSlot);
                }
            }

            specialArrowSlot.gameObject.SetActive(false);
        }

        public void InitArrowSlots(bool isActiveSlot_m, bool isActiveSlot_s, Sprite icon_m, Sprite icon_s,
                                   System.Action slotAction_m, System.Action slotAction_s)
        {
            //Active || Disable Arrow Swap Slot GameObject.
            //Event Registration according to the value of isActive boolean.

            if (isActiveSlot_m == true)
            {
                mainArrowSlot.gameObject.SetActive(true);
                var arrowicon            = mainArrowSlot.transform.GetChild(0).GetComponent<Image>();
                arrowicon.enabled        = true;
                arrowicon.preserveAspect = true;
                arrowicon.sprite         = icon_m;

                EventTrigger.Entry m_slotEntry = new EventTrigger.Entry();
                m_slotEntry.eventID = EventTriggerType.PointerClick;
                m_slotEntry.callback.AddListener((data) => slotAction_m());
                mainArrowSlot.triggers.Add(m_slotEntry);

                //Bow Controller Pulling Limit [Down, Up Event]
                //EventTrigger.Entry m_slotEntryDown = new EventTrigger.Entry();
                //m_slotEntryDown.eventID = EventTriggerType.PointerDown;
                //m_slotEntryDown.callback.AddListener((data) => GameManager.Instance.SetBowPullingStop(true));
                //mainArrowSlot.triggers.Add(m_slotEntryDown);
                //
                //EventTrigger.Entry m_slotEntryUp = new EventTrigger.Entry();
                //m_slotEntryUp.eventID = EventTriggerType.PointerUp;
                //m_slotEntryUp.callback.AddListener((data) => GameManager.Instance.SetBowPullingStop(false));
                //mainArrowSlot.triggers.Add(m_slotEntryUp);

                //Swap시, Bow Pulling 방지
                GameManager.Instance.PreventionPulling(mainArrowSlot);
            }
            else mainArrowSlot.gameObject.SetActive(false);

            if (isActiveSlot_s == true)
            {
                subArrowSlot.gameObject.SetActive(true);
                var arrowicon            = subArrowSlot.transform.GetChild(0).GetComponent<Image>();
                arrowicon.enabled        = true;
                arrowicon.preserveAspect = true;
                arrowicon.sprite         = icon_s;

                EventTrigger.Entry s_slotEntry = new EventTrigger.Entry();
                s_slotEntry.eventID = EventTriggerType.PointerClick;
                s_slotEntry.callback.AddListener((data) => slotAction_s());
                subArrowSlot.triggers.Add(s_slotEntry);

                //Swap시, Bow Pulling 방지
                GameManager.Instance.PreventionPulling(subArrowSlot);
            }
            else subArrowSlot.gameObject.SetActive(false);

            //Init Special Arrow Slot GameObject
            specialArrowSlot.gameObject.SetActive(false);

            //if Init the Special Arrow
            //if (isActiveSlot_m == false && isActiveSlot_s == false) return;
            //또는 SP Arrow가 장착된 경우
            //if(isActiveSlot_sp)
            //{
            //    if (isActiveSlot_m) { }
            //    if (isActiveSlot_s) { }
            //}
            //else
            //{
            //    if (isActiveSlot_s && isActiveSlot_m) { }
            //}
        }

        public void InitSkillSlots(AccessorySkillSlot.ActiveSkillSlotInitData[] datas)
        {
            foreach (var data in datas)
            {
                //if (data == null) continue; //-> 로직 변경으로 체크할 필요없어졌다.

                switch (data.ActiveType)
                {
                    case SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE:
                        var cooldownslot = Instantiate(prefCooldownType, skillSlotTr).GetComponent<AccessorySkillSlot>();
                        cooldownslot.InitCoolDownSkillButton(data); break;

                    case SKILL_ACTIVATIONS_TYPE.CHARGING_ACTIVE:
                        var chargingslot = Instantiate(prefChargingType, skillSlotTr).GetComponent<AccessorySkillSlot>();
                        chargingslot.InitStackingSkillButton(data); break;

                    case SKILL_ACTIVATIONS_TYPE.KILLCOUNT_ACTIVE: break;

                    case SKILL_ACTIVATIONS_TYPE.HITSCOUNT_ACTIVE: break;

                    default: break;
                }
            }
        }

        #endregion
    }
}
