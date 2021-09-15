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

        //PROPERTIES
        public GAMEPLATFORM GamePlay_Platform { get => gamePlatform; set => gamePlatform = value; }
        public GAMESTATE GameState { get => gameState; }

        //이거그냥 readonly두고 코드로 결정시켜버리기
        private bool isDevMode = true;
        public bool IsDevMode { get { return isDevMode; }}

        public void SetPlayerBow(Transform parent, Transform initpos)
        {
            if(CCPlayerData.equipments.GetBowItem() == null)
            {
                CatLog.WLog("Player Bow Item or GameObject Not in Bow Item, Return Function SetPlayerBow");
                return;
            }

            var bowObj = CCPlayerData.equipments.GetBowItem().GetBowObject();
            Instantiate(bowObj, initpos.position, Quaternion.Euler(0f, 0f, 90f), parent);

            //생성하고 바로 해줘야할거 있으면 여기서 해줌
        }

        public void SetPooler()
        {
            var equipment = CCPlayerData.equipments;

            if(equipment.IsEquippedArrowMain())
            {
                CCPooler.AddPoolList(AD_Data.TAG_MAINARROW,      1, equipment.GetMainArrow().GetObject_MainArrow());
                CCPooler.AddPoolList(AD_Data.TAG_MAINARROW_LESS, 1, equipment.GetMainArrow().GetObject_LessArrow());
            }

            if(equipment.IsEquippedArrowSub())
            {
                CCPooler.AddPoolList(AD_Data.TAG_SUBARROW,      1, equipment.GetSubArrow().GetObject_MainArrow());
                CCPooler.AddPoolList(AD_Data.TAG_SUBARROW_LESS, 1, equipment.GetSubArrow().GetObject_LessArrow());
            }
        }

        public void SetGameState(GAMESTATE gameState)
        {
            switch (gameState)
            {
                case GAMESTATE.STATE_BEFOREBATTLE: this.gameState = GAMESTATE.STATE_BEFOREBATTLE; break;
                case GAMESTATE.STATE_INBATTLE:     this.gameState = GAMESTATE.STATE_INBATTLE;     break;
                case GAMESTATE.STATE_BOSSBATTLE:   this.gameState = GAMESTATE.STATE_BOSSBATTLE;   break;
                case GAMESTATE.STATE_ENDBATTLE:    this.gameState = GAMESTATE.STATE_ENDBATTLE;    break;
            }
        }

        #region ITEM_DROP_METHOD

        public bool OnRollItemDrop(float stageDropRate, float monsterDropRateCorrection)
        {
            int chance = Random.Range(1, 100 + 1); //1~100의 수치
            float totalChance = stageDropRate + monsterDropRateCorrection;
            //최종 아이템 드랍 확률 = 스테이지 별 기본 아이템 드랍 확률 + 몬스터 각각이 가진 아이템 드랍 보정치

            if (totalChance >= chance) return true;     //아이템을 획득한 경우
            else                       return false;    //아이템을 획득하지 못한 경우
        }

        public ItemData OnRollItemList(ItemDropList.DropItems[] items)
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

            //RandomPoint 변수가 1f 리턴되어서 for문 조건안에서 ItemAsset으로 반환되지 못한경우
            //드랍 테이블 안에서 가장 최소의 확률을 가진 아이템을 반환해줌

            var minimunChanceOfItem = items[0];

            for (int i = 0; i < items.Length; i++)
            {
                if(minimunChanceOfItem.DropChance > items[i].DropChance)
                {
                    minimunChanceOfItem = items[i];
                }
            }

            return minimunChanceOfItem.ItemAsset;
        }

        #endregion
    }
}
