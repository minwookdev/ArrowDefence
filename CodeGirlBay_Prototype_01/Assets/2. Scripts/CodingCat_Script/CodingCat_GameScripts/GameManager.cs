using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingCat_Scripts
{
    public class GameManager : Singleton<GameManager>
    {
        public enum GamePlatform
        { 
            PLATFORM_PC,
            PLATFORM_MOBILE
        }

        public GamePlatform gamePlatform;
    }
}
