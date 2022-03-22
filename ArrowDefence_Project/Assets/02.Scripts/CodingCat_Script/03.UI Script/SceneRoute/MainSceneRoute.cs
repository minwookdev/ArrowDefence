using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using ActionCat;
using ActionCat.UI;
using ActionCat.Interface;

public class MainSceneRoute : MonoBehaviour {
    static MainSceneRoute _inst;

    public GameObject currentOpenedMenu;
    [Space(10)]
    public GameObject[] menuObjects = new GameObject[4];

    private Sequence menuOpenSeq;
    private bool isTweenDone = true;
    private float openMenuTime = 0.5f;
    private float closeMenuTime = 0.2f; //Less than Menu Open Time.

    [Header("CANVAS")]
    [SerializeField] RectTransform parentRectTr = null;

    [Header("FADE EFFECT")]
    [Space(10)]
    public CanvasGroup ImgFade = null;
    public float FadeTime = 2.0f;

    [Header("POPUP'S")]
    [Space(10)]
    public BattlePopup battlePop;
    public ItemInfoPop itemInfoPop;

    [Header("PANELS")]
    [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
    [SerializeField] UI_Inventory panelInventory = null;
    [SerializeField] UI_Crafting panelCrafting   = null;
    [SerializeField] UI_BattleSelect panelBattle = null;
    [SerializeField] UI_Shop panelShop           = null;
    [SerializeField] IMainMenu[] menus = null;

    [Header("SAVE & LOAD")]
    public bool isAutoLoad;

    private void Awake() => _inst = this;

    private void Start() {
        GameManager.Instance.Initialize();

        //스타트 시 초기 Scale 값 초기화 (테스트용)
        //MenuOpen Tween에 잔상 방지 -> 추후 수정
        foreach (var item in menuObjects) {
            if (item.activeSelf) {
                currentOpenedMenu = item;
            }
            else {
                item.transform.localScale = Vector3.zero;
            }
        }

        //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
        this.OnSceneEnteringFadeOut();

        if (openMenuTime <= closeMenuTime) {
            CatLog.WLog("OpenMenuTime is less than CloseMenuTime.");
        }

        //Auto Load SaveData
        if (isAutoLoad)
            ActionCat.GameManager.Instance.AutoLoadUserData();

        //Init Notify
        Notify.Inst.Init(parentRectTr);

        //string testString = "785162";
        //string subString = testString.Substring(0, 1);
        //CatLog.Log($"Sub String is : {subString}");
        //if (subString.Equals("8")) {
        //    CatLog.Log("SubStirng is Matched.");
        //}
        //else {
        //    CatLog.Log("SubString is Not Matched.");
        //}

        //System.Collections.Generic.List<string> stringList = new System.Collections.Generic.List<string>();
        //stringList.Add("A");
        //stringList.Add("B");
        //stringList.Add("C");
        //stringList.Add("D");
        //stringList.Add("E");
        //stringList.Add("F");
        //stringList.Add("G"); //:7
        //
        //int stringListCount = 0;
        //for (int i = stringList.Count - 1; i >= 0; i--) {
        //    CatLog.Log($"{stringList[i]}");
        //}

        //CatLog.Log($"String List Count: {stringListCount}");
        //string testString = I2.Loc.ScriptLocalization.Battle;
        //CatLog.Log(testString);

        //var tempArray = new string[3] { "A", "B", "C" };
        //var tempArraySeconds = new string[3] { "A", "A", "A" };
        //
        //var resultFirst   = tempArray.TrueForAll(element => element == "A");
        //var resultSeconds = tempArraySeconds.TrueForAll(element => element == "A");
        //CatLog.Log($"{resultFirst}");
        //CatLog.Log($"{resultSeconds}");
    }

    #region BUTTON_EVENT

    public void BE_OPEN_MAINMENU(int index) {
        //Ignore Panel
        bool isPossibleMenuChange = (menus.TrueForAll(menu => menu.IsTweenPlaying() == false));
        switch (index) {
            case 0: if (openedPanelType == PANELTYPE.INVENTORY) isPossibleMenuChange = false; break;
            case 1: if (openedPanelType == PANELTYPE.CRAFT)     isPossibleMenuChange = false; break;
            case 2: if (openedPanelType == PANELTYPE.SHOP)      isPossibleMenuChange = false; break;
            case 3: if (openedPanelType == PANELTYPE.BATTLE)    isPossibleMenuChange = false; break;
            default: throw new System.NotImplementedException();
        }
        if (isPossibleMenuChange == false) {
            CatLog.Log("Canceled Menu Change.");
            return;
        }

        //Close Opened Panel
        switch (openedPanelType) {
            case PANELTYPE.NONE:                            break;
            case PANELTYPE.INVENTORY: menus[0].MenuClose(); break;
            case PANELTYPE.CRAFT:     menus[1].MenuClose(); break;
            case PANELTYPE.SHOP:      menus[2].MenuClose(); break;
            case PANELTYPE.BATTLE:    menus[3].MenuClose(); break;
            default: throw new System.NotImplementedException();
        }

        //Open Panel
        switch (index) {
            case 0: menus[0].MenuOpen(); openedPanelType = PANELTYPE.INVENTORY; break;
            case 1: menus[1].MenuOpen(); openedPanelType = PANELTYPE.CRAFT;     break;
            case 2: menus[2].MenuOpen(); openedPanelType = PANELTYPE.SHOP;      break;
            case 3: menus[3].MenuOpen(); openedPanelType = PANELTYPE.BATTLE;    break;
            default: throw new System.NotImplementedException();
        }
    }

    public void BE_CLOSE_MAINMENU(int index) {
        switch (index) {
            case 0: menus[0].MenuClose(); break;
            case 1: menus[1].MenuClose(); break;
            case 2: menus[2].MenuClose(); break;
            case 3: menus[3].MenuClose(); break;
            default: throw new System.NotImplementedException();
        }
        openedPanelType = PANELTYPE.NONE;
    }

    #endregion

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            var allCraftingSlots = ActionCat.Data.CCPlayerData.infos.CraftingInfos;
            foreach (var slot in allCraftingSlots) {
                slot.Update();
            }
        }
    }

    private void OnDestroy() {
        _inst = null;
    }

    public static void OpenPreview(AD_item previewitem) {
        _inst.itemInfoPop.OpenPreview(previewitem);
    }

    /// <summary>
    /// Open Item Information Popup When Click in the Invnetory Items
    /// </summary>
    /// <param name="item"></param>
    public static void OpenItemInfo(AD_item item) {
        switch (item) {
            case Item_Consumable  conItem: _inst.itemInfoPop.OpenPopup_ConsumableItem(conItem);  break;
            case Item_Material    matItem: _inst.itemInfoPop.OpenPopup_MaterialItem(matItem);    break; 
            case Item_Equipment equipItem: _inst.itemInfoPop.OpenPopup_EquipmentItem(equipItem); break;
            default: throw new System.NotImplementedException();
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

    enum PANELTYPE {
        NONE,
        INVENTORY,
        CRAFT,
        SHOP,
        BATTLE
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
}

namespace ActionCat.UI.MainMenu {
    using DG.Tweening;
    internal sealed class MainMenuTween {
        Sequence menuSequence  = null;
        Sequence closeSequence = null;
        float openMenuTime = 0.5f;
        float closeMenuTime = 0.3f;
        
        //PROPERTY
        public bool IsTweenPlaying {
            get {
                return menuSequence.IsPlaying();
            }
        }

        public void MenuOpenTween(RectTransform rt, CanvasGroup cg) {
            rt.localScale = Vector3.zero;
            cg.alpha      = StNum.floatZero;
            rt.gameObject.SetActive(true);
            menuSequence = OpenSequence(rt, cg);
        }

        public void MenuCloseTween(RectTransform rt, CanvasGroup cg) {
            cg.DOFade(StNum.floatZero, closeMenuTime)
              .OnStart(() => cg.blocksRaycasts = false)
              .OnComplete(() => {
                  cg.blocksRaycasts = true;
                  rt.gameObject.SetActive(false);
              });
        }

        internal MainMenuTween(float openTime, float closeTime) {
            openMenuTime  = openTime;
            closeMenuTime = closeTime;
        }

        Sequence OpenSequence(RectTransform rt, CanvasGroup cg) {
            return DOTween.Sequence()
                          .SetAutoKill(true)
                          .OnStart(() => {
                              cg.blocksRaycasts = false;
                          })
                          .Append(rt.DOScale(StNum.floatOne, openMenuTime))
                          .Join(cg.DOFade(StNum.floatOne, openMenuTime))
                          .OnComplete(() => {
                              cg.blocksRaycasts = true;
                          });
        }

        Sequence CloseSequence(RectTransform rt, CanvasGroup cg) {
            return DOTween.Sequence()
                          .SetAutoKill(true)
                          .OnStart(() => {
                              cg.blocksRaycasts = false;
                          })
                          .Append(rt.DOScale(StNum.floatZero, closeMenuTime))
                          .Join(cg.DOFade(StNum.floatZero, closeMenuTime))
                          .OnComplete(() => {
                              cg.blocksRaycasts = false;
                          });
        }
    }
}
