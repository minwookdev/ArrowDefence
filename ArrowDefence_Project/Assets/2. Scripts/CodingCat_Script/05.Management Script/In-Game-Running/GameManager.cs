namespace CodingCat_Games
{
    using UnityEngine;
    using CodingCat_Scripts;
    using CodingCat_Games.Data;

    public enum GAMESTATE
    {
        STATE_BEFOREBATTLE,
        STATE_INBATTLE,
        STATE_BOSSBATTLE,
        STATE_ENDBATTLE
    }

    public class GameManager : Singleton<GameManager>
    {
        public enum GAMEPLATFORM
        { 
            PLATFORM_PC,
            PLATFORM_MOBILE
        }

        //FIELDS
        private GAMEPLATFORM gamePlatform;
        private GAMESTATE    gameState = GAMESTATE.STATE_BEFOREBATTLE;
        private float fixedDeltaTime;

        //DropList Variables
        private ItemDropList.DropTable[] dropListArray;
        private float totalDropChances;

        //PROPERTIES
        public GAMEPLATFORM GamePlay_Platform { get => gamePlatform; set => gamePlatform = value; }
        public GAMESTATE GameState { get => gameState; }

        /// <summary>
        /// Is Dev Mode Control Value
        /// </summary>
        private readonly bool isDevMode = true;
        public bool IsDevMode { get => isDevMode; }

        private void Start()
        {
            this.fixedDeltaTime = Time.fixedDeltaTime;
        }

        #region GAME_PROGRESS_LOGIC

        public void SetupPlayerEquipments(Transform bowObjInitPos, Transform bowObjParentTr, 
                                          string mainArrowObjTag, string mainArrowLessObjTag, int mainArrowObjPoolQuantity,
                                          string subArrowObjTag,  string subArrowLessObjTag,  int subArrowPoolQuantity)
        {
            CCPlayerData.equipments.SetupEquipments(bowObjInitPos, bowObjParentTr, 
                                                    mainArrowObjTag, mainArrowLessObjTag, mainArrowObjPoolQuantity,
                                                    subArrowObjTag, subArrowLessObjTag, subArrowPoolQuantity);
        }

        public void SetGameState(GAMESTATE gameState) => this.gameState = gameState;

        public void SetBowPullingStop(bool isStop)
        {
            if (AD_BowController.instance != null)
                AD_BowController.instance.IsPullingStop = isStop;
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

        #endregion

        #region ITEM_DROP_LOGIC

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
    }
}
