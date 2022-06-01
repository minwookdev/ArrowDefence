using System;
using UnityEngine;
using DG.Tweening;
using ActionCat;
using ActionCat.Interface;

public class MainSceneRoute : MonoBehaviour {
    static MainSceneRoute _inst;
    public static MainSceneRoute Inst {
        get => _inst;
    }
    [Header("CANVAS")]
    [SerializeField] RectTransform mainUICanvas = null;

    [Header("MENU")]
    [SerializeField] [ReadOnly] PANELTYPE openedPanelType = PANELTYPE.NONE;
    [SerializeField] GameObject[] panels = null;
    IMainMenu[] menus = new IMainMenu[] { };

    [Header("MODULE")]
    public BattlePopup battlePop;
    public ItemInfoPop itemInfoPop;
    [SerializeField] SettingsPanel settings = null;

    [Header("FADE")]
    public CanvasGroup ImgFade = null;
    public float FadeTime = 2.0f;

    [Header("SAVE & LOAD")]
    public bool isAutoLoad;

    [Header("Currency")]
    [SerializeField] TMPro.TextMeshProUGUI textGold  = null;
    [SerializeField] TMPro.TextMeshProUGUI textStone = null;

    [Header("I2")]
    [I2.Loc.TermsPopup]
    public string _stringWithTermPopup;

    private void Awake() {
        _inst = this;
        for (int i = 0; i < panels.Length; i++) {
            if (panels[i].TryGetComponent<IMainMenu>(out IMainMenu iMainMenu)) {
                menus = GameGlobal.AddArray<IMainMenu>(menus, iMainMenu);
            }
        }

        if(menus.Length != panels.Length) {
            CatLog.WLog("Not Full-Cached MainMenu Interface !");
        }

        //Try Initialize Manager Objects
        GameManager.Instance.Initialize();
        AdsManager.Instance.InitRuntimeMgr();
    }

    private void Start() {
        //Init Notify
        Notify.Inst.Init(mainUICanvas);

        //Fade Effect When Etnering The Battle Scene (if Don't Work This Function, Enable The DEV Variable)
        FadeOut();

        //User SaveData Auto Load
        if (isAutoLoad) {
            ActionCat.GameManager.Instance.AutoLoadUserData();
        }

        UpdatePlayerCurrency();

        //I2.Loc.LocalizedString localString = "ABILITY_DAMAGE";
        //string translate = localString;
        //CatLog.Log(translate);
        //CatLog.Log(_stringWithTermPopup);
        //I2.Loc.LocalizedString localString = _stringWithTermPopup;
        //CatLog.Log(localString);
        //string tempString = string.Format(formatString, "<color=green>", "</color>"); //<- Skill Description은 이런식? 으로 각각의 스킬 클래스 안에서 사용해주면 어떰?
        //CatLog.Log(tempString);

        //bool settingsExam = false;
        //ES3.Save<bool>("TestKey", settingsExam, )
        CatLog.Log($"Streaming Asset Path: {Application.streamingAssetsPath}");
        CatLog.Log($"Presistent Data Path: {Application.persistentDataPath}");
        CatLog.Log($"Data Path: {Application.dataPath}"); // -> Recommended
        CatLog.Log($"ES3 Settings FilePath: {ES3Settings.defaultSettings.FullPath}");

        //1. Application.dataPath의 경로로 잡고 진행,문제가 생기면 경로는 Presistent data path로 변경
        //2. 해당경로에 파일이 존재하는지 확인하고 없으면 GameSettings파일을 생성하도록 작성.
        //3. 게임이 시작되면 Player's Data와는 다르게 이 작업부터 시행해줌. PlayerData와는 다르게 관리하도록.
        //4. 의의는 각각의 다른 기기에서 유저세이브를 불러왔을 때, 기기에 저장된 GameSettings의 json은 온라인에 올리지 않고, PlayerData만 왔다갔다 하게끔 되는 것.

        //float tempValue = (float)0.125f / 0.125f;
        //CatLog.Log($"Calculate Grade: {Mathf.CeilToInt(tempValue)}");

        //string tempString = "This is Temp String"; // ---> 허용되지 않음
        //CatLog.Log(tempString);
        //tempString.Clear();
        //CatLog.Log(tempString);
        //string tempStringOne = "";
        //string tempStringTwo = "NotNull";
        //CatLog.Log($"TempStringOne is Empty?: {(tempStringOne.IsStringEmpty())}");
        //CatLog.Log($"TempStringTwo is Empty?: {(tempStringTwo.IsStringEmpty())}");
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            var allCraftingSlots = ActionCat.Data.CCPlayerData.infos.CraftingInfos;
            foreach (var slot in allCraftingSlots) {
                slot.Update();
                CatLog.Log("Increase All Crafting Timer.");
            }
        }

        if (Input.GetKeyDown(KeyCode.X)) {
            Notify.Inst.Message("This is Test Message.");
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            Notify.Inst.Message("This is Test Message. \n" +
                                "This is Test Message. \n" +
                                "This is Test Message.");
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            Notify.Inst.Message("This is Test Message. \n" +
                                "This is Test Message. \n" +
                                "This is Test Message. \n" +
                                "This is Test Message. "); // Notify Text Size Test !
        }
    }

    private void OnDestroy() {
        _inst = null;
    }

    #region BUTTON_EVENT

    //======================================================================================================================================
    //============================================================ [ MAIN MENU ] ===========================================================

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

    //======================================================================================================================================
    //============================================================== [ POPUP ] =============================================================

    public void BE_CLOSE_POPUP(GameObject go) {
        go.SetActive(false);
    }

    public void BE_SETTINGS() {
        settings.OpenPanel();
    }

    public void OpenBattlePopup(STAGETYPE type) {
        battlePop.EnablePopup(type);
    }

    #endregion

    //======================================================================================================================================
    //============================================================= [ ITEMINFO ] ===========================================================

    public static void OpenInfo_CraftingItem_Preview(AD_item previewitem) {
        _inst.itemInfoPop.OpenPreview_Crafting(previewitem);
    }

    public static void OpenInfo_DropListItem_Preview(AD_item previewItem) {
        _inst.itemInfoPop.OpenPreview_Crafting(previewItem, disableAmountText: true);
    }

    /// <summary>
    /// Open Item Information Popup When Click in the Invnetory Items
    /// </summary>
    /// <param name="item"></param>
    public static void OpenInfo_InventoryItem(AD_item item) {
        switch (item) {
            case Item_Consumable  conItem: _inst.itemInfoPop.OpenPopup(conItem);   break;
            case Item_Material    matItem: _inst.itemInfoPop.OpenPopup(matItem);   break; 
            case Item_Equipment equipItem: _inst.itemInfoPop.OpenPopup(equipItem); break;
            default: throw new System.NotImplementedException();
        }
    }

    //======================================================================================================================================
    //=============================================================== [ FADE ] =============================================================

    public static void FadeIn(Action startAction, Action completeAction) {
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

    private void FadeOut() {
        if (GameManager.Instance.IsDevMode) {
            return;
        }

        ImgFade.alpha = 1f;

        ImgFade.DOFade(0f, FadeTime)
               .OnStart(() => ImgFade.blocksRaycasts = false)
               .OnComplete(() => {  
                   ImgFade.blocksRaycasts = true;
                   ImgFade.gameObject.SetActive(false);
               });
    }

    //======================================================================================================================================
    //============================================================= [ CURRENCY ] ===========================================================

    public static void UpdatePlayerCurrency() {
        var currencyInfos = GameManager.Instance.GetPlayerCurrenciesToString;
        _inst.textGold.text  = currencyInfos[0];
        _inst.textStone.text = currencyInfos[1];
    }

    enum PANELTYPE {
        NONE,
        INVENTORY,
        CRAFT,
        SHOP,
        BATTLE
    }
}

namespace ActionCat.UI.MainMenu {
    using DG.Tweening;
    internal sealed class MainMenuTween {
        Sequence mainSequence  = null;
        float openMenuTime = 0.5f;
        float closeMenuTime = 0.3f;
        
        //PROPERTY
        public bool IsTweenPlaying {
            get {
                return mainSequence.IsActive();
            }
        }

        /// <summary>
        /// other가 지정되어 있다면, tween이 시작되기 전에, other object를 활성화 합니다.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="cg"></param>
        /// <param name="other"></param>
        public void MenuOpenTween(RectTransform rt, CanvasGroup cg, GameObject other = null) {
            rt.localScale = Vector3.zero;
            cg.alpha      = StNum.floatZero;
            if (other == null) {
                rt.gameObject.SetActive(true);
            }
            else {
                other.SetActive(true);
            }
            mainSequence = OpenSequence(rt, cg);
        }

        /// <summary>
        /// other가 지정되어 있다면, tween이 시작되기 전에, other object를 비 활성화 합니다.
        /// </summary>
        /// <param name="rt"></param>
        /// <param name="cg"></param>
        /// <param name="other"></param>
        public void MenuCloseTween(RectTransform rt, CanvasGroup cg, GameObject other = null) {
            cg.DOFade(StNum.floatZero, closeMenuTime)
              .OnStart(() => cg.blocksRaycasts = false)
              .OnComplete(() => {
                  cg.blocksRaycasts = true;
                  if(other == null) {
                      rt.gameObject.SetActive(false);
                  }
                  else {
                      other.SetActive(false);
                  }
              });
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

        internal MainMenuTween(float openTime, float closeTime) {
            openMenuTime  = openTime;
            closeMenuTime = closeTime;
        }
    }
}
