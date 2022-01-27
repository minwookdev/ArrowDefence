namespace ActionCat {
    using UnityEngine;
    using EasyMobile;

    public class AdsManager : Singleton<AdsManager> {
        public void InitAdsManager() {
            if(RuntimeManager.IsInitialized() == false) {
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
    }
}
