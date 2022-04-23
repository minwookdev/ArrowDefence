namespace ActionCat {
    using UnityEngine;
    using ActionCat.Data;

    public class GameManager : Singleton<GameManager> {
        public enum GAMEPLATFORM { 
            PLATFORM_EDITOR     = 0,
            PLATFORM_STANDALONE = 1,
            PLATFORM_MOBILE     = 2
        }
        
        //FIELDS
        private float fixedDeltaTime;
        private float totalDropChances;
        private float restoreTimeScale;
        private bool isLoadedUserData     = false;
        private bool isManagerInitialized = false;
        private ItemDropList.DropTable[] dropListArray;

        //PROPERTIES
        public GAMEPLATFORM PlayPlatform { get; private set; }
        public GAMESTATE GameState { get; private set; } = GAMESTATE.STATE_NONE;
        public bool IsDevMode { get; private set; }


        //Game Event Delegate
        public delegate void GameEventHandler();
        GameEventHandler OnStateEndBattle;
        GameEventHandler OnStateGameOver;

        public void Initialize() {
            if (isManagerInitialized == true) return;
#if UNITY_EDITOR
            PlayPlatform = GAMEPLATFORM.PLATFORM_EDITOR;
            IsDevMode    = true;
#elif UNITY_STANDALONE
            PlayPlatform = GAMEPLATFORM.PLATFORM_STANDALONE;
            IsDevMode    = false;
#elif UNITY_ANDRIOD
            PlayPlatform = GAMEPLATFORM.PLATFORM_MOBILE;
            IsDevMode    = false;
#endif
            fixedDeltaTime = Time.fixedDeltaTime;
            isManagerInitialized  = true;
        }

        #region SCREEN

        /// <summary>
        /// Rect Set of Target Camera with a  9 : 16 Portrait Resolition
        /// It must be called from the Awake function in the Script.
        /// </summary>
        /// <param name="targetCam"></param>
        public void ResolutionPortrait(Camera targetCam)
        {
            Rect rect = targetCam.rect;
            float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16); // (Width / Height)
            float scaleWidth = 1f / scaleHeight;
            if(scaleHeight < 1)
            {
                rect.height = scaleHeight;
                rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                rect.width = scaleHeight;
                rect.x = (1f - scaleWidth) / 2f;
            }
            targetCam.rect = rect;
        }

#endregion

#region PLAYER_DATA

        public void InitEquips(Transform bowInitPos, Transform bowParent, int mainArrPoolQuantity, int subArrPoolQuantity, 
                               out UI.ArrSSData[] arrSlotData, out AccessorySPEffect[] artifactEffects, UI.SwapSlots swapSlot) {
            CCPlayerData.equipments.InitEquipments(bowInitPos, bowParent, mainArrPoolQuantity, subArrPoolQuantity, swapSlot);
            arrSlotData     = GetArrSwapSlotData();
            artifactEffects = GetArtifactEffects();
        }

        public void SetBowPullingStop(bool isStop) {
            if (AD_BowController.instance != null)
                AD_BowController.instance.IsPullingStop = isStop;
            else
                CatLog.WLog("Controller Not Found.");
        }

        public ARROWTYPE GetFirstArrType() {
            if (CCPlayerData.equipments.IsEquippedArrowMain())     return ARROWTYPE.ARROW_MAIN;
            else if (CCPlayerData.equipments.IsEquippedArrowSub()) return ARROWTYPE.ARROW_SUB;
            else throw new System.Exception("There are no Arrow Equipped.");
        }

        public void InitArrowSlotData(out bool slot_m, out bool slot_s, 
                                      out Sprite arrowIconSprite_m, out Sprite arrowIconSprite_s) {
            slot_m = (CCPlayerData.equipments.IsEquippedArrowMain()) ? true : false;
            slot_s = (CCPlayerData.equipments.IsEquippedArrowSub())  ? true : false;

            if (slot_m) arrowIconSprite_m = CCPlayerData.equipments.GetMainArrow().GetSprite;
            else        arrowIconSprite_m = null;
            if (slot_s) arrowIconSprite_s = CCPlayerData.equipments.GetSubArrow().GetSprite;
            else        arrowIconSprite_s = null;
        }

        UI.ArrSSData[] GetArrSwapSlotData() {
            var equipment     = CCPlayerData.equipments;
            var slotDataList  = new System.Collections.Generic.List<UI.ArrSSData>();
            bool isEquippedMainArr = equipment.IsEquippedArrowMain();
            if (isEquippedMainArr) slotDataList.Add(new UI.ArrSSData(isEquippedMainArr, equipment.GetMainArrow().GetSprite, GetControllerInstOrNull().Swap));
            else                   slotDataList.Add(new UI.ArrSSData(isEquippedMainArr));
            bool isEquippedSubArr = equipment.IsEquippedArrowSub();
            if (isEquippedSubArr)  slotDataList.Add(new UI.ArrSSData(isEquippedSubArr, equipment.GetSubArrow().GetSprite, GetControllerInstOrNull().Swap));
            else                   slotDataList.Add(new UI.ArrSSData(isEquippedSubArr));
            bool isEquippedSpArr = equipment.IsEquippedSpArr;
            if (isEquippedSpArr)   slotDataList.Add(new UI.ArrSSData(isEquippedSpArr, equipment.GetSpArrOrNull.GetSprite, GetControllerInstOrNull().Swap));
            else                   slotDataList.Add(new UI.ArrSSData(isEquippedSpArr));
            return slotDataList.ToArray();
        }

        /// <summary>
        /// 발동형 스킬만 골라서 반환
        /// </summary>
        /// <returns></returns>
        AccessorySPEffect[] GetArtifactEffects() {
            var effectTempList = new System.Collections.Generic.List<AccessorySPEffect>();
            var artifacts = CCPlayerData.equipments.GetAccessories();
            for (int i = 0; i < artifacts.Length; i++) {
                var effect = (artifacts[i] != null && artifacts[i].IsExistEffect) ? artifacts[i].SPEffectOrNull : null;
                if (effect != null) {
                    switch (effect.SpEffectType) {
                        case ACSP_TYPE.SPEFFECT_NONE:     break;
                        case ACSP_TYPE.SPEFFECT_AIMSIGHT: break;
                        case ACSP_TYPE.SPEEFECT_SLOWTIME: effectTempList.Add(effect); break;
                        default: throw new System.NotImplementedException();
                    }
                }
            }
            return effectTempList.ToArray();
        }

        public AD_BowController GetControllerInstOrNull() {
            if (AD_BowController.instance != null)
                return AD_BowController.instance;
            else
                return null;
        }

        public ArrowSkillSet GetArrSkillSetsOrNull(string tag) { // <- Delete this..
            if (tag == AD_Data.POOLTAG_MAINARROW || tag == AD_Data.POOLTAG_MAINARROW_LESS) {
                var skillSets = CCPlayerData.equipments.GetMainArrow().ArrowSkillSets;
                if (skillSets != null) {
                    //Is Have Skill Data In Arrow
                    var arrowSkillSets = new ArrowSkillSet(skillSets);
                    return arrowSkillSets;
                }
                else //Empty Skill Data
                    return null;
            }
            else if (tag == AD_Data.POOLTAG_SUBARROW || tag == AD_Data.POOLTAG_SUBARROW_LESS) {
                var skillSets = CCPlayerData.equipments.GetSubArrow().ArrowSkillSets;
                if (skillSets != null) {
                    //Is Have Skill Data In Arrow
                    var arrowSkillSets = new ArrowSkillSet(skillSets);
                    return arrowSkillSets;
                }
                else //Empty Skill Data
                    return null;
            }
            else //WrongTag 
                return null;
        }

        public bool TryGetSkillSet(string tag, out ArrowSkillSet skillset) {
            ArrowSkillSet temp = null;
            var equipments = CCPlayerData.equipments;
            if (tag == AD_Data.POOLTAG_MAINARROW || tag == AD_Data.POOLTAG_MAINARROW_LESS) {
                temp = equipments.GetMainArrow().ArrowSkillSets;
            }
            else if (tag == AD_Data.POOLTAG_SUBARROW || tag == AD_Data.POOLTAG_SUBARROW_LESS) {
                temp = equipments.GetSubArrow().ArrowSkillSets;
            }
            else if (tag == AD_Data.POOLTAG_SPECIAL_ARROW) {
                if(equipments.GetSpArrow().TryGetSkillSet(out ArrowSkillSet tempskillset)) {
                    temp = tempskillset;
                }
            }
            else {
                CatLog.ELog($"Not Implemented this ArrowTag. (Tag: {tag})");
                skillset = null;
                return false;
            }

            if (temp == null) { //Arrow Skill is Empty
                skillset = null;
                return false;
            }

            skillset = new ArrowSkillSet(temp);
            return true;
        }

        public void ReleaseEquipments() {
            CCPlayerData.equipments.ReleaseEquipments();
        }

        public AD_item[] GetBluePrints(BLUEPRINTTYPE type) {
            switch (type) {
                case BLUEPRINTTYPE.ALL: return CCPlayerData.inventory.GetAllBluePrints();
                default:                return CCPlayerData.inventory.GetBluePrints(type);
            }
        }

        public AD_item[] FindUpgradeableItems(string[] keys) {
            return CCPlayerData.inventory.GetUpgradeableItems(keys);
        }

        public bool TryGetItemAmount(string targetId, out int amount) {
            return CCPlayerData.inventory.TryGetAmount(targetId, out amount);
        }

        public bool FindItemRef(AD_item target) {
            return CCPlayerData.inventory.IsExist(target);
        }

        public bool TryRemoveItem(string itemid, int removeAmount) {
            return CCPlayerData.inventory.RemoveItem(itemid, removeAmount);
        }

        public bool TryRemoveItem(AD_item itemref) {
            if(CCPlayerData.inventory.IsExist(itemref)) {
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

        public void CheckSaveData() {
            CCPlayerData.CheckSaveData();
        }

        public bool TryReceipt(int slotNumber, out ItemData resultItemRef, out int resultItemAmount) {
            return CCPlayerData.infos.CraftingInfos[slotNumber].TryReceipt(out resultItemRef, out resultItemAmount);
        }

        public GlobalAbility GetGoAbility() {
            return CCPlayerData.ability.GlobalAbilityField;
        }

#endregion

#region BATTLE

        public void ResumeBattle() {
            SetBowPullingStop(false);
            TimeToRestore();
        }

        public void PauseBattle() {
            SetBowPullingStop(true);
            TimeToPause();
        }

        /// <summary>
        /// Never Used this Method.
        /// </summary>
        /// <param name="trigger"></param>
        public void PreventionPulling(UnityEngine.EventSystems.EventTrigger trigger) {
            PullingPreventionDown(trigger);
            PullingPreventionUp(trigger);
        }

        void PullingPreventionDown(UnityEngine.EventSystems.EventTrigger trigger)
        {
            //Bow Controller Pulling Limit [Button Down Event]
            UnityEngine.EventSystems.EventTrigger.Entry m_slotEntryDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            m_slotEntryDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            m_slotEntryDown.callback.AddListener((data) => SetBowPullingStop(true));
            trigger.triggers.Add(m_slotEntryDown);
        }

        void PullingPreventionUp(UnityEngine.EventSystems.EventTrigger trigger)
        {
            //Bow Controller Pulling Limit [Button Down Event]
            UnityEngine.EventSystems.EventTrigger.Entry m_slotEntryUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            m_slotEntryUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            m_slotEntryUp.callback.AddListener((data) => SetBowPullingStop(false));
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
            if(GetControllerInstOrNull() == null) {
                CatLog.ELog("The Controller is Null."); return;
            }

            GetControllerInstOrNull().AutoSwitch(isDebug);
        }

#endregion

#region TIME-CONTROL

        public void TimeScaleSet(float targetTimeScaleVal)
        {
            if(targetTimeScaleVal == Time.timeScale)
            {
                CatLog.WLog("Value equal to the current TimeScale");
                return;
            }
            else
            {
                Time.timeScale      = targetTimeScaleVal;
                Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
            }
        }

        public void TimeToDefault() {
            Time.timeScale      = 1f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public void TimeToPause() {
            restoreTimeScale = Time.timeScale;
            Time.timeScale      = 0f;
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

        public void ReleaseDropList()
        {
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
                    //return new DropItem(GameGlobal.RandomIntInArray(dropListArray[i].QuantityRange), dropListArray[i].ItemAsset);
                }
                else {
                    randomPoint -= dropListArray[i].DropChance;
                }
            }

            //Last index Item
            var lastIndexItem = dropListArray[dropListArray.Length - 1];
            return new DropItem(lastIndexItem.DefaultQuantity, lastIndexItem.ItemAsset);
            //return new DropItem(GameGlobal.RandomIntInArray(dropListArray[last].QuantityRange), dropListArray[last].ItemAsset);

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

#region SAVE_LOAD

        public void SaveUserData()
        {
            CCPlayerData.SaveUserData();
        }

        public void LoadUserData()
        {
            CCPlayerData.LoadUserData();
        }

        public void AutoLoadUserData() {
            if (isLoadedUserData == false)
                LoadUserData();
            isLoadedUserData = true;
        }

#endregion

#region STATE_EVENT_HANDLER

        /// <summary>
        /// Change Current Game State
        /// </summary>
        /// <param name="targetState"></param>
        public void ChangeGameState(GAMESTATE targetState) {
            GameState = targetState; //Change Current GameState
            switch (GameState) {     //Activate Event
                case GAMESTATE.STATE_BEFOREBATTLE: break;   //No Event.
                case GAMESTATE.STATE_INBATTLE:     break;   //No Event.
                case GAMESTATE.STATE_BOSSBATTLE:   break;   //No Event.
                case GAMESTATE.STATE_ENDBATTLE:    OnStateEndBattle(); break;
                case GAMESTATE.STATE_GAMEOVER:     OnStateGameOver();  break;
            }
        }

        public void AddListnerEndBattle(System.Action action) {
            OnStateEndBattle += new GameEventHandler(action);
        }

        public void AddListnerGameOver(System.Action action) {
            OnStateGameOver += new GameEventHandler(action);
        }

        public void ReleaseAllEventsWithNoneState() {
            //Release Battle State
            OnStateEndBattle = null;
            OnStateGameOver  = null;

            //Restore GameState to NONE
            ChangeGameState(GAMESTATE.STATE_NONE);
        }

        public GameEventHandler EventBattleEnd() {
            return this.OnStateEndBattle;
        }

        public GameEventHandler EventGameOver() {
            return this.OnStateGameOver;
        }

#endregion
    }
}
