﻿namespace ActionCat {
    using UnityEngine;
    using ActionCat.Data;
    using UnityEngine.Events;

    /// <summary>
    /// 플레이어 데이터 교환 및 게임의 전반적인 흐름을 담당하는 Singleton Class
    /// </summary>
    public class GameManager : Singleton<GameManager> {
        public enum GAMEPLATFORM {
            PLATFORM_EDITOR = 0,
            PLATFORM_STANDALONE = 1,
            PLATFORM_MOBILE = 2
        }

        // FIELDS
        private float fixedDeltaTime;
        private float totalDropChances;
        private float restoreTimeScale;
        private bool isManagerInitialized = false;
        private bool isShowedAds = false;
        private ItemDropList.DropTable[] dropListArray = null;
        private SavingFeedback savingFeedbackPanel = null;
        private InAppUpdateSupport inAppUpdateSupport = null;

        // PROPERTIES
        public GAMEPLATFORM PlayPlatform { get; private set; }
        public bool IsDevMode { get; private set; } = false;
        public bool IsInitialized {
            get => isManagerInitialized;
        }
        public bool IsGameStateEnd {
            get {
                if (this.GameState == GAMESTATE.STATE_GAMEOVER || this.GameState == GAMESTATE.STATE_ENDBATTLE) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        public bool IsBattleState {
            get {
                return (this.GameState == GAMESTATE.STATE_INBATTLE || this.GameState == GAMESTATE.STATE_BOSSBATTLE);
            }
        }
        public bool? IsAppUpdateAvailable {
            get; private set;
        } = null;
        public string LanguageCode {
            get {
                string languageCode = CCPlayerData.settings.LanguageCode;
                return (!string.IsNullOrEmpty(languageCode)) ? languageCode : I2.Loc.LocalizationManager.CurrentLanguageCode;
            }
        }
        public Player_Equipments PlayerEquips {
            get {
                return CCPlayerData.equipments;
            }
        }
        public AD_Inventory PlayerInven {
            get {
                return CCPlayerData.inventory;
            }
        }
        public PULLINGTYPE GetPlayerPullType() {
            return CCPlayerData.settings.PullingType;
        }

        #region SETTINGS

        public void SetControlType(bool isChangeType) {
            CCPlayerData.settings.SetPullType(isChangeType);
        }

        public bool IsControlTypeChanged() {
            var controller = GetControllerInstOrNull();
            if (controller == null) {
                CatLog.WLog("Not Found Bow Controller this Scene.");
                return false;
            }

            return (controller.ControlType != CCPlayerData.settings.PullingType);
        }


        /// <summary>
        /// 언어 코드 변경
        /// </summary>
        /// <param name="languageCode"></param>
        public void ChangeLanguageCode(string languageCode) {
            if (string.Equals(languageCode, I2.Loc.LocalizationManager.CurrentLanguageCode)) {
                // 변경한 언어가 동일한 경우는 업데이트 하지않음
                return;
            }

            // 언어변경 후 코드 저장
            I2.Loc.LocalizationManager.CurrentLanguageCode = languageCode;
            CCPlayerData.settings.SetLanguageCode(languageCode);
        }

        #endregion

        #region SCREEN

        /// <summary>
        /// Rect Set of Target Camera with a  9 : 16 Portrait Resolition
        /// It must be called from the Awake function in the Script.
        /// </summary>
        /// <param name="targetCam"></param>
        public void ResolutionPortrait(Camera targetCam) {
            Rect rect = targetCam.rect;
            float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16); // (Width / Height)
            float scaleWidth = 1f / scaleHeight;
            if (scaleHeight < 1) {
                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else {
                rect.width = scaleHeight;
                rect.x = (1f - scaleWidth) / 2f;
            }
            targetCam.rect = rect;
        }

        #endregion

        #region PLAYER_DATA

        public void InitEquips(Transform bowInitPos, Transform bowParent, int mainArrPoolQuantity, int subArrPoolQuantity,
                               out UI.ArrSSData[] arrSlotData, out AccessorySPEffect[] artifactEffects, UI.SwapSlots swapSlot, out Sprite[] artifactIcons) {
            CCPlayerData.equipments.SetupEquipments(bowInitPos, bowParent, mainArrPoolQuantity, subArrPoolQuantity, swapSlot);
            arrSlotData = GetArrSwapSlotData();
            artifactEffects = GetArtifactEffects(out artifactIcons);
        }

        public void SetBowManualControl(bool isStop) {
            if (AD_BowController.instance != null) {
                AD_BowController.instance.IsStopManualControl = isStop;
            }
        }

        public ARROWTYPE GetFirstArrType() {
            if (CCPlayerData.equipments.IsEquippedArrMain) return ARROWTYPE.ARROW_MAIN;
            else if (CCPlayerData.equipments.IsEquippedArrSub) return ARROWTYPE.ARROW_SUB;
            else throw new System.Exception("There are no Arrow Equipped.");
        }

        public void InitArrowSlotData(out bool slot_m, out bool slot_s,
                                      out Sprite arrowIconSprite_m, out Sprite arrowIconSprite_s) {
            slot_m = (CCPlayerData.equipments.IsEquippedArrMain) ? true : false;
            slot_s = (CCPlayerData.equipments.IsEquippedArrSub) ? true : false;

            if (slot_m) arrowIconSprite_m = CCPlayerData.equipments.GetMainArrow().GetSprite;
            else arrowIconSprite_m = null;
            if (slot_s) arrowIconSprite_s = CCPlayerData.equipments.GetSubArrow().GetSprite;
            else arrowIconSprite_s = null;
        }

        UI.ArrSSData[] GetArrSwapSlotData() {
            var equipment = CCPlayerData.equipments;
            var slotDataList = new System.Collections.Generic.List<UI.ArrSSData>();
            bool isEquippedMainArr = equipment.IsEquippedArrMain;
            if (isEquippedMainArr) slotDataList.Add(new UI.ArrSSData(isEquippedMainArr, equipment.GetMainArrow().GetSprite, GetControllerInstOrNull().Swap));
            else slotDataList.Add(new UI.ArrSSData(isEquippedMainArr));
            bool isEquippedSubArr = equipment.IsEquippedArrSub;
            if (isEquippedSubArr) slotDataList.Add(new UI.ArrSSData(isEquippedSubArr, equipment.GetSubArrow().GetSprite, GetControllerInstOrNull().Swap));
            else slotDataList.Add(new UI.ArrSSData(isEquippedSubArr));
            bool isEquippedSpArr = equipment.IsEquippedSpArr;
            if (isEquippedSpArr) slotDataList.Add(new UI.ArrSSData(isEquippedSpArr, equipment.GetSpArrOrNull.GetSprite, GetControllerInstOrNull().Swap));
            else slotDataList.Add(new UI.ArrSSData(isEquippedSpArr));
            return slotDataList.ToArray();
        }

        /// <summary>
        /// 발동형 스킬만 골라서 반환
        /// </summary>
        /// <returns></returns>
        AccessorySPEffect[] GetArtifactEffects(out Sprite[] sprites) {
            var effectTempList = new System.Collections.Generic.List<AccessorySPEffect>();
            var tempSpriteList = new System.Collections.Generic.List<Sprite>();
            var artifacts = CCPlayerData.equipments.GetArtifacts();
            for (int i = 0; i < artifacts.Length; i++) {
                var effect = (artifacts[i] != null && artifacts[i].IsExistEffect) ? artifacts[i].SPEffectOrNull : null;
                if (effect != null) {
                    switch (effect.SpEffectType) {
                        //UI가 필요한 Artifact는 effectList에 담아줄 것
                        case ACSP_TYPE.SPEFFECT_NONE: break;
                        case ACSP_TYPE.SPEFFECT_AIMSIGHT: break; // PASSIVE
                        case ACSP_TYPE.SPEEFECT_SLOWTIME: effectTempList.Add(effect); tempSpriteList.Add(artifacts[i].GetSprite); break; // BUFF
                        case ACSP_TYPE.CURE: effectTempList.Add(effect); tempSpriteList.Add(artifacts[i].GetSprite); break; // BUFF
                        case ACSP_TYPE.CURSE_SLOW: effectTempList.Add(effect); tempSpriteList.Add(artifacts[i].GetSprite); break; // DEBUFF
                        default: throw new System.NotImplementedException();
                    }
                }
            }

            sprites = tempSpriteList.ToArray();
            return effectTempList.ToArray();
        }

        public AD_BowController GetControllerInstOrNull() {
            if (AD_BowController.instance != null)
                return AD_BowController.instance;
            else
                return null;
        }

        public bool TryGetCloneSkillSet(string tag, out ArrowSkillSet cloneSet) {
            if (tag.Equals(AD_Data.POOLTAG_MAINARROW) || tag.Equals(AD_Data.POOLTAG_MAINARROW_LESS)) {
                //TryGet Main Arrow Set.
                if (PlayerEquips.GetMainArrow().TryGetSkillSet(out ArrowSkillSet originSet) == true) {
                    cloneSet = originSet.GetClone();
                    return true;
                }
            }
            else if (tag.Equals(AD_Data.POOLTAG_SUBARROW) || tag.Equals(AD_Data.POOLTAG_SUBARROW_LESS)) {
                //TryGet Sub Arrow Set.
                if (PlayerEquips.GetSubArrow().TryGetSkillSet(out ArrowSkillSet originSet) == true) {
                    cloneSet = originSet.GetClone();
                    return true;
                }
            }
            else if (tag.Equals(AD_Data.POOLTAG_SPECIAL_ARROW)) {
                //TryGet Special Arrow Set.
                if (PlayerEquips.GetSpArrow().TryGetSkillSet(out ArrowSkillSet originSet) == true) {
                    cloneSet = originSet.GetClone();
                    return true;
                }
            }
            else {
                CatLog.ELog($"Not Implemented this Arrow Tag. (Input Tag: {tag})");
                cloneSet = null;
                return false;
            }

            //Not Exist SkillSet. (return false)
            cloneSet = null;
            return false;
        }

        public void ReleaseEquipments() {
            CCPlayerData.equipments.ReleaseEquipments();
        }

        public AD_item[] GetBluePrints(BLUEPRINTTYPE type) {
            switch (type) {
                case BLUEPRINTTYPE.ALL: return CCPlayerData.inventory.GetAllBluePrints();
                default: return CCPlayerData.inventory.GetBluePrints(type);
            }
        }

        public AD_item[] FindUpgradeableItems(string[] keys) {
            return CCPlayerData.inventory.GetUpgradeableItems(keys);
        }

        public bool TryGetItemAmount(string targetId, out int amount) {
            return CCPlayerData.inventory.TryGetAmount(targetId, out amount);
        }

        public bool FindItemByRef(AD_item target) {
            return CCPlayerData.inventory.IsExist(target);
        }

        public bool TryRemoveItem(string itemid, int removeAmount) {
            return CCPlayerData.inventory.RemoveItem(itemid, removeAmount);
        }

        public bool TryRemoveItem(AD_item itemref) {
            if (CCPlayerData.inventory.IsExist(itemref)) {
                CCPlayerData.inventory.RemoveItem(itemref);
                return true;
            }
            else {
                return false;
            }
        }

        public void AddItem(ItemData entity, int amount) {
            CCPlayerData.inventory.AddItem(entity, amount);
        }

        public int GetCraftingInfoSize() {
            return CCPlayerData.infos.CraftSlotSize;
        }

        public bool IsAvailableCraftingSlot(int slotNumber) {
            var craftingSlot = CCPlayerData.infos.CraftingInfos[slotNumber];
            if (craftingSlot == null || !craftingSlot.IsAvailable) {
                return false;
            }

            return true;
        }

        public CraftingInfo[] GetCraftingInfos() {
            return CCPlayerData.infos.CraftingInfos;
        }

        public CraftingInfo GetCraftingInfo(int index) {
            return CCPlayerData.infos.CraftingInfos[index];
        }

        public void CraftingStart(int index, CraftingRecipe recipe) {
            CCPlayerData.infos.CraftingStart(index, recipe);
        }

        public bool TryReceipt(int slotNumber, out ItemData resultItemRef, out int resultItemAmount) {
            return CCPlayerData.infos.CraftingInfos[slotNumber].TryReceipt(out resultItemRef, out resultItemAmount);
        }

        public GlobalAbility GetGoAbility() {
            return CCPlayerData.ability.GlobalAbilityField;
        }

        public int[] GetPlayerCurrencies {
            get {
                return new int[2] { CCPlayerData.infos.Gold, CCPlayerData.infos.Stone };
            }
        }

        public string[] GetPlayerCurrenciesToString {
            get {
                return new string[2] {
                    CCPlayerData.infos.Gold.ToString("#,##0"),
                    CCPlayerData.infos.Stone.ToString("#,##0")
                };
            }
        }

        public int PlayerGold {
            get {
                return CCPlayerData.infos.Gold;
            }
        }

        public int PlayerStone {
            get {
                return CCPlayerData.infos.Stone;
            }
        }

        public bool TryDeleteSaveJson(out string message) {
            return CCPlayerData.TryDeleteJson(out message);
        }

        public void InitPlayerData() {
            CCPlayerData.Initialize();
        }

        public bool IsReady2BattleStart(out string log) {
            if (!CCPlayerData.equipments.IsEquippedBow) {
                log = I2.Loc.ScriptLocalization.ErrorLog.NoEquippedBow;
                return false;
            }

            if (!CCPlayerData.equipments.IsEquippedArrMain && !CCPlayerData.equipments.IsEquippedArrSub) {
                log = I2.Loc.ScriptLocalization.ErrorLog.NoEquippedArrow;
                return false;
            }

            log = "";
            return true;
        }

        #endregion

        #region SAVE_LOAD
        public void AutoLoadUserData() {
            throw new System.Exception();
            ///Original Codes
            ///if (isLoadedUserData == false) {
            ///    LoadUserJsonFile(out string log, out bool recommendCreateNewJson);
            ///}
            ///isLoadedUserData = true;
        }

        public bool LoadUserJsonFile(out string log, out bool recommendCreateNewJson) {
            bool isSuccessLoadJson = CCPlayerData.LoadUserDataJson(out byte logCode);
            if (!isSuccessLoadJson) {
                switch (logCode) {
                    case 1: log = $"UserData Json Not Exist. [CODE: {logCode}]"; recommendCreateNewJson = true; return false;
                    case 2: log = $"Inventory Key Error. [CODE: {logCode}]"; recommendCreateNewJson = false; return false;
                    case 3: log = $"Equipment Key Error. [CODE: {logCode}]"; recommendCreateNewJson = false; return false;
                    case 4: log = $"Information Key Error. [CODE: {logCode}]"; recommendCreateNewJson = false; return false;
                    default: log = $"Unknown Error. [CODE: {logCode}]"; recommendCreateNewJson = false; return false;
                }
                //또 다른 에러. 에러의 종류에 따라 에러팝업이 뜨고, 어떻게 조치할 지 User에게 선택 제시, 또는 안내.
            }
            else {
                log = "NO LOG";
                recommendCreateNewJson = false;
                return true;
            }
        }

        public bool SaveUserJsonFile() {
            this.TryPlayFeedback();
            return CCPlayerData.SaveUserDataJson();
        }

        public void SupplyInitItem() {
            CCPlayerData.SupplyInitItem();
        }

        /// <summary>
        /// Save Settings는 Data타고 CCPlayerData에 직접 요청하기 (rs. 불안정
        /// </summary>
        private void SaveSettings() => CCPlayerData.SaveSettingsJson();

        public void LoadSettings() {
            CCPlayerData.LoadSettingsJson(out string log);
            CatLog.WLog(log);
        }

        #endregion

        #region IN_APP_UPDATE_HANDLER

        public void CheckAppUpdateAvailable() {
            StartCoroutine(AppUpdateAvailableCheckCo());
        }

        System.Collections.IEnumerator AppUpdateAvailableCheckCo() {
            inAppUpdateSupport = new InAppUpdateSupport();
            inAppUpdateSupport.Init();

            // 업데이트 체크 시작
            yield return StartCoroutine(inAppUpdateSupport.CheckForUpdate());

            // 가능한 업데이트가 존재하는지 받아옴
            IsAppUpdateAvailable = inAppUpdateSupport.IsUpdateAvailable();

            // 바로 사용가능한 업데이트가 존재하는 경우 인-앱 API를 사용한 팝업 띄워줌
            if (IsAppUpdateAvailable.Value) {
                CatLog.Log("사용 가능한 업데이트가 존재합니다.");
                yield return StartCoroutine(inAppUpdateSupport.StartImmediateUpdate());
            }
            else {
                CatLog.Log("사용 가능한 업데이트가 존재하지 않거나 오류가 발생했습니다.");
            }

            // 사용가능한 업데이트가 없거나 업데이트가 취소된 경우 (ex.유저 취소. 오류 취소)
            CatLog.Log("Clear In-App Supporter.");
            inAppUpdateSupport = null;

            // 현재는 Update Supporter를 Clear 해주고 있지만, 추후에 IsExistAppUpdate 변수에 따라
            // 메인화면에서 바로 업데이트로 이어지는 버튼을 추가하는것도 좋을 것 같다.
        }

        #endregion

        #region BATTLE

        public void ResumeBattle() {
            SetBowManualControl(false);
            TimeToRestore();
        }

        public void PauseBattle() {
            SetBowManualControl(true);
            TimeToPause();
        }

        /// <summary>
        /// Never Use this Method.
        /// </summary>
        /// <param name="trigger"></param>
        public void PreventionPulling(UnityEngine.EventSystems.EventTrigger trigger) {
            PullingPreventionDown(trigger);
            PullingPreventionUp(trigger);
        }

        /// <summary>
        /// Never Use this Method.
        /// </summary>
        /// <param name="trigger"></param>
        void PullingPreventionDown(UnityEngine.EventSystems.EventTrigger trigger) {
            //Bow Controller Pulling Limit [Button Down Event]
            UnityEngine.EventSystems.EventTrigger.Entry m_slotEntryDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            m_slotEntryDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            m_slotEntryDown.callback.AddListener((data) => SetBowManualControl(true));
            trigger.triggers.Add(m_slotEntryDown);
        }

        /// <summary>
        /// Never Use this Method.
        /// </summary>
        /// <param name="trigger"></param>
        void PullingPreventionUp(UnityEngine.EventSystems.EventTrigger trigger) {
            //Bow Controller Pulling Limit [Button Down Event]
            UnityEngine.EventSystems.EventTrigger.Entry m_slotEntryUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            m_slotEntryUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            m_slotEntryUp.callback.AddListener((data) => SetBowManualControl(false));
            trigger.triggers.Add(m_slotEntryUp);
        }

        public bool UpdateStageData(string stagekey, in BattleData data) {
            return CCPlayerData.infos.UpdateStageInfo(stagekey, in data);
        }

        public void UpdateCraftingInfo() {
            CCPlayerData.infos.UpdateCraftingInfo();
        }

        public bool TryGetStageData(string key, out StageInfo stageInfo) {
            return CCPlayerData.infos.TryGetStageData(key, out stageInfo);
        }

        public Data.StageData.StageSetting GetStageSetting(string key) {
            return CCPlayerData.settings.GetStageSetting(key);
        }

        public bool TryGetStageSetting(string stagekey, out Data.StageData.StageSetting setting) {
            return CCPlayerData.settings.TryGetStageSetting(stagekey, out setting);
        }

        public void AutoSwitch(bool isDebug = false) {
            if (GetControllerInstOrNull() == null) {
                CatLog.ELog("The Controller is Null."); return;
            }

            GetControllerInstOrNull().AutoSwitch(isDebug);
        }

        #endregion

        #region TIME-CONTROL

        public void TimeScaleSet(float targetTimeScaleVal) {
            if (targetTimeScaleVal == Time.timeScale) {
                CatLog.WLog("Value equal to the current TimeScale");
                return;
            }
            else {
                Time.timeScale = targetTimeScaleVal;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }
        }

        public void TimeToDefault() {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public void TimeToPause() {
            restoreTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public void TimeToRestore() {
            Time.timeScale = restoreTimeScale;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public bool IsTimeDefault() {
            bool isTimeDefault = (Time.timeScale == 1f) ? true : false;
            return isTimeDefault;
        }

        #endregion

        #region ITEM-DROP

        public void InitialDroplist(ItemDropList newDropList) {
            dropListArray = newDropList.DropTableArray;

            //Init Total Item Drop Chance
            totalDropChances = 0f;
            foreach (var item in dropListArray) {
                totalDropChances += item.DropChance;
            }
        }

        public void ReleaseDropList() {
            dropListArray = null;
            totalDropChances = 0f;

            CatLog.Log("Release Drop List");
        }

        public bool OnItemDropRoll(float stageDropRate, float monsterDropRate) {
            return (stageDropRate + monsterDropRate >= GameGlobal.GetItemDropRollChance()) ? true : false;
        }

        public DropItem OnItemDrop() {
            float randomPoint = Random.value * totalDropChances;

            for (int i = 0; i < dropListArray.Length; i++) {
                if (randomPoint < dropListArray[i].DropChance) {
                    return new DropItem(dropListArray[i].DefaultQuantity, dropListArray[i].ItemAsset);
                }
                else {
                    randomPoint -= dropListArray[i].DropChance;
                }
            }

            // Last index Item
            var lastIndexItem = dropListArray[dropListArray.Length - 1];
            return new DropItem(lastIndexItem.DefaultQuantity, lastIndexItem.ItemAsset);

            #region GET_LOW_CAHNCE_ITEM
            //var minimunChanceOfItem = dropListArray[0];
            //
            //for (int i = 0; i < dropListArray.Length; i++)
            //{
            //    if (minimunChanceOfItem.DropChance > dropListArray[i].DropChance)
            //    {
            //        //이거 만약에 가장 낮은 확률의 아이템이 두개라면 어떻게 되는지
            //        minimunChanceOfItem = dropListArray[i];
            //    }
            //}
            //
            //DropItem item = new DropItem(GameGlobal.RandomIntInArray(minimunChanceOfItem.QuantityRange), minimunChanceOfItem.ItemAsset);
            //return item;
            #endregion
        }

        #endregion

        #region BATTLE_STATE_EVENT

        // GameState Enum Variables
        public GAMESTATE GameState { get; private set; } = GAMESTATE.STATE_NONE;

        //Game Event Delegate
        public delegate void BattleStateChangeCallback();
        BattleStateChangeCallback OnStateInBattle;
        BattleStateChangeCallback OnStateEndBattle;
        BattleStateChangeCallback OnStateGameOver;
        BattleStateChangeCallback OnStatePause;

        /// <summary>
        /// Change Current Game State
        /// </summary>
        /// <param name="targetState"></param>
        public void ChangeGameState(GAMESTATE targetState) {
            GameState = targetState; //Change Current GameState
            switch (GameState) {     //Activate Event
                case GAMESTATE.STATE_BEFOREBATTLE:                          break; 
                case GAMESTATE.STATE_INBATTLE:  OnStateInBattle?.Invoke();  break;
                case GAMESTATE.STATE_BOSSBATTLE:                            break; 
                case GAMESTATE.STATE_ENDBATTLE: OnStateEndBattle?.Invoke(); break;
                case GAMESTATE.STATE_GAMEOVER:  OnStateGameOver?.Invoke();  break;
                case GAMESTATE.STATE_PAUSE:     OnStatePause?.Invoke();     break;
                case GAMESTATE.STATE_NONE:                                  break;
                default: CatLog.ELog("Not Implemented this State");         break;
            }
        }

        public void AddListnerEndBattle(System.Action action) => OnStateEndBattle += new BattleStateChangeCallback(action);

        public void AddListnerGameOver(System.Action action) => OnStateGameOver += new BattleStateChangeCallback(action);

        public void AddListnerInBattle(System.Action action) => OnStateInBattle += new BattleStateChangeCallback(action);

        public void AddListnerPause(System.Action pauseAction) => OnStatePause += new BattleStateChangeCallback(pauseAction);

        public void ReleaseAllEventsWithNoneState() {
            //Release Battle State
            OnStateEndBattle = null;
            OnStateGameOver = null;
            OnStateInBattle = null;

            //Release Pause Battle Delegates
            if (OnStatePause != null) {
                foreach (System.Delegate findDelegate in OnStatePause.GetInvocationList()) {
                    OnStatePause -= (BattleStateChangeCallback)findDelegate;
                }
            }

            //Restore GameState to NONE
            ChangeGameState(GAMESTATE.STATE_NONE);
        }

        public BattleStateChangeCallback EventBattleEnd() {
            return this.OnStateEndBattle;
        }

        public BattleStateChangeCallback EventGameOver() {
            return this.OnStateGameOver;
        }

        public BattleStateChangeCallback EventInBattle() {
            return this.OnStateInBattle;
        }

        public BattleStateChangeCallback EventPause() {
            return this.OnStatePause;
        }

        #endregion

        public void Initialize() {
            if (isManagerInitialized == true) { //이미 이니셜 했으면 넘김
                return;
            }
#if UNITY_EDITOR
            PlayPlatform = GAMEPLATFORM.PLATFORM_EDITOR;
            IsDevMode = false;
#elif UNITY_STANDALONE
            PlayPlatform = GAMEPLATFORM.PLATFORM_STANDALONE;
            IsDevMode    = false;
#elif UNITY_ANDROID
            PlayPlatform = GAMEPLATFORM.PLATFORM_MOBILE;
            IsDevMode    = false;

            // 앱-업데이트 체크
            //StartCoroutine(CheckAppUpdate());
#endif
            fixedDeltaTime = Time.fixedDeltaTime;
            LoadSettings();
#if !UNITY_EDITOR && UNITY_ANDROID
            ChangeLanguageCode(CCPlayerData.settings.LanguageCode);
#endif
            isManagerInitialized = true;
        }

        public void SetSavingFeedback(SavingFeedback feedbackPanel) {
            if (!feedbackPanel) {
                CatLog.WLog("null인 SavingFeedback 인스턴스가 Set시도되었습니다.");
                return;
            }
            if (this.savingFeedbackPanel) {
                CatLog.WLog("GameManager의 SavingFeedback Panel이 덮어씌워졌습니다.");
            }
            this.savingFeedbackPanel = feedbackPanel;
        }

        private void ReleaseFeedbackPanel() {
            if (this.savingFeedbackPanel) {
                this.savingFeedbackPanel = null;
            }
        }

        private void TryPlayFeedback() {
            if (this.savingFeedbackPanel) {
                this.savingFeedbackPanel.Play();
            }
        }

        private void Start() {
            //Apply Settings Volume Value
            SoundManager.Instance.Initialize();

            //AddEvent to Scene Change Callback
            SceneLoader.SceneChangeCallback += this.ReleaseFeedbackPanel;
        }

        private void OnDisable() {
            SceneLoader.SceneChangeCallback -= this.ReleaseFeedbackPanel;
        }

        /// <summary>
        /// 게임이 실행되고 1회에 한해 테스트 성 광고를 재생함
        /// </summary>
        public void ShowInterstitialAdsOnce() {
            // 해당 버전은 테스트 목적으로 게임 중 1회에 한해 광고를 송출
            if (!isShowedAds && AdsManager.Instance.IsReadyInterstitialAds()) {
                AdsManager.Instance.ShowInterstitialAds();
                isShowedAds = true;
            }
        }

        public void SetLanguageCode(string languageCode) {
            // 언어코드 변경
            CCPlayerData.settings.SetLanguageCode(languageCode);
        }
    }
}
