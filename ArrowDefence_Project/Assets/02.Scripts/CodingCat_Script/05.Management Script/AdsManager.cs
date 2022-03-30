namespace ActionCat {
    using UnityEngine;
    using EasyMobile;

    public class AdsManager : Singleton<AdsManager> {
        readonly string craftingAdsPlacement = "CraftingTimeSkip";
        readonly string upgradeAdsPlacement  = "UpgradeChanceIncrease";
        GameObject requester = null;
        public string CraftingAdKey {
            get => craftingAdsPlacement;
        }

        public bool IsInitialized {
            get {
                return RuntimeManager.IsInitialized();
            }
        }

        private void OnEnable() { //AdsManager 함부로 껏다 켯다 하면 안댐
            Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped   += RewardedAdSkippedHandler;
            CatLog.WLog("AdsManager OnEnabled."); // <- Scene변경되면서 다시 호출 되는 지 확인, 더시 호출되버리면 위 이벤트들 또 넣어버리니깐 수정해야함
        }

        private void OnDisable() { //AdsManager 함부로 껏다 켯다 하면 안댐
            Advertising.RewardedAdCompleted -= RewardedAdCompletedHandler;
            Advertising.RewardedAdSkipped   -= RewardedAdSkippedHandler;
        }

        public void InitRuntimeMgr() {
            if (RuntimeManager.IsInitialized() == false) {
                RuntimeManager.Init();
            }
        }

        #region IS_READY

        public bool IsReadyInterstitialAds() {
            if (Advertising.IsInterstitialAdReady() == true) return true;
            else                                             return false;
        }

        public bool IsReadyRewardAds() {
            if (Advertising.IsRewardedAdReady() == true) return true;
            else                                         return false;
        }

        public bool IsReadyRewardedAds(REWARDEDAD adsType) {
            switch (adsType) {
                case REWARDEDAD.DEFAULT:  return Advertising.IsRewardedAdReady();
                case REWARDEDAD.CRAFTING: return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement));
                case REWARDEDAD.UPGRADE:  return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement));
                default: throw new System.NotImplementedException();
            }
        }

        #endregion

        #region SHOW

        public void ShowBannerAds() {
            Advertising.ShowBannerAd(BannerAdPosition.Bottom);
        }

        public void ShowInterstitialAds() {
            if (Advertising.IsInterstitialAdReady())
                Advertising.ShowInterstitialAd();
        }

        public void ShowRewardedAds() {
            if (Advertising.IsRewardedAdReady())
                Advertising.ShowRewardedAd();
        }

        /// <summary>
        /// 크래프팅 광고 재생 가능여부 판단 후 재생
        /// </summary>
        public void ShowCraftingRewardedAds(GameObject requester) {
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement))) {
                if (requester.TryGetComponent<UI.UI_Crafting>(out UI.UI_Crafting crafting) == false) {
                    CatLog.ELog("ERROR: The Requester Type Not Matched.");
                    return;
                }

                this.requester = requester;
                Advertising.ShowRewardedAd(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement));
            }
        }

        /// <summary>
        /// 업그레이드 광고 재생 가능여부 판단 후 재생
        /// </summary>
        public void ShowUpgradeRewardedAds(GameObject requester) {
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement))) {
                if (requester.TryGetComponent<UI.UI_Crafting>(out UI.UI_Crafting crafting) == false) {
                    CatLog.ELog("ERROR: The Requester Type Not Matched.");
                    return;
                }

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
        void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location) {
            ///현 시점 network는 비교하지 않음. 필요 시 Ads Network를 사용해서 비교할 것.
            ///

            if (location.Name == craftingAdsPlacement) {
                var crafting = requester.GetComponent<UI.UI_Crafting>();
                crafting.CompletedCraftingAds();

                CatLog.Log("보상형 광고 시청 완료, 보상 코드 수령됨. <확인용>");
            }
            else if (location.Name == upgradeAdsPlacement) {
                var upgrade = requester.GetComponent<UI.UI_Crafting>();
                upgrade.CompletedUpgradeAds();

                CatLog.Log("보상형 광고 시청 완료, 보상 코드 수령됨. <확인용>");
            }

            this.requester = null;
        }

        /// <summary>
        /// 보상형 광고 스킵 시 콜백
        /// </summary>
        /// <param name="network"></param>
        /// <param name="location"></param>
        void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location) {
            //광고가 스킵 판정나면 하이어라키 최-하단(우선순위)에 있는 광고-스킵팝업이 나와서 스킵됐다고 알려줌
        }

    }

    public enum REWARDEDAD {
        DEFAULT,
        CRAFTING,
        UPGRADE,
    }
}
