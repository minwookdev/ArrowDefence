namespace ActionCat
{
    using ActionCat.Data;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

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

    public class BattleProgresser : MonoBehaviour
    {
        [Header("BATTLE PROGRESS")]
        public float StartBattleDelay = 3f;
        public float EndBattleDelay   = 2f;
        private bool IsResult = false;
        private float startWaitingTime;
        private float endWaitingTime;
        private bool isInitialized = false;

        [Header("PLAYER's INITIALIZING")]
        public Transform ParentTransform;
        public Transform BowInitPosition;
        public float MaxPlayerHealth = 200f;
        private float currentPlayerHealth;

        [Header("STAGE CLEAR COUNT")]
        [Range(100, 1200)] 
        public float MaxClearCount = 100;
        private float currentClearCount = 0f;
        float tempClearCount = 0f;

        [Header("SLIDER'S")]
        public Slider ClearSlider;
        public Slider PlayerHealthSlider;
        public Slider EnemyBossHealthSlider;
        public float SliderSmoothTime = 1f;
        private float clearSliderDest;

        [Header("CURRENT BATTLE STATE")]
        [ReadOnly]
        public GAMESTATE CurrentGameState;

        [Header("DROP LIST FOR THIS STAGE")]
        public ItemDropList DropListAsset;
        [Range(0, 100)] public int StageDropCorrection;
        [SerializeField] 
        private List<DropItem> dropItemList = new List<DropItem>();

        [Header("DEBUG OPTION")]
        public bool IsDebugClearStage = false;
        public bool IsDebugMonsterLogics = false;

        private BattleSceneRoute battleSceneUI;
        private MonsterSpawner   monsterSpawner;

        //각종 게임 진행 수치관련 대리자
        public delegate void ValueEventHandler(float value);
        public static ValueEventHandler OnIncreaseClearGauge;
        public static ValueEventHandler OnDropItemChance;
        public static ValueEventHandler OnDecreasePlayerHealthPoint;

        //Wait For ObjectPooler Variables //의미없는 수준이었기 때문에 주석처리 해둠.
        //float poolerWaitTime = 0f;
        //Coroutine waitCo = null;

        IEnumerator Start()
        {
            //Init-GameManager
            battleSceneUI  = GetComponent<BattleSceneRoute>();
            monsterSpawner = GetComponent<MonsterSpawner>();
            GameManager.Instance.SetGameState(GAMESTATE.STATE_BEFOREBATTLE);
            CurrentGameState = GameManager.Instance.GameState;

            //Init-Delegate
            if (DropListAsset != null) {
                GameManager.Instance.InitialDroplist(this.DropListAsset);
                OnDropItemChance += OnDropItemRoll;
            }

            OnIncreaseClearGauge += IncreaseClearGauge;

            #region OLD
            //Init-GameObject Player's Equipments
            //GameManager.Instance.InitEquipments(BowInitPosition, ParentTransform, 1, 1);

            //Init-Arrow Slots -> out 키워드 너무 많다 수정하자
            //PlayerData.Equipments와 관련된 로직이기 때문에, Progresser에서 GameManager 참조하여 처리
            //bool arrowSlot_m, arrowSlot_s;
            //Sprite iconSprite_m, iconSprite_s;
            //GameManager.Instance.InitArrowSlotData(out arrowSlot_m, out arrowSlot_s, out iconSprite_m, out iconSprite_s);

            //battleSceneUI.InitArrowSlots(arrowSlot_m, arrowSlot_s, iconSprite_m, iconSprite_s,
            //                            () => { GameManager.Instance.Controller().ArrowSwap(LOAD_ARROW_TYPE.ARROW_MAIN); },
            //                            () => { GameManager.Instance.Controller().ArrowSwap(LOAD_ARROW_TYPE.ARROW_SUB); });

            //Init-Arrow Swap Slots
            //var arrowSwapDatas = GameManager.Instance.ReturnArrowSlotData();
            //battleSceneUI.InitArrowSlots(arrowSwapDatas);

            //Init-Accessory Skill Slots
            //var accessorySkillDatas = GameManager.Instance.ReturnSkillSlotData();
            //battleSceneUI.InitSkillSlots(accessorySkillDatas);
            #endregion 

            //Start Wait Timer
            //waitCo = StartCoroutine(WaitObjectPooler());

            //Wait Until Object-Pooler
            yield return new WaitUntil(() => CCPooler.IsInitialized == true);

            //Stop Timer and Output TimeCount
            //StopCoroutine(waitCo);
            //CatLog.Log($"[BattleProgresser] CCPooler Waiting Time : {poolerWaitTime.ToString()}");

            //Init-Player Equipments [Init Pool, Arrow Swap Slot Data, Skill Slot Data]
            BattleSceneRoute.ArrowSwapSlotInitData[]     arrowSwapSlotDatas;
            AccessorySkillSlot.ActiveSkillSlotInitData[] accessorySkillSlotDatas;
            GameManager.Instance.InitEquipments(BowInitPosition, ParentTransform, 1, 1, out arrowSwapSlotDatas, out accessorySkillSlotDatas);

            //Init-Arrow, Skill Slots
            battleSceneUI.InitArrowSlots(arrowSwapSlotDatas);
            battleSceneUI.InitSkillSlots(accessorySkillSlotDatas);

            //Init-GameManager Event [TEST] (추후 특수효과 발동 및 특수 이벤트에 활용 예정)
            GameManager.Instance.AddEventMonsterHit(() => CatLog.Log("On Monster Hit"));
            GameManager.Instance.AddEventMonsterLessHit(() => CatLog.Log("On Monster Death"));
            GameManager.Instance.AddEventMonsterDeath(() => CatLog.Log("On Monster Hit Less Arrow"));

            //Init-Battle State Callback Event
            GameManager.Instance.AddEventEndBattle(() => { 
                GameManager.Instance.SetBowPullingStop(true);
                DisableMonsterAll();
            });

            //Init-Player Health Point [TEST] (플레이어 임시 체력, 추후 PlayerData에서 수치 받아오도록 설정)
            currentPlayerHealth = MaxPlayerHealth;
            OnDecreasePlayerHealthPoint += DecreaseHealthGauge;

            //Progresser Ready For Battle State Running
            isInitialized = true;
        }

        private void Update()
        {
            switch (GameManager.Instance.GameState)
            {
                case GAMESTATE.STATE_BEFOREBATTLE : OnUpdateBeforeBattle(); break;
                case GAMESTATE.STATE_INBATTLE     : OnUpdateInBattle();     break;
                case GAMESTATE.STATE_BOSSBATTLE   : OnUpdateBossBattle();   break;
                case GAMESTATE.STATE_ENDBATTLE    : OnUpdateEndBattle();    break;
            }

            if(Input.GetKeyDown(KeyCode.I))
            {

            }
            if(Input.GetKeyDown(KeyCode.O))
            {
                GameManager.Instance.ReleaseAccessory();
            }
        }

        private void OnDestroy()
        {
            OnIncreaseClearGauge        -= IncreaseClearGauge;
            OnDropItemChance            -= OnDropItemRoll;
            OnDecreasePlayerHealthPoint -= DecreaseHealthGauge;

            //Clear 처리 후, Main Scene으로 넘어가는 경우가 아닌 ApplicationQuit 되어 버리는 경우
            //GameManager가 먼저 지워질 수 있다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ReleaseDropList();
                GameManager.Instance.ReleaseAllEvent();
            }
        }

        #region GAUGE_CONTROLLER

        public void IncreaseClearGauge(float value)
        {
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

            float tempHealth = currentPlayerHealth;
            tempHealth -= value;
            if (tempHealth > 0)
                currentPlayerHealth = tempHealth;
            else
                currentPlayerHealth = 0;

            if(PlayerHealthSlider != null)
            {
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

        void DisableMonsterAll() {
            GameObject[] monsters = GameObject.FindGameObjectsWithTag(AD_Data.OBJECT_TAG_MONSTER);
            CatLog.Log($"Tag Monster's Count : {monsters.Length}, All Monster's Disable");
            foreach (var monster in monsters) {
                monster.SendMessage(nameof(IPoolObject.DisableRequest), monster, SendMessageOptions.DontRequireReceiver);
            }
            ///Scene에 Enable Monster개체를 비 활성화 처리
            ///GameObject.Enable된 개체만 Monsters에 담기는것을 확인.
        }

        #endregion

        #region ITEMDROP_METHOD

        private void OnDropItemRoll(float dropRateCorrection)
        {
            if (GameManager.Instance.OnRollItemDrop(StageDropCorrection, dropRateCorrection))
                AddItemInDropItems(GameManager.Instance.OnDropInItemList());
            else return;
        }

        private void AddItemInDropItems(DropItem item)
        {
            switch (item.ItemAsset.Item_Type)
            {
                case ITEMTYPE.ITEM_CONSUMABLE: StackOnDropItem(item);  break;
                case ITEMTYPE.ITEM_MATERIAL:   StackOnDropItem(item);  break;
                case ITEMTYPE.ITEM_EQUIPMENT:  dropItemList.Add(item); break;
                default:                                               break;
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
            dropItemList.ForEach((item) => {
                CCPlayerData.inventory.AddItem(item.ItemAsset, item.Quantity);
                CatLog.Log($"아이템 획득 : {item.ItemAsset.Item_Name}, 수량 : {item.Quantity}");
            });
        }

        #endregion

        #region UPDATE_BATTLE_PROGRESS

        private void OnUpdateBeforeBattle()
        {
#if UNITY_EDITOR
            //Debug : Stage Clear, Get All Item's in DropList Asset
            if(isInitialized && IsDebugClearStage == true)
            {
                startWaitingTime += Time.deltaTime;

                if(startWaitingTime >= StartBattleDelay)
                {
                    //Output Msg
                    CatLog.WLog(StringColor.YELLOW, $"DEBUGGING MODE TRUE : STAGE CLEAR, ALL DROP LIST ITEM ADDED");

                    //All DropList Item Assets in Drop Item List (Debug)
                    foreach (var item in DropListAsset.DropTableArray)
                    {
                        AddItemInDropItems(new DropItem(GameGlobal.RandomIntInArray(item.QuantityRange), item.ItemAsset));
                    }

                    //Out of Before Battle State 
                    CurrentGameState = GameManager.Instance.GameState;
                    GameManager.Instance.SetGameState(GAMESTATE.STATE_ENDBATTLE); return;
                }
            }
            //Debug : Monster Logic Test
            if(isInitialized && IsDebugMonsterLogics == true)
            {
                startWaitingTime += Time.deltaTime;

                if(startWaitingTime >= StartBattleDelay)
                {
                    monsterSpawner.MonsterDebug(AD_Data.POOLTAG_MONSTER_NORMAL, new Vector2(-0.05f, 4.5f));
                    startWaitingTime = 0f;
                }
            }
#endif
            if (isInitialized && IsDebugClearStage    == false
                              && IsDebugMonsterLogics == false)
            {
                startWaitingTime += Time.deltaTime;

                if (startWaitingTime >= StartBattleDelay)
                {
                    CurrentGameState = GameManager.Instance.GameState;
                    GameManager.Instance.SetGameState(GAMESTATE.STATE_INBATTLE);
                }
            }
        }

        private void OnUpdateInBattle()
        {
            //Is Game Clear
            if(currentClearCount >= MaxClearCount)
            {
                CurrentGameState = GameManager.Instance.GameState;
                GameManager.Instance.SetGameState(GAMESTATE.STATE_ENDBATTLE, GameManager.Instance.CallEndBattleEvent());
            }
        }

        private void OnUpdateBossBattle()
        {

        }

        private void OnUpdateEndBattle()
        {
            if (IsResult == false)
                endWaitingTime += Time.deltaTime;
            
            if (endWaitingTime >= EndBattleDelay)
            {
                IsResult = true;
                DropItemsAddInventory();
                battleSceneUI.OnEnableResultPanel(dropItemList);

                endWaitingTime = 0f;
            }
        }

        #endregion

        #region GAMEMANAGER_CALLBACK

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
