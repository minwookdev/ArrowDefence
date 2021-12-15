namespace ActionCat
{
    using UnityEngine;
    using ActionCat.Data;

    public enum GAMESTATE {
        STATE_BEFOREBATTLE,
        STATE_INBATTLE,
        STATE_BOSSBATTLE,
        STATE_ENDBATTLE,
        STATE_GAMEOVER,
    }

    public class GameManager : Singleton<GameManager>
    {
        public enum GAMEPLATFORM { 
            PLATFORM_PC,
            PLATFORM_MOBILE
        }

        //FIELDS
        private GAMEPLATFORM gamePlatform;
        private GAMESTATE    gameState = GAMESTATE.STATE_BEFOREBATTLE;
        private float fixedDeltaTime;
        private bool  isLoadedUserData = false;

        /// <summary>
        /// Is Dev Mode Control Value
        /// </summary>
        private readonly bool isDevMode = true;

        //DropList Variables
        private ItemDropList.DropTable[] dropListArray;
        private float totalDropChances;

        //PROPERTIES
        public GAMEPLATFORM GamePlay_Platform { get => gamePlatform; set => gamePlatform = value; }
        public GAMESTATE GameState { get => gameState; }


        public bool IsDevMode { get => isDevMode; }

        //Event Monster
        public delegate void BattleEventHandler();
        BattleEventHandler MonsterHitEvent;
        BattleEventHandler MonsterLessHitEvent;
        BattleEventHandler MonsterDeathEvent;

        //Event Battle State
        BattleEventHandler OnStateEndBattle;
        BattleEventHandler OnStateGameOver;

        private void Start() => this.fixedDeltaTime = Time.fixedDeltaTime;

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

        #region PLAYER_GEAR

        public void InitEquipments(Transform bowObjInitPos, Transform bowObjParentTr, 
                                    int mainArrowObjPoolQuantity, int subArrowPoolQuantity)
        {
            CCPlayerData.equipments.InitEquipments(bowObjInitPos, bowObjParentTr, 
                                                   mainArrowObjPoolQuantity, subArrowPoolQuantity);
        }

        public void InitEquipments(Transform bowinitpos, Transform bowparent, int mainarrowpoolquantity, int subarrowpoolquantity,
                                   out BattleSceneRoute.ArrowSwapSlotInitData[] arrowslotdata,
                                   out AccessorySkillSlot.ActiveSkillSlotInitData[] skillslotdata)
        {
            CCPlayerData.equipments.InitEquipments(bowinitpos, bowparent, mainarrowpoolquantity, subarrowpoolquantity);
            arrowslotdata = ReturnArrowSlotData(); skillslotdata = ReturnSkillSlotData();
        }

        public void SetBowPullingStop(bool isStop)
        {
            if (AD_BowController.instance != null)
                AD_BowController.instance.IsPullingStop = isStop;
        }

        public LOAD_ARROW_TYPE LoadArrowType()
        {
            LOAD_ARROW_TYPE type = (CCPlayerData.equipments.IsEquippedArrowMain()) ? LOAD_ARROW_TYPE.ARROW_MAIN : 
                                                                                     LOAD_ARROW_TYPE.ARROW_SUB;
            return type;
        }

        public void InitArrowSlotData(out bool slot_m, out bool slot_s, 
                                      out Sprite arrowIconSprite_m, out Sprite arrowIconSprite_s)
        {
            slot_m = (CCPlayerData.equipments.IsEquippedArrowMain()) ? true : false;
            slot_s = (CCPlayerData.equipments.IsEquippedArrowSub())  ? true : false;

            if (slot_m) arrowIconSprite_m = CCPlayerData.equipments.GetMainArrow().GetSprite;
            else        arrowIconSprite_m = null;
            if (slot_s) arrowIconSprite_s = CCPlayerData.equipments.GetSubArrow().GetSprite;
            else        arrowIconSprite_s = null;
        }

        public BattleSceneRoute.ArrowSwapSlotInitData[] ReturnArrowSlotData()
        {
            var equips = CCPlayerData.equipments;

            bool activeMain, activeSub;
            Sprite mainSprite, subSprite;
            System.Action mainCallback, subCallback;

            activeMain = equips.IsEquippedArrowMain();
            if(activeMain)
            {
                mainSprite   = equips.GetMainArrow().GetSprite;
                mainCallback = () => Controller().ArrowSwap(LOAD_ARROW_TYPE.ARROW_MAIN);
            }
            else
            {
                mainSprite   = null;
                mainCallback = null;
            }

            activeSub = equips.IsEquippedArrowSub();
            if(activeSub)
            {
                subSprite   = equips.GetSubArrow().GetSprite;
                subCallback = () => Controller().ArrowSwap(LOAD_ARROW_TYPE.ARROW_SUB);
            }
            else
            {
                subSprite   = null;
                subCallback = null;
            }

            BattleSceneRoute.ArrowSwapSlotInitData[] datas 
                = new BattleSceneRoute.ArrowSwapSlotInitData[2]
                { new BattleSceneRoute.ArrowSwapSlotInitData(activeMain, mainSprite, mainCallback),
                  new BattleSceneRoute.ArrowSwapSlotInitData(activeSub, subSprite, subCallback) };

            //List.ToArray로 반환하는게 나은가

            return datas;
        }

        public AccessorySkillSlot.ActiveSkillSlotInitData[] ReturnSkillSlotData()
        {
            //List -> ToArray Return
            System.Collections.Generic.List<AccessorySkillSlot.ActiveSkillSlotInitData> skillDataList 
                = new System.Collections.Generic.List<AccessorySkillSlot.ActiveSkillSlotInitData>();

            var accessories = CCPlayerData.equipments.GetAccessories();
            for (int i = 0; i < accessories.Length; i++)
            {
                if (accessories[i] == null) continue;

                if (accessories[i].SPEffect != null)
                {
                    if (accessories[i].SPEffect.SpEffectType == ACSP_TYPE.SPEEFECT_SLOWTIME)
                    {
                        var slowTime = accessories[i].SPEffect as Acsp_SlowTime;
                        if (slowTime != null) //한번 더 검증
                        {
                            //skillDatas[i].InitSkillData(slowTime.IconSprite, slowTime.TimeSlowRatio, false, 
                            //    SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE,
                            //    (mono) => slowTime.ActiveSlowTime(mono));
                            skillDataList.Add(new AccessorySkillSlot.ActiveSkillSlotInitData(
                                             accessories[i].GetSprite, slowTime.Cooldown, false,
                                             SKILL_ACTIVATIONS_TYPE.COOLDOWN_ACTIVE,
                                             (mono) => slowTime.ActiveSkill(mono),
                                             () => slowTime.OnStop()));
                        }
                        else {
                            CatLog.ELog("Accessory Skill Casting failed");
                        }
                    }
                }
            }

            return skillDataList.ToArray();
        }

        public AD_BowController Controller()
        {
            if (AD_BowController.instance != null)
                return AD_BowController.instance;
            else
                return null;
        }

        public ArrowSkillSet GetArrowSkillSets(string tag)
        {
            if (tag == AD_Data.POOLTAG_MAINARROW || tag == AD_Data.POOLTAG_MAINARROW_LESS)
            {
                var skillSets = CCPlayerData.equipments.GetMainArrow().ArrowSkillSets;
                if (skillSets != null)
                {
                    //Is Have Skill Data In Arrow
                    var arrowSkillSets = new ArrowSkillSet(skillSets);
                    return arrowSkillSets;
                }
                else //Empty Skill Data
                    return null;
            }
            else if (tag == AD_Data.POOLTAG_SUBARROW || tag == AD_Data.POOLTAG_SUBARROW_LESS)
            {
                var skillSets = CCPlayerData.equipments.GetSubArrow().ArrowSkillSets;
                if (skillSets != null)
                {
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

        public void ReleaseEquipments()
        {
            CCPlayerData.equipments.ReleaseEquipments();
        }

        #endregion

        #region BATTLE

        /// <summary>
        /// Set Battle State no params
        /// </summary>
        /// <param name="gameState"></param>
        public void SetGameState(GAMESTATE gameState) => this.gameState = gameState;

        /// <summary>
        /// Set Battle State with Event Handler
        /// </summary>
        /// <param name="gameState"></param>
        /// <param name="handler"></param>
        public void SetGameState(GAMESTATE gameState, BattleEventHandler handler) {
            this.gameState = gameState; handler();
        }

        public void ResumeBattle()
        {
            SetBowPullingStop(false);
            TimeDefault();
        }

        public void PauseBattle()
        {
            SetBowPullingStop(true);
            TimePause();
        }



        /// <summary>
        /// Battle Scene의 각종 버튼 이벤트에서 Bow Pulling을 방지하는 메서드입니다.
        /// 예외처리될 UI, Arrow Slot, Skill Slot 등에 반드시 추가해주세요
        /// </summary>
        /// <param name="trigger"></param>
        public void PreventionPulling(UnityEngine.EventSystems.EventTrigger trigger)
        {
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

        #endregion

        #region TIME

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

        public void TimeDefault()
        {
            Time.timeScale      = 1f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public void TimePause()
        {
            Time.timeScale      = 0f;
            Time.fixedDeltaTime = this.fixedDeltaTime * Time.timeScale;
        }

        public bool IsTimeDefault()
        {
            bool isTimeDefault = (Time.timeScale == 1f) ? true : false;
            return isTimeDefault;
        }

        #endregion

        #region ITEM_DROP

        public void InitialDroplist(ItemDropList newDropList)
        {
            dropListArray = newDropList.DropTableArray;

            totalDropChances = 0f;

            //Set Drop Chance
            foreach (var item in dropListArray)
            {
                totalDropChances += item.DropChance;
            }

            CatLog.Log($"Initialize Drop List : {newDropList.name}, Total Drop Chances : {totalDropChances.ToString()}");
        }

        public void ReleaseDropList()
        {
            dropListArray = null;
            totalDropChances = 0f;

            CatLog.Log("Release Drop List");
        }

        public bool OnRollItemDrop(float stageDropRate, float monsterDropRateCorrection)
        {
            int chance = Random.Range(1, 100 + 1); //1~100의 수치
            float totalChance = stageDropRate + monsterDropRateCorrection;
            //최종 아이템 드랍 확률 = 스테이지 별 기본 아이템 드랍 확률 + 몬스터 각각이 가진 아이템 드랍 보정치

            if (totalChance >= chance) return true;     //아이템을 획득한 경우
            else                       return false;    //아이템을 획득하지 못한 경우
        }

        public DropItem OnDropInItemList()
        {
            float randomPoint = Random.value * totalDropChances;

            for (int i = 0; i < dropListArray.Length; i++)
            {
                if (randomPoint < dropListArray[i].DropChance)
                {
                    DropItem newItem = new DropItem(GameGlobal.RandomIntInArray(dropListArray[i].QuantityRange), dropListArray[i].ItemAsset);
                    return newItem;
                }
                else
                {
                    randomPoint -= dropListArray[i].DropChance;
                }
            }

            var minimunChanceOfItem = dropListArray[0];

            for (int i = 0; i < dropListArray.Length; i++)
            {
                if (minimunChanceOfItem.DropChance > dropListArray[i].DropChance)
                {
                    //이거 만약에 가장 낮은 확률의 아이템이 두개라면 어떻게 되는지
                    minimunChanceOfItem = dropListArray[i];
                }
            }

            DropItem item = new DropItem(GameGlobal.RandomIntInArray(minimunChanceOfItem.QuantityRange), minimunChanceOfItem.ItemAsset);
            return item;
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

        public void AutoLoadUserData()
        {
            if (isLoadedUserData == false)
                LoadUserData();
            isLoadedUserData = true;
        }

        #endregion

        #region STATE_EVENT_HANDLER

        public void AddListnerEndBattle(System.Action action) {
            OnStateEndBattle += new BattleEventHandler(action);
        }

        public void AddListnerGameOver(System.Action action) {
            OnStateGameOver += new BattleEventHandler(action);
        }

        public void ReleaseAllEvent() {
            //Release Monster
            MonsterHitEvent     = null;
            MonsterDeathEvent   = null;
            MonsterLessHitEvent = null;

            //Release Battle State
            OnStateEndBattle = null;
            OnStateGameOver  = null;
        }

        public BattleEventHandler EventBattleEnd() {
            return this.OnStateEndBattle;
        }

        public BattleEventHandler EventGameOver() {
            return this.OnStateGameOver;
        }

        #endregion

        #region MONSTER_EVENT_HANDLER

        public void AddEventMonsterHit(System.Action action) {
            var newEvent = new BattleEventHandler(action);
            MonsterHitEvent += newEvent;
        }

        public void AddEventMonsterLessHit(System.Action action) {
            var newEvent = new BattleEventHandler(action);
            MonsterLessHitEvent += newEvent;
        }

        public void AddEventMonsterDeath(System.Action action) {
            var newEvent = new BattleEventHandler(action);
            MonsterDeathEvent += newEvent;
        }

        public BattleEventHandler CallMonsterHitEvent() {
            return MonsterHitEvent;
        }

        public BattleEventHandler CallMonsterLessHitEvent() {
            return MonsterLessHitEvent;
        }

        public BattleEventHandler CallMonsterDeathEvent() {
            return MonsterDeathEvent;
        }

        #endregion
    }
}
