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

        [Header("SLIDER")]
        [SerializeField] [RangeEx(1f, 5f, 1f)]   float maxOpenedTime = 3f;
        [SerializeField] [RangeEx(.1f, 3f, .1f)] float fadeSpeed     = 1f;
        [SerializeField] [ReadOnly] bool isOpen = true;
        [SerializeField] bool isUseUnscaledTime = true;
        float currOpenedTime = 0f;
        float openPosX;
        float closePosX;

        [Header("SLOT PREF")]
        [SerializeField] AccessorySkillSlot cooldownSlotPref = null;
        [SerializeField] AccessorySkillSlot chargingSlotPref = null;
        [SerializeField] AccessorySkillSlot hitTypeSlotPref  = null;
        [SerializeField] AccessorySkillSlot killTypeSlotPref = null;

        [Header("SLOTS")] [Tooltip("Do Not Modify this Field")]
        [SerializeField] [ReadOnly] AccessorySkillSlot[] slots = null;

        [Header("NOTIFY")]
        [SerializeField] SlotNotify notify = null;

        public void InitSlots(ACSData[] array) {
            var list = new System.Collections.Generic.List<AccessorySkillSlot>();
            for (int i = 0; i < array.Length; i++) {
                switch (array[i].ActiveType) {
                    case ACSPACTIVETYPE.COOLDOWN:  list.Add(Instantiate<AccessorySkillSlot>(cooldownSlotPref, slotGroupTr).InitSlot(array[i], PlayNotify)); break;
                    case ACSPACTIVETYPE.CHARGING:  list.Add(Instantiate<AccessorySkillSlot>(chargingSlotPref, slotGroupTr).InitSlot(array[i]));             break;
                    case ACSPACTIVETYPE.KILLCOUNT: list.Add(Instantiate<AccessorySkillSlot>(killTypeSlotPref, slotGroupTr).InitSlot(array[i]));             break;
                    case ACSPACTIVETYPE.HITCOUNT:  list.Add(Instantiate<AccessorySkillSlot>(hitTypeSlotPref, slotGroupTr).InitSlot(array[i]));              break;
                    default: throw new System.NotImplementedException("This SlotType is Not Implemented.");
                }
            }

            //Add Slots Array in Instantiated Slot Prefab.
            slots = list.ToArray();

            if(slots.Length == 0) {
                gameObject.SetActive(false);
                return;
            }
            else {
                var entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerDown;
                entry.callback.AddListener(eventdata => TimerReset());

                //Init Slot Notify System
                notify.Init();

                foreach (var slot in slots) {
                    if(slot.TryGetComponent<EventTrigger>(out EventTrigger eventTrigger)) {
                        eventTrigger.triggers.Add(entry);
                    }
                    else {
                        CatLog.WLog($"Accessory Special Skill Slot : {slot.name} is Not have EventTrigger Component.");
                        continue;
                    }
                }


                if(blockPanel.TryGetComponent<EventTrigger>(out EventTrigger panelEventTrigger)) {
                    panelEventTrigger.triggers.Add(entry);
                }
                else {
                    CatLog.WLog("ACSP SLOTS : BlockPanel EventTrigger Component is Null");
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
