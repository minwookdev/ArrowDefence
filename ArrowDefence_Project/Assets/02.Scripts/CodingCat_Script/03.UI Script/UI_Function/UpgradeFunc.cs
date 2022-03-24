namespace ActionCat.UI {
    using UnityEngine;
    using System.Collections.Generic;
    using TMPro;

    [System.Serializable]
    public class UpgradeFunc {
        [Header("COMPONENT")]
        [SerializeField] RectTransform[] navigateRectTr = null;
        [SerializeField] RectTransform[] upgradePanels = null;
        [SerializeField] RectTransform[] upgradePopups = null;

        [Header("INFORMATION")]
        [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
        [SerializeField] [ReadOnly] POPUPTYPE openedPopupType = POPUPTYPE.NONE;

        [Header("REQUIRED")]
        [SerializeField] RequirementSlot requirementSlotPref = null;
        [SerializeField] RectTransform requirementSlotParent = null;
        [SerializeField] RectTransform requirementTextRectTr = null;
        [SerializeField] UI_ItemSlot selectedItemSlot = null;
        [SerializeField] UI_ItemSlot previewItemSlot  = null;
        [SerializeField] TextMeshProUGUI textSuccessProb = null;
        [SerializeField] TextMeshProUGUI textUpgradeResultName = null;
        List<RequirementSlot> requirementSlotList = null;

        [Header("SELECT SLOT")]
        [SerializeField] UpgradeableSlot upgradeableSlotPref = null;
        [SerializeField] RectTransform upgradeableSlotParent = null;
        [SerializeField] RectTransform upgradeableTextRectTr = null;
        [SerializeField] CraftingFunc.Toggle toggleButton    = null;
        List<UpgradeableSlot> upgradeableSlotList = null;

        [Header("ITEM INFO POPUP")]
        [SerializeField] ItemInfoPop infoPopup = null;

        [Header("CONFIRM")]
        [SerializeField] TextMeshProUGUI textResultItemName = null;

        [Header("RESULT")]
        [SerializeField] UI_ItemSlot resultSlot = null;
        [SerializeField] CanvasGroup resultBackCanvasGroup = null;
        [SerializeField] TextMeshProUGUI resultMainText = null;
        [SerializeField] TextMeshProUGUI resultSubText = null;
        [SerializeField] UnityEngine.UI.Image resultHorizontalBar = null;

        //Upgrade Recipe Scriptable Object
        UpgradeRecipeSO recipe = null;

        //
        UpgradeableSlot selectedSlot = null;
        UpgradeRecipe selectedRecipe = null;
        AD_item selectedItemRef      = null;
        Item_Equipment previewCache  = null;

        TweenGetItemPopup itemGetPopupTween = null;
        #region PROPERTY
        public PANELTYPE OpenedPanelType {
            get {
                return openedPanelType;
            }
        }
        #endregion

        //======================================================================================================================================
        //=========================================================== [ LIFE CYCLE ] ===========================================================

        public void Start(UpgradeRecipeSO recipe) {
            this.recipe = recipe;
            if(this.recipe == null) {
                CatLog.ELog("Upgrade Recipe ScriptableObject is Empty !");
            }

            //Panel 초기화
            InitMainPanel();
            InitSelectPanel();

            itemGetPopupTween = new TweenGetItemPopup(resultBackCanvasGroup, resultMainText, resultSubText, resultHorizontalBar);
        }

        public void Enable() {
            if(selectedItemRef != null) {
                selectedItemRef = null;
            }
        }

        //======================================================================================================================================
        //============================================================== [ OPEN ] ==============================================================

        public RectTransform OpenPanel(PANELTYPE type, Vector2 anchoredPos, bool isclearpanel) {
            openedPanelType = type;
            switch (openedPanelType) {
                case PANELTYPE.MAIN:
                    if (isclearpanel) {
                        RefreshMainPanel();
                    }
                    upgradePanels[0].anchoredPosition = anchoredPos;
                    return upgradePanels[0];
                case PANELTYPE.CHOOSE: 
                    if (isclearpanel) {
                        RefreshSelectPanel();
                    }
                    upgradePanels[1].anchoredPosition = anchoredPos;
                    return upgradePanels[1];
                default: throw new System.NotImplementedException();
            }
        }

        public RectTransform OpenPopup(POPUPTYPE type, Vector2 anchoredPos) {
            openedPopupType = type;
            switch (openedPopupType) {
                case POPUPTYPE.ITEMINFO: upgradePopups[0].anchoredPosition = anchoredPos; return upgradePopups[0];
                case POPUPTYPE.ITEMGET:  upgradePopups[1].anchoredPosition = anchoredPos; return upgradePopups[1];
                case POPUPTYPE.CONFIRM:  upgradePopups[2].anchoredPosition = anchoredPos; return upgradePopups[2];
                default: throw new System.NotImplementedException();
            }
        }

        void OpenPopup(POPUPTYPE type) {
            openedPopupType = type;
            switch (openedPopupType) {
                case POPUPTYPE.ITEMINFO: upgradePopups[0].anchoredPosition = upgradePanels[1].anchoredPosition; break;
                case POPUPTYPE.ITEMGET:  upgradePopups[1].anchoredPosition = upgradePanels[0].anchoredPosition; break;
                case POPUPTYPE.CONFIRM:  upgradePopups[2].anchoredPosition = upgradePanels[0].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
        }

        public void CloseOpenedPanel() {
            switch (openedPanelType) {
                case PANELTYPE.NONE:                                                                          return;
                case PANELTYPE.MAIN:   upgradePanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; break;
                case PANELTYPE.CHOOSE: upgradePanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        public void CloseOpenedPopup() {
            switch (openedPopupType) {
                case POPUPTYPE.ITEMINFO: upgradePopups[0].anchoredPosition = navigateRectTr[2].anchoredPosition; break;
                case POPUPTYPE.ITEMGET:  upgradePopups[1].anchoredPosition = navigateRectTr[3].anchoredPosition; break;
                case POPUPTYPE.CONFIRM:  upgradePopups[2].anchoredPosition = navigateRectTr[4].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPopupType = POPUPTYPE.NONE;
        }

        //======================================================================================================================================
        //=========================================================== [ INITIALIZE ] ===========================================================

        void InitMainPanel() {
            requirementSlotList = new List<RequirementSlot>();
            var sceneExistSlots = requirementSlotParent.GetComponentsInChildren<RequirementSlot>();

            AddRequirementSlot(count: 6);

            foreach (var slot in requirementSlotList) {
                slot.DisableSlot();
            }

            requirementTextRectTr.gameObject.SetActive(true);

            //Scene에 존재하고 있던 Prefab 비-활성화, 새로 생성한 Prefab만 사용
            if(sceneExistSlots.Length > 1) {
                CatLog.WLog("Scene Exist Slots Size Over, Recommanded delete other Slots.");
            }
            foreach (var slot in sceneExistSlots) {
                slot.DisableSlot();
            }
        }

        void InitSelectPanel() {
            upgradeableSlotList = new List<UpgradeableSlot>();
            var sceneExistSlots = upgradeableSlotParent.GetComponentsInChildren<UpgradeableSlot>();

            AddUpgradeAbleSlot(count: 10);

            foreach (var slot in upgradeableSlotList) {
                slot.DisableSlot();
            }

            upgradeableTextRectTr.gameObject.SetActive(true);

            //Scene에 존재하고 있던 Prefab 비-활성화, 새로 생성한 Prefab만 사용
            if (sceneExistSlots.Length > 1) {
                CatLog.WLog("Scene Exist Slots Size Over, Recommanded delete other Slots.");
            }
            foreach (var slot in sceneExistSlots) {
                slot.DisableSlot();
            }
        }

        //======================================================================================================================================
        //============================================================= [ REFRESH ] ============================================================

        void RefreshMainPanel() {
            //Upgrade Main Panel is Always Clear on Refresh Function
            requirementSlotList.ForEach((slot) => slot.DisableSlot());
            requirementTextRectTr.gameObject.SetActive(true);
            textSuccessProb.text = "00 %";
            textUpgradeResultName.text = "ITEM NAME FIELD";

            previewItemSlot.DisableSlot();
            selectedItemSlot.DisableSlot();

            if(selectedItemRef != null) {
                selectedItemRef = null;
            }

            previewCache = null;

            //Clear Selected Data
            ClearSelected();
        }

        void RefreshSelectPanel() {
            upgradeableSlotList.ForEach(item => { //모든 슬롯 비-활성화
                if (item.IsActive) {
                    item.DisableSlot();
                }
            });

            var enableNumber = toggleButton.EnableSlotIndex;
            AD_item[] upgradeableItems;
            switch (enableNumber) {
                case 0: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.DictionaryKeys);                   break; // ALL
                case 1: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.BOW));      break; // BOW
                case 2: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.ARROW));    break; // ARROW
                case 3: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.ARTIFACT)); break; // ARTIFACT
                default: throw new System.NotImplementedException();
            }

            int requiredSlotCount = upgradeableItems.Length - upgradeableSlotList.Count;
            CatLog.Log($"Find Upgradeable Item Counts: {upgradeableItems.Length}");
            if (requiredSlotCount > 0) {
                AddUpgradeAbleSlot(requiredSlotCount);
            }

            for (int i = 0; i < upgradeableItems.Length; i++) {
                upgradeableSlotList[i].EnableSlot(upgradeableItems[i]);
            }

            bool isDisableText = upgradeableItems.Length <= 0;
            upgradeableTextRectTr.gameObject.SetActive(isDisableText);
        }

        public void ClearSelected() {
            selectedItemRef = null;
            selectedRecipe  = null;
            if (selectedSlot != null) {
                selectedSlot.DeSelected();
                selectedSlot = null;
            }
            previewCache = null;
        }
        
        //========================================================= [ SET PANEL OR POPUP ] =====================================================
        //======================================================================================================================================

        public void SetMainPanel() {
            if (selectedItemRef == null) {
                throw new System.Exception("Item Reference is null.");
            }

            if (recipe.TryGetRecipe(selectedItemRef.GetID, out UpgradeRecipe targetRecipe)) {
                var requireSlotCount = targetRecipe.Materials.Length - requirementSlotList.Count;
                if (requireSlotCount > 0) {
                    AddRequirementSlot(requireSlotCount);
                }

                for (int i = 0; i < targetRecipe.Materials.Length; i++) {
                    requirementSlotList[i].EnableSlot(targetRecipe.Materials[i]);
                }
            }
            else {
                throw new System.NotImplementedException($"Recipe Not Found !, Item ID: {selectedItemRef.GetID}");
            }

            requirementTextRectTr.gameObject.SetActive(false);
            textSuccessProb.text = string.Format("{0} %", (100f - targetRecipe.FailedProb));
            textUpgradeResultName.text = targetRecipe.Result.Item_Name;
            selectedItemSlot.EnableSlot(targetRecipe.KeyItem);
            previewItemSlot.EnableSlot(targetRecipe.Result);
            selectedRecipe = targetRecipe;
        }

        public void SetSelectPanel(sbyte enableNumber) {
            if(enableNumber == toggleButton.EnableSlotIndex) { //같은타입 불러오기는 리턴
                return;
            }

            ClearSelected();
            toggleButton.Switch(enableNumber);

            upgradeableSlotList.ForEach(slot => slot.DisableSlot());
            AD_item[] upgradeableItems;
            switch (toggleButton.EnableSlotIndex) {
                case 0: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.DictionaryKeys);                   break; //all Recipe
                case 1: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.BOW));      break; //bow Recipe
                case 2: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.ARROW));    break; //arrow Recipe
                case 3: upgradeableItems = GameManager.Instance.FindUpgradeableItems(recipe.GetKeys(EQUIP_ITEMTYPE.ARTIFACT)); break; //artifact recipe
                default: throw new System.Exception();
            }

            CatLog.Log($"Find Upgradeable Item Count : {upgradeableItems.Length}");
            var requiredSlotCount = upgradeableItems.Length - upgradeableSlotList.Count;
            if (requiredSlotCount > 0) {
                AddUpgradeAbleSlot(requiredSlotCount);
            }

            //Enable Slots
            for (int i = 0; i < upgradeableItems.Length; i++) {
                upgradeableSlotList[i].EnableSlot(upgradeableItems[i]);
            }

            //Text Enable/Disable
            upgradeableTextRectTr.gameObject.SetActive(upgradeableItems.Length <= 0);
        }

        public void SetResultPanel() {
            resultSlot.EnableSlot(selectedRecipe.Result);
            var slotRects = new RectTransform[1] { resultSlot.GetComponent<RectTransform>() };
            itemGetPopupTween.TweenStart(slotRects);
        }

        public void BE_RESULT() {
            if (itemGetPopupTween.IsPlaying) {
                itemGetPopupTween.TweenSkip();
            }
            else {
                CloseOpenedPopup();
            }
        }

        public void SetConfirmPopup() {
            textResultItemName.text = string.Format("[ {0} ]", selectedRecipe.Result.Item_Name);
        }

        public bool TryOpenPreview(Vector2 anchoredPosition, out string log) {
            if(previewCache == null) {
                if (selectedRecipe == null) {
                    log = "Please Select Upgrade Item First.";
                    return false;
                }

                if(selectedRecipe.Result == null) {
                    log = "Upgrade Result Item is Not Assignment.";
                    return false;
                }

                switch (selectedRecipe.Result) {
                    case ItemData_Equip_Bow equipmentEntity:       previewCache = new Item_Bow(equipmentEntity);       break;
                    case ItemData_Equip_Accessory equipmentEntity: previewCache = new Item_Accessory(equipmentEntity); break;
                    case ItemData_Equip_Arrow equipmentEntity:     previewCache = new Item_Arrow(equipmentEntity);     break;
                    default: log = "Not Supported Equipment Type"; return false;
                }
            }

            infoPopup.OpenPreview(previewCache);
            OpenPopup(POPUPTYPE.ITEMINFO, anchoredPosition);
            log = "";
            return true;
        }

        //============================================================= [ FUNCTION ] ===========================================================
        //======================================================================================================================================

        void AddRequirementSlot(int count) {
            if(count <= 0) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var newSlot = GameObject.Instantiate<RequirementSlot>(requirementSlotPref, requirementSlotParent);
                newSlot.name = "slot_requirement_pref";
                requirementSlotList.Add(newSlot);
            }
        }

        void AddUpgradeAbleSlot(int count) {
            if(count <= 0) {
                return;
            }

            for (int i = 0; i < count; i++) {
                var newSlot  = GameObject.Instantiate<UpgradeableSlot>(upgradeableSlotPref, upgradeableSlotParent);
                newSlot.name = "slot_upgradeable_pref";
                newSlot.SetAction(OpenItemInfoPopup, SelectSlotSwitch);
                upgradeableSlotList.Add(newSlot);
            }
        }

        void SelectSlotSwitch(UpgradeableSlot slot, bool isSelect, AD_item itemref = null) {
            if (!isSelect) {
                if(selectedSlot != null) {
                    selectedSlot.DeSelected();
                    selectedSlot = null;
                }

                if(itemref == null) {
                    throw new System.Exception("Item Referecne is Null.");
                }

                slot.Selected();
                selectedSlot = slot;
                selectedItemRef = itemref;
            }
            else {
                if (!ReferenceEquals(selectedSlot, slot)) {
                    throw new System.Exception("Upgrade Function Class is have Difference Select Slot Reference.");
                }

                slot.DeSelected();
                selectedSlot    = null;
                selectedItemRef = null;
            }
        }

        void OpenItemInfoPopup(AD_item item) {
            if(selectedSlot != null) {
                selectedSlot.DeSelected();
                selectedSlot    = null;
                selectedItemRef = null;
            }

            if(item == null) {
                throw new System.Exception("item reference is null");
            }

            selectedItemRef = item;

            //Set ItemInfo Popup to selected item reference
            if (selectedItemRef is Item_Equipment equipment) {
                infoPopup.OpenPopup_EquipmentItem(equipment);
            }
            else {
                CatLog.ELog("Failed Setting ItemInfo Popup, itemRef is Not Equipment Type.");
            }

            OpenPopup(POPUPTYPE.ITEMINFO);
        }

        public bool TryReleaseSelectedSlot() {
            if (selectedSlot == null) {
                Notify.Inst.Show("Please Select an Item to Upgrade.");
                return false;
            }

            selectedSlot.DeSelected();
            selectedSlot = null;
            return true;
        }

        public bool IsCheckUpgradeable(out byte exceptionNumber) {
            //Selected Item is null
            if (selectedItemRef == null) {
                exceptionNumber = 0; 
                return false;
            }

            //Reqruiement Material Items is null
            bool isPossible = (requirementSlotList.FindAll(slot => slot.gameObject.activeSelf == true).TrueForAll(slot => slot.IsRequirementCondition == true));
            if (!isPossible) {
                exceptionNumber = 1;
                return isPossible;
            }

            exceptionNumber = 255;
            return isPossible;
        }

        public bool TryItemUpgrade() {
            var findMaterialList = new List<bool>();
            for (int i = 0; i < selectedRecipe.Materials.Length; i++) {
                var isFind = GameManager.Instance.TryGetItemAmount(selectedRecipe.Materials[i].Mat.Item_Id, out int amount);
                if (isFind == true && amount >= selectedRecipe.Materials[i].Required) {
                    findMaterialList.Add(true);
                }
                else {
                    findMaterialList.Add(true);
                }
            }

            //업그레이드 선택된 아이템과 레시피 재료 아이템들의 요구 수량만큼 인벤토리에 있는지 체크
            bool isFindSelectedItem     = GameManager.Instance.FindItemRef(selectedItemRef);
            bool isFindAllMaterialItems = findMaterialList.TrueForAll(isFind => isFind);

            if (!isFindAllMaterialItems || !isFindSelectedItem) {
                //Close Confirm Popup & Upgrade Main Panel
                if (!isFindAllMaterialItems)  Notify.Inst.Show("Not Enough Upgrade Requirement");
                else if (!isFindSelectedItem) Notify.Inst.Show("Not Assignment Selected Item in Inventory");

                return false;
            }


            for (int i = 0; i < selectedRecipe.Materials.Length; i++) {
                if(GameManager.Instance.TryRemoveItem(selectedRecipe.Materials[i].Mat.Item_Id, selectedRecipe.Materials[i].Required) == false) {
                    throw new System.Exception("Warning !, Item Upgrade Failed.");
                }
            }

            if (GameManager.Instance.TryRemoveItem(selectedItemRef) == false) {
                throw new System.Exception("Warning !, Item Upgrade Failed. Selected Item Not Found");
            }

            GameManager.Instance.AddItem(selectedRecipe.Result, selectedRecipe.Result.Item_Amount);
            return true;
        }

        //=============================================================== [ ENUM ] =============================================================
        //======================================================================================================================================

        public enum PANELTYPE {
            NONE   = 0,
            MAIN   = 1,
            CHOOSE = 2,
        }

        public enum POPUPTYPE {
            NONE     = 0,
            ITEMINFO = 1,
            ITEMGET  = 2,
            CONFIRM  = 3,
        }

        //======================================================================================================================================
        //======================================================================================================================================
    }
}
