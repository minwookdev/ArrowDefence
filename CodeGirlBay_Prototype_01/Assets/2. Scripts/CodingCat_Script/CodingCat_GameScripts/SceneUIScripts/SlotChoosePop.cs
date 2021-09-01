namespace CodingCat_Games
{
    using CodingCat_Games.Data;
    using CodingCat_Scripts;
    using UnityEngine;

    public class SlotChoosePop : MonoBehaviour
    {
        public enum SLOTPANELTYPE
        {
            SLOT_ARROW,
            SLOT_ACCESSORY
        }

        [Header("LEFT PANEL")]
        [SerializeField] private GameObject objectLeftPanel;
        [SerializeField] private UI_ItemSlot arrowSlot0;
        [SerializeField] private UI_ItemSlot arrowSlot1;

        [Header("RIGHT PANEL")]
        [SerializeField] private GameObject objectRightPanel;
        [SerializeField] private UI_ItemSlot accessSlot0;
        [SerializeField] private UI_ItemSlot accessSlot1;
        [SerializeField] private UI_ItemSlot accessSlot2;

        private Item_Equipment itemAddress;

        public void Setup(SLOTPANELTYPE type, Item_Equipment item)
        {
            var playerEquips = CCPlayerData.equipments;

            if(type == SLOTPANELTYPE.SLOT_ARROW)
            {
                objectLeftPanel.SetActive(true);
                if (objectRightPanel.activeSelf) objectRightPanel.SetActive(false);

                itemAddress = item;

                if (playerEquips.IsEquipMainArrow())
                {
                    arrowSlot0.gameObject.SetActive(true);
                    arrowSlot0.Setup(playerEquips.GetMainArrow());
                }

                if(playerEquips.IsEquipSubArrow())
                {
                    arrowSlot1.gameObject.SetActive(true);
                    arrowSlot1.Setup(playerEquips.GetSubArrow());
                }

            }
            else if(type == SLOTPANELTYPE.SLOT_ACCESSORY)
            {
                objectRightPanel.SetActive(true);
                if (objectRightPanel.activeSelf) objectLeftPanel.SetActive(false);

                itemAddress = item;

                if(playerEquips.IsEquipAccessory())
                {
                    accessSlot0.gameObject.SetActive(true);
                    accessSlot0.Setup(playerEquips.GetAccessory());
                }
            }

            CatLog.Log($"Get Equip Item Address : {itemAddress.GetName}");
        }

        public void Clear()
        {
            if(objectLeftPanel.activeSelf)
            {
                if (arrowSlot0.gameObject.activeSelf) arrowSlot0.Clear();
                if (arrowSlot1.gameObject.activeSelf) arrowSlot1.Clear();

                objectLeftPanel.SetActive(false);
            }
            else if (objectRightPanel.activeSelf)
            {
                if (accessSlot0.gameObject.activeSelf) accessSlot0.Clear();
                if (accessSlot1.gameObject.activeSelf) accessSlot1.Clear();
                if (accessSlot2.gameObject.activeSelf) accessSlot2.Clear();

                objectRightPanel.SetActive(false);
            }
        }

        #region BUTTON_ACTION

        public void Button_Exit()
        {
            this.Clear();
            itemAddress = null;
            gameObject.SetActive(false);
        }

        public void Button_EquipSlot(int num)
        {
            if (itemAddress == null) return;

            switch (itemAddress)
            {
                case Item_Arrow arrow:         ChooseSlot(num, arrow);     break;
                case Item_Accessory accessory: ChooseSlot(num, accessory); break;
                default: break;
            }

            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();
            this.Button_Exit();
        }

        private void ChooseSlot(int num, Item_Arrow item)
        {
            switch (num)
            {
                case 0: CCPlayerData.equipments.Equip_ArrowItem(item); break;
                case 1: CCPlayerData.equipments.Equip_SubArrow(item);  break;
                default: CatLog.Log("Worng Slot Number's, Check Slot Button"); break;
            }
        }

        private void ChooseSlot(int num, Item_Accessory item)
        {
            switch (num)
            {
                case 2: CCPlayerData.equipments.Equip_AccessoryItem(item); break;
                case 3: break;
                case 4: break;
                default: CatLog.Log("Wrong Slot Number's Check Slot Button"); break;
            }
        }

        #endregion
    }
}
