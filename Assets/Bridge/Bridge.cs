using CGTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class Bridge : Singleton<Bridge>
{
    private void Start()
    {
        RegisterPurchaseComplete();
        IAPManager.Instance.Initialize(new ProductDefinition(IAPManager.kStarterPack, ProductType.NonConsumable), new ProductDefinition(IAPManager.kCoin1, ProductType.Consumable), new ProductDefinition(IAPManager.kCoin2, ProductType.Consumable), new ProductDefinition(IAPManager.kCoin3, ProductType.Consumable), new ProductDefinition(IAPManager.kCoin4, ProductType.Consumable), new ProductDefinition(IAPManager.kCoin5, ProductType.Consumable), new ProductDefinition(IAPManager.kCoin6, ProductType.Consumable));
    }

    void RegisterPurchaseComplete()
    {
        IAPManager.Instance.OnPurchaseCompleted += (product) =>
        {
            Debug.Log("OnPurchaseCompleted: " + product.definition.id);

            if (product.definition.id == IAPManager.kCoin1)
            {
                IAPHandlePurchase.instance.BuyCoin1();
            }

            if (product.definition.id == IAPManager.kCoin2)
            {
                IAPHandlePurchase.instance.BuyCoin2();
            }

            if (product.definition.id == IAPManager.kCoin3)
            {
                IAPHandlePurchase.instance.BuyCoin3();
            }

            if (product.definition.id == IAPManager.kCoin4)
            {
                IAPHandlePurchase.instance.BuyCoin4();
            }

            if (product.definition.id == IAPManager.kCoin5)
            {
                IAPHandlePurchase.instance.BuyCoin5();
            }

            if (product.definition.id == IAPManager.kCoin6)
            {
                IAPHandlePurchase.instance.BuyCoin6();
            }

            if (product.definition.id == IAPManager.kStarterPack)
            {
                IAPHandlePurchase.instance.BuyStarterPackBundle();
            }
        };
    }

    /// <summary>
    /// Log khi vừa bắt đầu level
    /// </summary>
    /// <param name="level"></param> level đang chơi
    public void OnGameStarted(int level)
    {
        Debug.Log($"OnGameStarted: {level}");
        TinySauce.OnGameStarted(level);
    }

    /// <summary>
    /// Log khi kết thúc game. Bao gồm: win, lose, replay ingame, thoát ra home ở ingame
    /// </summary>
    /// <param name="levelComplete"></param> trạng thái game thắng hay thua
    /// <param name="score"></param> điểm của level. Số khay còn lại trong level
    /// <param name="level"></param> level đang chơi
    public void OnGameFinished(bool levelComplete, int score, int level)
    {
        Debug.Log($"OnGameFinished: {levelComplete}, {score}, {level}");
        TinySauce.OnGameFinished(levelComplete, score, level);
    }

    /// <summary>
    /// Log khi user ấn mua booster
    /// </summary>
    /// <param name="nameBooster"></param> tên của loại booster
    /// <param name="level"></param> level đang chơi
    public void TrackBuyBooster(string nameBooster, int level)
    {
        Debug.Log($"buy_booster_{nameBooster}, {level}");
        TinySauce.TrackCustomEvent("buy_booster_" + nameBooster, level);
    }

    /// <summary>
    /// Log khi user dùng booster
    /// </summary>
    /// <param name="nameBooster"></param> tên của loại booster
    /// <param name="level"></param> level đang chơi
    public void TrackUseBooster(string nameBooster, int level)
    {
        Debug.Log($"use_booster_{nameBooster}, {level}");
        TinySauce.TrackCustomEvent("use_booster_" + nameBooster, level);
    }

    /// <summary>
    /// Log Spend Resource
    /// </summary>
    /// <param name="level"></param> 
    /// <param name="item_type"></param> currency or booster
    /// <param name="name"></param> 
    /// <param name="amount"></param>
    /// <param name="spend_reason"></param>
    /// <param name="balance"></param>
    public void LogSpendResource(int level, string item_type, string name, int amount, string spend_reason, int balance)
    {
        
    }

    /// <summary>
    /// Log Earn Resource
    /// </summary>
    /// <param name="level"></param>
    /// <param name="item_type"></param> currency or booster
    /// <param name="name"></param>
    /// <param name="amount"></param>
    /// <param name="item"></param>
    /// <param name="balance"></param>
    public void LogEarnCurrency(int level, string item_type, string name, int amount, string item, int balance)
    {
        
    }

    public void Purchase(eIAPKey eIAPKey)
    {
        Debug.Log("Purchase: " + eIAPKey);
        //SetResumeAds(true);
        IAPManager.Instance.Purchase(GetKeyIAP(eIAPKey), () =>
        {
            //SetResumeAds(false);
        });
    }

    public void RestorePurchase()
    {
        Debug.Log("Restore Purchase");
#if UNITY_IOS
        //CGTeamBridge.instance.SetResumeAds(true);
        IAPManager.Instance.RestorePurchases((success) =>
        {
            //CGTeamBridge.instance.SetResumeAds(false);
        });
#endif
    }

    public string GetProductCurrencyFromStore(eIAPKey eIAPKey)
    {
        return IAPManager.Instance.GetProductCurrencyFromStore(GetKeyIAP(eIAPKey));
    }

    public decimal GetProductPriceFromStore(eIAPKey eIAPKey)
    {
        return IAPManager.Instance.GetProductPriceFromStore(GetKeyIAP(eIAPKey));
    }

    public string GetProductPriceStringFromStore(eIAPKey eIAPKey)
    {
        return IAPManager.Instance.GetProductPriceStringFromStore(GetKeyIAP(eIAPKey));
    }

    public bool IsNonConsumablePurchased(eIAPKey eIAPKey)
    {
        return IAPManager.Instance.IsNonConsumablePurchased(GetKeyIAP(eIAPKey));
    }

    string GetKeyIAP(eIAPKey eIAPKey)
    {
        string key = "";
        switch (eIAPKey)
        {
            case eIAPKey.kCoin1:
                key = IAPManager.kCoin1;
                break;
            case eIAPKey.kCoin2:
                key = IAPManager.kCoin2;
                break;
            case eIAPKey.kCoin3:
                key = IAPManager.kCoin3;
                break;
            case eIAPKey.kCoin4:
                key = IAPManager.kCoin4;
                break;
            case eIAPKey.kCoin5:
                key = IAPManager.kCoin5;
                break;
            case eIAPKey.kCoin6:
                key = IAPManager.kCoin6;
                break;
            case eIAPKey.kStarterPack:
                key = IAPManager.kStarterPack;
                break;
        }
        return key;
    }
}
