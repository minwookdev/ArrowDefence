namespace ActionCat {
    using UnityEngine;
    using EasyMobile;

    public class AdsManager : Singleton<AdsManager> {
        readonly string craftingAdsPlacement = "CraftingTimeSkip";
        readonly string upgradeAdsPlacement  = "UpgradeChanceIncrease";

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
            CatLog.WLog("AdsManager OnEnabled."); // <- Scene변경되면서 다시 호출 되는 지 확인 
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

        public bool IsReadyInterstitialAds() {
            if (Advertising.IsInterstitialAdReady() == true) return true;
            else                                             return false;
        }

        public bool IsReadyRewardAds() {
            if (Advertising.IsRewardedAdReady() == true) return true;
            else                                         return false;
        }

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
        public void ShowCraftingRewardedAds() {
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement))) {
                Advertising.ShowRewardedAd(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement));
            }
        }

        /// <summary>
        /// 업그레이드 광고 재생 가능여부 판단 후 재생
        /// </summary>
        public void ShowUpgradeRewradedAds() {
            if (Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement))) {
                Advertising.ShowRewardedAd(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement));
            }
        }

        public bool IsReadyRewardedAds(REWARDEDAD adsType) {
            switch (adsType) {
                case REWARDEDAD.DEFAULT:  return Advertising.IsRewardedAdReady();
                case REWARDEDAD.CRAFTING: return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(craftingAdsPlacement));
                case REWARDEDAD.UPGRADE:  return Advertising.IsRewardedAdReady(RewardedAdNetwork.AdMob, AdPlacement.PlacementWithName(upgradeAdsPlacement));
                default: throw new System.NotImplementedException();
            }
        }

        void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location) {
            ///NOTE
            ///switch (location.Name) { // <-- 대략 이런식으로 location.Name 비교해서 완료된 광고의 종류를 파악한 뒤에 처리하면 될 듯?
            ///    case craftingAdsPlacement: break;
            ///    case upgradeAdsPlacement:  break;
            ///    default: break;
            ///}
        }

        void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location) {

        }

    }

    public enum REWARDEDAD {
        DEFAULT,
        CRAFTING,
        UPGRADE,
    }
}
