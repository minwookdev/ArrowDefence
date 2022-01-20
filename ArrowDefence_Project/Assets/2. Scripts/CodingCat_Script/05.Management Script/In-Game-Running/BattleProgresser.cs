namespace ActionCat {
    using ActionCat.Data;
    using ActionCat.UI;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    #region BATTLE_DATA
    public struct BattleData {
        public bool isCleared { get; private set; }
        public bool isUsedResurrect { get; private set; }
        public short totalKilledCount { get; private set; }
        public short maxComboCount { get; private set; }
        public short currentComboCount { get; private set; }

        public BattleData(short startComboCount) {
            isCleared       = false;
            isUsedResurrect = false;
            maxComboCount     = 0;
            totalKilledCount  = 0;
            currentComboCount = startComboCount;
        }

        public void ComboIncrease() {
            currentComboCount++;
            if(currentComboCount > maxComboCount) {
                maxComboCount = currentComboCount;
            }
        }

        public void ComboIncrease(byte inc) {
            currentComboCount += inc;
            if(currentComboCount > maxComboCount) {
                maxComboCount = currentComboCount;
            }
        }

        public short GetComboWithIncrease() {
            if (currentComboCount + 1 < GameGlobal.MaxComboCount) //0~9998
                currentComboCount++;
            else
                currentComboCount = GameGlobal.MaxComboCount;     //9999~

            if(currentComboCount > maxComboCount) {
                maxComboCount = currentComboCount;
            }

            return currentComboCount;
        }

        public void ComboClear() {
            currentComboCount = 0;
        }

        public void IncreaseKillCount() {
            totalKilledCount++;
        }

        public void OnStageCleared(bool isusedresurrect) {
            isCleared = true;
            isUsedResurrect = isusedresurrect;
        }

        public void OnUseResurrection() {
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

        public DropItem(int quantityValue, ItemData asset)
        {
            this.quantity = quantityValue; this.itemAsset = asset;
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

    public class BattleProgresser : MonoBehaviour {
        [Header("COMPONENT")]
        [SerializeField] BattleSceneRoute battleSceneUI  = null;
        [SerializeField] MonsterSpawner   monsterSpawner = null;

        [Header("BATTLE PROGRESS")]
        public float StartBattleDelay = 3f;
        public float EndBattleDelay   = 2f;
        private float startWaitingTime;
        private float endWaitingTime;
        private bool IsResult = false;
        private bool isInitialized = false;

        [Header("PLAYER")]
        public Transform ParentTransform;
        public Transform BowInitPosition;
        public float MaxPlayerHealth = 200f;
        BattleData battleData;
        float currentPlayerHealth;
        float tempPlayerHealth = 0f;
        bool isUsedResurrect = false;

        [Header("CLEAR COUNT")]
        [Range(100, 1200)]
        public float MaxClearCount = 100;
        private float currentClearCount = 0f;
        float tempClearCount = 0f;
        [SerializeField] [RangeEx(60f, 180f, step:10f, "Battle Time (Step:10f)")]
        private float maxBattleTime = 120f;
        [SerializeField] [ReadOnly]
        private float currentBattleTime = 0f;

        [Header("SLIDER")]
        public Slider ClearSlider;
        public Slider PlayerHealthSlider;
        public Slider EnemyBossHealthSlider;
        public float SliderSmoothTime = 1f;
        [SerializeField] ClearSlider clearSlider = null;
        private float clearSliderDest;

        [Header("BATTLE STATE")]
        [SerializeField] STAGETYPE stageType;
        [ReadOnly] public GAMESTATE tempGameState;
        [SerializeField] [ReadOnly]
        string stageKey = "";

        [Header("DROPS")]
        public ItemDropList DropListAsset = null;
        [SerializeField] [Range(0, 100)] 
        private int StageDropCorrection = 30;
        [SerializeField] 
        private List<DropItem> dropItemList = new List<DropItem>();

        [Header("COMBO")]
        [SerializeField] ComboCounter comboCounter = null;
        [Tooltip("Not Modified this Field.")]
        [SerializeField] float maxComboTimer = 0f;
        bool isComboActivating = false;
        float currentComboTime = 0f;

        [Header("DEBUG")]
        public bool IsDebugClearStage    = false;
        public bool IsDebugMonsterLogics = false;
        public bool IsDebugGameOver      = false;
        public bool IsOnAutoMode         = false;
        public bool IsAutoModeLogger     = false;

        //전투 진행 이벤트 핸들러 : 수치관련 이벤트
        public delegate void ValueEventHandler(float value);
        public static ValueEventHandler OnDecPlayerHealth;
        public static ValueEventHandler OnIncClearGauge;  // <- Not Used.
        public static ValueEventHandler OnItemDrop;

        //전투 진행 이벤트 핸들러 : 몬스터관련 이벤트
        public delegate void BattleEventHandler();
        public static BattleEventHandler OnMonsterHit;
        public static BattleEventHandler OnMonsterDeath;

        //Wait For ObjectPooler Variables //의미없는 수준이었기 때문에 주석처리 해둠.
        //float poolerWaitTime = 0f;
        //Coroutine waitCo = null;

        IEnumerator Start() {
            //================================================== << COMPONENTS >> ==================================================
            monsterSpawner = GetComponent<MonsterSpawner>();
            battleSceneUI  = GetComponent<BattleSceneRoute>();
            //======================================================================================================================

            //================================================ << ITEM DROP LIST >> ================================================
            if (DropListAsset != null) {
                GameManager.Instance.InitialDroplist(this.DropListAsset);
                OnItemDrop += OnItemDropRoll;
            }
            //======================================================================================================================

            //================================================ << COMBO SYSTEM >> ==================================================
            InitComboCounter();
            //======================================================================================================================

            //================================================== << DELEGATE >> ====================================================
            //Add Monster Method to Delegate   < 몬스터 관련 이벤트 >
            OnMonsterHit   += ComboOccurs;       //Increase Combo Count
            OnMonsterDeath += IncreaseKillCount; //Increase Killed Count

            //Add Numerical Events to Delegate < 게임 진행 수치관련 이벤트 >
            currentPlayerHealth = MaxPlayerHealth; // <- 임시 플레이어 체력 할당 <플레이어 데이터에서 받아올 것>
            OnDecPlayerHealth  += DecreaseHealthGauge;
            OnIncClearGauge    += IncreaseClearGauge;
            //======================================================================================================================

            //================================================= << BATTLE DATA >> ==================================================
            battleData = new BattleData(startComboCount: 0);    // New Battle Data Struct
            stageKey   = GameGlobal.GetStageKey(stageType);     // Get Current Stage Key
            //======================================================================================================================

            //Wait Until Object-Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized == true);

            //==================================================== << PLAYER >> ====================================================
            BattleSceneRoute.ArrowSwapSlotInitData[]     arrowSwapSlotDatas;        //Arrow Swap Slot Data Init       [need object-pooler initializing]
            AccessorySkillSlot.ActiveSkillSlotInitData[] accessorySkillSlotDatas;   //Accessory skill Slots Data Init [need object-pooler initializing]
            GameManager.Instance.InitEquipments(BowInitPosition, ParentTransform, 1, 1, out arrowSwapSlotDatas, out accessorySkillSlotDatas);

            battleSceneUI.InitArrowSlots(arrowSwapSlotDatas);       //Init Arrow Slots
            battleSceneUI.InitSkillSlots(accessorySkillSlotDatas);  //Init Accessory Skill Slots
            //======================================================================================================================

            //================================================ << BATTLE STATE >> ==================================================
            GameManager.Instance.AddListnerEndBattle(() => {  // < When Game Cleared >
                GameManager.Instance.SetBowPullingStop(true); // Disable Bow Pullable
                battleData.OnStageCleared(isUsedResurrect);   // Battle Data Update. <Stage Cleared>
                KillAllMonsters();                            // Kill All Alive Monsters
            });
            GameManager.Instance.AddListnerGameOver(() => {   // < When Game Over >
                GameManager.Instance.SetBowPullingStop(true); // Disable Bow Pullable
                ComboClear();                                 // Clear Current Combo Count
            });

            SetGameState(GAMESTATE.STATE_BEFOREBATTLE);       // Set Game State : Before Battle
            //======================================================================================================================

            //================================================ << CLEAR SLIDER >> ==================================================
            ClearSliderInit();
            //======================================================================================================================

            //================================================== << AUTO MODE >> ===================================================
            //1. BattleSceneRoute에서 버튼UI 활성화/비활성화 가지고 있게함. -> 1차 검증
            //2. progresser에서 해당 스테이지의 키 값으로 3별여부 확인.     -> 2차 검증
            //3. 3별 달성된 스테이지라면 -> 이게 아니고 스테이지 진입 할 때, isAuto체크되었는지 여부 확인
            //4. 체크됐으면 AutoMode 버튼 활성화 명령하고 버튼에 GameManager로 부터 함수받아와서 
            //5. 버튼에 리스너등록해주는 방식은 어떨까??
            //6. 일단 씬에 오토버튼 하나 만들어놓고, 만들어주는 프로그레서에서 켜고꺼주는 것부터 진행해보자.
            bool isEnableButton = true; //Test bool -> always true
            var autoButton = battleSceneUI.GetAutoButton();
            if(autoButton != null) {
                if(isEnableButton) {   //-> Auto Button Enable 조건
                    autoButton.Init(true, GameManager.Instance.AutoSwitch, IsAutoModeLogger); //-> 여기서 Auto 시작하는 메서드 참조 넣어주면 될 듯?
                }
                else {
                    autoButton.Init(false);
                }
            }
            //======================================================================================================================

            //================================================ << BATTLE READY >> ==================================================
            isInitialized = true;
            //======================================================================================================================
        }

        private void Update() {
            switch (GameManager.Instance.GameState) {
                case GAMESTATE.STATE_BEFOREBATTLE : OnUpdateBeforeBattle(); break;
                case GAMESTATE.STATE_INBATTLE     : OnUpdateInBattle();     break;
                case GAMESTATE.STATE_BOSSBATTLE   : OnUpdateBossBattle();   break;
                case GAMESTATE.STATE_ENDBATTLE    : OnUpdateEndBattle();    break;
                case GAMESTATE.STATE_GAMEOVER     : OnUpdateGameOver();     break;
            }
        }

        private void OnDestroy() {
            //씬 이동 시 변수 정리.
            //게임 수치관련 이벤트 해제.
            OnIncClearGauge        -= IncreaseClearGauge;
            OnItemDrop            -= OnItemDropRoll;
            OnDecPlayerHealth -= DecreaseHealthGauge;

            //Clear 처리 후, Main Scene으로 넘어가는 경우가 아닌 ApplicationQuit 되어 버리는 경우
            //GameManager가 먼저 지워질 수 있다.
            if (GameManager.Instance != null) {
                GameManager.Instance.ReleaseDropList();
                GameManager.Instance.ReleaseAllEvent();
                GameManager.Instance.ReleaseEquipments();

                GameManager.Instance.SetBowPullingStop(false);
            }
        }

        #region SLIDER

        public void IncreaseClearGauge(float value) {
            //Safe Calculate
            tempClearCount = currentClearCount + value;

            if (tempClearCount < MaxClearCount) {
                currentClearCount = tempClearCount;
            }
            else if (tempClearCount >= MaxClearCount) {
                currentClearCount = MaxClearCount;
            }

            //Increase Clear Slider Value [if the caching ui.slider]
            if (ClearSlider != null) {
                float dest = currentClearCount / MaxClearCount;
                ClearSlider.DOValue(dest, 1f);
            }
        }

        public void DecreaseHealthGauge(float value)
        {
            //currentPlayerHealth -= value;

            tempPlayerHealth = currentPlayerHealth;
            tempPlayerHealth -= value;
            if (tempPlayerHealth > 0) {
                currentPlayerHealth = tempPlayerHealth;

                //Player's Hit Effect Output
                ActivePlayerDamageEffect();
            }
            else {
                currentPlayerHealth = 0;
            }
                
            if(PlayerHealthSlider != null) {
                float dest = currentPlayerHealth / MaxPlayerHealth;
                PlayerHealthSlider.DOValue(dest, 0.5f);
            }
        }

        /// <summary>
        /// 호출되는 간격이 짧을때는 Update에다 두고 사용하는게 안정적임 -> 그냥 DoTween 사용해서 해결함
        /// </summary>
        /// <returns></returns>
        private IEnumerator MoveSlider()
        {
            //var value = currentStageCount / MaxStageCount;
            //float startValue = StageSlider.value;
            float lerp = 0f;

            while (lerp < SliderSmoothTime)
            {
                clearSliderDest = currentClearCount / MaxClearCount; //처음에만 잡아줘버리면 계속 변동하는 수치를 잡지못함
                lerp += Time.deltaTime;
                float lerpValue = lerp / SliderSmoothTime;
                ClearSlider.value = Mathf.Lerp(ClearSlider.value, clearSliderDest, lerpValue);

                yield return null;
            }

            //ClearSlider.value = clearSliderDest;
            //adding Time.deltaTime will probably never add to a full Number, this is just rounding Slider value
            //so it's exactly what we want
        }

        void KillAllMonsters() {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag(AD_Data.OBJECT_TAG_MONSTER);
            CatLog.Log($"Tag Monster's Count : {monsters.Length}, All Monster's Disable");
            foreach (var monster in monsters) {
                monster.SendMessage(nameof(MonsterState.SetStateDeath), SendMessageOptions.DontRequireReceiver);
            }
            ///Scene에 Enable Monster개체를 비 활성화 처리
            ///GameObject.Enable된 개체만 Monsters에 담기는것을 확인.
        }

        void ActivePlayerDamageEffect() {
            //Active Red Panel 
            battleSceneUI.OnHitScreen();

            //Active Camera Shake
            CineCam.Inst.ShakeCamera(5f, .2f);
        }

        void ClearSliderInit() {
            if (clearSlider == null) {
                CatLog.ELog("Clear Slider Component is Null", true);
            }
            else {
                currentBattleTime = maxBattleTime;
                clearSlider.InitSlider(false, maxBattleTime);
            }
        }

        void ClearSliderUpdate() {
            clearSlider.UpdateSlider(currentBattleTime);
        }

        #endregion

        #region ITEMDROP_METHOD

        private void OnItemDropRoll(float dropRateCorrection) {
            if (GameManager.Instance.OnItemDropRoll(StageDropCorrection, dropRateCorrection) == true) {
                AddDropList(GameManager.Instance.OnItemDrop());
            }
        }

        private void AddDropList(DropItem item) {
            switch (item.ItemAsset.Item_Type) {
                case ITEMTYPE.ITEM_CONSUMABLE: StackOnDropItem(item);  break;
                case ITEMTYPE.ITEM_MATERIAL:   StackOnDropItem(item);  break;
                case ITEMTYPE.ITEM_EQUIPMENT:  dropItemList.Add(item); break;
                default:                                               break;
            }
        }

        private void StackOnDropItem(DropItem item) {
            var duplicateItem = dropItemList.Find(x => x.ItemAsset.Item_Id == item.ItemAsset.Item_Id);
            if (duplicateItem != null) duplicateItem.IncAmount(item.Quantity);
            else dropItemList.Add(item);
        }

        /// <summary>
        /// Add DropItems to Player Inventory
        /// </summary>
        private void DropItemsAddInventory()
        {
            dropItemList.ForEach((item) => {
                CCPlayerData.inventory.AddItem(item.ItemAsset, item.Quantity);
                CatLog.Log($"아이템 획득 : {item.ItemAsset.Item_Name}, 수량 : {item.Quantity}");
            });
        }

        #endregion

        #region UPDATE_BATTLE_STATE

        private void OnUpdateBeforeBattle()
        {
            if (isInitialized == false) return;

            startWaitingTime += Time.deltaTime;
            if(startWaitingTime >= StartBattleDelay) {
                if (IsDebugClearStage) {        //Debug 1. Stage Clear, Get All Item's in DropList Asset
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : ALL DROP ITEM LIST.");
                    foreach (var item in DropListAsset.DropTableArray) {
                        AddDropList(new DropItem(GameGlobal.RandomIntInArray(item.QuantityRange), item.ItemAsset));
                    }
                    //GameState Set Clear Game
                    SetGameState(GAMESTATE.STATE_ENDBATTLE);
                }
                else if (IsDebugMonsterLogics) { //Debug 2. Monster Logic Testing
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : MONSTER LOGIC TEST.");
                }
                else if (IsDebugGameOver) {      //Debug 3. Go GameOver State
                    CatLog.WLog(StringColor.YELLOW, "DEBUGGING MODE TRUE : GAME OVER.");
                    //GameState Set GameOver
                    SetGameState(GAMESTATE.STATE_GAMEOVER);
                }
                else { //Normal Battle Start
                    CatLog.Log("Start Battle ! [Normal Game Mode]");
                    SetGameState(GAMESTATE.STATE_INBATTLE);
                }
            }
        }

        private void OnUpdateInBattle() {
            //===================================================== << DECREASE TIME >> ===================================================
            currentBattleTime -= Time.deltaTime;
            //=============================================================================================================================

            //======================================================= << GAME CLEAR >> ====================================================
            if(currentBattleTime <= 0f) {
                SetGameState(GAMESTATE.STATE_ENDBATTLE);
            }
            //=============================================================================================================================

            //======================================================== << GAME OVER >> ====================================================
            if(currentPlayerHealth <= 0f) {
                SetGameState(GAMESTATE.STATE_GAMEOVER);
            }
            //=============================================================================================================================

            //Update Combo System
            ComboSystemUpdate();

            //Update Clear Slider
            ClearSliderUpdate();
        }

        private void OnUpdateBossBattle() {
            //Update Combo System
            ComboSystemUpdate();
        }

        private void OnUpdateEndBattle() {
            if (IsResult == false) //waiting time.
                endWaitingTime += Time.deltaTime;
            
            if (endWaitingTime >= EndBattleDelay) {
                IsResult = true;

                //Update Stage Info
                SendStageInfo();

                //Add Items in Player Inventory
                DropItemsAddInventory();

                //Open Clear Result Panel
                battleSceneUI.OnEnableResultPanel(dropItemList);
                endWaitingTime = 0f;
            }
        }

        void OnUpdateGameOver() {
            if(IsResult == false) { //waiting time.
                endWaitingTime += Time.unscaledDeltaTime;
            }

            if(endWaitingTime >= EndBattleDelay) {
                IsResult = true;

                //Open GameOver Panel
                battleSceneUI.OnEnableGameOverPanel();
                endWaitingTime = 0f;
            }
        }

        #endregion

        #region GAMEMANAGER_CALLBACK

        void SetGameState(GAMESTATE state) {
            GameManager.Instance.ChangeGameState(state);
            tempGameState = state;
        }

        #endregion

        #region STAGE_INFO

        void SendStageInfo() {
            GameManager.Instance.UpdateStageData(stageKey, in battleData);
        }

        void IncreaseKillCount() {
            battleData.IncreaseKillCount();
        }

        #endregion

        #region COMBO_SYSTEM

        void InitComboCounter() {
            if (comboCounter != null) {
                maxComboTimer = GameGlobal.ComboDuration;
                comboCounter.InitComboCounter(maxComboTimer, false);
            }
            else {
                CatLog.ELog("ComboCounter Component is Null, need Caching", true);
            }
        }

        void ComboOccurs() {
            //Increase Current Combo and, Update Combo Counter UI
            comboCounter.UpdateComboCounter(battleData.GetComboWithIncrease());
            currentComboTime  = maxComboTimer;
            isComboActivating = true;
        }

        void ComboSystemUpdate() {
            //Running Combo System
            if(currentComboTime > 0) {
                currentComboTime -= Time.deltaTime;
            }
            else {
                if(isComboActivating == true) {
                    //Clear Current Combo Count
                    ComboClear();
                    isComboActivating = false;
                }
            }
        }

        void ComboClear() {
            battleData.ComboClear();
            comboCounter.ComboClear();
        }

        #endregion

        //Time.time Timer 참고.
        //IEnumerator WaitObjectPooler() {
        //    float startTime = Time.time;
        //    while (poolerWaitTime < 3f)
        //    {
        //        poolerWaitTime = Time.time - startTime;
        //        yield return null;
        //    }
        //
        //    CatLog.ELog("Battle Progresser : CCPooler Wait Time Over, Stop Editor.");
        //    Debug.Break();
        //}
    }
}
