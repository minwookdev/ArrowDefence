using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using CodingCat_Scripts;

public class MainSceneRoute : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup fadeCanvasGroup;
    private bool isFadePlaying;

    public float fadeTime = 2f;

    [Header("Main Scene UI Elements")]
    public GameObject optionPanel;
    public GameObject battleScenePanel;

    private void Start()
    {
        //StartFadeEffect(false);
    }

    #region Btn_Actions

    public void OptionBtnAction() => optionPanel.SetActive(optionPanel.activeSelf ? false : true);

    public void BtnActionBattleSceneLoad() => SceneManagement.Instance.LoadBattleScene();

    public void BtnActionBattle() => battleScenePanel.SetActive(battleScenePanel.activeSelf ? false : true);

    public void BtnLoadStartScene() => SceneManagement.Instance.LoadStartScene();

    #endregion

    #region Fade Effect Test Function

    public void FadeTestBtn() => StartFadeEffect(true);  

    private void StartFadeEffect(bool isFade)
    {
        if (isFadePlaying) return;

        if (isFade)
        {
            fadeCanvasGroup.gameObject.SetActive(true);
        }

        StartCoroutine(this.Fade(isFade));
    }

    private IEnumerator Fade(bool isFadeIn)
    {
        isFadePlaying = true;

        float timer = 0f;

        while (timer <= 1f)
        {
            yield return null;
            timer += Time.deltaTime / fadeTime;

            fadeCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }

        if(!isFadeIn)
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        isFadePlaying = false;
    }

    #endregion
}

//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
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
