namespace CodingCat_Scripts
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

    /// <summary>
    /// CatLog class helps not to include "Debug.Log" Method when building.
    /// But, if you need to log in other environments such as mobile, you can output the log by adding "ENABLE_LOG" Define.
    /// </summary>
    public static class CatLog
    {
        //Currently Not Using StringBuilders, (Low utility)
        //private static StringBuilder _sb = new StringBuilder();

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

        #region DLog_Stop_Method
        [System.Diagnostics.Conditional("ENABLE_LOG")]
        public static void ELog(string msg, bool isStopEditor)
        {
#if ENABLE_LOG
            UnityEngine.Debug.LogError(msg);
            if(isStopEditor)
            {
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
