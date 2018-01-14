using GoogleMobileAds.Api;

public class AdController
{
    private static BannerView bannerView;
    private static RewardBasedVideoAd videoAd;

    public static void ConnectAds()
    {
        SetDeviceAds();
    }

    public static void ShowVideo()
    {
        if (videoAd.IsLoaded())
            videoAd.Show();
    }

    public static void DestroyAds()
    {
        bannerView.Destroy();
    }

    private static void SetDeviceAds()
    {
#if UNITY_ANDROID
        string appId = ProjectConstants.AppId;
#endif
        MobileAds.Initialize(appId);
        videoAd = RewardBasedVideoAd.Instance;
        RequestBanner();
        RequestRewardedVideo();
    }

    private static void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = ProjectConstants.TestBannerAdId;
#endif
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        bannerView.LoadAd(request);
    }

    private static void RequestRewardedVideo()
    {
#if UNITY_ANDROID
        string adUnitId = ProjectConstants.TestVideoAdId;
#endif
        AdRequest request = new AdRequest.Builder().Build();
        videoAd.LoadAd(request, adUnitId);
        SetVideoCallbackHandlers();
    }

    private static void SetVideoCallbackHandlers()
    {
        videoAd.OnAdRewarded += HandleVideoReward;
    }

    private static void HandleVideoReward(object sender, Reward reward)
    {
        UnityEngine.Debug.Log("Reward received: " + reward.Type + " " + reward.Amount
             + " from" + sender.ToString());
        ProjectConstants.UpdateUserCredit((int)reward.Amount);
    }

}
