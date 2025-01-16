namespace ActionCat.UI {
    using UnityEngine;
    using TMPro;

    public class PopupItemSell : MonoBehaviour {
        [Header("REQUIRED")]
        

        [Header("BODY TYPE")]
        [SerializeField] Transform trNormalSell = null;
        [SerializeField] Transform trEquipSell  = null;

        [Header("NORMAL ITEM")]
        [SerializeField] UI_ItemSlot normalItemSlot = null;
        [SerializeField] TextMeshProUGUI tmpPossessionItemAmount = null;
        [SerializeField] TextMeshProUGUI tmpNormalPrice = null;
        [SerializeField] TextMeshProUGUI tmpTotalPrice  = null;
        [SerializeField] TextMeshProUGUI tmpSaleAmount  = null;
        [SerializeField] TextMeshProUGUI tmpItemName    = null;

        [Header("EQUIP ITEM")]
        [SerializeField] UI_ItemSlot equipItemSlot = null;

        [Header("SALE INFO")]
        [SerializeField] short saleAmount = 1;

        public void EnableSalePopup(AD_item item) {
            // Close All Popup. 이미 열려있는 팝업들 정리
            if (trNormalSell.gameObject.activeSelf) {
                trNormalSell.gameObject.SetActive(false);
            }
            if (trEquipSell.gameObject.activeSelf) {
                trEquipSell.gameObject.SetActive(false);
            }

            // Set Sale Popup
            var equipmentItem = item as Item_Equipment;
            if (equipmentItem.IsNull()) {
                // Normal Item Sale
                EnableNormalSaleBody(item);
            }
            else {
                // Equipment Item Sale
                EnableEquipSaleBody(equipmentItem);
            }
        }

        public void EnableNormalSaleBody(AD_item item) {
            // Get Information
            if (GameManager.Instance.PlayerInven.TryGetAmount(item.GetID, out int amount) == false) {
                CatLog.ELog("소지 중인 아이템 개수 가져오기 실패.");
                DisablePopup();
            }

            // Init Sale Amount
            saleAmount = 1;

            // Set Normal Item Sale Information
            tmpItemName.text             = item.GetNameByTerms;
            tmpPossessionItemAmount.text = amount.ToString();
            tmpNormalPrice.text          = GameGlobal.GetCurrencyUnitStr(amount);
            tmpSaleAmount.text           = saleAmount.ToString();

        }

        public void EnableEquipSaleBody(Item_Equipment item) {

        }

        public void DisablePopup() {
            // Clear Normal Item Sale Popup

            // Clear Equip Item Sale Popup

        }

        private void DisableNormalSalePopup() {

        }

        private void DisableEquipSalePopup() {

        }
    }
}
