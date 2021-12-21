namespace ActionCat.Testing
{
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;
    using ActionCat.Data;

    public class DevTestPanel : MonoBehaviour
    {
        [Header("DEV MODE UI")]
        public MainSceneRoute MainSceneUI;
        public Image OpenButton;
        public Image DevPanel;

        [Header("PLAYER DATA")]
        public AD_PlayerData playerData;

        [Header("Ads")]
        [SerializeField] Image ImgInterstitialAds = null;
        [SerializeField] Image ImgRewardAds = null;
        [SerializeField] Color ColorNotReady;
        Color enabledColor;

        private float endPosX;
        private float startPosX;
        private bool isOpen = false;
        private WaitForSeconds awaitTime = new WaitForSeconds(.5f);

        private void OnEnable()
        {
            //startPosX = TestPanel.localPosition.x;
            //endPosX = startPosX + TestPanel.rect.width;
            //CatLog.Log($"Panel Pos X : {TestPanel.localPosition.x}");
            //CatLog.Log($"Rect Width : {TestPanel.rect.width}");
            //CatLog.Log($"endPosX : {endPosX.ToString()}");

            RectTransform panelRect = DevPanel.GetComponent<RectTransform>();
            startPosX = panelRect.localPosition.x;
            endPosX = startPosX + panelRect.rect.width;
        }

        private void Start() {
            //광고 매니저 초기화
            AdsManager.Instance.InitAdsManager();
            enabledColor = ImgInterstitialAds.color;
        }

        private void Update() {
            //Check the Ready interstitialAds
            if(AdsManager.Instance.IsReadyInterstitialAds()) {
                ImgInterstitialAds.color = enabledColor;
            }
            else {   
                ImgInterstitialAds.color = ColorNotReady;
            }

            //Check The Ready RewardAds
            if(AdsManager.Instance.IsReadyRewardAds()) {
                ImgRewardAds.color = enabledColor;
            }
            else {
                ImgRewardAds.color = ColorNotReady;
            }
        }

        #region ADS_BUTTON

        public void OnShowinterstitialAds() {
            AdsManager.Instance.ShowInterstitialAds();
        }

        public void OnShowRewardedAds() {
            AdsManager.Instance.ShowRewardedAds();
        }

        #endregion

        #region BUTTON_METHOD

        public void Button_PanelOpen()
        {
            if (isOpen == false)
            {
                DevPanel.transform.DOLocalMoveX(endPosX, 1f, false)
                    .OnStart(() => {
                    OpenButton.raycastTarget = false;
                    DevPanel.raycastTarget   = false;
                }).OnComplete(() => {
                    OpenButton.raycastTarget = true;
                    DevPanel.raycastTarget   = true;
                    isOpen = true;
                });
            }
            else
            {
                DevPanel.transform.DOLocalMoveX(startPosX, 1f, false)
                    .OnStart(() => {
                    OpenButton.raycastTarget = false;
                    DevPanel.raycastTarget   = false;
                }).OnComplete(() => {
                    OpenButton.raycastTarget = true;
                    DevPanel.raycastTarget   = true;
                    isOpen = false;
                });
            }
        }

        public void Button_SaveData()
        {
            CCPlayerData.SaveUserData();

            MainSceneUI.Message("SAVE USER DATA");
        }

        public void Button_LoadData()
        {
            CCPlayerData.LoadUserData();

            MainSceneUI.Message("LOAD USER DATA");
        }

        public void Button_Additems()
        {
            if(playerData == null)
            {
                CatLog.WLog("Player Data Scriptable Object is NULL");
                return;
            }

            var itemList = playerData.GetItemData();

            if (itemList.Count <= 0)
            {
                CatLog.Log("Player Data Scriptable has no Items"); 
                return;
            }

            foreach (var item in itemList)
            {
                CCPlayerData.inventory.AddItem(item, item.Item_Amount);
            }

            CatLog.Log($"{itemList.Count} 개의 아이템이 전달되었습니다.");

            MainSceneUI.Message($"INVENTORY IN {itemList.Count} ITEMS");
        }

        public void Button_ClearInventory()
        {
            CCPlayerData.inventory.Clear();
            CCPlayerData.equipments.Clear();

            MainSceneUI.Message("CLEAR INVENTORY, EQUIPMENTS DATA");
        }

        public void Button_PullingTypeChange()
        {
            switch (CCPlayerData.settings.PullingType)
            {
                case PULLINGTYPE.AROUND_BOW_TOUCH: CCPlayerData.settings.SetPullingType(PULLINGTYPE.FREE_TOUCH);       break;
                case PULLINGTYPE.FREE_TOUCH:       CCPlayerData.settings.SetPullingType(PULLINGTYPE.AROUND_BOW_TOUCH); break;
                case PULLINGTYPE.AUTOMATIC: break;
            }

            MainSceneUI.Message($"PULLING TYPE CHANGE, CURRENT TYPE {CCPlayerData.settings.PullingType.ToString()}");
        }

        #endregion

        //뒤집힌 그래픽에 대한 EventTrigger를 정상적으로 실행하지 않는 현상에 대한 기록할 것.
    }
}
