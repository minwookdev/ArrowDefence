namespace ActionCat.UI {
    using UnityEngine;
    using DG.Tweening;

    public class UI_Crafting : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform mainPanelTr = null;
        [SerializeField] [ReadOnly] RectTransform openedPanelTr = null;
        [SerializeField] [ReadOnly] PANEL openedPanelType = PANEL.NONE;

        [Header("MAIN")]
        [SerializeField] RectTransform mainPanelRectTr = null;
        [SerializeField] CanvasGroup mainCanvasGroup   = null;

        [Header("TABLE")]
        [SerializeField] CraftingRecipeSO craftingRecipeTable = null;
        [SerializeField] UpgradeRecipeSO   upgradeRecipeTable = null;

        [Header("CRFTING")]
        [SerializeField] CraftingFunc craftingFunction = null;

        [Header("UPGRADE")]
        [SerializeField] UpgradeFunc upgradeFunction  = null;

        #region PROPERTY
        Vector2 mainAnchoredPos {
            get {
                return mainPanelTr.anchoredPosition;
            }
        }
        #endregion

        private void OnEnable() {
            craftingFunction.Enable();
            upgradeFunction.Enable();
        }

        public void OpenMain() {
            //Open Panel Tween
            mainCanvasGroup.alpha = 0f;
            gameObject.SetActive(true);
            mainCanvasGroup.alpha = 1f;
        }

        public void CloseMain() {
            //Close Panel Tween
            gameObject.SetActive(false);
        }

        private void OnDisable() {
            CloseOpenedPanel();
            upgradeFunction.ClearSelected();
        }

        private void Start() {
            //Get Temp Crafting Slot
            GameManager.Instance.TEST_CREATE_TEMP_CRAFTING_SLOT();

            craftingFunction.Start(BE_CT_OPENSELECTPANEL, 5, 10, craftingRecipeTable);
            upgradeFunction.Start(upgradeRecipeTable);
        }

        private void Update() {
            craftingFunction.Update();
        }

        #region BUTTON_EVENT
        //======================================================== [ UPGRADE ] ========================================================
        public void ButtonEvent_OpenUpgrade() {
            if (openedPanelType == PANEL.UPGRADE) { //똑같은 버튼을 다시 누르면 패널 닫고 리턴
                if(upgradeFunction.OpenedPanelType == UpgradeFunc.PANELTYPE.MAIN) {
                    CloseOpenedPanel(); return;
                }
            }

            CloseOpenedPanel();
            openedPanelTr = upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.MAIN, mainAnchoredPos, true);
            openedPanelType = PANEL.UPGRADE;
        }

        public void ButtonEvent_OpenUpgradeChoosePanel() {
            CloseOpenedPanel();
            openedPanelTr = upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.CHOOSE, mainAnchoredPos, true);
            openedPanelType = PANEL.UPGRADE;
        }

        public void ButtonEvent_Upgrade_CloseItemInfo() {
            upgradeFunction.CloseOpenedPopup();
        }

        public void ButtonEvent_Upgrade_SelectItem() {
            upgradeFunction.SetMainPanel();
            upgradeFunction.CloseOpenedPopup();
            upgradeFunction.CloseOpenedPanel();
            upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.MAIN, mainAnchoredPos, false);
        }

        public void ButtonEvent_Upgrade_Start() {
            if (upgradeFunction.IsCheckUpgradeable(out byte exceptionNumnber)) {
                upgradeFunction.SetConfirmPopup();
                upgradeFunction.OpenPopup(UpgradeFunc.POPUPTYPE.CONFIRM, mainAnchoredPos);
                CatLog.Log("업그레이드 조건 충족.");
            }
            else {
                switch (exceptionNumnber) {
                    case 0:  Notify.Inst.Show("Please Select Upgradeable Item.");  break;
                    case 1:  Notify.Inst.Show("Insufficient material.");           break;
                    default: Notify.Inst.Show("Not Reported Error, Failed Check"); break;
                }
                CatLog.Log("업그레이드 조건 불충족.");
            }
        }

        public void ButtonEvent_Upgrade_Confirm() {
            if (upgradeFunction.TryItemUpgrade()) {
                upgradeFunction.SetResultPanel();
                upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.MAIN, mainAnchoredPos, true); //Clear Panel
                upgradeFunction.CloseOpenedPopup();
                upgradeFunction.OpenPopup(UpgradeFunc.POPUPTYPE.ITEMGET, mainAnchoredPos);
            }
            else {
                throw new System.Exception();
            }
        }

        public void ButtonEvent_Upgrade_Confirm_Back() {
            upgradeFunction.CloseOpenedPopup();
        }

        public void ButtonEvent_Upgade_ItemGet() {
            upgradeFunction.CloseOpenedPopup();
        }

        public void ButtonEvent_Upgrade_MainWithSelectedItem() {
            if (upgradeFunction.TryReleaseSelectedSlot() == false) {
                return;
            }
            upgradeFunction.SetMainPanel();
            upgradeFunction.CloseOpenedPanel();
            upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.MAIN, mainAnchoredPos, false);
        }

        public void ButtonEvent_Upgrade_SelectItemType(int enableNumber) {
            upgradeFunction.SetSelectPanel((sbyte)enableNumber);
        }

        public void BE_UG_PREVIEW() {
            bool success = upgradeFunction.TryOpenPreview(mainAnchoredPos, out string log);
            if (!success) {
                Notify.Inst.Show(log);
            }
        }

        public void BE_UG_INCPROB() {
            Notify.Inst.Show("This is an unimplemented featrue.");
        }

        //======================================================== [ CRAFTING ] ========================================================
        public void ButtonEvent_OpenCrafting() {
            if(openedPanelType == PANEL.CRAFTING) { //똑같은 버튼을 다시 누르면 패널 닫고 리턴
                if(craftingFunction.OpenedPanelType == CraftingFunc.PANELTYPE.MAIN) {
                    CloseOpenedPanel(); return;
                }
            }
            else if (openedPanelType == PANEL.UPGRADE) {
                upgradeFunction.ClearSelected();
            }

            CloseOpenedPanel();
            openedPanelTr = craftingFunction.OpenPanel(CraftingFunc.PANELTYPE.MAIN, mainAnchoredPos);
            openedPanelType = PANEL.CRAFTING;
        }

        /// <summary>
        /// Crafting Slot의 Button Event로 등록되는 메서드
        /// </summary>
        void BE_CT_OPENSELECTPANEL(int slotNumber) {
            CloseOpenedPanel();

            if(GameManager.Instance.IsAvailableCraftingSlot(slotNumber) == false) {
                throw new System.Exception("This Slot is Not Available or Not Assignment.");
            }

            craftingFunction.SelectedSlotNumner = slotNumber;

            openedPanelTr = craftingFunction.OpenPanel(CraftingFunc.PANELTYPE.CHOOSE, mainAnchoredPos);
            openedPanelType = PANEL.CRAFTING;
        }

        public void ButtonEvent_SelectBluePrintType(int loadNumber) {
            switch (loadNumber) {
                case 0: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.ALL);        break;
                case 1: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.BOW);        break;
                case 2: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.ARROW);      break;
                case 3: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.ARTIFACT);   break;
                case 4: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.MATERIAL);   break;
                case 5: craftingFunction.RefreshSelectPanel(BLUEPRINTTYPE.CONSUMABLE); break;
                default: throw new System.NotImplementedException();
            }
        }

        public void ButtonEvent_Close(GameObject target) {
            target.SetActive(false);
        }

        public void BE_CT_OPENITEMINFO() {
            bool isSuccess = craftingFunction.OpenItemInfo(out string log);
            if (!isSuccess) {
                Notify.Inst.Show(log);
            }
        }

        public void BE_CT_BACKTOMAIN() {
            craftingFunction.OpenPanel(CraftingFunc.PANELTYPE.MAIN, mainAnchoredPos);
        }

        public void BE_CT_CLOSEITEMINFO() {
            craftingFunction.CloseItemInfo();
        }

        public void BE_CT_TRYOPENCONFIRM() {
            craftingFunction.TryOpenConfirm();
        }

        public void BE_CT_CLOSECONFIRM() {
            craftingFunction.CloseConfirm();
        }

        public void BE_CT_CRAFTINGSTART() {
            craftingFunction.Confirm(mainAnchoredPos);
        }

        public void BE_CT_CLOSERESULT() {
            craftingFunction.CloseResult();
        }

        //==============================================================================================================================
        public void CloseOpenedPanel() {
            switch (openedPanelType) {
                case PANEL.NONE:                                         return;
                case PANEL.UPGRADE:  upgradeFunction.CloseOpenedPanel(); break;
                case PANEL.CRAFTING: craftingFunction.ClosePanel();      break;
                case PANEL.ENHANCE:                                      break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANEL.NONE;
        }
        //==============================================================================================================================
        #endregion

        enum PANEL {
            NONE     = 0,
            UPGRADE  = 1,
            CRAFTING = 2,
            ENHANCE  = 3,
        }
    }
}
