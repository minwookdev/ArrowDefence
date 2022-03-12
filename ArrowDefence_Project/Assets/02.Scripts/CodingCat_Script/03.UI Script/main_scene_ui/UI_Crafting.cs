namespace ActionCat.UI {
    using UnityEngine;

    public class UI_Crafting : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform mainPanelTr = null;
        [SerializeField] [ReadOnly] RectTransform openedPanelTr = null;
        [SerializeField] [ReadOnly] PANEL openedPanelType = PANEL.NONE;

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

        private void Start() {
            craftingFunction.Start(ButtonEvent_OpenCraftingChoosePanel, 1, craftingRecipeTable);
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
            openedPanelTr = upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.MAIN, mainAnchoredPos);
            openedPanelType = PANEL.UPGRADE;
        }

        public void ButtonEvent_OpenUpgradeChoosePanel() {
            CloseOpenedPanel();
            openedPanelTr = upgradeFunction.OpenPanel(UpgradeFunc.PANELTYPE.CHOOSE, mainAnchoredPos);
            openedPanelType = PANEL.UPGRADE;
        }

        public void ButtonEvent_ItemInfoPopupClose() {
            upgradeFunction.CloseItemInfoPopup();
        }

        public void ButtonEvent_ItemInfoPopupSelect() {
            upgradeFunction.SelectUpgradeableItem();
        }
        //======================================================== [ CRAFTING ] ========================================================
        public void ButtonEvent_OpenCrafting() {
            if(openedPanelType == PANEL.CRAFTING) { //똑같은 버튼을 다시 누르면 패널 닫고 리턴
                if(craftingFunction.OpenedPanelType == CraftingFunc.PANELTYPE.MAIN) {
                    CloseOpenedPanel(); return;
                }
            }

            CloseOpenedPanel();
            openedPanelTr = craftingFunction.OpenPanel(CraftingFunc.PANELTYPE.MAIN, mainAnchoredPos);
            openedPanelType = PANEL.CRAFTING;
        }

        void ButtonEvent_OpenCraftingChoosePanel() {
            CloseOpenedPanel();
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

        //==============================================================================================================================
        public void CloseOpenedPanel() {
            switch (openedPanelType) {
                case PANEL.NONE:                                    return;
                case PANEL.UPGRADE:   upgradeFunction.ClosePanel(); break;
                case PANEL.CRAFTING: craftingFunction.ClosePanel(); break;
                case PANEL.ENHANCE:                                 break;
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
