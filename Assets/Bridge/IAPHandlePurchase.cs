using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

public enum eIAPKey
{
    kRemoveAdsBundle,
    kCoin1,
    kCoin2,
    kCoin3,
    kCoin4,
    kCoin5,
    kCoin6,
    kStarterPack
}

public class IAPHandlePurchase : MonoBehaviour
{
    public static IAPHandlePurchase instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void BuyRemoveAdsBundle()
    {
        EventManager.EmitEvent(EventVariables.BuyRemoveAds);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 2000;
            UserConfig.Instance.AmountUndo += 5;
            UserConfig.Instance.AmountExtraSlot += 5;
            UserConfig.Instance.AmountSwap += 5;
        }
    }

    public void BuyStarterPackBundle()
    {
        EventManager.EmitEvent(EventVariables.BuyStarterPack);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 2400;
            UserConfig.Instance.AmountUndo += 1;
            UserConfig.Instance.AmountExtraSlot += 1;
            UserConfig.Instance.AmountSwap += 1;
            UserConfig.Instance.AmountMagnet += 1;
        }
    }

    public void BuyCoin1()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin1);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 1000;
        }
    }

    public void BuyCoin2()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin2);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 5000;
        }
    }

    public void BuyCoin3()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin3);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 10000;
        }
    }

    public void BuyCoin4()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin4);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 25000;
        }
    }

    public void BuyCoin5()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin5);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 50000;
        }
    }

    public void BuyCoin6()
    {
        EventManager.EmitEventData(EventVariables.PurchaseCoin, eIAPKey.kCoin6);

        if (UserConfig.Instance.isLoading)
        {
            UserConfig.Instance.Coin += 100000;
        }
    }
}
