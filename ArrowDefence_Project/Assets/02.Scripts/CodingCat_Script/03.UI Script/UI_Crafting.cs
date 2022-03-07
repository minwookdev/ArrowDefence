namespace ActionCat.UI {
    using UnityEngine;

    public class UI_Crafting : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField]
        RectTransform mainPanelTr = null;
        [SerializeField] 
        RectTransform[] navigateRectTr = null;
        [SerializeField] [ReadOnly] 
        RectTransform openedPanelTr = null;
        [SerializeField] [ReadOnly]
        PANEL openedPanelType = PANEL.NONE;

        [Header("CREATES")]
        [SerializeField] RectTransform craftPanelTr = null;

        [Header("UPGRATE")]
        [SerializeField] RectTransform upgradePanelTr = null;

        [Header("ENHANCE")]
        [SerializeField] RectTransform enhancePanelTr = null;

        #region BUTTON_EVENT

        public void BtnEvent_OpenUpgrade() {
            if (openedPanelType != PANEL.NONE) {
                if (openedPanelType == PANEL.UPGRADE) {
                    CloseOpenedPanel(); return;
                }
                CloseOpenedPanel();
            }
            upgradePanelTr.anchoredPosition = mainPanelTr.anchoredPosition;
            openedPanelType = PANEL.UPGRADE;
        }

        public void BtnEvent_OpenCraft() {
            if (openedPanelType != PANEL.NONE) {
                if (openedPanelType == PANEL.CRAFT) {
                    CloseOpenedPanel();
                    return;
                }
                CloseOpenedPanel();
            }

            craftPanelTr.anchoredPosition = mainPanelTr.anchoredPosition;
            openedPanelType = PANEL.CRAFT;
        }

        void CloseOpenedPanel() {
            switch (openedPanelType) {
                case PANEL.CRAFT:   craftPanelTr.anchoredPosition   = navigateRectTr[0].anchoredPosition; break;
                case PANEL.UPGRADE: upgradePanelTr.anchoredPosition = navigateRectTr[5].anchoredPosition; break;
                case PANEL.ENHANCE: enhancePanelTr.anchoredPosition = navigateRectTr[2].anchoredPosition; break;
                default:                                                                                  break;
            }

            openedPanelType = PANEL.NONE;
        }

        #endregion

        enum PANEL {
            NONE,
            UPGRADE,
            CRAFT,
            ENHANCE
        }
    }
}
