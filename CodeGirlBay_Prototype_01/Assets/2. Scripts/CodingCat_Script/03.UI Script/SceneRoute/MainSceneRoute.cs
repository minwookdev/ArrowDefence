using System;
using UnityEngine;
using DG.Tweening;
using CodingCat_Games;
using CodingCat_Scripts;
using System.Linq;
using UnityEditor;

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

    static MainSceneRoute _inst;

    public GameObject currentOpenedMenu;
    [Space(10)]
    public GameObject[] menuObjects = new GameObject[4];

    private Sequence exitSeq;
    private bool isTweenDone = true;
    private float openMenuTime = 0.5f;
    private float closeMenuTime = 0.2f; //Less than Menu Open Time.

    [Header("Fade Effect Option")]
    [Space(10)]
    public CanvasGroup ImgFade = null;
    public float FadeTime = 2.0f;

    [Header("DEV Options")]
    public bool IsDevMode = true;

    [Header("Popup")]
    [Space(10)]
    public BattlePopup battlePop;
    public ItemInfoPop itemInfoPop;

    public enum STAGELIST
    {
        STAGE_FOREST  = 1,
        STAGE_DESERT  = 2,
        STAGE_DUNGEON = 3,
        STAGE_DEV     = 0
    }

    private void Awake() => _inst = this;

    private void Start()
    {
        //스타트 시 초기 Scale 값 초기화 (테스트용)
        //MenuOpen Tween에 잔상 방지 -> 추후 수정
        foreach (var item in menuObjects)
        {
            if (item.activeSelf)
            {
                currentOpenedMenu = item;
            }
            else
            {
                item.transform.localScale = Vector3.zero;
            }
        }

        //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
        this.OnSceneEnteringFadeOut();

        if (openMenuTime <= closeMenuTime)
        {
            CatLog.WLog("OpenMenuTime is less than CloseMenuTime. need to modify the variable.");
        }

        Action TestAction = () =>
        {
            //List<string> TestIntList = new List<string> { "Animal_Cat", "Animal_Dog", "Animal_Rabbit" };
            //string DebugString = null;
            //foreach (var item in TestIntList)
            //{
            //    DebugString += $"{item}, ";
            //}
            //CatLog.Log("Test string List Value : " + DebugString);
            //DebugString = null;

            //var Num3 = TestIntList.FindAll(x => x == 3);
            //CatLog.Log(Num3.Count.ToString() + "개 만큼의 객체를 찾았습니다");
            //for(int i = 0; i < Num3.Count; i++)
            //{
            //    CatLog.Log("변경 대상 객체 : " + Num3[i].ToString());
            //    Num3[i] = 5;
            //    CatLog.Log($"{Num3[i].ToString()}로 값이 변경되었음");
            //}
            //CatLog.Log("Num3 int value : " + Num3[0].ToString());

            //값이 3인 변수를 5로 변경
            //TestIntList.FindAll(x => x == 3).ForEach(x => x = 5);
            //foreach(var item in TestIntList.FindAll(x => x == 3))
            //{
            //    item = 5;
            //}

            //TestIntList.Where(x => x == 3).ToList().ForEach(s => s = 5);

            //var index = TestIntList.FindIndex(x => x == 3); //정상작동
            //TestIntList[index] = 5;

            //var findNum = TestIntList.FindAll(x => x == "Animal_Cat");
            //for(int i =0;i<findNum.Count;i++)
            //{
            //    findNum[i] = "Animal_Whale";
            //}

            //TestIntList.FirstOrDefault

            //foreach(var item in TestIntList.FindAll(x => x == 3))
            //{
            //    item = 5;
            //}

            //foreach (var item in TestIntList)
            //{
            //    DebugString += $"{item}, ";
            //}
            //CatLog.Log("Test string List Value : " + DebugString);

            //변경되지 않음..FindAll로 찾은 객체를 깊은 복사로 인해 완전히 다른 List로 되는건지

        }; TestAction();
    }

    /// <summary>
    /// Open Item Information Popup When Click in the Invnetory Items
    /// </summary>
    /// <param name="item"></param>
    public static void OpenItemInfo(AD_item item)
    {
        switch (item)
        {
            case Item_Consumable conItem:
                _inst.itemInfoPop.gameObject.SetActive(true);
                _inst.itemInfoPop.Open_Popup_ConItem(conItem);
                break;
            case Item_Material matItem:
                _inst.itemInfoPop.gameObject.SetActive(true);
                _inst.itemInfoPop.Open_Popup_MatItem(matItem);     
                break;
            case Item_Equipment equipItem:
                _inst.itemInfoPop.gameObject.SetActive(true);
                _inst.itemInfoPop.Open_Popup_EquipItem(equipItem); 
                break;
            default: break;
        }
    }

    /// <summary>
    /// Update Inventory UI Method
    /// </summary>
    public static void UpdateInvenUI()
    {

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

    public void StageSelect(int stagedata)
    {
        Action<string> actPopup = (str) =>
        {
            battlePop.SelectStage = str;
            battlePop.gameObject.SetActive(true);
        };

        switch (stagedata)
        {
            case (int)STAGELIST.STAGE_DEV:
                actPopup(AD_Data.StageInfoDev);
                break;
            case (int)STAGELIST.STAGE_FOREST:
                actPopup(AD_Data.StageInfoForest);
                break;
            case (int)STAGELIST.STAGE_DESERT:
                actPopup(AD_Data.StageInfoDesert);
                break;
            case (int)STAGELIST.STAGE_DUNGEON:
                actPopup(AD_Data.StageInfoDungeon);
                break;
            default:
                CatLog.WLog("Not Support This Stage");
                break;
        }
    }

    public void Button_ClosePopup(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void OnBtnLoadBattle()
    {
        //현재 DEV 스테이지에 한해서 Scene 이동하도록 구현
        ImgFade.DOFade(1f, FadeTime)
               .OnStart(() =>
               {   ImgFade.blocksRaycasts = false;
                   ImgFade.gameObject.SetActive(true);
               })
               .OnComplete(() => { SceneLoader.Instance.LoadScene(AD_Data.Scene_Battle_Dev); });
    }

    public void OnBtnLoadTitle()
    {
        //현재 Settings Button 을 누르면 바로 Title Scene 으로 이동하도록 구현
        ImgFade.DOFade(1f, FadeTime)
               .OnStart(() =>
               {
                   ImgFade.blocksRaycasts = false;
                   ImgFade.gameObject.SetActive(true);
               })
               .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.Scene_Title));
    }

    private void OnSceneEnteringFadeOut()
    {
        if (IsDevMode) return;

        ImgFade.alpha = 1f;

        ImgFade.DOFade(0f, FadeTime)
               .OnStart(() => ImgFade.blocksRaycasts = false)
               .OnComplete(() => 
               {   ImgFade.blocksRaycasts = true;
                   ImgFade.gameObject.SetActive(false);
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
