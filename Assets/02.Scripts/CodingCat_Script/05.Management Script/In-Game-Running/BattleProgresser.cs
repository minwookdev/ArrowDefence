﻿namespace ActionCat
{
    using ActionCat.Data;
    using ActionCat.UI;
    using ActionCat.Audio;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using UnityEditorInternal;

    #region BATTLE_DATA
    public struct BattleData
    {
        public bool isCleared { get; private set; }
        public bool isUsedResurrect { get; private set; }
        public short totalKilledCount { get; private set; }
        public short maxComboCount { get; private set; }
        public short currentComboCount { get; private set; }

        public BattleData(short startComboCount)
        {
            isCleared = false;
            isUsedResurrect = false;
            maxComboCount = 0;
            totalKilledCount = 0;
            currentComboCount = startComboCount;
        }

        public void ComboIncrease()
        {
            currentComboCount++;
            if (currentComboCount > maxComboCount)
            {
                maxComboCount = currentComboCount;
            }
        }

        public void ComboIncrease(byte inc)
        {
            currentComboCount += inc;
            if (currentComboCount > maxComboCount)
            {
                maxComboCount = currentComboCount;
            }
        }

        public short GetComboWithIncrease()
        {
            if (currentComboCount + 1 < GameGlobal.MaxComboCount) //0~9998
                currentComboCount++;
            else
                currentComboCount = GameGlobal.MaxComboCount;     //9999~

            if (currentComboCount > maxComboCount)
            {
                maxComboCount = currentComboCount;
            }

            return currentComboCount;
        }

        public void ComboClear()
        {
            currentComboCount = 0;
        }

        public void IncreaseKillCount()
        {
            totalKilledCount++;
        }

        public void OnStageCleared(bool isusedresurrect)
        {
            isCleared = true;
            isUsedResurrect = isusedresurrect;
        }

        public void OnUseResurrection()
        {
            isUsedResurrect = true;
        }
    }
    #endregion

    #region DROPS
    [System.Serializable]
    public class DropItem : IStackable
    {
        [SerializeField] private int quantity;
        [SerializeField] private ItemData itemAsset;

        public int Quantity { get => quantity; }

        public ItemData ItemAsset { get => itemAsset; }

        public bool IsReward
        {
            get; private set;
        } = false;

        public DropItem(int quantityValue, ItemData asset, bool isRewardDrop = false)
        {
            this.quantity = quantityValue; this.itemAsset = asset; IsReward = isRewardDrop;
        }

        public void SetAmount(int value) => quantity = value;

        public void IncAmount(int value) => quantity += value;

        public void DecAmount(int value)
        {
            if (quantity - value < 0) quantity = 0;
            else quantity -= value;
        }
    }
    #endregion

    public class BattleProgresser : MonoBehaviour
    {
        [Header("REQUIRED")]
        [SerializeField]
        [ReadOnly]
        BattleSceneRoute sceneRoute = null;
        [SerializeField] MonsterSpawner monsterSpawner = null;

        [Header("STAGE INFO")]
        [SerializeField] STAGETYPE stageType;
        [ReadOnly] public GAMESTATE tempGameState;
        GAMESTATE previousState = GAMESTATE.STATE_NONE;
        [SerializeField]
        [ReadOnly]
        string stageKey = "";

        [Header("TIME")]
        [SerializeField]
        [RangeEx(60f, 180f, step: 10f, "Battle Time (Step:10f)")]
        private float maxBattleTime = 120f;
        [SerializeField]
        [ReadOnly]
        private float currentBattleTime = 0f;

        [Header("WAITING")]
        [SerializeField] Image ImageCover = null;
        [SerializeField][RangeEx(0.5f, 5f, 0.5f)] float StartBattleWait = 3f;
        [SerializeField][RangeEx(0.5f, 5f, 0.5f)] float ResultPanelOpenDelaySeconds = 2f;
        private float currentStartDelayTime;
        private bool IsOpenedResult = false;
        private bool isInitialized = false;

        [Header("PLAYER")]
        public Transform ParentTransform;
        public Transform BowInitPosition;
        [SerializeField][ReadOnly] float MaxPlayerHealth = 200f;
        BattleData battleData;
        float currentPlayerHealth;
        float tempPlayerHealth = 0f;
        bool isUsedResurrect = false;

        [Header("DROPS")]
        [SerializeField] ItemDropList DropListAsset = null;
        [SerializeField]
        [RangeEx(0f, 70f, 5f)]
        private int dropCorrection = 30;
        [SerializeField] List<DropItem> dropItemList = new List<DropItem>();

        //COMBO TIME
        float maxComboTimer = 0f;
        bool isComboActivating = false;
        float currentComboTime = 0f;

        [Header("SOUND CHANNEL")]
        [SerializeField] ACSound channelArrowHit = null;
        [SerializeField] ACSound channelProjectile = null;
        [SerializeField] ACSound channelPlayer = null;
        [SerializeField] ACSound channelMonster = null;
        [SerializeField] ACSound musicAudioSource = null;

        [Header("DEBUG")]
        public bool IsQuickGameClear = false;
        public bool IsQuickGameOver = false;
        public bool IsQuickAutoButton = false;
        public bool IsForceStopSpawn = false;

        //전투 진행 이벤트 핸들러 : 수치관련 이벤트
        public delegate void ValueEventHandler(float value);
        public static ValueEventHandler OnDecPlayerHealth = null;
        public static ValueEventHandler OnIncPlayerHealth = null;
        public static ValueEventHandler OnIncClearGauge;  // <- Not Used.
        public static ValueEventHandler OnItemDrop;

        //전투 진행 이벤트 핸들러 : 몬스터관련 이벤트
        public delegate void BattleEventHandler();
        public static BattleEventHandler OnMonsterHit;
        public static BattleEventHandler OnMonsterDeathByAttack;

        public bool IsEnteringPause
        {
            get
            {
                switch (tempGameState)
                {
                    case GAMESTATE.STATE_NONE: return false;
                    case GAMESTATE.STATE_BEFOREBATTLE: return false;
                    case GAMESTATE.STATE_INBATTLE: return true;
                    case GAMESTATE.STATE_BOSSBATTLE: return true;
                    case GAMESTATE.STATE_ENDBATTLE: return false;
                    case GAMESTATE.STATE_GAMEOVER: return false;
                    case GAMESTATE.STATE_PAUSE: return false;
                    default: throw new System.NotImplementedException();
                }
            }
        }

        void Cover(bool isOpen)
        {
            var tempColor = ImageCover.color;
            tempColor.a = (isOpen == true) ? StNum.floatZero : StNum.floatOne;
            ImageCover.color = tempColor;
            if (isOpen == true)
            {
                ImageCover.gameObject.SetActive(false);
            }
        }

        void Awake()
        {
            Cover(false);
            GameManager.Instance.Initialize();
            AdsManager.Instance.InitRuntimeMgr();

            //Find ES3 Manager.
            var es3RefMgr = FindObjectOfType<ES3ReferenceMgr>();
            if (es3RefMgr == null)
            {
                CatLog.ELog("ES3ReferenceMgr does Not Exist in This Scene. follow this ↓ \n 'Assets > Easy Save 3 > Add Manager to Scene'", true);
            }

            //Add Sound Channels
            SoundManager.Instance.AddChannel2Dic(CHANNELTYPE.ARROW, channelArrowHit);
            SoundManager.Instance.AddChannel2Dic(CHANNELTYPE.PROJECTILE, channelProjectile);
            SoundManager.Instance.AddChannel2Dic(CHANNELTYPE.PLAYER, channelPlayer);
            SoundManager.Instance.AddChannel2Dic(CHANNELTYPE.MONSTER, channelMonster);
        }

        IEnumerator Start()
        {
            //================================================= << LOAD UI SCENE >> ================================================
            yield return StartCoroutine(SceneLoader.Instance.AdditiveLoadUIScene());
            Cover(true);
            //================================================== << COMPONENTS >> ==================================================
            monsterSpawner = GetComponent<MonsterSpawner>();
            sceneRoute = FindObjectOfType<BattleSceneRoute>(); // SceneRoute Object is Moved other Scene.
            sceneRoute.SetProgresser = this;
            //======================================================================================================================

            //================================================ << ITEM DROP LIST >> ================================================
            if (DropListAsset != null)
            {
                GameManager.Instance.InitialDroplist(this.DropListAsset);
                OnItemDrop += OnItemDropRoll;
            }
            else
            {
                CatLog.WLog("DropTable Scriptable Object is Not Set this Stage.");
                OnItemDrop += Foo; //init empty method for block errors
            }
            //======================================================================================================================

            //================================================ << COMBO SYSTEM >> ==================================================
            InitComboCounter();
            //======================================================================================================================

            //================================================== << DELEGATE >> ====================================================
            //Add Monster Method to Delegate   < 몬스터 관련 이벤트 >
            OnMonsterHit += ComboOccurs;       //Increase Combo Count
            OnMonsterDeathByAttack += IncreaseKillCount; //Increase Killed Count

            //Add Numerical Events to Delegate < 게임 진행 수치관련 이벤트 >
            MaxPlayerHealth = GameManager.Instance.GetGoAbility().MaxPlayerHealth;
            currentPlayerHealth = MaxPlayerHealth; // <- 임시 플레이어 체력 할당 <플레이어 데이터에서 받아올 것>
            OnDecPlayerHealth += DecreaseHealthGauge;
            OnIncPlayerHealth += IncreaseHealthGauge;
            //OnIncClearGauge  += IncreaseClearGauge;
            //======================================================================================================================

            //================================================= << BATTLE DATA >> ==================================================
            battleData = new BattleData(startComboCount: 0);    // New Battle Data Struct
            stageKey = GameGlobal.GetStageKey(stageType);     // Get Current Stage Key
            //======================================================================================================================

            //Wait Until Object-Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized == true);

            //==================================================== << PLAYER >> ====================================================
            GameManager.Instance.InitEquips(BowInitPosition, ParentTransform, 1, 1, out ArrSSData[] arrSlotArray, out AccessorySPEffect[] artifactEffects, sceneRoute.SlotArrSwap, out Sprite[] artifactSprites);

            sceneRoute.SlotArrSwap.InitSlots(arrSlotArray);                      //Init ArrowSwap Slot
            sceneRoute.SlotAcSkill.InitSlots(artifactEffects, artifactSprites);  //Init New Artifact Effect Slots !
            //======================================================================================================================

            //================================================ << BATTLE STATE >> ==================================================
            GameManager.Instance.AddListnerEndBattle(() =>
            {  // < When Game Cleared >
                battleData.OnStageCleared(isUsedResurrect);   // Battle Data Update. <Stage Cleared>
                KillAllMonsters();                            // Kill All Alive Monsters
            });
            GameManager.Instance.AddListnerGameOver(() =>
            {   // < When Game Over >
                ComboClear();                                 // Clear Current Combo Count
            });
            GameManager.Instance.AddListnerPause(() =>
            {      // < When Game Paused >

            });

            SetGameState(GAMESTATE.STATE_BEFOREBATTLE);       // Set Game State : Before Battle
            //======================================================================================================================

            //================================================ << CLEAR SLIDER >> ==================================================
            ClearSliderInit();
            //======================================================================================================================

            //================================================== << AUTO MODE >> ===================================================
            var autoButton = sceneRoute.ButtonAuto;
            autoButton.Disable();
            bool isExistSettings = GameManager.Instance.TryGetStageSetting(stageKey, out Data.StageData.StageSetting setting);
            bool isExistStageData = GameManager.Instance.TryGetStageData(stageKey, out StageInfo info);
            if (isExistSettings == true && isExistStageData == true)
            {
                CatLog.Log($"isOnAutoMode: {setting.isOnAutoMode}, UseableAutoMode: {info.IsUseableAuto}");
                if (setting.isOnAutoMode && info.IsUseableAuto)
                {
                    autoButton.Init(GameManager.Instance.AutoSwitch, IsQuickAutoButton);
                    GameManager.Instance.AddListnerGameOver(autoButton.ForceStopAuto);
                    GameManager.Instance.AddListnerEndBattle(autoButton.ForceStopAuto);
                }
            }
            else if (IsQuickAutoButton == true)
            {
#if UNITY_EDITOR
                autoButton.Init(GameManager.Instance.AutoSwitch, IsQuickAutoButton);
                GameManager.Instance.AddListnerGameOver(autoButton.ForceStopAuto);
                GameManager.Instance.AddListnerEndBattle(autoButton.ForceStopAuto);
#endif
            }
            //======================================================================================================================

            //================================================ << BATTLE READY >> ==================================================
            isInitialized = true;
            //======================================================================================================================
            GameManager.Instance.SetSavingFeedback(sceneRoute.SavingFeedbackPanel);
            musicAudioSource.PlaySoundWithDelayed(1f, true); //Play BGM
        }

        private void Update()
        {
            switch (GameManager.Instance.GameState)
            {
                case GAMESTATE.STATE_BEFOREBATTLE: OnUpdateBeforeBattle(); break;
                case GAMESTATE.STATE_INBATTLE: OnUpdateInBattle(); break;
                case GAMESTATE.STATE_BOSSBATTLE: OnUpdateBossBattle(); break;
                case GAMESTATE.STATE_ENDBATTLE: OnUpdateEndBattle(); break;
                case GAMESTATE.STATE_GAMEOVER: OnUpdateGameOver(); break;
                case GAMESTATE.STATE_PAUSE: break;
                case GAMESTATE.STATE_NONE: break;
                default: throw new System.NotImplementedException();
            }
        }

        private void OnDestroy()
        {
            //씬 이동 시 변수 정리.
            //게임 수치관련 이벤트 해제
            //OnIncClearGauge   -= IncreaseClearGauge;
            OnItemDrop -= OnItemDropRoll;
            OnDecPlayerHealth -= DecreaseHealthGauge;
            OnIncPlayerHealth -= IncreaseHealthGauge;

            //몬스터 관련 이벤트 헤제
            OnMonsterHit -= ComboOccurs;
            OnMonsterDeathByAttack -= IncreaseKillCount;

            //Clear 처리 후, Main Scene으로 넘어가는 경우가 아닌 ApplicationQuit 되어 버리는 경우
            //GameManager가 먼저 지워질 수 있다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReleaseDropList();
                GameManager.Instance.ReleaseAllEventsWithNoneState();
                GameManager.Instance.ReleaseEquipments();

                //위 이벤트들 GameManager안에서 하나로 묶고, 여기서는 하나만 호출하는거 가능?
                //GameManager.Instance.SetBowPullingStop(false);
            }
        }

        #region SLIDER

        public void DecreaseHealthGauge(float value)
        {
            tempPlayerHealth = currentPlayerHealth;
            tempPlayerHealth -= value;
            if (tempPlayerHealth > 0)
            {
                currentPlayerHealth = tempPlayerHealth;

                //Player's Hit Effect Output
                ActivePlayerDamageEffect();
            }
            else
            {
                currentPlayerHealth = 0;
            }

            float dest = currentPlayerHealth / MaxPlayerHealth;
            sceneRoute.PlayerSliderDec(dest);

            //Play Play-Hit Sound
            channelPlayer.PlayOneShot(0);
        }

        public void IncreaseHealthGauge(float value)
        {
            tempPlayerHealth = currentPlayerHealth;
            tempPlayerHealth += value;
            if (tempPlayerHealth > MaxPlayerHealth)
            {
                currentPlayerHealth = MaxPlayerHealth;
            }
            else
            {
                currentPlayerHealth = tempPlayerHealth;
            }

            float sliderDest = currentPlayerHealth / MaxPlayerHealth;
            sceneRoute.PlayerSliderDec(sliderDest);
        }

        void KillAllMonsters()
        {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag(AD_Data.OBJECT_TAG_MONSTER);
            CatLog.Log($"Tag Monster's Count : {monsters.Length}, All Monster's Disable");
            foreach (var monster in monsters)
            {
                monster.SendMessage(nameof(MonsterState.SetStateDeath), SendMessageOptions.DontRequireReceiver);
            }
            ///Scene에 Enable Monster개체를 비 활성화 처리
            ///GameObject.Enable된 개체만 Monsters에 담기는것을 확인.
        }

        void ActivePlayerDamageEffect()
        {
            //Active Red Panel 
            sceneRoute.OnHitScreen();

            //Active Camera Shake
            CineCam.Inst.ShakeCamera(4f, .3f);
        }

        void ClearSliderInit()
        {
            currentBattleTime = maxBattleTime;
            sceneRoute.ClearSliderInit(false, maxBattleTime);
        }

        void BattleTimerTextAndSliderUpdate()
        {
            sceneRoute.ClearSliderUpdate(currentBattleTime);
        }

        #endregion

        #region ITEMDROP_METHOD

        private void OnItemDropRoll(float dropRateCorrection)
        {
            if (GameManager.Instance.OnItemDropRoll(dropCorrection, dropRateCorrection) == true)
            {
                AddDropList(GameManager.Instance.OnItemDrop());
            }
        }

        private void Foo(float temp) { }

        private void AddDropList(DropItem item)
        {
            switch (item.ItemAsset.Item_Type)
            {
                case ITEMTYPE.ITEM_CONSUMABLE: StackOnDropItem(item); break;
                case ITEMTYPE.ITEM_MATERIAL: StackOnDropItem(item); break;
                case ITEMTYPE.ITEM_EQUIPMENT: dropItemList.Add(item); break;
                default: break;
            }
        }

        private void StackOnDropItem(DropItem item)
        {
            var duplicateItem = dropItemList.Find(x => x.ItemAsset.Item_Id == item.ItemAsset.Item_Id);
            if (duplicateItem != null) duplicateItem.IncAmount(item.Quantity);
            else dropItemList.Add(item);
        }

        /// <summary>
        /// Add DropItems to Player Inventory
        /// </summary>
        private void DropItemsAddInventory()
        {
            dropItemList.ForEach((item) =>
            {
                CCPlayerData.inventory.AddItem(item.ItemAsset, item.Quantity);
                CatLog.Log($"아이템 획득 : {item.ItemAsset.NameByTerms}, 수량 : {item.Quantity}");
            });
        }

        #endregion

        #region UPDATE_BATTLE_STATE

        private void OnUpdateBeforeBattle()
        {
            if (isInitialized == false) return;

            currentStartDelayTime += Time.deltaTime;
            if (currentStartDelayTime >= StartBattleWait)
            {
                if (IsQuickGameClear)
                {        //Debug 1. Stage Clear, Get All Item's in DropList Asset
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : ALL DROP ITEM LIST.");
                    foreach (var item in DropListAsset.DropTableArray)
                    {
                        AddDropList(new DropItem(GameGlobal.RandomIntInArray(item.QuantityRange), item.ItemAsset));
                    }
                    //GameState Set Clear Game
                    SetGameState(GAMESTATE.STATE_ENDBATTLE);
                }
                else if (IsForceStopSpawn)
                { //Debug 2. Monster Logic Testing
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : MONSTER LOGIC TEST.");
                    monsterSpawner.IsForceBreak = true;
                    SetGameState(GAMESTATE.STATE_INBATTLE);
                }
                else if (IsQuickGameOver)
                {      //Debug 3. Go GameOver State
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : GAME OVER.");
                    //GameState Set GameOver
                    SetGameState(GAMESTATE.STATE_GAMEOVER);
                }
                else
                { //Normal Battle Start
                    CatLog.Log("Start Battle ! [Normal Game Mode]");
                    SetGameState(GAMESTATE.STATE_INBATTLE);
                }
            }
        }

        private void OnUpdateInBattle()
        {
            //===================================================== << DECREASE TIME >> ===================================================
            currentBattleTime -= Time.deltaTime;
            //=============================================================================================================================

            //======================================================= << GAME CLEAR >> ====================================================
            if (currentBattleTime <= 0f)
            {
                SetGameState(GAMESTATE.STATE_ENDBATTLE);
                if (IsOpenedResult)
                {
                    CatLog.ELog("IsOpenedPanel 변수가 초기화되지 않아 결과패널이 열리지않습니다.", true);
                }
            }
            //=============================================================================================================================

            //======================================================== << GAME OVER >> ====================================================
            if (currentPlayerHealth <= 0f)
            {
                SetGameState(GAMESTATE.STATE_GAMEOVER);
                if (IsOpenedResult)
                {
                    CatLog.ELog("IsOpenedPanel 변수가 초기화되지 않아 결과패널이 열리지않습니다.", true);
                }
            }
            //=============================================================================================================================

            //Update Combo System
            ComboSystemUpdate();

            //Update Clear Slider
            BattleTimerTextAndSliderUpdate();
        }

        // 전투 진행 업데이트
        private void OnUpdateBattleState()
        {
            // 전투 시간 흐름
            currentBattleTime -= Time.deltaTime;

            // 전투 시간 종료 체크 (게임 클리어)
            if (currentBattleTime <= 0f)
            { // 전투의 상태를 변경하고 게임 클리어 패널 활성화
                SetGameState(GAMESTATE.STATE_ENDBATTLE);
                EnableBattleClearedPanel();
            }
            // 플레이어 체력 체크 (게임 오버)
            else if (currentPlayerHealth <= 0f)
            { // 전투의 상태를 변경하고 게임 오버 패널 활성화
                SetGameState(GAMESTATE.STATE_GAMEOVER);
                EnableBattleFailedPanel();
            }

            // 콤보 시스템 업데이트
            ComboSystemUpdate();

            // 전투 타이머 텍스트 및 슬라이더 업데이트
            BattleTimerTextAndSliderUpdate();
        }

        private void EnableBattleClearedPanel()
        {

        }

        private void EnableBattleFailedPanel()
        {

        }

        private void OnUpdateBossBattle()
        {
            //Update Combo System
            ComboSystemUpdate();
        }

        /// <summary>
        /// 스테이지 클리어 후 플레이어의 클리어 데이터 업데이트
        /// </summary>
        /// <param name="delaySeconds"></param>
        /// <returns></returns>
        private IEnumerator PlayerInfoUpdateAfterStageClear(float delaySeconds)
        {
            #region Stop BGM and Wait Delay Seconds
            /// Stop BGM With Sound FadeIn
            musicAudioSource.StopSoundWithFadeIn(1f, true);

            // DelaySeconds 동안 대기
            float timeElapsed = 0f;
            while (timeElapsed >= delaySeconds)
            {
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }
            #endregion

            DropItemsToInventory(); // 스테이지에서 획득한 아이템을 인벤토리에 추가
            UpdateStageData();      // 스테이지의 클리어 정보를 업데이트 
            GameManager.Instance.SaveUserJsonFile();           // 플레이어의 데이터 Json 저장
            // 스테이지 클리어 패널 출력 (획득 아이템 슬롯을 표시하기 위해 DropList을 전달)
            sceneRoute.OpenClearPanel(dropItemList.ToArray());
        }

        /// <summary>
        /// 스테이지에서 획득한 아이템을 플레이어의 인벤토리로 추가
        /// </summary>
        private void DropItemsToInventory()
        {
            dropItemList.ForEach((dropItem) =>
            {
                CCPlayerData.inventory.AddItem(dropItem.ItemAsset, dropItem.Quantity);
            });
        }

        private void OnUpdateEndBattle()
        {
            //Start:Result Panel Open Coroutine
            if (!IsOpenedResult)
            {
                IsOpenedResult = true;
                StartCoroutine(OpenResultPanelCo(ResultPanelOpenDelaySeconds));
            }
        }

        void OnUpdateGameOver()
        {
            //Start:GameOver Panel Open Coroutine
            if (!IsOpenedResult)
            {
                IsOpenedResult = true;
                StartCoroutine(OpenGameOverPanelCo(ResultPanelOpenDelaySeconds));
            }
        }

        IEnumerator OpenResultPanelCo(float delayTime)
        {
            //Stop BGM
            musicAudioSource.StopSoundWithFadeIn(1f, true);

            float timeElapsed = 0f;
            while (timeElapsed < delayTime)
            {
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            DropItemsAddInventory();
            UpdateStageData();
            GameManager.Instance.SaveUserJsonFile();
            sceneRoute.OpenClearPanel(dropItemList.ToArray());
        }

        /// <summary>
        /// GameOver State Update
        /// </summary>
        private void OnUpdateGameOverState()
        {
            if (!IsOpenedResult)
            {
                StartCoroutine(GameOverRoutineAsync(ResultPanelOpenDelaySeconds));
                IsOpenedResult = true;
            }
        }

        /// <summary>
        /// GameOver Routine by Coroutine
        /// </summary>
        /// <param name="panelEnableDelayTime"></param>
        /// <returns></returns>
        private IEnumerator GameOverRoutineAsync(float panelEnableDelayTime)
        {
            // Stop BGM With Sound FadeIn
            musicAudioSource.StopSoundWithFadeIn(1f, true);

            // Wait DelayTime
            float timeElapsed = 0f;
            while (timeElapsed < panelEnableDelayTime)
            {
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            // Save User Data and Open GameOverPanel
            GameManager.Instance.SaveUserJsonFile();
            sceneRoute.OpenGameOverPanel();
        }

        IEnumerator OpenGameOverPanelCo(float delayTime)
        {
            //Stop BGM
            musicAudioSource.StopSoundWithFadeIn(1f, true);

            float timeElapsed = 0f;
            while (timeElapsed < delayTime)
            {
                timeElapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            GameManager.Instance.SaveUserJsonFile();
            sceneRoute.OpenGameOverPanel();
        }

        #endregion

        #region GAMEMANAGER_CALLBACK

        /// <summary>
        /// 현재 BattleState를 저장하고 변경
        /// </summary>
        /// <param name="state"></param>
        void SetGameState(GAMESTATE state)
        {
            previousState = GameManager.Instance.GameState;
            GameManager.Instance.ChangeGameState(state);
        }

        #endregion

        #region PAUSE

        public void EnteringPauseMode()
        {
            //위에서 게임이 시작되는 상태거나, 종료된 상태였을경우 PAUSE모드에 진입되지 않도록 조건줌. IN-BATTLE중이거나, BOSS-BATTLE 상태임
            previousState = tempGameState;
            SetGameState(GAMESTATE.STATE_PAUSE);
        }

        public void ExitPauseMode()
        {
            SetGameState(previousState);
            previousState = GAMESTATE.STATE_NONE;
        }

        #endregion

        #region STAGE_INFO

        /// <summary>
        /// 플레이어의 스테이지 클리어 정보 업데이트
        /// </summary>
        void UpdateStageData()
        {    // 호출하는 즉시 해당 스테이지 클리어에 대한 데이터 저장됨 ↓
            var isExistStageData = GameManager.Instance.UpdateStageData(stageKey, in battleData);
            if (!isExistStageData)
            { // 기존에 이 스테이지에 대한 플레이 데이터가 없었을 경우, 초회 클리어 아이템 지급
                var rewards = DropListAsset.GetRewardTable;
                for (int i = 0; i < rewards.Length; i++)
                {
                    // Player Inventroy의 Reward Item 추가
                    var rewardItemAmount = rewards[i].DefaultQuantity;
                    CCPlayerData.inventory.AddItem(rewards[i].ItemAsset, rewardItemAmount);

                    // DropItems List에 Reward Item 추가 -> ClearPanel에서 보여주기위한 용도로 추가해줌
                    AddDropList(new DropItem(rewardItemAmount, rewards[i].ItemAsset, true));
                }
            }
            // 스테이지 클리어에 따른 아이템 제작 진행률 증가
            GameManager.Instance.UpdateCraftingInfo();
        }

        void IncreaseKillCount()
        {
            battleData.IncreaseKillCount();
        }

        #endregion

        #region COMBO_SYSTEM

        void InitComboCounter()
        {
            maxComboTimer = GameGlobal.ComboDuration;
            sceneRoute.ComboCounterInit(maxComboTimer, false);
        }

        void ComboOccurs()
        {
            //Increase Current Combo and, Update Combo Counter UI
            sceneRoute.ComboCounterUpdate(battleData.GetComboWithIncrease());
            currentComboTime = maxComboTimer;
            isComboActivating = true;
        }

        void ComboSystemUpdate()
        {
            //Running Combo System
            if (currentComboTime > 0)
            {
                currentComboTime -= Time.deltaTime;
            }
            else
            {
                if (isComboActivating == true)
                {
                    //Clear Current Combo Count
                    ComboClear();
                    isComboActivating = false;
                }
            }
        }

        void ComboClear()
        {
            battleData.ComboClear();
            sceneRoute.ComboCounterClear();
        }

        #endregion
    }
}
