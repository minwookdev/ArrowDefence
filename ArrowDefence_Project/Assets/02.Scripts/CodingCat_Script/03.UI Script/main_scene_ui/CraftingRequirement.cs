namespace ActionCat.UI.Crafting {
    using UnityEngine;
    using TMPro;

    internal sealed class CraftingRequirement : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] UI_ItemSlot itemSlot = null;
        [SerializeField] TextMeshProUGUI textName = null;
        [SerializeField] TextMeshProUGUI textCurrentAmount = null;
        [SerializeField] TextMeshProUGUI textNeedAmount = null;
        string colorStartString = "";

        public void SetSlot(ItemData item, int currentAmount, int needAmount) {
            itemSlot.EnableSlot(item, needAmount);

            colorStartString       = (currentAmount < needAmount) ? "<color=red>" : "<color=green>";
            textName.text          = string.Format("[ {0} ]", item.Item_Name);
            textCurrentAmount.text = string.Format("{1}{0}{2}", currentAmount, colorStartString, "</color>");
            textNeedAmount.text    = needAmount.ToString();

            gameObject.SetActive(true);
        }

        public void Disable() {
            if (gameObject.activeSelf) {
                gameObject.SetActive(false);
            }
        }
    }
}
