namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    public class AcspSlots : MonoBehaviour, IPointerDownHandler {
        [Header("COMPONENT")]
        [SerializeField] RectTransform slotBodyTr  = null;
        [SerializeField] RectTransform slotGroupTr = null;
        [SerializeField] CanvasGroup canvasGroup   = null;
        [SerializeField] GameObject blockPanel     = null;
        [SerializeField] TouchPosDetector touchPosDetector = null;

        [Header("SLIDER")]
        [SerializeField] [RangeEx(1f, 5f, 1f)]   float maxOpenedTime = 3f;
        [SerializeField] [RangeEx(.1f, 3f, .1f)] float fadeSpeed     = 1f;
        [SerializeField] [ReadOnly] bool isOpen = true;
        [SerializeField] bool isUseUnscaledTime = true;
        float currOpenedTime = 0f;
        float openPosX;
        float closePosX;

        [Header("SPELL SLOT PREFAB")] // Condition에 따른 발동슬롯 분류
        [SerializeField] ArtifactSlot_Trigger triggerTypeSlotPref = null;
        [SerializeField] ArtifactSlot_Buff buffTypeSlotPref       = null;
        [SerializeField] ArtifactSlot_Debuff debuffTypeSlotPref   = null; /*
        [SerializeField] AccessorySkillSlot switchTypeSlotPref    = null; <--- NotImplemented*/

        [Header("SLOTS")] [Tooltip("Do Not Modify this Field")]
        [SerializeField] [ReadOnly] AccessorySkillSlot[] enabledSlots = null;

        [Header("NOTIFY")]
        [SerializeField] SlotNotify notify = null;

        public void InitSlots(AccessorySPEffect[] effects) {
            var tempSlotList = new System.Collections.Generic.List<AccessorySkillSlot>();
            for (int i = 0; i < effects.Length; i++) {
                switch (effects[i].Condition.ConditionType) {
                    case ARTCONDITION.TRIGGER: tempSlotList.Add(Instantiate<ArtifactSlot_Trigger>(triggerTypeSlotPref, slotGroupTr).Init(effects[i]));       break;
                    case ARTCONDITION.BUFF:    tempSlotList.Add(Instantiate<ArtifactSlot_Buff>(buffTypeSlotPref, slotGroupTr).Init(effects[i], PlayNotify)); break;
                    case ARTCONDITION.DEBUFF:  tempSlotList.Add(Instantiate<ArtifactSlot_Debuff>(debuffTypeSlotPref, slotGroupTr).Init(effects[i]));         break;
                    default: throw new System.NotImplementedException();
                }
            }

            enabledSlots = tempSlotList.ToArray();
            if (enabledSlots.Length <= 0) { //활성화된 발동슬롯이 없다면 Disable
                gameObject.SetActive(false);
                return;
            }
            else {
                //Effect Activation Notify System Init
                notify.Init();

                //활성화 된 슬롯과 패널의 트리거 메서드 추가
                var slotOpenEntry = new EventTrigger.Entry();
                slotOpenEntry.eventID = EventTriggerType.PointerDown;
                slotOpenEntry.callback.AddListener(eventData => TimerReset());

                foreach (var slot in enabledSlots) {
                    if (slot.TryGetComponent<EventTrigger>(out EventTrigger eventTrigger)) {
                        eventTrigger.triggers.Add(slotOpenEntry);
                    }
                    else {
                        CatLog.ELog($"Accessory Special Skill Slot : {slot.name} is Not have EventTrigger Component.");
                    }
                }

                if (blockPanel.TryGetComponent<EventTrigger>(out EventTrigger backPanelEventTrigger)) {
                    backPanelEventTrigger.triggers.Add(slotOpenEntry);
                }
                else {
                    CatLog.WLog("Backpanel EventTrigger is Null.");
                }
            }
        }

        void Start() {
            openPosX  = slotBodyTr.anchoredPosition.x;
            closePosX = slotBodyTr.anchoredPosition.x + slotBodyTr.rect.width;

            TimerReset();
        }

        void Update() {
            if(currOpenedTime <= 0) {
                if(isOpen == true) {
                    HidePanel();
                }
            }
            else {
                if (isUseUnscaledTime == true) currOpenedTime -= Time.unscaledDeltaTime;
                else                           currOpenedTime -= Time.deltaTime;

                if(isOpen == false) {
                    ShowPanel();
                }
            }
        }

        void ShowPanel() {
            canvasGroup.DOFade(StNum.floatOne, fadeSpeed).OnStart(() => blockPanel.SetActive(false));
            isOpen = true;
        }

        void HidePanel() {
            canvasGroup.DOFade(StNum.floatZero, fadeSpeed).OnStart(() => blockPanel.SetActive(true));
            isOpen = false;
        }

        void TimerReset() {
            currOpenedTime = maxOpenedTime;
            if (notify.IsPlaying()) {
                notify.Stop();
            }
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            TimerReset();
        }

        void PlayNotify() {
            if (isOpen) {
                return;
            }

            notify.Play();
        }

        void OpenTouchPosDetector(float radius, ITouchPosReceiver receiver) {
            touchPosDetector.OpenDetector(radius, receiver);
        }
    }

    public sealed class ACSData {
        public ACSPACTIVETYPE ActiveType { get; private set; } = ACSPACTIVETYPE.NONE;
        public Sprite IconSprite { get; private set; } = null;
        public float MaxCount { get; private set; } = 0f;
        public bool IsPrepared { get; private set; } = false;
        public System.Func<MonoBehaviour, float> SkillFunc { get; private set; } = null;
        public System.Action SkillStopCallback { get; private set; } = null;

        public ACSData(ACSPACTIVETYPE type, Sprite icon, System.Func<MonoBehaviour, float> skillFunc, System.Action stopSkill,
                       float maxCount, bool prepared = false) {
            ActiveType = type;
            IconSprite = icon;
            MaxCount   = maxCount;
            IsPrepared = prepared;
            SkillFunc  = skillFunc;
            SkillStopCallback = stopSkill;
        }
    }
}
