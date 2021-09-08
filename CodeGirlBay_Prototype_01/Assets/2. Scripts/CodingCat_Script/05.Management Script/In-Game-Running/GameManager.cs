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
    }
}
