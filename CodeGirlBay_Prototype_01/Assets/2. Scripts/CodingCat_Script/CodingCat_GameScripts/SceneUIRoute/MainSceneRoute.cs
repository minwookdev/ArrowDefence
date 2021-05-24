using CodingCat_Scripts;
using UnityEngine;
using DG.Tweening;

public class MainSceneRoute : MonoBehaviour
{
    #region Fade Effect Test Function
    //
    //public void FadeTestBtn() => StartFadeEffect(true);  
    //
    //private void StartFadeEffect(bool isFade)
    //{
    //    if (isFadePlaying) return;
    //
    //    if (isFade)
    //    {
    //        fadeCanvasGroup.gameObject.SetActive(true);
    //    }
    //
    //    StartCoroutine(this.Fade(isFade));
    //}
    //
    //private IEnumerator Fade(bool isFadeIn)
    //{
    //    isFadePlaying = true;
    //
    //    float timer = 0f;
    //
    //    while (timer <= 1f)
    //    {
    //        yield return null;
    //        timer += Time.deltaTime / fadeTime;
    //
    //        fadeCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
    //    }
    //
    //    if(!isFadeIn)
    //    {
    //        fadeCanvasGroup.gameObject.SetActive(false);
    //    }
    //
    //    isFadePlaying = false;
    //}
    //
    #endregion

    public GameObject currentOpenedMenu;
    public GameObject[] menuObjects = new GameObject[4];

    private Sequence exitSeq;
    private bool isTweenDone = true;
    private float openMenuTime = 0.5f;
    private float closeMenuTime = 0.2f;

    private void Start()
    {
        //스타트 시 초기 Scale 값 초기화
        //MenuOpen Tween에 잔상 방지 -> 추후 수정
        foreach (var item in menuObjects)
        {
            item.transform.localScale = Vector3.zero;
        }

        if(openMenuTime <= closeMenuTime)
        {
            CatLog.WLog("OpenMenuTime is less than CloseMenuTime. need to modify the variable.");
        }
    }

    public void OpenMenuItem(GameObject target)
    {
        if(target.activeSelf == false && isTweenDone)
        {
            if (currentOpenedMenu != null)
            {
                currentOpenedMenu.GetComponent<CanvasGroup>()
                                 .DOFade(0, closeMenuTime)
                                 .OnStart(() => currentOpenedMenu.GetComponent<CanvasGroup>()
                                                                 .blocksRaycasts = false)
                                 .OnComplete(() => { currentOpenedMenu.SetActive(false);
                                 });
            }

            this.MenuOpenTween(target.transform);
        }
        else
        {
            CatLog.Log("This Menu is Already Opened or Menu Openning");
        }
    }

    public void ExitMenuItem(GameObject target)
    {
        if(currentOpenedMenu == target)
        {
            //target.SetActive(false);
            //currentOpenedMenu = null;
            target.GetComponent<CanvasGroup>().DOFade(0, closeMenuTime)
                                              .OnStart(() => target.GetComponent<CanvasGroup>()
                                                                  .blocksRaycasts = false)
                                              .OnComplete(() => { target.SetActive(false);
                                                                  currentOpenedMenu = null;});
        }
        else
        {
            CatLog.WLog("Wrong Exit Button Route Check This Button. ");
        }
    }

    //매번 값을 넣어주는 방식이 아닌 Repeat처럼 활용할 방법은 없는지?
    private void MenuOpenTween(Transform target)
    {
        var targetCG = target.GetComponent<CanvasGroup>();

        exitSeq = DOTween.Sequence();
        exitSeq.SetAutoKill(true).
           OnStart(() => {
               this.isTweenDone = false;
               target.gameObject.SetActive(true);
               targetCG.blocksRaycasts = false;
               targetCG.alpha = 0;
               target.localScale = Vector3.zero;
           })
          .Append(target.DOScale(1f, openMenuTime))
          .Join(targetCG.DOFade(1f, openMenuTime))
          .OnComplete(() => { this.isTweenDone = true;
                              targetCG.blocksRaycasts = true;
                              currentOpenedMenu = target.gameObject;
          });
    }
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
