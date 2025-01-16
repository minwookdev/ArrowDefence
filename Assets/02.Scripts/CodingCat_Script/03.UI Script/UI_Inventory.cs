namespace ActionCat {
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using ActionCat.Data;
    using ActionCat.Interface;

    [Serializable]
    public class SwitchButton
    {
        public GameObject ObjButton;
        public bool isOn = false;

        public void Button_Switch(bool isOn)
        {
            this.isOn = (isOn) ? true : false;

            if (this.isOn) ObjButton.SetActive(true);
            else           ObjButton.SetActive(false);
        }
    }

    [Serializable]
    public class ItemSortPanel
    {
        public enum Type_ItemSort
        {
            SortType_All       = 0,
            SortType_Normal    = 1,
            SortType_Bow       = 2,
            SortType_Arrow     = 3,
            SortType_ArrowSub  = 4,
            SortType_Accessory = 5
        }

        public enum ItemSort_Type
        {
            SortType_All       = 0,
            SortType_Bow       = 1,
            SortType_Arrow     = 2,
            SortType_Accessory = 3,
            SortType_Etc       = 4
        }

        //public Type_ItemSort SortType = Type_ItemSort.SortType_All;
        //같은 SortType이면 Inventory Load 할때 불러오지 않도록 하기

        [ReadOnly] public ItemSort_Type SortType = ItemSort_Type.SortType_All;
        public Transform DecoBar;
        public SwitchButton[] Buttons;

        public void Update_Tab(int num) => SortType = (ItemSort_Type)num;

        public void Button_Switch(int num)
        {
            Buttons[num].Button_Switch(true);

            for(int i = 0; i < Buttons.Length; i++)
            {
                if (i == num) continue;
                Buttons[i].Button_Switch(false);
            }

            if (DecoBar == null) return;
            var decoBarX = DecoBar.position;
            decoBarX.x = Buttons[num].ObjButton.transform.position.x;
            DecoBar.position = decoBarX;
            //추후에 트윈작업 하기도 편하겠다

            //CatLog.Log("여기까지 잘 들어오고 있음 !");
            //CatLog.Log($"버튼 X Local Position : {Buttons[num].ObjButton.transform.position.x}");
        }
    }

    /// <summary>
    /// Main Scene UI Inventory Script
    /// </summary>
    public class UI_Inventory : MonoBehaviour, IMainMenu {
        [Header("Player Data Setting")]
        //public AD_PlayerData playerData;
        private List<AD_item> inventoryList = new List<AD_item>();

        [Header("Inventory UI Control")]
        [Space(15)]
        [SerializeField] private Transform itemSlotContainer;
        [SerializeField] private GameObject itemSlotPref;
        [SerializeField] private List<UI_ItemSlot> slotList = new List<UI_ItemSlot>(); 
        //GameObject형이 아닌 ItemSlot 형으로 받아오는건 어떨지?

        [Header("Item Sort Panel")]
        public ItemSortPanel SortPanel;

        [Header("MAIN")]
        [SerializeField] CanvasGroup mainCanvasGroup = null;
        [SerializeField] RectTransform mainRectTr    = null;

        public delegate void InventoryUpdate();
        public static InventoryUpdate InvenUpdate;
        //event 붙여버리면 다른데에서 사용이 불가능해진다
        UI.MainMenu.MainMenuTween tween = new UI.MainMenu.MainMenuTween(.5f, .3f);
       
        bool IMainMenu.IsTweenPlaying() {
            return tween.IsTweenPlaying;
        }

        void IMainMenu.MenuOpen() {
            tween.MenuOpenTween(mainRectTr, mainCanvasGroup);
        }

        void IMainMenu.MenuClose() {
            tween.MenuCloseTween(mainRectTr, mainCanvasGroup);
        }

        private void Awake()
        {
            for (int i = 0; i < itemSlotContainer.childCount; i++)
            {
                slotList.Add(itemSlotContainer.GetChild(i).GetComponent<UI_ItemSlot>());
                //Slot Prefab 오브젝트들 미리 깔아두고 Instantiate 하지말고 Enable/Disable 하는 방향으로
                //Slot Object를 컨트롤하면 될거같다
                //Start에 놨더니 OnEnable과 차이가 좁아 UpdateUIinventory할때 문제가 발생함
                //추후에도 문제가 발생하지 않는지 체크 필요

                //GameObject 형이 아닌 UI_ItemSlot형으로 변경했지만, 인벤토리가 열리는 시점에 심하게
                //프레임 다운이 발생하지 않는지 검증 필요. -> 빌드때 심하면 외부에서 미리 Add해주는 작업 요청필요
            }

            InvenUpdate += ClearUIinventory;
            InvenUpdate += UpdateUIinventory;
        }

        private void Start() => SortPanel.Button_Switch(0); //Inventory 처음에 열고 시작하면 Bar살짝 밀리는거 왜그런지

        private void OnEnable() => InvenUpdate();

        private void OnDisable()
        {
            ClearUIinventory();
        }

        private void OnDestroy()
        {
            InvenUpdate -= ClearUIinventory;
            InvenUpdate -= UpdateUIinventory;
        }

        public void CheckPlayerData()
        {
            if(CCPlayerData.inventory == null)
            {
                CatLog.ELog("UI_Inventory : Player Data is return Null");
                gameObject.SetActive(false);
            }
        }

        private void UpdateUIinventory(int num)
        {
            inventoryList = CCPlayerData.inventory.GetAllItemList();

            //깔려있는 Slot이 부족하면 그만큼 새로 생성
            if (inventoryList.Count > slotList.Count)
            {
                var count = inventoryList.Count - slotList.Count;
                for(int i =0; i < count; i++)
                {
                    var newSlot = Instantiate(itemSlotPref, itemSlotContainer).GetComponent<UI_ItemSlot>();
                    newSlot.gameObject.SetActive(false);
                    slotList.Add(newSlot); //부족한 만큼 생성해준다
                }
            }

            for (int i = 0; i < inventoryList.Count; i++)
            {
                slotList[i].gameObject.SetActive(true);
                slotList[i].EnableSlot(inventoryList[i]);
                //아이템이 100개 200개씩 되어도 문제가 없도록 구현
            }
        }

        private void UpdateUIinventory() {
            //inventoryList.Clear();  //List 불러오기 전 기존 List 정리해줌 아니
            //;; 여기서 지워버리면 어떡함 주소값 다 가지고있는데
            //기존에 있던 UI_Slot들 지워주는 로직 필요함. -> 탭 넘길때마다 기존에있던 아이템Slot들이 남음
            //1. 일단 GetItemList 하고 Count는 제대로 적용되는지 확인하고
            //2. Count는 제대로 넘겨받으면 Tab 바뀔때마다 Clear UI해주는 로직 추가
            
            switch (SortPanel.SortType) {
                case ItemSortPanel.ItemSort_Type.SortType_All       : inventoryList = CCPlayerData.inventory.GetAllItemList();       break;
                case ItemSortPanel.ItemSort_Type.SortType_Bow       : inventoryList = CCPlayerData.inventory.GetBowItemList();       break;
                case ItemSortPanel.ItemSort_Type.SortType_Arrow     : inventoryList = CCPlayerData.inventory.GetArrowItemList();     break;
                case ItemSortPanel.ItemSort_Type.SortType_Accessory : inventoryList = CCPlayerData.inventory.GetAccessoryItemList(); break;
                case ItemSortPanel.ItemSort_Type.SortType_Etc       : inventoryList = CCPlayerData.inventory.GetItemList();          break;
                default: throw new System.NotImplementedException("Wrong Sorting Number.");
            }

            CatLog.Log(StringColor.YELLOW ,$"Inventory Interface Update, Load Item Counts: {inventoryList.Count}");

            //if (inventoryList.Count <= 0) return; //Inventory 아무것도 없으면 return 하는 로직 추가

            if(inventoryList.Count > slotList.Count) {
                var count = inventoryList.Count - slotList.Count;
                for(int i = 0; i < count; i++) {
                    var newSlot = Instantiate(itemSlotPref, itemSlotContainer).GetComponent<UI_ItemSlot>();
                    newSlot.gameObject.SetActive(false);
                    slotList.Add(newSlot);  //부족한 만큼 생성해서 SlotPrefList에 Add
                }
            }

            for(int i =0;i<inventoryList.Count;i++) {
                slotList[i].gameObject.SetActive(true);
                slotList[i].EnableSlot(inventoryList[i]);
            }
        }
        

        private void ClearUIinventory() {
            slotList.FindAll(x => x.gameObject.activeSelf == true).ForEach(x => x.DisableSlot());
            //CatLog.Log(StringColor.YELLOW, "Inventory: Clear All Slots.");
        }

        #region BUTTON_METHOD

        /// <summary>
        /// 매개변수 Number에 따라 SortType을 바꿔주는 Method
        /// </summary>
        /// <param name="num"></param>
        public void Button_SortTab(int num)
        {
            if (num == (int)SortPanel.SortType) //같은 탭을 실행하려고 하면 return
            {
                CatLog.WLog("같은 Tab버튼이 입력되어 Sort Action : return 되었습니다.");
                return;
            }
            SortPanel.Update_Tab(num);

            InvenUpdate();
        }

        /// <summary>
        /// Button Toggle Method
        /// </summary>
        /// <param name="num"></param>
        public void Button_Switch(int num)
        {
            if (SortPanel.Buttons[num].isOn) 
            {
                CatLog.WLog("같은 Tab버튼이 입력되어 Switch Button : return 되었습니다.");
                return;
            }
            SortPanel.Button_Switch(num);
        }

        #endregion
    }
}
