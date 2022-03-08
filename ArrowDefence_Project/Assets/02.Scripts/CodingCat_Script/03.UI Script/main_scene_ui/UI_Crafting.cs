namespace ActionCat.UI {
    using UnityEngine;

    public class UI_Crafting : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] RectTransform mainPanelTr = null;
        [SerializeField] [ReadOnly] RectTransform openedPanelTr = null;
        [SerializeField] [ReadOnly] PANEL openedPanelType = PANEL.NONE;

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

        private void Start() {
            craftingFunction.Start(ButtonEvent_OpenCraftingChoosePanel);
        }

        private void OnEnable() {
            //현재 생성된 Crafting 슬롯과 갯수가 일치하는지 체크해야함
            //일치하지않으면 어떤조건에따라 슬롯하나를 더 사용할 수 있게됐다는 뜻이니까 하나 생성하는 로직포함
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
            CatLog.Log("Waorked CloseOpenedPanel !");
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
