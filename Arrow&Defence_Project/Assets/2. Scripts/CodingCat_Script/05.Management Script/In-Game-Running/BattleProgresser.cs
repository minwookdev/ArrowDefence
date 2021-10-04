namespace CodingCat_Games
{
    using CodingCat_Games.Data;
    using CodingCat_Scripts;
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

        [Header("STAGE CLEAR COUNT")]
        [Range(100, 1200)] 
        public float MaxClearCount = 100;
        private float currentClearCount = 0f;

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

        public delegate void OnIncreaseValue(float value);
        public static OnIncreaseValue OnIncreaseClearGauge;
        public static OnIncreaseValue OnDropItemChance;
        //위에 두놈 하나로 묶어도 괜찮겠다 -> 몬스터를 Kill하면 무조건 발동되는 이벤트들이라서

        private void Start()
        {
            //Init GameManager
            battleSceneUI  = GetComponent<BattleSceneRoute>();
            monsterSpawner = GetComponent<MonsterSpawner>();
            GameManager.Instance.SetGameState(GAMESTATE.STATE_BEFOREBATTLE);
            CurrentGameState = GameManager.Instance.GameState;

            //Initial Delegate
            if (DropListAsset != null)
            {
                GameManager.Instance.InitialDroplist(this.DropListAsset);
                OnDropItemChance += OnDropItemRoll;
            }

            OnIncreaseClearGauge += IncreaseClearGauge;

            //Initializing GameObject Player's Equipments
            GameManager.Instance.SetupPlayerEquipments(BowInitPosition, ParentTransform,
                                                       AD_Data.POOLTAG_MAINARROW, AD_Data.POOLTAG_MAINARROW_LESS, 1,
                                                       AD_Data.POOLTAG_SUBARROW,  AD_Data.POOLTAG_SUBARROW_LESS, 1);

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

            if (Input.GetKeyDown(KeyCode.O)) Time.timeScale = 0.3f;
        }

        private void OnDestroy()
        {
            OnIncreaseClearGauge -= IncreaseClearGauge;
            OnDropItemChance     -= OnDropItemRoll;

            //종료 되어버리는 경우 GameManager가 먼저 지워져버리기 때문에 null 체크함
            if (GameManager.Instance != null) GameManager.Instance.ReleaseDropList();
        }

        #region GAME_GAUGE_LOGIC's

        public void IncreaseClearGauge(float value)
        {
            currentClearCount += value;

            if(currentClearCount >= MaxClearCount)
            {
                //살아있는 몬스터들 비활성화
                GameObject[] monsters = GameObject.FindGameObjectsWithTag(AD_Data.OBJECT_TAG_MONSTER);
                CatLog.Log($"Tag Monster's Count : {monsters.Length}"); //Scene에 활성화된 Monster 개체만 담는것을 확인

                foreach (var monster in monsters)
                {
                    monster.SendMessage("DisableObject_Req", monster, SendMessageOptions.DontRequireReceiver);
                }

                currentClearCount = MaxClearCount;
            }

            if (ClearSlider != null)
            {
                float dest = currentClearCount / MaxClearCount;
                ClearSlider.DOValue(dest, 1f);
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
            if(currentClearCount >= MaxClearCount)
            {
                CurrentGameState = GameManager.Instance.GameState;
                GameManager.Instance.SetGameState(GAMESTATE.STATE_ENDBATTLE);
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
    }
}
