using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity Time.time 변수를 사용한 Timer Example
/// </summary>
public class UnityTimeTimer : MonoBehaviour {
    public UnityEngine.UI.Text text_timer = null;
    float time_start;
    float time_current;
    float time_Max = 5f;
    bool isEnded = false;

    private void Start() {
        ResetTimer();
    }

    private void Update() {
        if (isEnded == true)
            return;
        
    }

    void UpdateTimer() {
        time_current = Time.time - time_start;
        if(time_current < time_Max) {
            text_timer.text = $"{time_current:N2}";
            ActionCat.CatLog.Log(time_current.ToString());
        }
        else if (isEnded == false) {
            EndTimer();
        }
    }

    void ResetTimer() {
        time_start = Time.time;
        time_current = 0f;
        text_timer.text = $"{time_current:N2}";
        isEnded = false;
        ActionCat.CatLog.Log("Timer Start");
    }

    void EndTimer() {
        ActionCat.CatLog.Log("Timer End");
        time_current = time_Max;
        text_timer.text = $"{time_current}";
        isEnded = true;
    }
}
