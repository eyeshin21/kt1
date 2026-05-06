using CGTeam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;

public class CGTeamBridge : Singleton<CGTeamBridge>
{
    public int RetendDay
    {
        get
        {
            return PlayerPrefs.GetInt("retendDay", 0);
        }
        set
        {
            PlayerPrefs.SetInt("retendDay", value);
        }
    }

    public int DayPlayed
    {
        get
        {
            return PlayerPrefs.GetInt("dayPlayed", 0);
        }
        set
        {
            PlayerPrefs.SetInt("dayPlayed", value);
        }
    }

    public int DailyLogin
    {
        get
        {
            return dailyLogin;
        }
        set
        {
            dailyLogin = value;
            PlayerPrefs.SetInt("dailyLogin", value);
        }
    }

    private int dailyLogin;
    private bool canShowDailyBonus;
    private bool isFirebaseReady;

    float startTimeLevel = 0;
    float startTimeLevelReal = 0;

    private void Start()
    {
        CheckDailyLogin();

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
        };
    }

    public bool HasInternet()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return false;
        }
        return true;
    }

    #region ADS
    public void ShowMaxDebugger()
    {
    }

    public void SetResumeAds(bool resume)
    {
    }

    public void ShowBanner()
    {
    }

    public void HideBanner()
    {
    }

    public void ShowInterstitial(string placement, UnityEvent onClose)
    {
        onClose?.Invoke();
    }

    public void ShowRewarded(string placement, UnityEvent onStart, UnityEvent onCompleted, UnityEvent onFailed)
    {
        onStart?.Invoke();
        onCompleted?.Invoke();
    }

    public bool IsRewardReady()
    {
        return true;
    }
    #endregion

    #region Analytics
    void LogEvent(string eventName, Dictionary<string, object> parameters)
    {
        Debug.Log("Firebase Analytics: " + eventName);
    }

    float startTimeLoading;
    public void TrackLoadingStart(string placement)
    {
        startTimeLoading = Time.time;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="placement"></param>
    /// <param name="is_load"></param>
    public void TrackLoadingFinish(string placement, bool is_load)
    {
        float timeLoad = Time.time - startTimeLoading;
    }

    public void TrackCustomEvent(string eventName, Dictionary<string, object> parameters)
    {
        if (parameters != null)
        {
            foreach (KeyValuePair<string, object> kv in parameters)
            {
                Debug.Log(string.Format("FirebaseAnalytics: {0} - Parameters: {1} -- {2}", eventName, kv.Key, kv.Value));
            }
        }
        LogEvent(eventName, parameters);
    }

    public void TrackTutAction(string action_name)
    {
        var parameters = new Dictionary<string, object>
            {
                {"action_name", action_name}
            };
        TrackCustomEvent("tut_action", parameters);
    }

    public void LogLevelStartWithParameter(int play_count, int lose_count)
    {
        startTimeLevel = Time.time;
        startTimeLevelReal = Time.realtimeSinceStartup;

        //TinySauce.OnGameStarted(UserConfig.Instance.CurLevel);
    }

    public void LogLevelEndWithParamter(int play_count, int lose_count, bool success, string reason, int total_items, int cleared_items)
    {
        int timePlay = (int)(Time.time - startTimeLevel);
        int timePlayWithAds = (int)(Time.realtimeSinceStartup - startTimeLevelReal);
        int adsDuration = (timePlayWithAds - timePlay);

        //TinySauce.OnGameFinished(success, timePlay, UserConfig.Instance.CurLevel);
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
        var parameters = new Dictionary<string, object>
            {
                {"play_mode", "normal"},
                {"level", UserConfig.Instance.CurLevel},
                {"item_type", item_type},
                {"name", name},
                {"amount", amount},
                {"spend_reason", spend_reason},
                {"balance", balance}
            };
        TrackCustomEvent("resource_sink", parameters);
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
        var parameters = new Dictionary<string, object>
            {
                {"play_mode", "normal"},
                {"level", UserConfig.Instance.CurLevel},
                {"item_type", item_type},
                {"name", name},
                {"amount", amount},
                {"item", item},
                {"balance", balance}
            };
        TrackCustomEvent("resource_source", parameters);
    }

    void SetUserProperty(string name, string value)
    {
        Debug.Log(name + ": " + value);
    }

    public void SetLevelProperty(int level)
    {
        SetUserProperty("current_level", level.ToString());
    }

    #endregion

    #region Remote Config
    public int GetScoreRate()
    {
        return 4;
    }

    public int GetShowRateFrequency()
    {
        return 10;
    }
    #endregion

    #region Social
    [Header("Social")]
    private const string AndroidRatingURI = "market://details?id={0}";
    private const string IOSRatingURI = "https://apps.apple.com/app/id{0}";
    string url = string.Empty;
    [SerializeField]
    string STORE_APP_ID = string.Empty;
    public void RateGame()
    {
        Debug.Log("Rate Game");
    }

    public void OpenStore()
    {
#if UNITY_ANDROID
        url = AndroidRatingURI.Replace("{0}", Application.identifier);
#elif UNITY_IPHONE || UNITY_IOS
               url = IOSRatingURI.Replace("{0}", STORE_APP_ID);
                           if (!string.IsNullOrEmpty(url))
            {
                Application.OpenURL(url);
            }
            else
            {
                Debug.LogWarning("Unable to open URL, invalid OS");
            }
#endif
    }

    public void ShareGame()
    {

    }
    #endregion

    #region IAP
    public void Purchase(eIAPKey eIAPKey)
    {
        Debug.Log("Purchase: " + eIAPKey);

        var firebase_evt = new System.Collections.Generic.Dictionary<string, object>
            {
                {"level", UserConfig.Instance.CurLevel}
            };
        TrackCustomEvent("purchase_" + eIAPKey, firebase_evt);
        SetResumeAds(true);
    }

    public void RestorePurchase()
    {
        Debug.Log("Restore Purchase");
        //#if UNITY_IOS
        //        CGTeamBridge.instance.SetResumeAds(true);
        //        IAPManager.Instance.RestorePurchases((success) =>
        //        {
        //            CGTeamBridge.instance.SetResumeAds(false);
        //        });
        //#endif
    }

    public string GetProductCurrencyFromStore(eIAPKey eIAPKey)
    {
        return "";
    }

    public string GetProductPriceStringFromStore(eIAPKey eIAPKey)
    {
        return "";
    }

    public decimal GetProductPriceFromStore(eIAPKey eIAPKey)
    {
        return 0;
    }

    public bool IsNonConsumablePurchased(eIAPKey eIAPKey)
    {
        return true;
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
        }
        return key;
    }
    #endregion

    #region Daily Login
    void CheckDailyLogin()
    {
        if (DailyLogin == 0)
        {
            //SetPropertyDayPlayed((DailyLogin + 1).ToString());
            //SetPropertyRetendDay("0");
            PlayerPrefs.SetInt("retent_type", 0);

            canShowDailyBonus = true;
            return;
        }

        System.DateTime currentTimeDate = System.DateTime.Now.Date;
        System.DateTime lastTimeOpen = System.DateTime.Parse(PlayerPrefs.GetString("lastTimeOpen", System.DateTime.Now.ToString()));
        Debug.Log(PlayerPrefs.GetString("lastTimeOpen", System.DateTime.Now.ToString()));

        int dayOpen = (currentTimeDate - lastTimeOpen).Days;
        if (dayOpen > 0)
        {
            int retend_type = PlayerPrefs.GetInt("retent_type");
            retend_type += dayOpen;
            PlayerPrefs.SetInt("retent_type", retend_type);
            //SetPropertyRetendDay(retend_type.ToString());
            //SetPropertyDayPlayed((DailyLogin + 1).ToString());
            canShowDailyBonus = true;
        }
    }

    public bool CanShowDailyBonus()
    {
        return canShowDailyBonus;
    }

    public void DailyLoginSuccessful()
    {
        DailyLogin++;
        PlayerPrefs.SetString("lastTimeOpen", System.DateTime.Now.Date.ToString());
    }
    #endregion
}