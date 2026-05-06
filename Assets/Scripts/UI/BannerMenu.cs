using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class BannerMenu : MonoBehaviour
{
    [SerializeField] GameObject bannerObj;

    private void Start()
    {
        EventManager.StartListening(EventVariables.BuyRemoveAds, BuyRemoveAds);
        EventManager.StartListening(EventVariables.BannerLoaded, BannerLoaded);

        //if (CGTeamBridge.instance.IsBannerShowing)
        //{
        //    bannerObj.SetActive(true);
        //}
    }

    void BannerLoaded()
    {
        bannerObj.SetActive(true);
    }

    void BuyRemoveAds()
    {
        bannerObj.SetActive(false);
    }

    public void PressedCloseBannerBtn()
    {
        //UIManager.Instance.ShowRemoveAdsMenu();
    }
}
