namespace ActionCat
{
    public enum StringColor
    { 
        YELLOW,
        BLUE,
        RED,
        GREEN,                    
        WHITE,
        BLACK
    }

    ///System.Diagnotics.Conditional Attribute는 함수 자체를 전처리 시켜주는 역할을 수행
    ///매개변수는 Unity Editor의 Define Symbol.
    ///#if 와 같은 역할을 하는데 타겟이 함수이다.
    ///각각의 메서드 내부의 Debug.Log 등의 내용을 한번 더 전처리 지시문으로 감싼 이유는 컴파일 했을 때,
    ///함수자체가 실행되지는 않지만
    ///함수 내부의 코드자체는 남는것으로 확인이 되어 이 함수내부의 내용마저 컴파일 당시에 제외되도록 하기 위해
    ///전처리 지시문으로 한번 더 감싸주었다 

    /// <summary>
    /// CatLog class helps not to include "Debug.Log" Method when building.
    /// But, if you need to log in other environments such as mobile, you can output the log by adding "ENABLE_LOG" Define.
    /// </summary>
    public static class CatLog
    {

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.Log(msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(params string[] msg)
        {
#if ENABLE_LOG
            string msgLog = null;

            for(int i =0;i<msg.Length;i++)
            {
                msgLog += msg[i];
            }

            UnityEngine.Debug.Log(msgLog);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Log(StringColor color, string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogFormat("<color={0}>{1}</color>", ReturnColor(color), msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void WLog(string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogWarning(msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void WLog(params string[] msg)
        {
#if ENABLE_LOG
            string debugMsg = null;

            for(int i =0;i<msg.Length;i++)
            {
                debugMsg += msg[i];
            }

            UnityEngine.Debug.LogWarning(debugMsg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void WLog(StringColor scolor, string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogWarningFormat("<color={0}>{1}</color>", ReturnColor(scolor), msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void ELog(string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogError(msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void ELog(params string[] msg)
        {
#if ENABLE_LOG
            string dangerLog = null;

            for(int i =0;i<msg.Length;i++)
            {
                dangerLog += msg[i];
            }

            UnityEngine.Debug.LogError(dangerLog);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void ELog(StringColor scolor, string msg)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogErrorFormat("<color={0}>{1}</color>", ReturnColor(scolor), msg);
#endif
        }

        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void Break()
        {
#if ENABLE_LOG
            UnityEngine.Debug.Break();
#endif
        }

        #region DLog_Stop_Method
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void ELog(string msg, bool isStopEditor)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogError(msg);
            if(isStopEditor) {
                UnityEngine.Debug.Break();
            }
            //UnityEngine.Debug.
#endif
        }
        #endregion

        public static string ReturnColor(StringColor scolor)
        {
            switch (scolor)
            {
                case StringColor.YELLOW: return "yellow";
                case StringColor.BLUE:   return "blue";
                case StringColor.RED:    return "red";
                case StringColor.GREEN:  return "green";
                default:                 return "white";
            }
        }
    }
}
