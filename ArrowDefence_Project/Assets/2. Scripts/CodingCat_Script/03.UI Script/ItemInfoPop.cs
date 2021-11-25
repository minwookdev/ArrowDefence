namespace ActionCat
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;
    using ActionCat.Data;

    public enum Popup_Type
    { 
        None             = 0,
        Popup_NormalItem = 1,
        Popup_BowItem    = 2,
        Popup_ArrowItem  = 3,
        Popup_Accessory  = 4
    }

    [System.Serializable]
    public class SkillSlot
    {
        public GameObject SlotGO;
        public TextMeshProUGUI TmpSkillName;
        public TextMeshProUGUI TmpSkillDesc;
        public Image[] ImgSkillGradeStar = new Image[3];
        public Image ImgIcon;

        public bool isActiveSlotGO()
        {
            if (SlotGO.activeSelf) return true;
            else                   return false;
        }

        public void ActiveSlot(string name, string desc, SKILL_LEVEL skillLevel, Sprite iconSprite) {
            TmpSkillName.text = name;
            TmpSkillDesc.text = desc;

            //Skill Icon Sprite
            if (iconSprite != null)
                ImgIcon.sprite = iconSprite;

            //Setting Grade Star Image
            SetGradeStar(skillLevel);

            //Active Skill Slot Object
            if (SlotGO.activeSelf == false)
                SlotGO.SetActive(true);
        }

        void SetGradeStar(SKILL_LEVEL level) {
            switch (level)
            {
                case SKILL_LEVEL.LEVEL_LOW:    EnableStar(1); break;
                case SKILL_LEVEL.LEVEL_MEDIUM: EnableStar(2); break;
                case SKILL_LEVEL.LEVEL_HIGH:   EnableStar(3); break;
                case SKILL_LEVEL.LEVEL_UNIQUE: EnableStar(3); break;
                default: EnableStar(0); break;
            }
        }

        void EnableStar(byte count) {
            for (int i = 0; i < ImgSkillGradeStar.Length; i++) {
                if(i < count) {
                    ImgSkillGradeStar[i].gameObject.SetActive(true);
                }
                else {
                    ImgSkillGradeStar[i].gameObject.SetActive(false);
                }
            }
        }

        public void DisableSlot()
        {
            if (SlotGO.activeSelf)
            {
                //Clean up Skill Slot variables
                TmpSkillName.text = "";
                TmpSkillDesc.text = "";

                //TmpSkillGrade.text = "";
                //ImgIcon.sprite = null;

                SlotGO.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class AbilitySlot {
        [SerializeField] GameObject[] slots;

        public void EnableSlot() {
            foreach (var slot in slots) {
                slot.gameObject.SetActive(true);
            }
        }

        public void DisableSlot() {
            foreach (var slot in slots) {
                slot.gameObject.SetActive(false);
            }
        }
    }

    public class ItemInfoPop : MonoBehaviour
    {
        [System.Serializable]
        public class ItemPopupIntegrated {
            //Default Item Info
            [Header("Default Item Info")]
            [SerializeField] Transform  parent;
            [SerializeField] GameObject popupGameObject;
            [SerializeField] Image img_ItemIcon;
            [SerializeField] Image img_Frame;
            [SerializeField] TextMeshProUGUI tmp_ItemName;
            [SerializeField] TextMeshProUGUI tmp_ItemDesc;
            [SerializeField] TextMeshProUGUI tmp_ItemType;
            [SerializeField] TextMeshProUGUI tmp_ItemCount;
            [SerializeField] Button btn_SellItem;
            [SerializeField] Button btn_Fuse;

            //Equip & Release Entry
            [Header("Equip & Release Button")]
            [SerializeField] GameObject equipmentButtonGroup;
            [SerializeField] EventTrigger btn_Equip;
            [SerializeField] EventTrigger btn_Release;
            EventTrigger.Entry equipEntry;
            EventTrigger.Entry releaseEntry;

            //Equipment Item Info
            [Header("Skill Slot")]
            [SerializeField] SkillSlot[] SkillSlots;

            //Ability Slots
            [Header("Ability Slots")]
            [SerializeField] AbilitySlot abilitySlots;

            //Item Address
            AD_item itemAddress;

            public void EnablePopup_Material(Item_Material address, Sprite frame) {
                //Default Item Data Setting
                tmp_ItemName.text   = address.GetName;
                tmp_ItemDesc.text   = address.GetDesc;
                tmp_ItemType.text   = "MATERIAL";
                tmp_ItemCount.text  = address.GetAmount.ToString();
                img_ItemIcon.sprite = address.GetSprite;
                img_Frame.sprite    = frame;

                //Check Description GameObject [TESTING]
                if (tmp_ItemDesc.gameObject.activeSelf == false)
                    tmp_ItemDesc.gameObject.SetActive(true);

                //Disable Ability Slots
                DisableAbilitySlot();

                //Disable All SkillSlots
                DisableSkillSlot();

                //Disable Equip & Release Button Group
                equipmentButtonGroup.gameObject.SetActive(false);

                //Enable Button
                //btn_SellItem.gameObject.SetActive(true);
                //btn_Fuse.gameObject.SetActive(true);

                //Get Item Address
                itemAddress = address;

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            public void EnablePopup_Consumable(Item_Consumable address, Sprite frame) {
                //Default Item Data Setting
                tmp_ItemName.text   = address.GetName;
                tmp_ItemDesc.text   = address.GetDesc;
                tmp_ItemType.text   = "CONSUMABLE";
                tmp_ItemCount.text  = address.GetAmount.ToString();
                img_ItemIcon.sprite = address.GetSprite;
                img_Frame.sprite    = frame;

                //Check Description GameObject [TESTING]
                if (tmp_ItemDesc.gameObject.activeSelf == false)
                    tmp_ItemDesc.gameObject.SetActive(true);

                //Disable Ability Slots
                DisableAbilitySlot();

                //Disable All Skill Slot
                DisableSkillSlot();

                //Disable Equipments Button Group
                equipmentButtonGroup.gameObject.SetActive(false);

                //Enable Button
                //btn_SellItem.gameObject.SetActive(true);
                //btn_Fuse.gameObject.SetActive(true);

                //Get Item Address
                itemAddress = address;

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            public void EnablePopup_Bow(Item_Bow address, Sprite frame) {
                //Default Item Data Setting
                tmp_ItemName.text   = address.GetName;
                tmp_ItemDesc.text   = address.GetDesc;
                tmp_ItemType.text   = "MATERIAL";
                tmp_ItemCount.text  = "";
                img_ItemIcon.sprite = address.GetSprite;
                img_Frame.sprite    = frame;

                //Disable Equipment Item Description [TESTING]
                tmp_ItemDesc.gameObject.SetActive(false);

                //Active Ability Slots
                abilitySlots.EnableSlot();

                //Enable Skill Slots
                var skills = address.GetSkills();
                for (int i = 0; i < skills.Length; i++) {
                    if(skills[i] != null) {
                        SkillSlots[i].ActiveSlot(skills[i].Name, skills[i].Description,
                                                 skills[i].Level, skills[i].IconSprite);
                    }
                    else {
                        SkillSlots[i].DisableSlot();
                    }
                }

                bool isAvailableEquip = false;
                if (ReferenceEquals(CCPlayerData.equipments.GetBowItem(), address) == false) {
                    isAvailableEquip = true;
                }
                
                if(isAvailableEquip == true) { //현재 장착중인 아이템이 아닌 경우 : 장착 버튼 활성화
                    //init-Equip EventEntry
                    equipEntry = new EventTrigger.Entry();
                    equipEntry.eventID = EventTriggerType.PointerClick;
                    equipEntry.callback.AddListener((eventdata) => EventEntryEquipBow(address));
                    btn_Equip.triggers.Add(equipEntry);
                }
                else { //현재 장착중인 아이템인 경우 : 해제 버튼 활성화
                    //Init-Release EventEntry
                    releaseEntry = new EventTrigger.Entry();
                    releaseEntry.eventID = EventTriggerType.PointerClick;
                    releaseEntry.callback.AddListener((eventdata) => EventEntryReleaseBow());
                    btn_Release.triggers.Add(releaseEntry);
                }

                //Enable Condition-Match Button
                SwitchEquipButton(isAvailableEquip);

                //Get Item Address
                itemAddress = address;

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            public void EnablePopup_Arrow(Item_Arrow address, Sprite frame) {
                //Default Item Data Setting
                tmp_ItemName.text   = address.GetName;
                tmp_ItemDesc.text   = address.GetDesc;
                tmp_ItemType.text   = "ARROW";
                tmp_ItemCount.text  = "";
                img_ItemIcon.sprite = address.GetSprite;
                img_Frame.sprite    = frame;

                //Disable Equipment item Description [TESTING]
                tmp_ItemDesc.gameObject.SetActive(false);

                //Enable Ability Slots
                abilitySlots.EnableSlot();

                //Enable Skill Slots
                var skills = address.ArrowSkillInfos;   //Get Skill Array Size : 2
                for (int i = 0; i < skills.Length; i++) {
                    if(skills[i] != null) {
                        SkillSlots[i].ActiveSlot(skills[i].SkillName, skills[i].SkillDesc,
                                                 skills[i].SkillLevel, skills[i].IconSprite);
                    }
                    else {
                        SkillSlots[i].DisableSlot();
                    }
                }

                //Check address Item is Equipped ?
                bool isEquippedItem = (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), address) ||
                                       ReferenceEquals(CCPlayerData.equipments.GetSubArrow(),  address)) ? false : true;
                //Init-EventEntry Condition-Match
                if(isEquippedItem) {
                    //Equip EventTrigger Event Add
                    equipEntry = new EventTrigger.Entry();
                    equipEntry.eventID = EventTriggerType.PointerClick;
                    equipEntry.callback.AddListener((eventdata) => EventEntryEquipArrow(address));
                    btn_Equip.triggers.Add(equipEntry);
                }
                else {
                    //Release EventTrigger Event Add
                    releaseEntry = new EventTrigger.Entry();
                    releaseEntry.eventID = EventTriggerType.PointerClick;
                    releaseEntry.callback.AddListener((eventdata) => EventEntryReleaseArrow(address));
                    btn_Release.triggers.Add(releaseEntry);
                }

                //Enable Condition-Match Button
                SwitchEquipButton(isEquippedItem);

                //Get Item Address
                itemAddress = address;

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            public void EnablePopup_Artifact(Item_Accessory address, Sprite frame) {
                //Default Item Data Setting
                tmp_ItemName.text   = address.GetName;
                tmp_ItemDesc.text   = address.GetDesc;
                tmp_ItemType.text   = "ARTIFACT";
                tmp_ItemCount.text  = "";
                img_ItemIcon.sprite = address.GetSprite;
                img_Frame.sprite    = frame;

                //Disable Equipment Item Description [TESTING]
                tmp_ItemDesc.gameObject.SetActive(false);

                //Enable Ability slots
                abilitySlots.EnableSlot();

                //Enable Skill-Slots
                var spEffect = address.SPEffect;
                if(spEffect != null) {
                    SkillSlots[0].ActiveSlot(spEffect.Name, spEffect.Description, 
                                             spEffect.Level, spEffect.IconSprite);
                }
                else {
                    SkillSlots[0].DisableSlot();
                }   SkillSlots[1].DisableSlot();    //현재 Special Effect는 무조건 한개뿐임. 두번째 슬롯 비활성화.

                //장착중 유물 체크, 장착 슬롯 확인작업.
                byte artifactIdx    = 0; 
                bool isEquippedItem = false;
                foreach (var artifact in CCPlayerData.equipments.GetAccessories()) {
                    if(ReferenceEquals(artifact, address)) {
                        isEquippedItem = true; 
                        break;
                    }
                    artifactIdx++;
                }

                //현재 Equipment에 장착중인 유물 아이템.
                if(isEquippedItem == true) {
                    //Release Button Event 할당하고, Button 활성화
                    releaseEntry = new EventTrigger.Entry();
                    releaseEntry.eventID = EventTriggerType.PointerClick;   // ↓ 클로저 확인 필요 NULL 잡는지 확인
                    releaseEntry.callback.AddListener((eventdata) => EventEntryReleaseArtifact(artifactIdx));
                    btn_Release.triggers.Add(releaseEntry);

                    SwitchEquipButton(false);
                }
                else { //장착중인 유물아이템이 아님.
                    //Equip Button Event 할당 후, Button 활성화
                    equipEntry = new EventTrigger.Entry();
                    equipEntry.eventID = EventTriggerType.PointerClick;
                    equipEntry.callback.AddListener((eventdata) => EventEntryEquipArtifact(address));
                    btn_Equip.triggers.Add(equipEntry);

                    SwitchEquipButton(true);
                }

                //Get Item Address
                itemAddress = address;

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            void DisablePopup() {
                //Clear Default Item Info
                tmp_ItemName.text = "";
                tmp_ItemDesc.text = "";
                tmp_ItemType.text = "";
                tmp_ItemCount.text = "";

                //Clear Skill Slots
                foreach (var skillslot in SkillSlots) {
                    skillslot.DisableSlot();
                }

                //Clear Event Entry
                equipEntry   = null;
                releaseEntry = null;

                //Clear EventTriggers
                btn_Equip.triggers.Clear(); 
                btn_Release.triggers.Clear();

                //Disable Release, Equip Button
                btn_Equip.gameObject.SetActive(false);
                btn_Release.gameObject.SetActive(false);

                //Disable Equipments Button Group
                equipmentButtonGroup.gameObject.SetActive(false);    

                //Clear item Address
                itemAddress = null;

                //Disable Popup
                popupGameObject.SetActive(false);
            }

            #region EVENT_TRIGGER

            void SwitchEquipButton(bool isActiveEquipButton) {
                //Active Equip & Release Button Group
                equipmentButtonGroup.gameObject.SetActive(true);

                if(isActiveEquipButton == true) {
                    btn_Equip.gameObject.SetActive(true);
                    btn_Release.gameObject.SetActive(false);
                }
                else {
                    btn_Equip.gameObject.SetActive(false);
                    btn_Release.gameObject.SetActive(true);
                }
            }

            void EventEntryEquipBow(Item_Bow item) {
                CCPlayerData.equipments.Equip_BowItem(item); 
                Close();
            }

            void EventEntryReleaseBow() {
                CCPlayerData.equipments.Release_BowItem(); 
                Close();
            }

            void EventEntryEquipArrow(Item_Arrow item) {
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ARROW, item);
                Close();
            }

            void EventEntryReleaseArrow(Item_Arrow item) {
                if (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), item) == true)
                    CCPlayerData.equipments.Release_ArrowItem();
                else CCPlayerData.equipments.Release_SubArrow();
                Close();
            }

            void EventEntryEquipArtifact(Item_Accessory item) {
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ACCESSORY, item);
                Close();
            }

            void EventEntryReleaseArtifact(byte idx) {
                CCPlayerData.equipments.Release_Accessory(idx);
                Close();
            }

            #endregion

            #region SKILL_SLOTS

            void DisableSkillSlot() {
                foreach (var slot in SkillSlots) {
                    slot.DisableSlot();
                }
            }

            #endregion

            #region ABILITY_SLOTS

            void DisableAbilitySlot() {
                abilitySlots.DisableSlot();
            }

            #endregion

            #region CLOSE_POPUP

            public void Close() {
                //Update Inventory, Equipments UI
                UI_Equipments.Instance.UpdateEquipUI();
                UI_Inventory.InvenUpdate();

                //Close Self
                DisablePopup();
                parent.gameObject.SetActive(false);
            }

            #endregion
        }


        [Serializable]
        public class ItemPop_Normal
        {
            //Consumeable & Material Item
            public GameObject Popup_Object;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemType;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public TextMeshProUGUI Text_ItemCount;

            private AD_item itemAddress; //어떤 아이템의 주소가 들어가있는지 Custom INspector를 통해 확인할 수 있도록?

            public void EnablePopup(Item_Consumable item, Sprite frame)
            {
                Text_ItemType.text  = "Consumable";
                Text_ItemName.text  = item.GetName;
                Text_ItemDesc.text  = item.GetDesc;
                Text_ItemCount.text = item.GetAmount.ToString();
                Image_Item.sprite   = item.GetSprite;
                Image_Frame.sprite  = frame;
                itemAddress         = item;

                Popup_Object.SetActive(true);
            }

            public void EnablePopup(Item_Material item, Sprite frame)
            {
                Text_ItemType.text  = "Consumable";
                Text_ItemName.text  = item.GetName;
                Text_ItemDesc.text  = item.GetDesc;
                Text_ItemCount.text = item.GetAmount.ToString();
                Image_Item.sprite   = item.GetSprite;
                Image_Frame.sprite  = frame;
                itemAddress         = item;

                Popup_Object.SetActive(true);
            }

            public void DisablePop()
            {
                Text_ItemType.text  = "";
                Text_ItemName.text  = "";
                Text_ItemDesc.text  = "";
                Text_ItemCount.text = "";
                itemAddress         = null;

                Popup_Object.SetActive(false);
            }
        }

        [Serializable]
        public class ItemPop_Equip_Bow
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemType;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;
            
            //Skill Slot Variables
            public SkillSlot[] SkillSlots;

            //Item Address [TEMP]
            private Item_Bow itemAddress;

            public void EnablePopup(Item_Bow item, Sprite sprite)
            {
                Text_ItemType.text = "Equipment";
                Text_ItemName.text = item.GetName;
                //Text_ItemDesc.text = item.GetDesc;    //장비아이템 팝업은 설명 tmp가 없다 [현재 tooltip에서만 사용]
                Image_Item.sprite  = item.GetSprite;
                Image_Frame.sprite = sprite;
                itemAddress        = item;

                for (int i = 0; i < item.GetSkills().Length; i++)
                {
                    if (item.GetSkills()[i] != null) 
                        SkillSlots[i].ActiveSlot(item.GetSkill(i).Name, 
                                                 item.GetSkill(i).Description,
                                                 item.GetSkill(i).Level,
                                                 item.GetSkill(i).IconSprite);
                    else                                
                        SkillSlots[i].DisableSlot();
                }

                //if (CCPlayerData.equipments.GetBowItem() != itemAddress) SwitchButtons(false);
                //else                                                     SwitchButtons(true);

                //플레이어의 현재 장비아이템이 선택한 장비아이템인지 비교해서 버튼 띄워줌
                if (ReferenceEquals(CCPlayerData.equipments.GetBowItem(), itemAddress)) SwitchButtons(true);
                else                                                                    SwitchButtons(false);

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemType.text = "";
                Text_ItemName.text = "";
                itemAddress = null;

                foreach (var item in SkillSlots)
                {
                    if (item.isActiveSlotGO())
                        item.DisableSlot();
                }

                PopObject.SetActive(false);
            }

            public Item_Bow GetItemAddress()
            {
                if (itemAddress != null)
                {
                    CatLog.Log("Bow Item Address 전달되었습니다.");
                    return itemAddress;
                }
                else
                {
                    CatLog.WLog("Bow Item Address 존재하지 않습니다.");
                    return null;
                }
            }

            public void Button_EquipAction()
            {
                CCPlayerData.equipments.Equip_BowItem(itemAddress);
            }

            public void Button_ReleaseAction()
            {
                CCPlayerData.equipments.Release_BowItem();
            }

            //장착버튼 교체
            private void SwitchButtons(bool isEquip)
            {
                if (isEquip) //장착중인 아이템이면 해제버튼을 활성화
                {
                    Button_Release.gameObject.SetActive(true);
                    Button_Equip.gameObject.SetActive(false);
                }
                else        //장착중인 아이템이 아니면 장착버튼을 활성화
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        [Serializable]
        public class ItemPop_Equip_Arrow
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;

            //Skill Slot Variables
            public SkillSlot[] SkillSlots;

            //Item Address
            private Item_Arrow itemAddress;

            public void EnablePopup(Item_Arrow address, Sprite frameSprite)
            {
                itemAddress = address;

                Text_ItemName.text = address.GetName;
                Image_Item.sprite  = address.GetSprite;
                Image_Frame.sprite = frameSprite;

                //Skil Slot Enable Logic
                foreach (var item in SkillSlots)
                {
                    item.DisableSlot();
                }

                //Enable / Disable Equip Button Logic 현재 들고있는 Item Reference랑 비교해서 Equip / Release Button의 Enable 결정
                bool isEquipped = (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), itemAddress) || 
                                   ReferenceEquals(CCPlayerData.equipments.GetSubArrow(),  itemAddress)) ? true : false;

                SwitchButton(isEquipped);

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemName.text = "";
                itemAddress = null;

                foreach (var item in SkillSlots)
                {
                    if (item.isActiveSlotGO() == true)
                        item.DisableSlot();
                }

                PopObject.SetActive(false);
            }

            public void Button_EquipAction()
            {
                //Open Equip Slot Choose Panel
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ARROW, itemAddress);
            }

            public void Button_ReleaseAction()
            {
                //Release 버튼이 떳다는건 일단 Main, Sub Arrow 둘중에 하나 끼고있는거니까 둘간에 비교해서 Release 해주면 된다
                if (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), itemAddress))
                    CCPlayerData.equipments.Release_ArrowItem();
                else CCPlayerData.equipments.Release_SubArrow();
            }

            private void SwitchButton(bool isEquip)
            {
                if (isEquip)
                {
                    Button_Equip.gameObject.SetActive(false);
                    Button_Release.gameObject.SetActive(true);
                }
                else
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        [Serializable]
        public class ItemPop_Equip_Accessory
        {
            public GameObject PopObject;
            public Image Image_Item;
            public Image Image_Frame;
            public TextMeshProUGUI Text_ItemName;
            public TextMeshProUGUI Text_ItemDesc;
            public Button Button_Equip;
            public Button Button_Release;

            //Skill Slot variables
            public SkillSlot[] SkillSlots;

            private Item_Accessory itemAddress;
            private byte accessoryIdx = 0;

            public void EnablePopup(Item_Accessory address, Sprite frameSprite)
            {
                itemAddress = address;

                Text_ItemName.text = address.GetName;
                Image_Item.sprite = address.GetSprite;
                Image_Frame.sprite = frameSprite;

                //Active Skill Slot if the having Special Effect
                for (int i = 0; i < SkillSlots.Length; i++)
                {
                    if (i == 0)
                    {
                        if (itemAddress.SPEffect != null)
                            SkillSlots[i].ActiveSlot(itemAddress.SPEffect.Name,
                                                     itemAddress.SPEffect.Description,
                                                     itemAddress.SPEffect.Level,
                                                     itemAddress.SPEffect.IconSprite);
                        else
                            SkillSlots[i].DisableSlot();
                    }
                    else
                        SkillSlots[i].DisableSlot();
                }

                //Enable / Disable Equip Button Logic
                foreach (var item in CCPlayerData.equipments.GetAccessories())
                {
                    if (ReferenceEquals(item, itemAddress))
                    {
                        SwitchButton(true);
                        break;
                    }
                    else SwitchButton(false);

                    accessoryIdx++;
                }

                PopObject.SetActive(true);
            }

            public void DisablePopup()
            {
                Text_ItemName.text = "";
                itemAddress = null;
                accessoryIdx = 0;

                foreach (var item in SkillSlots)
                {
                    if (item.isActiveSlotGO() == true)
                        item.DisableSlot();
                }

                PopObject.SetActive(false);
            }

            public void Button_EquipAction()
            {
                //Open Equipment Slot Choose Panel
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ACCESSORY, itemAddress);

                //CCPlayerData.equipments.Equip_AccessoryItem(itemAddress);
            }

            public void Button_ReleaseAction()
            {
                //새로운 Release 메서드를 사용하도록 수정
                //릴리즈 버튼이 떳다는건 지금 악세칸에 있다는거임.
                //CCPlayerData.equipments.Release_AccessoryItem();

                CCPlayerData.equipments.Release_Accessory(accessoryIdx);
            }

            private void SwitchButton(bool isEquip)
            {
                if(isEquip)
                {
                    Button_Equip.gameObject.SetActive(false);
                    Button_Release.gameObject.SetActive(true);
                }
                else
                {
                    Button_Equip.gameObject.SetActive(true);
                    Button_Release.gameObject.SetActive(false);
                }
            }
        }

        [Header("Item Grade Frames")]
        public Sprite[] Frames;


        [Header("Item Popup Type")]
        [Space(10)]
        public ItemPop_Normal    ItemPop;               //Material, Consumable Item Popup
        public ItemPop_Equip_Bow ItemPop_Bow;           //Equipment Bow Item Popup
        public ItemPop_Equip_Arrow ItemPop_Arrow;       //Equipment Arrow Item Popup
        public ItemPop_Equip_Accessory ItemPop_Access;  //Equipment Accessory Item Popup
        [SerializeField] ItemPopupIntegrated itemPopup; //통합 아이템 팝업 [모든 아이템 종류의 정보 팝업을 하나로 관리]
        private Popup_Type       popType = Popup_Type.None; //현재 열려있는 팝업타입 통합되면 Type 필요없을듯

        /*
            index[0] Material Item, Consumable Item
            index[1] Equipment Item (Non-Skill)
            index[2] Equipment Item (One-SKill)
            index[3] Equipment Item (Two-Skill)
            index[4] Accessory Item ()
        */

        public void Open_Popup_ConItem(Item_Consumable item)
        {
            //ItemPop.EnablePopup(item, Frames[(int)item.GetGrade]);

            //Enable Integrated Item Popup
            itemPopup.EnablePopup_Consumable(item, Frames[(int)item.GetGrade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_MatItem(Item_Material item)
        {
            //ItemPop.EnablePopup(item, Frames[(int)item.GetGrade]);

            //Enable Integrated Item Popup
            itemPopup.EnablePopup_Material(item, Frames[(int)item.GetGrade]);
            popType = Popup_Type.Popup_NormalItem;
        }

        public void Open_Popup_EquipItem(Item_Equipment item)
        {
            switch (item)
            {
                case Item_Bow bowItem:          ItemPop_Bow.EnablePopup(bowItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_BowItem;  break;
                case Item_Arrow arrowItem:      ItemPop_Arrow.EnablePopup(arrowItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_ArrowItem;  break;
                case Item_Accessory accessItem: ItemPop_Access.EnablePopup(accessItem, Frames[(int)item.GetGrade]); popType = Popup_Type.Popup_Accessory; break;
                default: break;
            }
        }

        #region OPEN_POPUP_NEW

        public void OpenPopup_MaterialItem(Item_Material item) {
            gameObject.SetActive(true);
            itemPopup.EnablePopup_Material(item, Frames[(int)item.GetGrade]);
        }

        public void OpenPopup_ConsumableItem(Item_Consumable item) {
            gameObject.SetActive(true);
            itemPopup.EnablePopup_Consumable(item, Frames[(int)item.GetGrade]);
        }

        public void OpenPopup_EquipmentItem(Item_Equipment equipItem) {
            gameObject.SetActive(true);
            switch (equipItem) {
                case Item_Bow bow: 
                    itemPopup.EnablePopup_Bow(bow, Frames[(int)bow.GetGrade]); break;
                case Item_Arrow arrow: 
                    itemPopup.EnablePopup_Arrow(arrow, Frames[(int)arrow.GetGrade]); break;
                case Item_Accessory artifact: 
                    itemPopup.EnablePopup_Artifact(artifact, Frames[(int)artifact.GetGrade]); break;
            }
        }

        #endregion

        #region BUTTON_METHOD

        public void Button_EquipBowItem()
        {
            ItemPop_Bow.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();
            
            Close_Popup(3);
        }

        public void Button_ReleaseBowItem()
        {
            ItemPop_Bow.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(3);
        }

        public void Button_EquipArrowItem()
        {
            ItemPop_Arrow.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();


            Close_Popup(1);
        }

        public void Button_ReleaseArrowItem()
        {
            ItemPop_Arrow.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(1);
        }
        
        public void Button_EquipAccessItem()
        {
            ItemPop_Access.Button_EquipAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(2);
        }

        public void Button_ReleaseAccessItem()
        {
            ItemPop_Access.Button_ReleaseAction();
            UI_Equipments.Instance.UpdateEquipUI();
            UI_Inventory.InvenUpdate();

            Close_Popup(2);
        }

        public void Close_Popup(int popnum)
        {
            //itemAddress = null;

            switch (popnum)
            {
                case 0: ItemPop.DisablePop();           break;   //Con, Mat
                case 1: ItemPop_Arrow.DisablePopup();   break;   //Equip Non-Skill
                case 2: ItemPop_Access.DisablePopup();  break;   //Equip One-Skill
                case 3: ItemPop_Bow.DisablePopup();     break;   //New Bow Item
                default: break;
            }

            popType = Popup_Type.None;
            this.gameObject.SetActive(false);
        }

        public void Close_Popup()
        {
            switch (popType)
            {
                case Popup_Type.None:             CatLog.Log("현재 열려있는 팝업 타입 Enum 없음."); break;
                case Popup_Type.Popup_NormalItem: ItemPop.DisablePop();                           break;
                case Popup_Type.Popup_BowItem:    ItemPop_Bow.DisablePopup();                     break;
                case Popup_Type.Popup_ArrowItem:  ItemPop_Arrow.DisablePopup();                   break;
                case Popup_Type.Popup_Accessory:  ItemPop_Access.DisablePopup();                  break;
                default:                                                                          break;
            }

            popType = Popup_Type.None;
            this.gameObject.SetActive(false);
        }

        public void Close() {
            itemPopup.Close();
        }

        #endregion
    }
}
