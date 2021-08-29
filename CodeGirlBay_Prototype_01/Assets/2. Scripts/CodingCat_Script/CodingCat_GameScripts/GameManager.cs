namespace CodingCat_Scripts
{
    using UnityEngine;
    using CodingCat_Games.Data;

    public class GameManager : Singleton<GameManager>
    {
        public enum GamePlatform
        { 
            PLATFORM_PC,
            PLATFORM_MOBILE
        }

        public GamePlatform gamePlatform;

        public void SetPlayerBow(Transform parent, Transform initpos)
        {
            if (null == CCPlayerData.equipments.GetBowItem())
            {
                CatLog.ELog("Player Bow Item is Null");
                CatLog.Break();
                return;
            }

            var bowObj = CCPlayerData.equipments.GetBowItem().GetBowObject();

            if(null == bowObj)
            {
                CatLog.Log("Player Bow Item is Not Have Bow Object");
                CatLog.Break();
                return;
            }

            Instantiate(bowObj, initpos.position, Quaternion.Euler(0f, 0f, 90f), parent);

            //생성하고 바로 해줘야할거 있으면 여기서 해줌
        }
    }
}
