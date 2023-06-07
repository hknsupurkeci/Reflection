using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdController : MonoBehaviour
{
    public static AdController current;
    public BannerView bannerView;
    public InterstitialAd interstitial;
    public RewardedAd rewardedAd;

    string adUnitIdBanner = "ca-app-pub-9324963738813081/1838166320";
    string adUnitInit = "ca-app-pub-9324963738813081/9525084658";
    string adUnitReward = "ca-app-pub-9324963738813081/6319518575";
    public void InitializeAds()
    {
        current = this;
        //load ads
        MobileAds.Initialize(initStatus => { });
        this.RequestBanner();
        this.RequestInterstitial();
        this.RequestRewardAdRequest();
    }
    private void RequestBanner()
    {

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitIdBanner, AdSize.Banner, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }
    private void RequestInterstitial()
    {

        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitInit);
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpening;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    private void HandleOnAdClosed(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        RequestInterstitial();
    }

    private void HandleOnAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
    }

    private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestInterstitial();
    }

    public void RequestRewardAdRequest()
    {

        this.rewardedAd = new RewardedAd(adUnitReward);

        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        Time.timeScale = 1;
        RequestRewardAdRequest();
    }

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        //reward
    }

    private void HandleRewardedAdOpening(object sender, EventArgs e)
    {
        Time.timeScale = 0;
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
    {
        RequestRewardAdRequest();
    }
}
