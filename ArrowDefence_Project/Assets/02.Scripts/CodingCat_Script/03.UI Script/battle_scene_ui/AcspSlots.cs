namespace ActionCat.UI {
    using UnityEngine;

    public class AcspSlots : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform slotBodyTr  = null;
        [SerializeField] RectTransform slotGroupTr = null;

        [Header("SLOT PREF")]
        [SerializeField] AccessorySkillSlot cooldownTypeSlotPref = null;
        [SerializeField] AccessorySkillSlot chargingTypeSlotPref = null;
        [SerializeField] AccessorySkillSlot hitTypeSlotPref  = null;
        [SerializeField] AccessorySkillSlot killTypeSlotPref = null;

        [Header("SLOTS")]
        [SerializeField] [ReadOnly] AccessorySkillSlot[] slots = null;

        public void InitSlots(ACSData[] array) {
            var list = new System.Collections.Generic.List<AccessorySkillSlot>();
            //for (int i = 0; i < array.Length; i++) {
            //    switch (array[i].ActiveType) {
            //        case ACSPACTIVETYPE.COOLDOWN: Instantiate<AccessorySkillSlot>(cooldownTypeSlotPref, slotGroupTr).InitSlot(array[i]); break;
            //        case ACSPACTIVETYPE.CHARGING:  slotPref = chargingTypeSlotPref; break;
            //        case ACSPACTIVETYPE.KILLCOUNT: slotPref = hitTypeSlotPref;      break;
            //        case ACSPACTIVETYPE.HITCOUNT:  slotPref = killTypeSlotPref;     break;
            //        default: throw new System.NotImplementedException("This SlotType is Not Implemented.");
            //    }
            //}
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
