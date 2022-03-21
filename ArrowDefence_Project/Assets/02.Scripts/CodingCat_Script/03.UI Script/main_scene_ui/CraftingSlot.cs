namespace ActionCat.UI {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;

    public class CraftingSlot : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] GameObject rectTrBefore = null;
        [SerializeField] GameObject rectTrAfter  = null;
        [SerializeField] GameObject rectTrLocked = null;

        [Header("BEFORE CRAFTING")]
        [SerializeField] EventTrigger selectBluePrintTrigger = null;

        [Header("CRAFTING")]
        [SerializeField] Image imageSlider = null;
        [SerializeField] TextMeshProUGUI textProgress = null;
        [SerializeField] TextMeshProUGUI textPreviewItemName = null;
        [SerializeField] TextMeshProUGUI textState = null;
        [SerializeField] Button buttonQuick = null;
        [SerializeField] Button buttonReceipt = null;

        [Header("SLOT")]
        [SerializeField] Image imageSlotIcon = null;
        [SerializeField] Image imageFrame = null;
        [SerializeField] Sprite[] frames = null;

        int craftSlotNumber = -1;

        string colorStartString = "";
        string colorEndString   = "</color>";

        public bool IsActive {
            get {
                return this.gameObject.activeSelf;
            }
        }

        public void AddListnerToButton(UnityEngine.Events.UnityAction<int> unityAction) {
            if(unityAction == null) {
                throw new System.Exception("Unity Action is Null.");
            }

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(eventData => unityAction(craftSlotNumber));
            selectBluePrintTrigger.triggers.Add(entry);
        }

        public void EnableSlot(Data.CraftingInfo craftinginfo, int slotNumber) {
            gameObject.SetActive(true); //alwyas enable

            if (!craftinginfo.IsAvailable) {
                //Assign Null to Slot Number
                craftSlotNumber = -1;
                rectTrLocked.SetActive(true);
                rectTrBefore.SetActive(true);
                rectTrAfter.SetActive(false);
                return;
            }

            //Assign Slot Number
            craftSlotNumber = slotNumber;
            rectTrLocked.SetActive(false);

            rectTrBefore.SetActive((!craftinginfo.InProgress));
            rectTrAfter.SetActive(craftinginfo.InProgress);

            //Is Crafting Slot InProgress?
            if (!craftinginfo.InProgress) {
                return;
            }

            //Set Result Item Slot
            colorStartString = (craftinginfo.IsComplete) ? "<color=green>" : "<color=red>";
            imageSlotIcon.sprite = craftinginfo.Result.Item_Sprite;
            imageFrame.sprite    = frames[(int)craftinginfo.Result.Item_Grade];

            //Set Name, Progress Text
            textProgress.text = string.Format("[ {2}{0}{3} / {1} ]", craftinginfo.Current, craftinginfo.Max, colorStartString, colorEndString);
            textPreviewItemName.text = craftinginfo.Result.Item_Name;

            //Set Progress Slider [value = current / max]
            imageSlider.fillAmount = craftinginfo.Progress;

            //Switch QUICK, RECEIPT Buttons
            buttonQuick.gameObject.SetActive(!craftinginfo.IsComplete);
            buttonReceipt.gameObject.SetActive(craftinginfo.IsComplete);

            textState.text = (craftinginfo.IsComplete) ? "CRAFTING COMPLETE RECIEPT THE ITEM !" : "STAGE CLEAR REMAINING";
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
            craftSlotNumber = -1;
        }

        public void QuickComplete() {
            Notify.Inst.Show("This is an unimplemented featrue.");
        }
    }
}
