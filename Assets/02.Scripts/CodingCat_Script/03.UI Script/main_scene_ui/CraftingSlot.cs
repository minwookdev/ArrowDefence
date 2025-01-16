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

        [Header("ADS")]
        [SerializeField] Image imageAdBtn = null;
        [SerializeField] TextMeshProUGUI textAdBtn = null;
        Color tempColor;
        Color enableColor  = new Color(1f, 1f, 1f, 1f);
        Color disableColor = new Color(.35f, .35f, .35f, 1f);

        CraftingFunc craftingParent = null; //너무 많은 System.Action을 정리하고, Parent를 가지고있도록 변경
        //System.Action<ItemData, int> receiptAction = null;
        //System.Action quickButtonAction = null;

        int craftSlotNumber = -1;

        string colorStartString = "";
        string colorEndString   = "</color>";
        bool isReadyAds = false;

        public bool IsActive {
            get {
                return this.gameObject.activeSelf;
            }
        }

        private void Update() {
            isReadyAds = AdsManager.Instance.IsReadyRewardedAds(REWARDEDAD.CRAFTING);
            tempColor = (isReadyAds) ? enableColor : disableColor;
            SetColorButton(tempColor);
            
        }

        public void AddListnerToSelectButton(UnityEngine.Events.UnityAction<int> unityAction) {
            if(unityAction == null) {
                throw new System.Exception("Unity Action is Null.");
            }

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener(eventData => unityAction(craftSlotNumber));
            selectBluePrintTrigger.triggers.Add(entry);
        }

        //public void AddListnerToReceiptButton(System.Action<ItemData, int> action) => receiptAction = action;

        public void EnableSlot(Data.CraftingInfo craftinginfo, int slotNumber, CraftingFunc parent) {
            craftingParent = parent;
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
            textPreviewItemName.text = craftinginfo.Result.NameByTerms;

            //Set Progress Slider [value = current / max]
            imageSlider.fillAmount = craftinginfo.Progress;

            //Switch QUICK, RECEIPT Buttons
            buttonQuick.gameObject.SetActive(!craftinginfo.IsComplete);
            buttonReceipt.gameObject.SetActive(craftinginfo.IsComplete);

            string remainingString = (craftinginfo.IsComplete) ? I2.Loc.ScriptLocalization.UI.craftingslot_remaining_complete : I2.Loc.ScriptLocalization.UI.craftingslot_remaining_inprogress;
            textState.text = remainingString;
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
            craftSlotNumber = -1;
            craftingParent  = null;
        }

        public void BE_RECEIPT() {
            var success = GameManager.Instance.TryReceipt(craftSlotNumber, out ItemData resultItem, out int itemAmount);
            if (!success) {
                throw new System.Exception();
            }

            //receiptAction(resultItem, itemAmount);
            craftingParent.ReceiptResult(resultItem, itemAmount);
        }

        public void BE_QUICK() {
            if (!isReadyAds) { //광고가 준비되지 않음
                Notify.Inst.Message("Please try again in a few Seconds.");
                return;
            }

            var craftingInfo = GameManager.Instance.GetCraftingInfo(craftSlotNumber);
            if (craftingInfo == null || craftingInfo.IsComplete) {
                //잘못된 슬롯 넘버 할당되거나 이미 크래프팅 완료됨
                throw new System.Exception("invalid access.");
            }

            if (!craftingInfo.IsSkipable) {
                Notify.Inst.Message("This Crafting is non-skipable.");
                return;
            }

            //광고 확인팝업
            craftingParent.OpenAdsPopup(craftSlotNumber);
        }

        public void QuickComplete() {
            var craftingInfo = GameManager.Instance.GetCraftingInfo(craftSlotNumber);
            craftingInfo.QuickComplete();
            EnableSlot(craftingInfo, craftSlotNumber, craftingParent); //자체적으로 해당 슬롯만 업데이트
        }

        void SetColorButton(Color color) {
            if (imageAdBtn.color != color) {
                imageAdBtn.color = color;
            }

            if (textAdBtn.color != color) {
                textAdBtn.color = color;
            }
        }
    }
}
