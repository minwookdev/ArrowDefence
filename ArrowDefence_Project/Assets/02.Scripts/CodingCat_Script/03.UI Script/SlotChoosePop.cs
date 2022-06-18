namespace ActionCat
{
    using ActionCat.Data;
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
        public UI_ItemSlot[] accessSlots = new UI_ItemSlot[3];

        private Item_Equipment itemAddress;

        [Header("SOUND EFFECT")]
        [SerializeField] private Audio.ACSound soundEffect = null;

        public void Setup(SLOTPANELTYPE type, Item_Equipment item)
        {
            if (this.gameObject.activeSelf == false)
                gameObject.SetActive(true);

            var playerEquips = CCPlayerData.equipments;
            itemAddress = item;

            if(type == SLOTPANELTYPE.SLOT_ARROW)
            {
                objectLeftPanel.SetActive(true);
                if (objectRightPanel.activeSelf)
                    objectRightPanel.SetActive(false);

                if (playerEquips.IsEquippedArrMain)
                {
                    arrowSlot0.gameObject.SetActive(true);
                    arrowSlot0.EnableSlot(playerEquips.GetMainArrow());
                }

                if(playerEquips.IsEquippedArrSub)
                {
                    arrowSlot1.gameObject.SetActive(true);
                    arrowSlot1.EnableSlot(playerEquips.GetSubArrow());
                }

            }
            else if(type == SLOTPANELTYPE.SLOT_ACCESSORY)
            {
                objectRightPanel.SetActive(true);
                if (objectLeftPanel.activeSelf)
                    objectLeftPanel.SetActive(false);

                var accessories = playerEquips.GetAccessories();

                for (int i = 0; i < accessories.Length; i++)
                {
                    if(accessories[i] != null)
                    {
                        accessSlots[i].gameObject.SetActive(true);
                        accessSlots[i].EnableSlot(accessories[i]);
                    }
                    continue;
                }
            }

            CatLog.Log($"Get Equip Item Address : {itemAddress.GetNameByTerms}");
        }

        public void Clear()
        {
            if(objectLeftPanel.activeSelf)
            {
                if (arrowSlot0.gameObject.activeSelf) arrowSlot0.DisableSlot();
                if (arrowSlot1.gameObject.activeSelf) arrowSlot1.DisableSlot();

                objectLeftPanel.SetActive(false);
            }
            else if (objectRightPanel.activeSelf)
            {
                foreach (var slot in accessSlots)
                {
                    if (slot.gameObject.activeSelf) slot.DisableSlot();
                }

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
            }

            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();
            this.Button_Exit();
        }

        private void ChooseSlot(int num, Item_Arrow item) {
            switch (num) {
                case 0: CCPlayerData.equipments.EquipItem_MainArr(item); soundEffect.PlayOneShot(2); break;
                case 1: CCPlayerData.equipments.EquipItem_SubArr(item);  soundEffect.PlayOneShot(2); break;
                default: throw new System.NotImplementedException("잘못된 인덱스 넘버");
            }
        }

        private void ChooseSlot(int num, Item_Accessory item) {
            switch (num) {
                case 0: CCPlayerData.equipments.EquipItem_Artifact(item, 0); soundEffect.PlayOneShot(2); break;
                case 1: CCPlayerData.equipments.EquipItem_Artifact(item, 1); soundEffect.PlayOneShot(2); break;
                case 2: CCPlayerData.equipments.EquipItem_Artifact(item, 2); soundEffect.PlayOneShot(2); break;
                default: throw new System.NotImplementedException("잘못된 인덱스 넘버");
            }
        }

        #endregion
    }
}
