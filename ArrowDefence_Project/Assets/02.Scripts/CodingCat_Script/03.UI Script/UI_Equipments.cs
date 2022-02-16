namespace ActionCat
{
    using ActionCat.Data;
    using UnityEngine;
    using UnityEngine.UI;

    public class UI_Equipments : MonoBehaviour
    {
        static public UI_Equipments Instance;

        [Header("Player Equipment UI")]
        public UI_ItemSlot BowItem_Slot;

        [Space(10)]
        public UI_ItemSlot ArrowItem_Slot0;
        public UI_ItemSlot ArrowItem_Slot1;
        public UI_ItemSlot ArrowItem_Slot_Sp;

        [Space(10)]
        public UI_ItemSlot Access_Slot0;
        public UI_ItemSlot Access_Slot1;
        public UI_ItemSlot Access_Slot2;

        [Header("Equip Item Slot Prefab")]
        public GameObject equipSlotPref;

        [Header("Choose Equipment Slot Panel")]
        [SerializeField] private SlotChoosePop choosePanel;

        [Header("Show case")]
        [SerializeField] Image imageShowcase = null;

        /*
           EquipSlot Update 되는 시점 : 인벤토리 UI 열린 시점, 
                                       장비 아이템을 장착 시점, 
                                       장비 아이템을 해제 시점                           
        */

        private void Awake() => Instance = this;

        void OnEnable()
        {
            UpdateEquipUI();
        }

        private void OnDestroy() {
            Instance = null;
        }

        public void UpdateEquipUI()
        {
            var equipment = CCPlayerData.equipments;

            //Equipment Item : Bow
            if(equipment.IsEquippedBow()) {
                var item = equipment.GetBowItem();

                //Init-Bow Slot
                BowItem_Slot.gameObject.SetActive(true);
                BowItem_Slot.Setup(item);

                //Init-Bow ShowCase
                UpdateShowcase(item);
            }
            else {
                if (BowItem_Slot.gameObject.activeSelf)
                    BowItem_Slot.Clear();
                imageShowcase.enabled = false;
            }

            //Equipment Item : Arrow (Main)
            if(equipment.IsEquippedArrowMain())
            {
                ArrowItem_Slot0.gameObject.SetActive(true);
                ArrowItem_Slot0.Setup(CCPlayerData.equipments.GetMainArrow());
            }
            else
            {
                if (ArrowItem_Slot0.gameObject.activeSelf) ArrowItem_Slot0.Clear();
            }

            //Equipment Item : Arrow (Sub)
            if(equipment.IsEquippedArrowSub())
            {
                ArrowItem_Slot1.gameObject.SetActive(true);
                ArrowItem_Slot1.Setup(CCPlayerData.equipments.GetSubArrow());
            }
            else
            {
                if (ArrowItem_Slot1.gameObject.activeSelf) ArrowItem_Slot1.Clear();
            }

            //Equipment Item : Accessories
            var accessories = equipment.GetAccessories();

            for (int i = 0; i < accessories.Length; i++)
            {
                if (accessories[i] != null)
                {
                    switch (i) { 
                        case 0: Access_Slot0.gameObject.SetActive(true); Access_Slot0.Setup(accessories[i]); break;
                        case 1: Access_Slot1.gameObject.SetActive(true); Access_Slot1.Setup(accessories[i]); break;
                        case 2: Access_Slot2.gameObject.SetActive(true); Access_Slot2.Setup(accessories[i]); break;
                    }
                }
                else
                {
                    switch (i) { 
                        case 0: if (Access_Slot0.gameObject.activeSelf) Access_Slot0.Clear(); break;
                        case 1: if (Access_Slot1.gameObject.activeSelf) Access_Slot1.Clear(); break;
                        case 2: if (Access_Slot2.gameObject.activeSelf) Access_Slot2.Clear(); break;
                    }
                }
            }

            //Equipment Item Slot :: Special Arrow
            if (equipment.IsEquippedSpArr) {
                ArrowItem_Slot_Sp.Setup(equipment.GetSpArrOrNull);
                ArrowItem_Slot_Sp.gameObject.SetActive(true);
            }
            else {
                if(ArrowItem_Slot_Sp.gameObject.activeSelf) {
                    ArrowItem_Slot_Sp.Clear();
                }
            }
        }

        public void OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE type, Item_Equipment itemAddress) {
            choosePanel.Setup(type, itemAddress);
        }

        void UpdateShowcase(Item_Bow item) {
            if (item.GetGameObjectOrNull() == null) {
                imageShowcase.enabled = false; 
                return;
            }

            if(item.GetGameObjectOrNull().TryGetComponent<BowSprite>(out BowSprite bowSprite)) {
                if(bowSprite.GetUISprite() == null) {
                    imageShowcase.enabled = false; 
                    return;
                }

                imageShowcase.enabled = true;
                imageShowcase.sprite = bowSprite.GetUISprite();
            }
            else {
                imageShowcase.enabled = false;
            }
        }
    }
}
