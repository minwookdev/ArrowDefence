namespace CodingCat_Scripts
{
    using JetBrains.Annotations;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class FrameRateCheck : MonoBehaviour
    {
        #region OLD_STYLE
        //[Range(1, 100)]
        //public int FontSize;
        //[Range(0, 1)]
        //public float Red, Green, Blue;
        //
        ////public TextAnchor AnchorText;
        //
        //private float deltaTime = 0f;
        //
        //public void Start()
        //{
        //    //FontSize를 지정하지 않아서 0으로 들어가있으면 자동으로 50으로 맞춘다고 ㅇㅇ 아니면 지정해준 사이즈로 가고
        //    FontSize = FontSize == 0 ? 50 : FontSize;
        //}
        //
        //public void Update()
        //{
        //    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        //}
        //
        //private void OnGUI()
        //{
        //    int w = Screen.width, h = Screen.height;
        //
        //    GUIStyle style = new GUIStyle();
        //
        //    Rect rect = new Rect(0, 0, w, h * 2 / 100);
        //    style.alignment = TextAnchor.UpperLeft;
        //    style.fontSize = h * 2 / FontSize;
        //    style.normal.textColor = new Color(Red, Green, Blue, 1.0f);
        //    float msec = deltaTime * 1000.0f;
        //    float fps = 1.0f / deltaTime;
        //    string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        //    GUI.Label(rect, text, style);
        //}
        #endregion

        float deltaTime = 0.0f;

        GUIStyle style;
        Rect rect;
        float msec;
        float fps;
        float worstFps = 100f;
        string text;

        void Awake()
        {
            int w = Screen.width, h = Screen.height;

            rect = new Rect(0, 0, w, h * 4 / 100);

            style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;
            style.fontSize = h * 2 / 80;
            style.normal.textColor = Color.black;

            StartCoroutine("worstReset");
        }


        IEnumerator worstReset() //코루틴으로 15초 간격으로 최저 프레임 리셋해줌.
        {
            while (true)
            {
                yield return new WaitForSeconds(15f);
                worstFps = 100f;
            }
        }


        void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()//소스로 GUI 표시.
        {

            msec = deltaTime * 1000.0f;
            fps = 1.0f / deltaTime;  //초당 프레임 - 1초에

            if (fps < worstFps)  //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
                worstFps = fps;
            text = msec.ToString("F1") + "ms (" + fps.ToString("F1") + ") // worst : " + worstFps.ToString("F1");
            GUI.Label(rect, text, style);
        }
    }
}


