namespace ActionCat
{
    using UnityEngine;
    using EasyMobile;

    /// <summary>
    /// 광고 송출을 담당하는 Manager
    /// </summary>
    public class AdsManager : Singleton<AdsManager>
    {
        private void OnEnable()
        { // 보상형 광고 시청 보상 콜백 등록
            Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;
        }

        private void OnDisable()
        { // 비활성화 등록 해제
            Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped -= RewardedAdSkippedHandler;
        }

        // 보상형 광고 키
        readonly string craftingAdsPlacement = "CraftingTimeSkip";
        readonly string upgradeAdsPlacement = "UpgradeChanceIncrease";

        // 보상형 광고 송출 요청자 변수
        GameObject requester = null;

        public bool IsInitialized
        {
            get
            {
                return RuntimeManager.IsInitialized();
            }
        }

        public void InitRuntimeMgr()
        {
            if (RuntimeManager.IsInitialized() == false)
            {
                RuntimeManager.Init();
            }
        }

        #region IS_READY

        public bool IsReadyInterstitialAds()
        {
            if (Advertising.IsInterstitialAdReady() == true) return true;
            else return false;
        }

        public bool IsReadyRewardAds()
        {
            if (Advertising.IsRewardedAdReady() == true) return true;
            else return false;
        }

        public bool IsReadyRewardedAds(REWARDEDAD adsType)
        {
            switch (adsType)
            {
                case REWARDEDAD.DEFAULT: return Advertising.IsRewardedAdReady();
                case REWARDEDAD.CRAFTING: return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement));
                case REWARDEDAD.UPGRADE: return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement));
                default: throw new System.NotImplementedException();
            }
        }

        #endregion

        #region SHOW

        public void ShowBannerAds()
        {
            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
        }

        public void ShowRewardedAds()
        {
            if (Advertising.IsRewardedAdReady())
                Advertising.ShowRewardedAd();
        }

        /// <summary>
        /// 일반형 광고 송출 함수
        /// </summary>
        public void ShowInterstitialAds()
        {
            if (Advertising.IsInterstitialAdReady())
                Advertising.ShowInterstitialAd();
        }

        /// <summary>
        /// 보상형 (아이템 제작) 광고 재생 가능여부 판단 후 재생
        /// </summary>
        public void ShowCraftingRewardedAds(GameObject requester)
        {
            // 보상형 광고가 준비되었는지 광고 네트워크(AdManager, AdSense, Admob 등등) 와 광고키를 통해서 체크
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement)))
            {
                // 보상형 광고의 요청자가 올바른지 컴포넌트를 통한 체크 (아이템 조합에 대한 보상형 광고)
                if (requester.TryGetComponent<UI.UI_Crafting>(out UI.UI_Crafting crafting) == false)
                {
                    CatLog.ELog("The Requester Type Not Matched.");
                    return;
                }

                // (보상 지급 함수 호출을 위한) 요청자를 저장하고 광고를 송출
                this.requester = requester;
                Advertising.ShowRewardedAd(RewardedAdNetwork.AdMob, 
                    AdPlacement.PlacementWithName(craftingAdsPlacement));
            }
        }

        /// <summary>
        /// 업그레이드 광고 재생 가능여부 판단 후 재생
        /// </summary>
        public void ShowUpgradeRewardedAds(GameObject requester)
        {
            // 보상형 광고가 준비되었는지 광고 네트워크(AdManager, AdSense, Admob 등등) 와 광고키를 통해서 체크
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement)))
            {
                // 크래프팅 광고의 요청자가 올바른지 컴포넌트를 통한 체크
                if (requester.TryGetComponent<UI.UI_Crafting>(out UI.UI_Crafting crafting) == false)
                {
                    CatLog.ELog("The Requester Type Not Matched.");
                    return;
                }

                // 요청자를 저장하고 광고를 송출
                this.requester = requester;
                Advertising.ShowRewardedAd(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement));
            }
        }

        #endregion

        /// <summary>
        /// 보상형 광고 시청 완료 시 콜백
        /// </summary>
        /// <param name="network"></param>
        /// <param name="location"></param>
        void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location)
        {
            // NOTE: 현 시점 network는 비교하지 않음. 필요 시 Ads Network를 사용해서 비교할 것

            // 재생된 보상형 광고의 키를 비교하여 어떤 보상형 광고가 송출되었는지 확인
            // 제작 시간 단축 보상
            if (location.Name == craftingAdsPlacement)
            {
                // 요청자를 확인하고 보상 함수 실행
                var crafting = requester.GetComponent<UI.UI_Crafting>();
                crafting.CompletedCraftingAds();
            }
            // 강화 성공률 증가 보상
            else if (location.Name == upgradeAdsPlacement)
            {
                // 요청자를 확인하고 보상 함수 실행
                var upgrade = requester.GetComponent<UI.UI_Crafting>();
                upgrade.CompletedUpgradeAds();
            }
            // 매치되는 보상형 광고 키가 없음
            else
                return;

            // 요청자 초기화
            this.requester = null;
            CatLog.Log("보상형 광고 시청 완료, 보상 코드 수령");
        }


        /// <summary>
        /// 보상형 광고 스킵 시 콜백
        /// </summary>
        /// <param name="network"></param>
        /// <param name="location"></param>
        void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location)
        {
            //광고가 스킵 판정나면 하이어라키 최-하단(우선순위)에 있는 광고-스킵팝업이 나와서 스킵됐다고 알려줌
        }

    }

    public enum REWARDEDAD
    {
        DEFAULT,
        CRAFTING,
        UPGRADE,
    }
}
