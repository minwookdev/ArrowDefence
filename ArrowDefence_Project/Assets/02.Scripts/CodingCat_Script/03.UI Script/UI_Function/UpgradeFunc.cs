namespace ActionCat.UI {
    using UnityEngine;

    [System.Serializable]
    public class UpgradeFunc {
        [Header("COMPONENT")]
        [SerializeField] RectTransform[] navigateRectTr = null;
        [SerializeField] RectTransform[] upgradePanels = null;
        [SerializeField] RectTransform[] upgradePopups = null;

        [Header("INFORMATION")]
        [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
        [SerializeField] [ReadOnly] POPUPTYPE openedPopupType = POPUPTYPE.NONE;
        #region PROPERTY
        public PANELTYPE OpenedPanelType {
            get {
                return openedPanelType;
            }
        }
        #endregion

        public RectTransform OpenPanel(PANELTYPE type, Vector2 anchoredPos) {
            openedPanelType = type;
            switch (openedPanelType) {
                case PANELTYPE.MAIN:   upgradePanels[0].anchoredPosition = anchoredPos; return upgradePanels[0];
                case PANELTYPE.CHOOSE: upgradePanels[1].anchoredPosition = anchoredPos; return upgradePanels[1];
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

        public void ClosePanel() {
            switch (openedPanelType) {
                case PANELTYPE.NONE:                                                                          return;
                case PANELTYPE.MAIN:   upgradePanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; break;
                case PANELTYPE.CHOOSE: upgradePanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        public void ClosePopup() {

        }
        
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
    }
}
