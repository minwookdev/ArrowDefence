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

        public void SetupPlayerEquipments(Transform bowObjInitPos, Transform bowObjParentTr, 
                                          string mainArrowObjTag, string mainArrowLessObjTag, int mainArrowObjPoolQuantity,
                                          string subArrowObjTag,  string subArrowLessObjTag,  int subArrowPoolQuantity)
        {
            CCPlayerData.equipments.SetupEquipments(bowObjInitPos, bowObjParentTr, 
                                                    mainArrowObjTag, mainArrowLessObjTag, mainArrowObjPoolQuantity,
                                                    subArrowObjTag, subArrowLessObjTag, subArrowPoolQuantity);
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
            #region RETURN_RANGEOFAMOUNT_(TEST)
            //범위내 아이템 갯수 드랍
            //var exItem = items[0];
            //exItem.ItemAsset.Item_Amount = exItem.GetQuantityInRange();
            //return exItem.ItemAsset;
            //대충 요로케..? 해보면 될듯 !

            //for 문 안에서 쓸거니까 요로케? 
            //for (int i = 0; i < items.Length; i++)
            //{
            //    //var amountOfItem = items[i].GetQuantityInRange();
            //    //items[i].ItemAsset.Item_Amount = amountOfItem;
            //    //return items[i].ItemAsset;
            //
            //    if(items[i].ItemAsset is ItemData_Con || items[i].ItemAsset is ItemData_Mat)
            //    {
            //        GameGlobal.RandomIntInRange(items[i].QuantityRange, ref items[i].ItemAsset.Item_Amount);
            //        return items[i].ItemAsset;
            //    }
            //}
            //내일 이거한번 테스트해보고 원본 ItemData Asset Amount 안바뀌면 이걸로 가자
            //원본 건드리는로직 되버리면 안된당..

            #endregion

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
                    //기존 로직
                    return items[i].ItemAsset;

                    #region TEST_LOGIC
                    //수정 로직 테스트 원본 값이 바뀌는지 테스트 원본 값이 바뀌어버림
                    //if (items[i].ItemAsset is ItemData_Con || items[i].ItemAsset is ItemData_Mat) 캐스트 하지말고 Type 가져와서 비교하는거 괜찮겠다
                    //{
                    //    GameGlobal.RandomIntInRange(items[i].QuantityRange, ref items[i].ItemAsset.Item_Amount);
                    //    return items[i].ItemAsset;
                    //}
                    //else return items[i].ItemAsset;
                    //기존 값이 바뀌어 버리긴 하는데, 전투가 끝나고 난 뒤에 기존 Amount로 변경해주거나 해서 사용해면 충분히 될듯하다 (멋져)
                    #endregion
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
