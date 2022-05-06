namespace ActionCat {
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using TMPro;
    using ActionCat.Data;

    [System.Serializable]
    public class SkillSlot {
        public GameObject SlotGO;
        public TextMeshProUGUI TmpSkillName;
        public TextMeshProUGUI TmpSkillDesc;
        [SerializeField] Image[] imagesGrade = null;
        public Image ImgIcon;

        private Sprite enableSprite = null;
        private Sprite disableSprite = null;
        private Sprite halfSprite = null;

        public void Init(params Sprite[] sprites) {
            enableSprite  = sprites[0];
            disableSprite = sprites[1];
            halfSprite    = sprites[2];
        }

        public bool isActiveSlotGO() {
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
            switch (level) {
                case SKILL_LEVEL.LEVEL_LOW:    EnableStar(1); break;
                case SKILL_LEVEL.LEVEL_MEDIUM: EnableStar(2); break;
                case SKILL_LEVEL.LEVEL_HIGH:   EnableStar(3); break;
                case SKILL_LEVEL.LEVEL_UNIQUE: EnableStar(3); break;
                default: throw new System.NotImplementedException();
            }
        }

        void EnableStar(byte count) {
            for (int i = 0; i < imagesGrade.Length; i++) {
                imagesGrade[i].sprite = (i < count) ? enableSprite : disableSprite;
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
        [System.Serializable]
        public class Slots {
            [SerializeField] GameObject parent = null;
            [SerializeField] TextMeshProUGUI abilityName = null;
            [SerializeField] Image[] imageStars = null;

            Sprite enableStar  = null;
            Sprite disableStar = null;
            Sprite halfStar    = null;

            bool isInitialized = false;

            public void Init(Sprite[] sprites) {
                enableStar = sprites[0];
                disableStar = sprites[1];
                halfStar = sprites[2];
            }

            public void EnableSlot(Ability ability) {
                //Set Grade Star
                //int tempLength = 0;
                //for (int i = 0; i < ability.GetGrade(); i++) {
                //    enableStars[i].SetActive(true);
                //    tempLength++;
                //}
                //
                ////Disable remaining star
                //if(tempLength < enableStars.Length) {
                //    for (int i = tempLength; i < enableStars.Length; i++) {
                //        enableStars[i].SetActive(false);
                //    }
                //}

                float calcGradeCount = (float)ability.GetGrade() / 2;
                var isHalf = !(calcGradeCount % 1 == 0f);
                byte lastEnableIndex = 0;
                for (int i = 0; i < imageStars.Length; i++) {
                    bool enable = (i < calcGradeCount);
                    imageStars[i].sprite = (enable) ? enableStar : disableStar;
                    lastEnableIndex = (enable) ? (byte)i : lastEnableIndex;
                }
                if (isHalf) {
                    imageStars[lastEnableIndex].sprite = halfStar;
                }

                //Set Ability Name
                abilityName.text = ability.GetNameByTerms();
                //Active Parent GameObject
                parent.SetActive(true);
            }

            public void DisableSlot() {
                foreach (var image in imageStars) {
                    image.sprite = disableStar;
                }
                abilityName.text = "ABILITY NAME";
                parent.SetActive(false);
            }
        }

        [SerializeField] Slots[] abilitySlots = null;

        public void Init(Sprite[] sprites) {
            foreach (var slot in  abilitySlots) {
                slot.Init(sprites);
            }
        }

        public void EnableSlots(Ability[] abilities) {
            if(abilities == null) {
                DisableAllSlots(); 
                return;
            }
            else if (abilities.Length > 5) {
                CatLog.ELog("Item Ability Size OverRange, Max ability Range is 5.");
                return;
            }

            int count = abilitySlots.Length;
            for (int i = 0; i < abilities.Length; i++) {
                abilitySlots[i].EnableSlot(abilities[i]);
                count--;
            }

            //Disable remaining slots.
            if(count > 0) {
                for (int i = abilitySlots.Length - 1; i >= abilitySlots.Length - count; i--) {
                    abilitySlots[i].DisableSlot();
                }
            }
        }

        public void DisableAllSlots() {
            for (int i = 0; i < abilitySlots.Length; i++) {
                abilitySlots[i].DisableSlot();
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
            [SerializeField] TextMeshProUGUI tmp_ItemType;
            [SerializeField] TextMeshProUGUI tmp_ItemCount;
            [SerializeField] TextMeshProUGUI tmp_ItemDesc;
            [SerializeField] Button btn_SellItem;
            [SerializeField] Button btn_Fuse;
            [SerializeField] RectTransform rectTrBtnGroup = null;
            [SerializeField] RectTransform descRectTr = null;

            //Equip & Release Entry
            [Header("Equip & Release Button")]
            [SerializeField] GameObject equipmentButtonGroup;
            [SerializeField] EventTrigger btn_Equip;
            [SerializeField] EventTrigger btn_Release;
            EventTrigger.Entry equipEntry;
            EventTrigger.Entry releaseEntry;
            bool isExistBtns {
                get {
                    if (btn_Equip != null && btn_Release != null) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }

            //Equipment Item Info
            [Header("Skill Slot")]
            [SerializeField] SkillSlot[] SkillSlots;

            //Ability Slots
            [Header("Ability Slots")]
            [SerializeField] AbilitySlot abilitySlots;

            //Item Address
            AD_item itemAddress;

            [SerializeField] [ReadOnly] bool isPreviewOpenMode = false;

            public void Init(Sprite[] sprites) {
                abilitySlots.Init(sprites);
                foreach (var slot in SkillSlots) {
                    slot.Init(sprites);
                }

                //Disable All Skill Slots
                SkillSlots.Foreach(slot => slot.DisableSlot());
            }

            void FixDescriptionRectSize(string itemDesc) {
                var rectTransform = tmp_ItemDesc.gameObject.GetComponent<RectTransform>();
                Vector2 tempSizeDelta = rectTransform.sizeDelta;
                if (itemDesc.Length <= 10)      tempSizeDelta.y = 15f;
                else if (itemDesc.Length <= 40) tempSizeDelta.y = 75f;
                else if (itemDesc.Length <= 80) tempSizeDelta.y = 90f;
                else                            tempSizeDelta.y = 105f;
                rectTransform.sizeDelta = tempSizeDelta;
            }

            void SetDefaultItemInfo(AD_item item, string type, bool isCounting, Sprite frame) {
                tmp_ItemName.text = item.GetNameByTerms;
                tmp_ItemType.text = type;
                if (isCounting)
                     tmp_ItemCount.text = item.GetAmount.ToString();
                else tmp_ItemCount.text = "";
                img_ItemIcon.sprite = item.GetSprite;
                img_Frame.sprite = frame;

                //Set Item Description Field
                tmp_ItemDesc.text = item.GetDescByTerms;
                if (string.IsNullOrEmpty(item.GetDescByTerms)) {
                    descRectTr.gameObject.SetActive(false);
                }
                else {
                    descRectTr.gameObject.SetActive(true);
                }
            }

            /// <summary>
            /// isShowAmount가 true일때, default amount가 들어감
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="isShowAmount"></param>
            void SetDefaultItemInfo(ItemData entity, Sprite frame, bool isShowAmount = false) {
                tmp_ItemName.text = entity.NameByTerms;
                switch (entity.Item_Type) {
                    case ITEMTYPE.ITEM_MATERIAL:   tmp_ItemType.text = "MATERIAL";   break;
                    case ITEMTYPE.ITEM_CONSUMABLE: tmp_ItemType.text = "CONSUMABLE"; break;
                    case ITEMTYPE.ITEM_EQUIPMENT:  tmp_ItemType.text = "EQUIPMENT";  break;
                    default: throw new System.NotImplementedException();
                }

                tmp_ItemCount.text  = (isShowAmount) ? entity.DefaultAmount.ToString() : "";
                img_ItemIcon.sprite = entity.Item_Sprite;
                img_Frame.sprite    = frame;

                //Set Item Description Field
                string description = entity.DescByTerms;
                if (string.IsNullOrEmpty(description)) {
                    descRectTr.gameObject.SetActive(false);
                }
                else {
                    descRectTr.gameObject.SetActive(true);
                    tmp_ItemDesc.text = description;
                }
            }

            public void DisableAmountText() {
                tmp_ItemCount.text = "";
            }

            /// <summary>
            /// 재료 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enableButtonGroup">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_Material address, Sprite frame, bool enableButtonGroup = true, bool isPreview = false) {
                //Enable Default Item Info.
                SetDefaultItemInfo(address, "MATERIAL", true, frame);

                //Disable Ability Slots
                DisableAbilitySlot();

                //Disable All SkillSlots
                DisableSkillSlot();

                rectTrBtnGroup.gameObject.SetActive(enableButtonGroup); //전체 버튼그룹 비활성화
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {                                      //장착/해제 버튼 활성화 여부: 재료 아이템 비활성화
                    equipmentButtonGroup.gameObject.SetActive(false);
                }

                if (!isPreviewOpenMode) { //assignment item address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            /// <summary>
            /// 소모성 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enablebuttons">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_Consumable address, Sprite frame, bool enablebuttons = true, bool isPreview = false) {
                //Default Item Data Setting
                SetDefaultItemInfo(address, "CONSUMABLE", true, frame);

                //Disable Ability Slots
                DisableAbilitySlot();

                //Disable All Skill Slot
                DisableSkillSlot();

                rectTrBtnGroup.gameObject.SetActive(enablebuttons);     //전체 버튼그룹 비활성화
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {                                      //장착/해제 버튼 활성화 여부: 소모성 아이템 비활성화
                    equipmentButtonGroup.gameObject.SetActive(false);
                }

                if (!isPreviewOpenMode) { //assignment item Address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            /// <summary>
            /// 활 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enablebuttons">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_Bow address, Sprite frame, bool enablebuttons = true, bool isPreview = false) {
                //Default Item Data Setting
                SetDefaultItemInfo(address, "BOW", false, frame);

                //Active Ability Slots
                abilitySlots.EnableSlots(address.AbilitiesOrNull);

                //Enable Skill Slots
                var skills = address.GetSkillsOrNull(); //Get Skill Array Size: 0 ~ 3
                if (skills.Length - SkillSlots.Length > 0) {
                    CatLog.ELog($"Item Info Popup의 Skill Holder가 부족합니다.");
                }
                for (int i = 0; i < skills.Length; i++) {
                    if (skills[i] != null) {
                        SkillSlots[i].ActiveSlot(skills[i].GetNameByTerms(), skills[i].GetDescByTerms(), skills[i].Level, skills[i].IconSprite);
                    }
                }

                rectTrBtnGroup.gameObject.SetActive(enablebuttons);
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {
                    bool isAvailableEquip = false;
                    if (ReferenceEquals(CCPlayerData.equipments.GetBowItem(), address) == false) {
                        isAvailableEquip = true;
                    }

                    if (isAvailableEquip == true) { //현재 장착중인 아이템이 아닌 경우 : 장착 버튼 활성화
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
                }

                if (!isPreviewOpenMode) { //assignment Item Address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            /// <summary>
            /// 화살 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enablebuttons">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_Arrow address, Sprite frame, bool enablebuttons = true, bool isPreview = false) {
                //Default Item Data Setting
                SetDefaultItemInfo(address, "ARROW", false, frame);

                //Enable Ability Slots
                abilitySlots.EnableSlots(address.AbilitiesOrNull);

                //Enable Skill Slots
                var skills = address.SkillInfosOrNull;   //Get Skill Array Size : 0 ~ 3
                if (skills.Length - SkillSlots.Length > 0) {
                    CatLog.ELog($"Item Info Popup의 Skill Holder가 부족합니다.");
                }
                for (int i = 0; i < skills.Length; i++) {
                    if (skills[i] != null) {
                        SkillSlots[i].ActiveSlot(skills[i].GetNameByTerms(), skills[i].GetDescByTerms(), skills[i].SkillLevel, skills[i].IconSprite);
                    }
                }

                rectTrBtnGroup.gameObject.SetActive(enablebuttons);
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {
                    //Check address Item is Equipped ?
                    bool isEquippedItem = (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), address) ||
                                           ReferenceEquals(CCPlayerData.equipments.GetSubArrow(), address)) ? false : true;
                    //Init-EventEntry Condition-Match
                    if (isEquippedItem) {
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
                }

                if (!isPreviewOpenMode) { //assignment Item Address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            /// <summary>
            /// 유물 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enablebuttons">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_Accessory address, Sprite frame, bool enablebuttons = true, bool isPreview = false) {
                //Default Item Data Setting
                SetDefaultItemInfo(address, "ARTIFACT", false, frame);

                //Enable Ability slots
                abilitySlots.EnableSlots(address.AbilitiesOrNull);

                //Enable Skill-Slots (Artifact Special Skill Max : Only 1)
                // 유물 스킬의 최대 길이가 1이라서 이런식을 해놨는데, 나중에 스킬 배열의 길이가 늘어나면 수정들어가야 된다.
                var spEffect = address.SPEffectOrNull;
                if (spEffect != null) {
                    SkillSlots[0].ActiveSlot(spEffect.GetNameByTerms(), spEffect.GetDescByTerms(), spEffect.Level, spEffect.IconSprite);
                }


                rectTrBtnGroup.gameObject.SetActive(enablebuttons);
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {
                    //장착중 유물 체크, 장착 슬롯 확인작업.
                    byte artifactIdx = 0;
                    bool isEquippedItem = false;
                    foreach (var artifact in CCPlayerData.equipments.GetAccessories()) {
                        if (ReferenceEquals(artifact, address)) {
                            isEquippedItem = true;
                            break;
                        }
                        artifactIdx++;
                    }

                    //현재 Equipment에 장착중인 유물 아이템.
                    if (isEquippedItem == true) {
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
                }

                if (!isPreviewOpenMode) { //assignment item address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);
            }

            /// <summary>
            /// 특수 화살 아이템 팝업
            /// </summary>
            /// <param name="address"></param>
            /// <param name="frame"></param>
            /// <param name="enablebuttons">버튼 그룹 활성화 여부</param>
            /// <param name="isPreview">인벤토리에서 사용하는 아이템 팝업이 아닌 모든 경우</param>
            public void EnablePopup(Item_SpArr address, Sprite frame, bool enablebuttons = true, bool isPreview = false) {
                SetDefaultItemInfo(address, "SPECIAL ARROW", false, frame); // Set Default Item Data
                abilitySlots.EnableSlots(address.AbilitiesOrNull);          // Set Item Ability Data
                var skills      = address.GetSkillInfos;                    // Set Equipment Item Skill Data
                var slotsLength = SkillSlots.Length;
                for (int i = 0; i < skills.Length; i++) {
                    if (i == 3) {
                        CatLog.WLog("Need More Skill Slots. Slot Index Over !"); break;
                    }
                    SkillSlots[i].ActiveSlot(skills[i].GetNameByTerms(), 
                                             skills[i].GetDescByTerms(), 
                                             skills[i].SkillLevel, 
                                             skills[i].IconSprite);
                    slotsLength--;
                }
                if (slotsLength > 0) {
                    for (int i = SkillSlots.Length - 1; i >= SkillSlots.Length - slotsLength; i--) {
                        SkillSlots[i].DisableSlot();
                    }
                }

                //var skillInfos = address.GetSkillInfos;
                //for (int i = 0; i < SkillSlots.Length; i++) {
                //    if(skillInfos[i] != null) SkillSlots[i].ActiveSlot(skillInfos[i].SkillName, skillInfos[i].SkillDesc, skillInfos[i].SkillLevel, skillInfos[i].IconSprite);
                //    else                      SkillSlots[i].DisableSlot();
                //}

                rectTrBtnGroup.gameObject.SetActive(enablebuttons);
                isPreviewOpenMode = isPreview;
                if (isExistBtns) {
                    //Check this item is Equipped
                    bool isActiveEquipButton = (ReferenceEquals(CCPlayerData.equipments.GetSpArrOrNull, address)) ? false : true;
                    if (isActiveEquipButton == true) { //Equip Button Event Entry add
                        equipEntry = new EventTrigger.Entry();
                        equipEntry.eventID = EventTriggerType.PointerClick;
                        equipEntry.callback.AddListener((eventData) => EventEntryEquipSpArr(address));
                        btn_Equip.triggers.Add(equipEntry);
                    }
                    else {
                        releaseEntry = new EventTrigger.Entry();
                        releaseEntry.eventID = EventTriggerType.PointerClick;
                        releaseEntry.callback.AddListener((eventData) => EventEntryReleaseSpArr());
                        btn_Release.triggers.Add(releaseEntry);
                    }

                    SwitchEquipButton(isActiveEquipButton);
                }

                if (!isPreviewOpenMode) { //assignment item address
                    itemAddress = address;
                }

                //Enable Popup
                popupGameObject.SetActive(true);    
            }

            /// <summary>
            /// !! Not Use this Method. (Not Implemented Method//)
            /// </summary>
            /// <param name="entity"></param>
            /// <param name="frame"></param>
            public void OpenEntityInfo(ItemData entity, Sprite frame) {
                throw new System.NotImplementedException();
                //SetDefaultItemInfo(entity, frame);
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

                //Clear Ability Slots
                abilitySlots.DisableAllSlots();

                if (isExistBtns) {
                    //Clear Event Entry
                    equipEntry = null;
                    releaseEntry = null;

                    //Clear EventTriggers
                    btn_Equip.triggers.Clear();
                    btn_Release.triggers.Clear();

                    //Disable Release, Equip Button
                    btn_Equip.gameObject.SetActive(false);
                    btn_Release.gameObject.SetActive(false);

                    //Disable Equipments Button Group
                    equipmentButtonGroup.gameObject.SetActive(false);
                }

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
                CCPlayerData.equipments.EquipItem_Bow(item); 
                Close();
            }

            void EventEntryReleaseBow() {
                CCPlayerData.equipments.ReleaseItem_Bow(); 
                Close();
            }

            void EventEntryEquipArrow(Item_Arrow item) {
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ARROW, item);
                Close();
            }

            void EventEntryReleaseArrow(Item_Arrow item) {
                if (ReferenceEquals(CCPlayerData.equipments.GetMainArrow(), item) == true) {
                    CCPlayerData.equipments.ReleaseItem_MainArr();
                }
                else {
                    CCPlayerData.equipments.ReleaseItem_SubArr();
                }
                Close();
            }

            void EventEntryEquipArtifact(Item_Accessory item) {
                UI_Equipments.Instance.OpenChoosePanel(SlotChoosePop.SLOTPANELTYPE.SLOT_ACCESSORY, item);
                Close();
            }

            void EventEntryReleaseArtifact(byte idx) {
                CCPlayerData.equipments.ReleaseItem_Artifact(idx);
                Close();
            }

            void EventEntryEquipSpArr(Item_SpArr item) {
                CCPlayerData.equipments.Equip_SpArrow(item);
                Close();
            }

            void EventEntryReleaseSpArr() {
                CCPlayerData.equipments.Release_SpArrow();
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
                abilitySlots.DisableAllSlots();
            }

            #endregion

            #region CLOSE_POPUP

            /// <summary>
            /// 프리뷰 팝업이 아닐 때만 이걸로 비활성화 해줌.
            /// </summary>
            public void Close() {
                if (!isPreviewOpenMode) {
                    //Update Inventory, Equipments UI Only Normal Mode
                    UI_Equipments.Instance.UpdateEquipUI();
                    UI_Inventory.InvenUpdate();
                }

                //Close Self
                DisablePopup();
                parent.gameObject.SetActive(false);
            }

            #endregion
        }

        [Header("INFO")]
        [SerializeField] ItemPopupIntegrated itemPopup; //통합 아이템 팝업
        [SerializeField] Sprite[] gradeStarSprites = null;

        [Header("SLOT FRAME")]
        public Sprite[] Frames;

        [Header("ONLY UPGRADE INFO")] [Tooltip("Only Assign Upgrade Item Info Panel")]
        [SerializeField] GameObject btn_Select = null; //업그레이드 아이템 선택 패널에서만 할당해줌

        /*
            index[0] Material Item, Consumable Item
            index[1] Equipment Item (Non-Skill)
            index[2] Equipment Item (One-SKill)
            index[3] Equipment Item (Two-Skill)
            index[4] Accessory Item ()
        */

        private void Awake() {
            itemPopup.Init(gradeStarSprites);
        }

        #region OPEN_ITEM_INFO

        public void OpenPopup(Item_Material item) {
            gameObject.SetActive(true);
            itemPopup.EnablePopup(item, Frames[(int)item.GetGrade]);
        }

        public void OpenPopup(Item_Consumable item) {
            gameObject.SetActive(true);
            itemPopup.EnablePopup(item, Frames[(int)item.GetGrade]);
        }

        public void OpenPopup(Item_Equipment equipItem) {
            gameObject.SetActive(true);
            switch (equipItem) {
                case Item_Bow       equipment: itemPopup.EnablePopup(equipment, Frames[(int)equipment.GetGrade]); break;
                case Item_SpArr     equipment: itemPopup.EnablePopup(equipment, Frames[(int)equipment.GetGrade]); break;
                case Item_Arrow     equipment: itemPopup.EnablePopup(equipment, Frames[(int)equipment.GetGrade]); break;
                case Item_Accessory equipment: itemPopup.EnablePopup(equipment, Frames[(int)equipment.GetGrade]); break;
            }
        }

        /// <summary>
        /// CRAFTING PANEL의 완성품 프리뷰 기능
        /// [MainScene.Route의 InfoPopup을 사용] 
        /// </summary>
        /// <param name="item"></param>
        public void OpenPreview_Crafting(AD_item item, bool disableAmountText = false) {
            var frame = Frames[(int)item.GetGrade];
            switch (item.GetItemType) {
                case ITEMTYPE.ITEM_MATERIAL:   
                    itemPopup.EnablePopup((Item_Material)item, frame, false, true);   
                    if (disableAmountText) {
                        itemPopup.DisableAmountText();
                    }
                    break;
                case ITEMTYPE.ITEM_CONSUMABLE: 
                    itemPopup.EnablePopup((Item_Consumable)item, frame, false, true); 
                    if (disableAmountText) {
                        itemPopup.DisableAmountText();
                    }
                    break;
                case ITEMTYPE.ITEM_EQUIPMENT:
                    switch (item) {
                        case Item_Bow       equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                        case Item_SpArr     equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                        case Item_Arrow     equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                        case Item_Accessory equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                        default: throw new System.NotImplementedException();
                    }
                    break;
                default: throw new System.NotImplementedException();
            }

            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// 업그레이드 가능 아이템 팝업 
        /// [UpgradeFunction의 독자적인 InfoPopup을 사용]
        /// </summary>
        /// <param name="equipItem"></param>
        public void OpenPopup_Upgradeable(Item_Equipment equipItem) {
            var frame = Frames[(int)equipItem.GetGrade];
            switch (equipItem) {
                case Item_Bow       equipment: itemPopup.EnablePopup(equipment, frame, true, true); break;
                case Item_SpArr     equipment: itemPopup.EnablePopup(equipment, frame, true, true); break;
                case Item_Arrow     equipment: itemPopup.EnablePopup(equipment, frame, true, true); break;
                case Item_Accessory equipment: itemPopup.EnablePopup(equipment, frame, true, true); break;
                default: throw new System.NotImplementedException();
            }

            btn_Select.SetActive(true);
        }

        /// <summary>
        /// UPGRADE PANEL의 완성품 프리뷰 기능
        /// [UpgradeFunction의 독자적인 InfoPopup을 사용]
        /// </summary>
        /// <param name="previewItem"></param>
        /// <param name="enableSelectButton"></param>
        public void OpenPreview_Upgrade(Item_Equipment previewItem, bool enableSelectButton) {
            var frame = Frames[(int)previewItem.GetGrade];
            switch (previewItem) {
                case Item_Bow       equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                case Item_Arrow     equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                case Item_Accessory equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                case Item_SpArr     equipment: itemPopup.EnablePopup(equipment, frame, false, true); break;
                default: throw new System.NotImplementedException();
            } //Preview Open은 항상 모든 버튼을 숨김처리
            
            if (!gameObject.activeSelf) {
                gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Never Use Preview Item Popup
        /// </summary>
        public void Close() {
            itemPopup.Close();
        }

        #endregion
    }
}
