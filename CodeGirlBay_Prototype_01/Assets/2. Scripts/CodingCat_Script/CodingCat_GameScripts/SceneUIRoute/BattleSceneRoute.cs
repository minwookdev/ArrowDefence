using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using CodingCat_Scripts;

/*[System.Serializable]
public class BtnEvent
{

}

public class BattleSceneRoute : MonoBehaviour
{
    [SerializeField]
    private Text timeText = null;
    private static string gameTimeStr = null;

    public bool isStart = false;

    private void Start()
    {
        //timeText.text = BattleManager.Instance.
        //ActionBtn(PauseAction);
    }

    public BtnEvent btnEvent;

    public void OnOffObjAction(GameObject targetObj)
    {
        targetObj.SetActive(targetObj.activeSelf ? false : true);
    }

    public void PauseAction() => BattleManager.Instance.PauseTimeScale();

    public void ResumeAction() => BattleManager.Instance.DefaultTimeScale();

    public void BtnActionReturn()
    {
        BattleManager.Instance.DefaultTimeScale();
        BattleManager.Instance.InitialBattleSystem();

        SceneManagement.Instance.LoadMainScene();
    }

    private void ActionBtn(System.Action call)
    {
        call();
    }

    private void Update()
    {
        if(isStart)
        {
            BattleManager.Instance.isGameStart = true;
            isStart = false;
        }

        if (BattleManager.Instance != null)
        {
            if(BattleManager.Instance.isGameStart)
            {
                this.GameTimeTextUpdate();
            }
        }
    }

    private void GameTimeTextUpdate()
    {
        float timer = BattleManager.Instance.GameTime;
        gameTimeStr = string.Format("{0} sec", Mathf.Floor(timer).ToString());
        timeText.text = gameTimeStr;
    }
}*/
