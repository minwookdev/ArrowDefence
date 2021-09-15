namespace CodingCat_Games
{
    using CodingCat_Games.Data;
    using CodingCat_Scripts;
    using DG.Tweening;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class BattleProgresser : MonoBehaviour
    {
        [Header("BATTLE PROGRESS")]
        public GAMESTATE InitGameState;
        public float StartTime = 3f;
        public float EndTime = 2f;
        private bool IsResult = false;
        private float startWaitingTime;
        private float endWaitingTime;
        private bool isInitialized = false;

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
        [ReadOnly] public string BattleStr;

        [Header("DROP LIST FOR THIS STAGE")]
        public ItemDropList DropListAsset;
        [Range(0, 100)] public int StageDropCorrection;
        public List<ItemData> DropItems = new List<ItemData>();

        private BattleSceneRoute battleSceneUI;

        public delegate void OnIncreaseValue(float value);
        public static OnIncreaseValue OnIncreaseClearGauge;
        public static OnIncreaseValue OnDropItemChance;
        //위에 두놈 하나로 묶어도 괜찮겠다 -> 몬스터를 Kill하면 무조건 발동되는 이벤트들이라서

        private void Start()
        {
            battleSceneUI = GetComponent<BattleSceneRoute>();
            GameManager.Instance.SetGameState(InitGameState);

            if (DropListAsset != null)
                OnDropItemChance += OnDropItem;
            OnIncreaseClearGauge += IncreaseClearGauge;

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
        }

        private void OnDestroy()
        {
            OnIncreaseClearGauge -= IncreaseClearGauge;
            OnDropItemChance     -= OnDropItem;
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

        private void OnDropItem(float dropRateCorrection)
        {
            bool isItemDrop = GameManager.Instance.OnRollItemDrop(StageDropCorrection, dropRateCorrection);
            if (isItemDrop)
            { 
                //Item DropList를 그때그때 보내도 되지 않게끔, Start에서 미리 GameManager에 전달하는 식으로 변경
                var ItemData = GameManager.Instance.OnRollItemList(DropListAsset.DropListArray);
                DropItems.Add(ItemData);
            }
            else
            {

            }
        }

        private void OnRollDropList(float dropRateCorrection)
        {
            //기본 드롭량을 BattleProgress에 작성한 이유는 Stage마다 스폰량이 달리하게 되면
            //이를 보정해주기 쉽게 하기위해 각 Stage Battle Progress에 작성함

            int chance = Random.Range(1, 100 + 1);  //1~100
            float totalChance = StageDropCorrection + dropRateCorrection;

            if (totalChance >= chance)    //아이템을 획득한 경우
            {
                var itemData = ChooseItemInDropList(DropListAsset.DropListArray);
                DropItems.Add(itemData);
            }
            else                              //아이템을 획득하지 못한 경우
            {

            }
        }

        private ItemData ChooseItemInDropList(ItemDropList.DropItems[] items)
        {
            float total = 0f;

            foreach (var item in items)
            {
                total += item.DropChance;
            }

            float randomPoint = Random.value * total;

            for (int i = 0; i < items.Length; i++)
            {
                if(randomPoint < items[i].DropChance)
                {
                    return items[i].ItemAsset;
                }
                else
                {
                    randomPoint -= items[i].DropChance;
                }
            }

            //randomPoint 가 극한의 확률로 Random.value = 1f 나오고 리턴되지 못한 경우
            //드랍 테이블 안에서 가장 최소의 확률을 가진 아이템을 리턴함

            var minimunChanceOfItem = items[0];

            for (int i = 0; i < items.Length; i++)
            {
                if(minimunChanceOfItem.DropChance > items[i].DropChance)
                {
                    minimunChanceOfItem = items[i];
                }
            }

            return minimunChanceOfItem.ItemAsset;

            //Linq를 사용해서 최소확률의 아이템 찾기
            //var minimunChanceOfItem = (from item in items
            //                           select item).Min(item => item.DropChance);
        }

        private void DropItemsInPlayerInventory()
        {
            DropItems.ForEach((item) => { CCPlayerData.inventory.AddItem(item);
                                          CatLog.Log($"아이템 획득 : {item.Item_Name}, 수량 : {item.Item_Amount}");
            });
        }

        #endregion

        #region UPDATE_BATTLE_PROGRESS

        private void OnUpdateBeforeBattle()
        {
            if (isInitialized)
            {
                startWaitingTime += Time.deltaTime;

                if (startWaitingTime >= StartTime)
                {
                    GameManager.Instance.SetGameState(GAMESTATE.STATE_INBATTLE);
                }
            }

            BattleStr = "BEFORE BATTLE";
        }

        private void OnUpdateInBattle()
        {
            if(currentClearCount >= MaxClearCount)
            {
                GameManager.Instance.SetGameState(GAMESTATE.STATE_ENDBATTLE);
            }

            BattleStr = "IN BATTLE";
        }

        private void OnUpdateBossBattle()
        {

        }

        private void OnUpdateEndBattle()
        {
            BattleStr = "END BATTLE";

            if (IsResult == false)
                endWaitingTime += Time.deltaTime;
            
            if (endWaitingTime >= EndTime)
            {
                IsResult = true;
                DropItemsInPlayerInventory();
                battleSceneUI.OnResultPanel(DropItems);

                endWaitingTime = 0f;
            }
        }

        #endregion
    }
}
