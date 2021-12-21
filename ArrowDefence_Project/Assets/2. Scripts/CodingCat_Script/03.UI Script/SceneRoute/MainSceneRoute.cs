using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using ActionCat;

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

    private Sequence menuOpenSeq;
    private bool isTweenDone = true;
    private float openMenuTime = 0.5f;
    private float closeMenuTime = 0.2f; //Less than Menu Open Time.

    [Header("FADE EFFECT")]
    [Space(10)]
    public CanvasGroup ImgFade = null;
    public float FadeTime = 2.0f;

    [Header("POPUP'S")]
    [Space(10)]
    public BattlePopup battlePop;
    public ItemInfoPop itemInfoPop;

    [Header("MESSAGE")]
    public TMPro.TextMeshProUGUI MessageText;
    Sequence messageSeq;

    [Header("SAVE & LOAD")]
    public bool isAutoLoad;

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
            CatLog.WLog("OpenMenuTime is less than CloseMenuTime.");
        }

        //Set Message Tweening Initialize [Use Testing]
        messageSeq = DOTween.Sequence()
                            .Append(MessageText.DOFade(0f, 3f))
                            .Join(MessageText.transform.DOShakePosition(1f, 5f, 15, 90, false, true))
                            .SetAutoKill(false)
                            .Pause(); //Disable Auto Start

        //Auto Load SaveData
        if (isAutoLoad)
            ActionCat.GameManager.Instance.AutoLoadUserData();


    }

    private void OnDestroy() {
        _inst = null;
    }

    public void Message(string msg)
    {
        if (MessageText != null)
        {
            //MessageText.DOKill();
            MessageText.color = new Color(MessageText.color.r, MessageText.color.g, MessageText.color.b, 1f);
            MessageText.text = msg;

            messageSeq.Restart();
        }
    }

    /// <summary>
    /// Open Item Information Popup When Click in the Invnetory Items
    /// </summary>
    /// <param name="item"></param>
    public static void OpenItemInfo(AD_item item)
    {
        switch (item)
        {
            case Item_Consumable conItem: _inst.itemInfoPop.OpenPopup_ConsumableItem(conItem); break;
                //_inst.itemInfoPop.gameObject.SetActive(true);
                //_inst.itemInfoPop.Open_Popup_ConItem(conItem);
            case Item_Material matItem: _inst.itemInfoPop.OpenPopup_MaterialItem(matItem); break;
                //_inst.itemInfoPop.gameObject.SetActive(true);
                //_inst.itemInfoPop.Open_Popup_MatItem(matItem);   
            case Item_Equipment equipItem: _inst.itemInfoPop.OpenPopup_EquipmentItem(equipItem); break;
            //_inst.itemInfoPop.gameObject.SetActive(true);
            //_inst.itemInfoPop.Open_Popup_EquipItem(equipItem); 
            default: break;
        }
    }

    public static void Fade(Action startAction, Action completeAction) {
        _inst.ImgFade.DOFade(1f, _inst.FadeTime)
                     .OnStart(() => {
                         _inst.ImgFade.blocksRaycasts = false;
                         _inst.ImgFade.gameObject.SetActive(true);
                         startAction();
                     })
                     .OnComplete(() => {
                         completeAction();
                     });
    }

    public void OpenMenuItem(GameObject target)
    {
        if(target.activeSelf == false && isTweenDone)
        {
            if (currentOpenedMenu != null)
            {
                currentOpenedMenu.GetComponent<CanvasGroup>()
                                 .DOFade(0, closeMenuTime)
                                 .OnStart(() => currentOpenedMenu.GetComponent<CanvasGroup>().blocksRaycasts = false)
                                 .OnComplete(() => { currentOpenedMenu.SetActive(false);
                                 });
            }

            //this.MenuOpenTween(target.transform);
            menuOpenSeq = MenuOpenSequence(target.transform);
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

        menuOpenSeq = DOTween.Sequence();
        menuOpenSeq.SetAutoKill(true).
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

    Sequence MenuOpenSequence(Transform target)
    {
        var targetCG = target.GetComponent<CanvasGroup>();

        return DOTween.Sequence()
                      .SetAutoKill(true)
                      .OnStart(() =>
                      {
                          this.isTweenDone = false;
                          target.gameObject.SetActive(true);
                          targetCG.blocksRaycasts = false;
                          targetCG.alpha = 0f;
                          target.localScale = Vector3.zero;
                      })
                      .Append(target.DOScale(1f, openMenuTime))
                      .Join(targetCG.DOFade(1f, openMenuTime))
                      .OnComplete(() =>
                      {
                          this.isTweenDone = true;
                          targetCG.blocksRaycasts = true;
                          currentOpenedMenu = target.gameObject;
                      });
    }

    //public void StageSelect(int stagedata)
    //{
    //    Action<string> actPopup = (str) =>
    //    {
    //        battlePop.stageInfo = str;
    //        battlePop.gameObject.SetActive(true);
    //    };
    //
    //    switch (stagedata)
    //    {
    //        case (int)STAGELIST.STAGE_DEV:
    //            actPopup(AD_Data.STAGEINFO_DEV);
    //            break;
    //        case (int)STAGELIST.STAGE_FOREST:
    //            actPopup(AD_Data.STAGEINFO_FOREST);
    //            break;
    //        case (int)STAGELIST.STAGE_DESERT:
    //            actPopup(AD_Data.STAGEINFO_DESERT);
    //            break;
    //        case (int)STAGELIST.STAGE_DUNGEON:
    //            actPopup(AD_Data.STAGEINFO_DUNGEON);
    //            break;
    //        default:
    //            CatLog.WLog("Not Support This Stage");
    //            break;
    //    }
    //}

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
               .OnComplete(() => { SceneLoader.Instance.LoadScene(AD_Data.SCENE_BATTLE_DEV); });
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
               .OnComplete(() => SceneLoader.Instance.LoadScene(AD_Data.SCENE_TITLE));
    }

    private void OnSceneEnteringFadeOut()
    {
        if (ActionCat.GameManager.Instance.IsDevMode) return;

        ImgFade.alpha = 1f;

        ImgFade.DOFade(0f, FadeTime)
               .OnStart(() => ImgFade.blocksRaycasts = false)
               .OnComplete(() => 
               {   ImgFade.blocksRaycasts = true;
                   ImgFade.gameObject.SetActive(false);
               });

    }

    #region BATTLE_POPUP

    public void BattleStageSelector(int idx) {
        battlePop.EnablePopup(idx);
    }

    #endregion
}
