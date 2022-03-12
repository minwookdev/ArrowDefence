namespace ActionCat.UI {
    using UnityEngine;
    using System.Collections.Generic;

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
        List<RequirementSlot> requirementSlotList = null;

        [Header("SELECT SLOT")]
        [SerializeField] UpgradeableSlot upgradeableSlotPref = null;
        [SerializeField] RectTransform upgradeableSlotParent = null;
        [SerializeField] RectTransform upgradeableTextRectTr = null;
        List<UpgradeableSlot> upgradeableSlotList = null;
        UpgradeableSlot selectedSlot = null;

        UpgradeRecipeSO recipe = null;
        AD_item itemRef = null;

        string selectedItemCode = "";
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
        }

        public void Enable() {

        }

        //======================================================================================================================================
        //============================================================== [ OPEN ] ==============================================================

        public RectTransform OpenPanel(PANELTYPE type, Vector2 anchoredPos) {
            openedPanelType = type;
            switch (openedPanelType) {
                case PANELTYPE.MAIN:   RefreshMainPanel();   upgradePanels[0].anchoredPosition = anchoredPos; return upgradePanels[0];
                case PANELTYPE.CHOOSE: RefreshSelectPanel(); upgradePanels[1].anchoredPosition = anchoredPos; return upgradePanels[1];
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

        public void ClosePanel() {
            switch (openedPanelType) {
                case PANELTYPE.NONE:                                                                          return;
                case PANELTYPE.MAIN:   upgradePanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; break;
                case PANELTYPE.CHOOSE: upgradePanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        //======================================================================================================================================
        //=========================================================== [ INITIALIZE ] ===========================================================

        void InitMainPanel() {
            requirementSlotList = new List<RequirementSlot>();
            var sceneExistSlots = requirementSlotParent.GetComponentsInChildren<RequirementSlot>();

            AddRequiredSlot(count: 6);

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

        }

        void RefreshSelectPanel() {
            upgradeableSlotList.ForEach(item => { //모든 슬롯 비-활성화
                if (item.IsActive) {
                    item.DisableSlot();
                }
            });

            var upgradeableItems  = GameManager.Instance.GetUpgradeableItems(recipe.DictionaryKeys);
            int requiredSlotCount = upgradeableItems.Length - upgradeableSlotList.Count;
            CatLog.Log($"Upgradeable Item Counts: {upgradeableItems.Length}");
            if (requiredSlotCount > 0) {
                AddUpgradeAbleSlot(requiredSlotCount);
            }

            for (int i = 0; i < upgradeableItems.Length; i++) {
                upgradeableSlotList[i].EnableSlot(upgradeableItems[i]);
            }
        }
        
        //============================================================= [ FUNCTION ] ===========================================================
        //======================================================================================================================================

        void AddRequiredSlot(int count) {
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
                newSlot.SetAction(OpenItemInfoPopup, SelectSlot, DeSelectSlot);
                upgradeableSlotList.Add(newSlot);
            }
        }

        void SelectSlot(UpgradeableSlot slot, AD_item item) {
            if(selectedSlot != null) {
                selectedSlot.DeSelected();
                selectedSlot = null;
            }

            slot.Selected();
            selectedSlot = slot;
            itemRef = item;
        }

        void DeSelectSlot(UpgradeableSlot slot) {
            slot.DeSelected();
            selectedSlot = null;
            itemRef = null;
        }

        //============================================================== [ POPUP ] =============================================================
        //======================================================================================================================================

        void OpenItemInfoPopup(AD_item item) {
            OpenPopup(POPUPTYPE.ITEMINFO);
            if(selectedSlot != null) {
                itemRef = null;
                selectedSlot.DeSelected();
                selectedSlot = null;
            }

            itemRef = item;

            //item변수가 null로 들어왔을때 처리구문 필요
        }

        public void CloseItemInfoPopup() {
            upgradePopups[0].anchoredPosition = navigateRectTr[2].anchoredPosition;
            openedPopupType = POPUPTYPE.NONE;

            //Clear Popup Data 필요
        }

        public void SelectUpgradeableItem() {
            if(itemRef == null) {
                throw new System.Exception("Item Reference is null.");
            }

            CloseItemInfoPopup();
            // Item Required Panel Setting..
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
