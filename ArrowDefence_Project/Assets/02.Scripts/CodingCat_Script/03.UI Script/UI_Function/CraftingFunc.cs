namespace ActionCat.UI {
    using UnityEngine;
    using System.Collections.Generic;
    using TMPro;

    [System.Serializable]
    public class CraftingFunc {
        [Header("COMPONENT")]
        [SerializeField] RectTransform[] navigateRectTr = null;
        [SerializeField] RectTransform[] craftingPanels = null;
        [SerializeField] RectTransform[] craftingPopups = null;

        [Header("INFORMATION")]
        [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
        [SerializeField] [ReadOnly] POPUPTYPE openedPopupType = POPUPTYPE.NONE;

        [Header("PANEL MAIN")]
        [SerializeField] RectTransform slotParentRectTr = null;
        [SerializeField] CraftingSlot slotPrefab = null;
        [SerializeField] [ReadOnly] [LabelEx("Slot Debugging")] 
        List<CraftingSlot> slots = null;

        [Header("BLUEPRINT SELECT")]
        [SerializeField] RectTransform bpSlotParentTr = null;
        [SerializeField] BluePrintSlot bpSlotPrefab   = null;
        [SerializeField] BluePrintSlot bpSelectedSlot = null;
        [SerializeField] RectTransform noResultRectTr = null;
        [SerializeField] Toggle toggle = null;
        private BLUEPRINTTYPE loadBluePrintType = BLUEPRINTTYPE.ALL;
        [SerializeField] [ReadOnly] [LabelEx("Slot Debugging")]
        List<BluePrintSlot> bpSlotList = null;

        [Header("BLUEPRINT INFO POPUP")]
        [SerializeField] BluePrintInfoPopup infoPopup = null;

        [Header("CONFIRM")]
        [SerializeField] TextMeshProUGUI textConfirmItemName = null;

        [Header("RESULT")]
        [SerializeField] UI_ItemSlot resultItemSlot = null;
        [SerializeField] CanvasGroup resultPopupBack = null;
        [SerializeField] TextMeshProUGUI textResultMain = null;
        [SerializeField] TextMeshProUGUI textResultSub = null;
        [SerializeField] UnityEngine.UI.Image imageResultHorizontal = null;

        [Space(5f)]
        [Header("DEBUG")]
        AD_item itemRefSelected = null;
        CraftingRecipeSO recipe = null;
        [SerializeField] [ReadOnly] int selectedSlotNumber = 0;

        //New ItemGet Popup Tween Class
        TweenGetItemPopup itemGetPopupTween = null;

        

        public int SelectedSlotNumner {
            get {
                return selectedSlotNumber;
            }
            set {
                if (value <= -1) {
                    throw new System.Exception();
                }

                selectedSlotNumber = value;
            }
        }
        #region PROPERTY
        public PANELTYPE OpenedPanelType {
            get {
                return openedPanelType;
            }
        }
        #endregion

        //======================================================================================================================================
        //========================================================= [ PANEL, POPUP ] ===========================================================

        public RectTransform OpenPanel(PANELTYPE type, Vector2 anchoredPos) {
            openedPanelType = type;
            switch (openedPanelType) {
                case PANELTYPE.MAIN:   RefreshCraftingSlot(); craftingPanels[0].anchoredPosition = anchoredPos; return craftingPanels[0];
                case PANELTYPE.CHOOSE: RefreshSelectPanel();  craftingPanels[1].anchoredPosition = anchoredPos; return craftingPanels[1];
                default: throw new System.NotImplementedException();
            }
        }

        public RectTransform OpenPopup(POPUPTYPE type, Vector2 anchoredPos) {
            openedPopupType = type;
            switch (openedPopupType) {
                case POPUPTYPE.ITEMINFO: craftingPopups[0].anchoredPosition = anchoredPos; return craftingPopups[0];
                case POPUPTYPE.GETITEM:  craftingPopups[1].anchoredPosition = anchoredPos; return craftingPopups[1];
                case POPUPTYPE.ADS:      craftingPopups[2].anchoredPosition = anchoredPos; return craftingPopups[2];
                case POPUPTYPE.CONFIRM:  craftingPopups[3].anchoredPosition = anchoredPos; return craftingPopups[3];
                default: throw new System.NotImplementedException();
            }
        }

        public void ClosePanel() {
            switch (openedPanelType) {
                case PANELTYPE.NONE:   return;
                case PANELTYPE.MAIN:   
                    craftingPanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; 
                    break;
                case PANELTYPE.CHOOSE: 
                    craftingPanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition;
                    itemRefSelected = null;
                    if (bpSelectedSlot != null) {
                        bpSelectedSlot.DeSelected();
                        bpSelectedSlot = null;
                    }
                    bpSlotList.ForEach(slot => slot.DisableSlot());
                    //Restore Select Panel Position.
                    //Clear Item Reference.
                    //Clear Selected Slot.
                    //Disable All Enabled Slots.
                    break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        /// <summary>
        /// 선택된 설계도 슬롯을 버튼을 사용하여 팝업을 호출하는 메서드.
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public bool OpenItemInfo(out string log) {
            if(bpSelectedSlot == null) {
                log = "First, Select a BluePrint.";
                return false;
            }

            if(itemRefSelected == null) {
                log = "BluePrint Reference is Null";
                return false;
            }

            //Find Recipe Data to Selected BluePrint Item
            var isFindRecipe = recipe.TryGetRecipe(itemRefSelected.GetID, out CraftingRecipe findRecipe);
            if (!isFindRecipe) {
                log = "Not Found Recipe Data";
                return false;
            }

            //Setting Item Info Popup
            infoPopup.SetPopup(itemRefSelected, findRecipe);
            craftingPopups[0].anchoredPosition = craftingPanels[1].anchoredPosition;
            openedPopupType = POPUPTYPE.ITEMINFO;

            bpSelectedSlot.DeSelected();
            bpSelectedSlot = null;

            log = "";
            return true;
        }

        /// <summary>
        /// 설계도 슬롯 자체에서 PointerClick 이벤트의 팝업을 호출하는 메서드.
        /// </summary>
        /// <param name="item"></param>
        public void OpenItemInfo(AD_item item) {
            if(bpSelectedSlot != null) {
                bpSelectedSlot.DeSelected();
                bpSelectedSlot = null;
            }

            //Find Recipe Data to Selected BluePrint Item
            itemRefSelected = item;
            var isFindRecipe = recipe.TryGetRecipe(itemRefSelected.GetID, out CraftingRecipe findRecipe);
            if (!isFindRecipe) {
                throw new System.Exception($"Not Found Recipe, Try Key: {itemRefSelected.GetID}, Name:{itemRefSelected.GetName}");
            }

            //Setting Item Info Popup
            infoPopup.SetPopup(itemRefSelected, findRecipe);

            //Move Center to Item Info Popup
            craftingPopups[0].anchoredPosition = craftingPanels[1].anchoredPosition;
            openedPopupType = POPUPTYPE.ITEMINFO;
        }

        public void SelectBluePirnt(BluePrintSlot slot, bool isSelectedSlot, AD_item itemRef) {
            if (isSelectedSlot) {                            //이미 Selected 상태인 Slot에서 DeSelected 요청
                if(!ReferenceEquals(slot, bpSelectedSlot)) { //요청자가 지금 들고있는 슬롯인지 확인
                    throw new System.Exception("Slot Not Matched !");
                }

                bpSelectedSlot.DeSelected();
                bpSelectedSlot = null;

                //Relaese, Selected Item
                itemRefSelected = null;
            }
            else { //Selected 상태 요청
                if (bpSelectedSlot != null) { //다른 슬롯을 이미 들고있다면 DeSelected 처리
                    bpSelectedSlot.DeSelected();
                    bpSelectedSlot = null;
                }

                bpSelectedSlot = slot;
                bpSelectedSlot.Selected();

                //Assign New Item, Recipe Reference
                itemRefSelected = itemRef;
            }
        }

        public void CloseItemInfo() {
            craftingPopups[0].anchoredPosition = navigateRectTr[2].anchoredPosition;
            openedPopupType = POPUPTYPE.NONE;
            itemRefSelected = null;
        }

        public bool TryOpenConfirm() {
            if(!IsCraftable(out string resultItemName)) {
                Notify.Inst.Show(resultItemName);
                return false;
            }

            if (bpSelectedSlot != null) {
                bpSelectedSlot.DeSelected();
                bpSelectedSlot = null;
            }

            if(openedPopupType == POPUPTYPE.ITEMINFO) {
                craftingPopups[0].anchoredPosition = navigateRectTr[2].anchoredPosition;
            }

            textConfirmItemName.text = resultItemName;
            craftingPopups[3].anchoredPosition = craftingPanels[1].anchoredPosition;
            openedPopupType = POPUPTYPE.CONFIRM;
            return true;
        }

        public void CloseConfirm() {
            itemRefSelected = null;
            craftingPopups[3].anchoredPosition = navigateRectTr[4].anchoredPosition;
            openedPopupType = POPUPTYPE.NONE;
        }

        public void Confirm(Vector2 anchoredPosition) {
            bool successCrafting = TryCrafting(out string failedLog, out CraftingRecipe findRecipe);
            if (!successCrafting) {
                CatLog.ELog(failedLog, true);
                return;
            }

            itemRefSelected = null;
            openedPopupType = POPUPTYPE.NONE;
            craftingPopups[3].anchoredPosition = navigateRectTr[4].anchoredPosition;

            //Start Crafting
            GameManager.Instance.CraftingStart(selectedSlotNumber, findRecipe);

            ClosePanel();
            OpenPanel(PANELTYPE.MAIN, anchoredPosition);
        }

        void ReceiptResult(ItemData item, int amount) {
            //Reuslt Popup Slot Set, Popup Positioning
            resultItemSlot.EnableSlot(item, amount);

            //Play ResultPopup Tween
            var slotRectTransform = resultItemSlot.GetComponent<RectTransform>();
            itemGetPopupTween.TweenStart(slotRectTransform);
            
            craftingPopups[1].anchoredPosition = craftingPanels[0].anchoredPosition;
            openedPopupType = POPUPTYPE.GETITEM;

            //Refresh Crafting Slot Panel
            RefreshCraftingSlot();
        }

        public void CloseResult() {
            if (itemGetPopupTween.IsPlaying) {
                itemGetPopupTween.TweenSkip();
            }
            else {
                craftingPopups[1].anchoredPosition = navigateRectTr[3].anchoredPosition;
                openedPopupType = POPUPTYPE.NONE;
            }
        }

        //======================================================================================================================================
        //============================================================ [ FUNCTION ] ============================================================

        bool IsCraftable(out string resultName) {
            if(itemRefSelected == null) {
                resultName = "Please, Select Item First.";
                return false;
            }

            bool isFind = recipe.TryGetRecipe(itemRefSelected.GetID, out CraftingRecipe findRecipe);
            if (!isFind) throw new System.Exception($"Recipe Not Found, BluePrint Name: {itemRefSelected.GetName}");
            for (int i = 0; i < findRecipe.Mats.Length; i++) {
                if (GameManager.Instance.TryGetItemAmount(findRecipe.Mats[i].Mateiral.Item_Id, out int amount) && amount >= findRecipe.Mats[i].Required) {
                    continue;
                }
                else {
                    resultName = "Don't have enough materials to crafting.";
                    return false;
                }
            }

            resultName = findRecipe.Result.Item.Item_Name;
            return true;
        }

        bool TryCrafting(out string log, out CraftingRecipe craftRecipe) {
            recipe.TryGetRecipe(itemRefSelected.GetID, out CraftingRecipe findRecipe);
            //Remove Material Items
            var mats = findRecipe.Mats;
            for (int i = 0; i < mats.Length; i++) {
                bool successItemRemove = GameManager.Instance.TryRemoveItem(mats[i].Mateiral.Item_Id, mats[i].Required);
                if (!successItemRemove) {
                    log = $"WARNING ! CRAFTING ERROR -> {mats[i].Mateiral.Item_Name}";
                    craftRecipe = null;
                    return false;
                }
            }

            //Remove BluePrint Item
            if(!GameManager.Instance.TryRemoveItem(findRecipe.BluePrint.Item_Id, 1)) {
                log = $"WARNING ! CRAFTING ERROR -> {findRecipe.BluePrint.Item_Name}";
                craftRecipe = null;
                return false;
            }

            log = "";
            craftRecipe = findRecipe;
            return true;
        }

        //======================================================================================================================================
        //=========================================================== [ INITIALIZE ] ===========================================================

        void InitPanelMain(UnityEngine.Events.UnityAction<int> unityAction, int slotCount) { 
            //기존 씬에 있던 TempSlot들 비-활성화
            var sceneExistSlots = slotParentRectTr.GetComponentsInChildren<CraftingSlot>();
            foreach (var slot in sceneExistSlots) {
                slot.DisableSlot();
            }

            slots = new List<CraftingSlot>();
            AddCraftingSlot(slotCount, unityAction);
        }

        void InitPanelSelect(byte spawnSlotCount) {
            //씬에 깔려있는 Slot들 비활성화 처리 후 BluePrint Prefab으로 잡힌 오브젝트 생성
            bpSlotList = new List<BluePrintSlot>();
            var sceneExistSlots = bpSlotParentTr.GetComponentsInChildren<BluePrintSlot>();
            foreach (var slot in sceneExistSlots) {
                slot.DisableSlot();
            }
            bpSlotList = new List<BluePrintSlot>();
            AddBluePrintSlot(spawnSlotCount);
        }

        void AddBluePrintSlot(byte spawnSlotCount) {
            for (int i = 0; i < spawnSlotCount; i++) {
                bpSlotList.Add(GameObject.Instantiate<BluePrintSlot>(bpSlotPrefab, bpSlotParentTr).InitSlot(this));
            }
        }

        void AddCraftingSlot(int spawnSlotCount, UnityEngine.Events.UnityAction<int> unityAction) {
            for (int i = 0; i < spawnSlotCount; i++) {
                var newSlot = GameObject.Instantiate<CraftingSlot>(slotPrefab, slotParentRectTr);
                newSlot.gameObject.name = "crafting_slot_pref";
                newSlot.AddListnerToSelectButton(unityAction);
                newSlot.AddListnerToReceiptButton(ReceiptResult);
                slots.Add(newSlot);
            }
        }

        //======================================================================================================================================
        //============================================================ [ REFRESH ] =============================================================

        void RefreshCraftingSlot() {
            slots.ForEach(slot => slot.DisableSlot());

            var craftingSlotCount = GameManager.Instance.GetCraftingInfoSize();
            if(craftingSlotCount <= 0) { //유저의 제작슬롯이 없으면 로드하지 않음
                return;
            }

            var craftingSlots = GameManager.Instance.GetCraftingInfos();
            int needSlotCount = craftingSlots.Length - slots.Count;
            if (needSlotCount > 0) {
                CatLog.ELog("Must Allocate More Crafting Slots.");
                //깔려있는 Crafting Slot이 부족할 때 ERROR 출력
                //CRAFTING SLOT은 EVENT와 엮여있어서 현재로직으로는 INIT당시의 생성된 이후, 추가적으로 생성하기 힘듦.
                //추가적인 슬롯 생성이 필요한 시점에 CRAFTING SLOT에 매개변수로 들어가는 UNITY ACTON를 개선하면 가능하다.
            }

            for (int i = 0; i < craftingSlots.Length; i++) {
                slots[i].EnableSlot(craftingSlots[i], i); // <- Input CraftingSlot Data
            }
        }

        /// <summary>
        /// 항상 LoadBluePrintType field를 사용해서 BluePrint를 가져옴
        /// </summary>
        void RefreshSelectPanel() {
            if(bpSelectedSlot != null) {
                bpSelectedSlot.DeSelected();
                bpSelectedSlot = null;
            }

            itemRefSelected = null;

            bpSlotList.ForEach((item) => { //모든 슬롯 비-활성화
                if (item.IsActive) {
                    item.DisableSlot();
                }
            });

            var bluePrints = GameManager.Instance.GetBluePrints(loadBluePrintType);
            CatLog.Log($"Get BluePrint Item Count: {bluePrints.Length}");
            if (bluePrints.Length <= 0) { // 소지하고 있는 설계도 타입 아이템이 없다면 로고 활성화 후 메서드 반환
                noResultRectTr.gameObject.SetActive(true);
                return;
            } 

            noResultRectTr.gameObject.SetActive(false);
            int needSlotCount = bluePrints.Length - bpSlotList.Count;
            if (needSlotCount > 0) { // 부족한 슬롯 만큼 생성 후 캐싱
                for (int i = 0; i < needSlotCount; i++) {
                    var newSlot = GameObject.Instantiate<BluePrintSlot>(bpSlotPrefab, bpSlotParentTr);
                    bpSlotList.Add(newSlot);
                }
            }
            else if (needSlotCount < 0) {
                //BluePrint Slot이 더 많은 경우, 사용되지 않는 슬롯 비-활성화
                //Refresh 로직 초기에 모든 슬롯을 비-활성화 처리하기 때문에 해당 로직이 필요하지 않음
                //for (int i = bpSlots.Count; i >= bpSlots.Count - needSlotCount; --i) {
                //    bpSlots[i].DisableSlot();
                //}
            }

            for (int i = 0; i < bluePrints.Length; i++) {
                bpSlotList[i].EnableSlot(bluePrints[i]);
            }
        }

        /// <summary>
        /// 가져올 BluePrintType을 지정, 사용된 type을 저장
        /// </summary>
        /// <param name="type"></param>
        public void RefreshSelectPanel(BLUEPRINTTYPE type) {
            if(type == loadBluePrintType) {
                CatLog.Log("Same Type of Current Loaded.");
                return; //열려있는 Type과 동일하면 return
            }

            bpSlotList.ForEach((slot) => { //모든 설계도 슬롯 비-활성화
                if (slot.IsActive) {
                    slot.DisableSlot();
                }
            });

            loadBluePrintType = type;
            var blueprints = GameManager.Instance.GetBluePrints(loadBluePrintType);
            CatLog.Log($"Get BluePrint Count: {blueprints.Length}");

            //Toggle Button Images
            toggle.Switch((sbyte)loadBluePrintType);

            if (blueprints.Length <= 0) { // 소지하고 있는 설계도 타입 아이템이 없다면 로고 활성화 후 반환
                noResultRectTr.gameObject.SetActive(true);
                return;
            }

            noResultRectTr.gameObject.SetActive(false);
            int requiredSlotCount = blueprints.Length - bpSlotList.Count;
            if (requiredSlotCount > 0) {
                for (int i = 0; i < requiredSlotCount; i++) {
                    var newSlot = GameObject.Instantiate<BluePrintSlot>(bpSlotPrefab, bpSlotParentTr);
                    bpSlotList.Add(newSlot);
                }
            }
            else if (requiredSlotCount < 0) {
                //BluePrint Slot이 더 많은 경우, 사용되지 않는 슬롯 비-활성화
                //Refresh 로직 초기에 모든 슬롯을 비-활성화 처리하기 때문에 해당 로직이 필요하지 않음
                //for (int i = bpSlots.Count; i >= bpSlots.Count - needSlotCount; --i) {
                //    bpSlots[i].DisableSlot();
                //}
            }

            for (int i = 0; i < blueprints.Length; i++) {
                bpSlotList[i].EnableSlot(blueprints[i]);
            }
        }

        //======================================================================================================================================
        //========================================================== [ LIFE CYCLE ] ============================================================

        /// <summary>
        /// 해당 클래스는 MonoBehaviour가 아님, UI_Crafting에 의존하여 처리
        /// </summary>
        public void Start(UnityEngine.Events.UnityAction<int> unityAction, int slotCount, byte bluePrintSlotCount, CraftingRecipeSO recipeTable) {
            InitPanelMain(unityAction, slotCount);
            InitPanelSelect(bluePrintSlotCount);
            recipe = recipeTable;

            itemGetPopupTween = new TweenGetItemPopup(resultPopupBack, textResultMain, textResultSub, imageResultHorizontal);
        }

        public void Enable() {

        }

        /// <summary>
        /// 해당 클래스는 MonoBehaviour가 아님, UI_Crafting에 의존하여 처리
        /// </summary>
        public void Update() {
            switch (openedPanelType) {
                case PANELTYPE.NONE:                      break;
                case PANELTYPE.MAIN:   UpdatePanelMain(); break;
                case PANELTYPE.CHOOSE:                    break;
                default: throw new System.NotImplementedException();
            }
        }

        void UpdatePanelMain() {

        }

        //======================================================================================================================================
        //============================================================= [ ENUM ] ===============================================================

        public enum POPUPTYPE {
            NONE     = 0,
            ITEMINFO = 1,
            GETITEM  = 2,
            ADS      = 3,
            CONFIRM  = 4
        }

        public enum PANELTYPE {
            NONE   = 0,
            MAIN   = 1,
            CHOOSE = 2,
        }

        //======================================================================================================================================
        //======================================================================================================================================

        [System.Serializable]
        internal sealed class Toggle {
            [SerializeField] GameObject[] enables  = null;
            [SerializeField] RectTransform[] highlightPosArray = null;
            [SerializeField] RectTransform highlightBar = null;
            Vector2 tempPos = Vector2.zero;
            public sbyte EnableSlotIndex { get; private set; } = 0;

            /// <summary>
            /// Range: [0] ~ [SlotLength - 1]
            /// </summary>
            /// <param name="index"></param>
            public void Switch(sbyte index) {
                if(index >= enables.Length) {
                    throw new System.Exception("Index Range Over");
                }

                if(EnableSlotIndex != -1) {
                    enables[EnableSlotIndex].SetActive(false);
                }

                EnableSlotIndex = index;
                enables[EnableSlotIndex].SetActive(true);

                //Set Position Highlight Bar
                tempPos   = highlightBar.anchoredPosition;
                tempPos.x = highlightPosArray[EnableSlotIndex].anchoredPosition.x;
                highlightBar.anchoredPosition = tempPos;
            }
        }
    }
}
