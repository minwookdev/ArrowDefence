namespace ActionCat
{
    using ActionCat.Data;
    using UnityEngine;

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
            if(equipment.IsEquippedBow())
            {
                BowItem_Slot.gameObject.SetActive(true);
                BowItem_Slot.Setup(CCPlayerData.equipments.GetBowItem());
            }
            else
            {
                if (BowItem_Slot.gameObject.activeSelf) BowItem_Slot.Clear();
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
        }

        public void OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE type, Item_Equipment itemAddress)
        {
            //choosePanel.gameObject.SetActive(true);
            choosePanel.Setup(type, itemAddress);
        }

        //public static void Update_EquipUI()
        //{
        //    //Bow Item Slot
        //    if(CCPlayerData.equipments.IsEquipBow())
        //    {
        //        Instance.BowItem_Slot.gameObject.SetActive(true);
        //        Instance.BowItem_Slot.Setup(CCPlayerData.equipments.GetBowItem());
        //    }
        //    else
        //    {
        //        if (Instance.BowItem_Slot.gameObject.activeSelf) Instance.BowItem_Slot.Clear();
        //    }
        //
        //    //Arrow Item Main Slot
        //    if (CCPlayerData.equipments.IsEquipMainArrow())
        //    {
        //        Instance.ArrowItem_Slot0.gameObject.SetActive(true);
        //        Instance.ArrowItem_Slot0.Setup(CCPlayerData.equipments.GetMainArrow());
        //    }
        //    else
        //    {
        //        if (Instance.ArrowItem_Slot0.gameObject.activeSelf) Instance.ArrowItem_Slot0.Clear();
        //    }
        //
        //    //Arrow Item Sub Slot
        //    if (CCPlayerData.equipments.IsEquipSubArrow())
        //    {
        //        Instance.ArrowItem_Slot1.gameObject.SetActive(true);
        //        Instance.ArrowItem_Slot1.Setup(CCPlayerData.equipments.GetSubArrow());
        //    }
        //    else
        //    {
        //        if (Instance.ArrowItem_Slot1.gameObject.activeSelf) Instance.ArrowItem_Slot1.Clear();
        //    }
        //
        //    //Accessory Item Slot 
        //    if(CCPlayerData.equipments.IsEquipAccessory())
        //    {
        //        Instance.EquipItem_Slot_0.gameObject.SetActive(true);
        //        Instance.EquipItem_Slot_0.Setup(CCPlayerData.equipments.GetAccessory());
        //    }
        //    else
        //    {
        //        if (Instance.EquipItem_Slot_0.gameObject.activeSelf) Instance.EquipItem_Slot_0.Clear();
        //    }
        //}

        //public static void 
    }
}
