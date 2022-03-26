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
        Color enableColor = new Color(1f, 1f, 1f, 1f);
        Color disableColor = new Color(.35f, .35f, .35f, 1f);

        System.Action<ItemData, int> receiptAction = null;
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

        public void AddListnerToReceiptButton(System.Action<ItemData, int> action) => receiptAction = action;

        //public void AddListnerToQuickButton(System.Action action) => quickButtonAction = action;

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

            textState.text = (craftinginfo.IsComplete) ? "CRAFTING COMPLETE !" : "STAGE CLEAR REMAINING";
        }

        public void DisableSlot() {
            gameObject.SetActive(false);
            craftSlotNumber = -1;
        }

        public void QuickComplete() {
            Notify.Inst.Show("This is an unimplemented featrue.");
        }

        public void BE_RECEIPT() {
            var success = GameManager.Instance.TryReceipt(craftSlotNumber, out ItemData resultItem, out int itemAmount);
            if (!success) {
                throw new System.Exception();
            }

            receiptAction(resultItem, itemAmount);
        }

        public void BE_QUICK() {
            if (!isReadyAds) { //광고가 준비되지 않음
                Notify.Inst.Show("Please try again Later.");
                return;
            }

            var craftingInfo = GameManager.Instance.GetCraftingInfo(craftSlotNumber);
            if (craftingInfo == null || craftingInfo.IsComplete) {
                //잘못된 슬롯 넘버 할당되거나 이미 크래프팅 완료됨
                throw new System.Exception("invalid access.");
            }

            if (!craftingInfo.IsSkipable) {
                Notify.Inst.Show("This crafting is non-skipable.");
                return;
            }
#if UNITY_EDITOR
            //빠른 제작 조건 달성
            craftingInfo.QuickComplete();
#elif UNITY_ANDROID
            //보상형 광고 시청여부에 따라 Quick Complete 발생시켜줌
#endif

            //A. 광고 보는데 성공하면 액션-콜 하고 실패하면 그냥 넘기기
            //B. 자체적으로 해당 슬록만 업데이트 진행

            //자체적으로 본 슬롯만 업데이트
            EnableSlot(craftingInfo, craftSlotNumber);
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
