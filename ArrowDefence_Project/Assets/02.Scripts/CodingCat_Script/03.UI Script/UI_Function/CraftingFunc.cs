namespace ActionCat.UI {
    using UnityEngine;
    using System.Collections.Generic;

    [System.Serializable]
    public class CraftingFunc {
        [Header("COMPONENT")]
        [SerializeField] RectTransform[] navigateRectTr = null;
        [SerializeField] RectTransform[] craftingPanels = null;
        [SerializeField] RectTransform[] craftingPopups = null;

        [Header("INFORMATION")]
        [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
        [SerializeField] [ReadOnly] POPUPTYPE openedPopupType = POPUPTYPE.NONE;

        [Header("SLOT")]
        [SerializeField] RectTransform slotParentRectTr = null;
        [SerializeField] CraftingSlot slotPrefab = null;
        [SerializeField] [ReadOnly] List<CraftingSlot> slots = null;
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
                case PANELTYPE.MAIN:   craftingPanels[0].anchoredPosition = anchoredPos; return craftingPanels[0];
                case PANELTYPE.CHOOSE: craftingPanels[1].anchoredPosition = anchoredPos; return craftingPanels[1];
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
                case PANELTYPE.MAIN:   craftingPanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; break;
                case PANELTYPE.CHOOSE: craftingPanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        public void ClosePopup() {

        }

        void UpdatePanelMain() {

        }

        #region LIFE_CYCLE

        /// <summary>
        /// 해당 클래스는 MonoBehaviour가 아님, UI_Crafting에 의존하여 처리
        /// </summary>
        public void Start(UnityEngine.Events.UnityAction unityAction) {
            var enableSlotCount = 1; //이 부분 추후에 PlayerData에서 현재 가용가능한 슬롯 수 받아와서 처리.
            for (int i = 0; i < enableSlotCount; i++) {
                var newSlot = GameObject.Instantiate<CraftingSlot>(slotPrefab, slotParentRectTr);
                newSlot.AddListnerToButton(unityAction);
                slots.Add(newSlot);
            }
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

        #endregion

        #region BUTTON_EVENT

        #endregion

        public enum POPUPTYPE {
            NONE     = 0,
            ITEMINFO = 1,
            GETITEM  = 2,
            ADS      = 3,
            CONFIRM  = 4
        }

        public enum PANELTYPE {
            NONE     = 0,
            MAIN     = 1,
            CHOOSE   = 2,
        }
    }
}
