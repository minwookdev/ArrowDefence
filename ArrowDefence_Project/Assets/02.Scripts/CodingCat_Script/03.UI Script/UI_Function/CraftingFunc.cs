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

        [Header("PANEL MAIN")]
        [SerializeField] RectTransform slotParentRectTr = null;
        [SerializeField] CraftingSlot slotPrefab = null;
        [SerializeField] [ReadOnly] [LabelEx("Slot Debugging")] 
        List<CraftingSlot> slots = null;

        [Header("PANEL CHOOSE")]
        [SerializeField] RectTransform choosePanelParentTr = null;
        [SerializeField] BluePrintSlot bluePrintSlotPrefab = null;
        [SerializeField] [ReadOnly] [LabelEx("Slot Debugging")]
        List<BluePrintSlot> bpSlots = null;
        [SerializeField] RectTransform noResultRectTr = null;
        [SerializeField] Toggle toggle = null;
        private BLUEPRINTTYPE loadBluePrintType = BLUEPRINTTYPE.ALL;

        CraftingRecipeSO recipe = null;
        #region PROPERTY
        public PANELTYPE OpenedPanelType {
            get {
                return openedPanelType;
            }
        }
        #endregion

        //======================================================================================================================================
        //=========================================================== [ LIFE CYCLE ] ===========================================================

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
                case PANELTYPE.MAIN:   craftingPanels[0].anchoredPosition = navigateRectTr[0].anchoredPosition; break;
                case PANELTYPE.CHOOSE: craftingPanels[1].anchoredPosition = navigateRectTr[1].anchoredPosition; break;
                default: throw new System.NotImplementedException();
            }
            openedPanelType = PANELTYPE.NONE;
        }

        public void ClosePopup() {

        }

        //======================================================================================================================================
        //=========================================================== [ INITIALIZE ] ===========================================================

        void InitPanelMain(UnityEngine.Events.UnityAction unityAction, byte slotCount) {
            //기존 씬에 있던 TempSlot들 비-활성화
            var sceneSlots = slotParentRectTr.GetComponentsInChildren<CraftingSlot>();
            foreach (var slot in sceneSlots) {
                slot.DisableSlot();
            }

            slots = new List<CraftingSlot>();
            for (int i = 0; i < slotCount; i++) {
                var newSlot = GameObject.Instantiate<CraftingSlot>(slotPrefab, slotParentRectTr);
                newSlot.AddListnerToButton(unityAction);
                slots.Add(newSlot);
            }

        }

        void InitPanelSelect() {
            //현재 Parnet에 깔려있는 Slot Prefab 캐싱 및 비-활성화
            bpSlots = new List<BluePrintSlot>(choosePanelParentTr.GetComponentsInChildren<BluePrintSlot>());
            for (int i = 0; i < bpSlots.Count; i++) {
                bpSlots[i].DisableSlot();
            }
        }

        //======================================================================================================================================
        //============================================================ [ REFRESH ] =============================================================

        void RefreshCraftingSlot() {
            var userCraftingSlotCount = GameManager.Instance.GetCraftingSlotCount();
            var needSlotCount = userCraftingSlotCount - slots.Count;
            if (needSlotCount > 0) {
                ///for (int i = 0; i < needSlotCount; i++) {
                ///    var newSlot = GameObject.Instantiate<CraftingSlot>(slotPrefab, slotParentRectTr);
                ///    slots.Add(newSlot);
                ///}
                CatLog.WLog("초기화된 CraftingSlot Count의 값 차이가 발생했습니다. (Refresh 로직의 개선이 필요)");
            }
            else if (needSlotCount < 0) {
                ///for (int i = slots.Count; i >= slots.Count - needSlotCount; --i) {
                ///    if (slots[i].IsActive) {
                ///        slots[i].DisableSlot();
                ///    }
                ///}
                CatLog.WLog("초기화된 CraftingSlot Count의 값 차이가 발생했습니다. (Refresh 로직의 개선이 필요)");
            }

            for (int i = 0; i < userCraftingSlotCount; i++) {
                slots[i].EnableSlot(); // <- Input CraftingSlot Data
            }
        }

        /// <summary>
        /// 항상 LoadBluePrintType field를 사용해서 BluePrint를 가져옴
        /// </summary>
        void RefreshSelectPanel() {
            bpSlots.ForEach((item) => { //모든 슬롯 비-활성화
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
            int needSlotCount = bluePrints.Length - bpSlots.Count;
            if (needSlotCount > 0) { // 부족한 슬롯 만큼 생성 후 캐싱
                for (int i = 0; i < needSlotCount; i++) {
                    var newSlot = GameObject.Instantiate<BluePrintSlot>(bluePrintSlotPrefab, choosePanelParentTr);
                    bpSlots.Add(newSlot);
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
                bpSlots[i].EnableSlot(bluePrints[i]);
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

            bpSlots.ForEach((slot) => { //모든 설계도 슬롯 비-활성화
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
            int requiredSlotCount = blueprints.Length - bpSlots.Count;
            if (requiredSlotCount > 0) {
                for (int i = 0; i < requiredSlotCount; i++) {
                    var newSlot = GameObject.Instantiate<BluePrintSlot>(bluePrintSlotPrefab, choosePanelParentTr);
                    bpSlots.Add(newSlot);
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
                bpSlots[i].EnableSlot(blueprints[i]);
            }

        }

        //======================================================================================================================================
        //========================================================== [ LIFE CYCLE ] ============================================================

        /// <summary>
        /// 해당 클래스는 MonoBehaviour가 아님, UI_Crafting에 의존하여 처리
        /// </summary>
        public void Start(UnityEngine.Events.UnityAction unityAction, byte slotCount, CraftingRecipeSO recipeTable) {
            InitPanelMain(unityAction, slotCount);
            InitPanelSelect();
            recipe = recipeTable;
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
            [SerializeField] GameObject[] disables = null;
            [SerializeField] RectTransform[] highlightPosArray = null;
            [SerializeField] RectTransform highlightBar = null;
            sbyte enableSlotIndex = 0; // 초기에 Enable 되어있는 Index값 default: -1
            Vector2 tempPos = Vector2.zero;

            /// <summary>
            /// Range: [0] ~ [SlotLength - 1]
            /// </summary>
            /// <param name="index"></param>
            public void Switch(sbyte index) {
                if(index >= enables.Length) {
                    throw new System.Exception("Index Range Over");
                }

                if(enableSlotIndex != -1) {
                    enables[enableSlotIndex].SetActive(false);
                }

                enableSlotIndex = index;
                enables[enableSlotIndex].SetActive(true);

                //Set Position Highlight Bar
                tempPos   = highlightBar.anchoredPosition;
                tempPos.x = highlightPosArray[enableSlotIndex].anchoredPosition.x;
                highlightBar.anchoredPosition = tempPos;
            }
        }
    }
}
