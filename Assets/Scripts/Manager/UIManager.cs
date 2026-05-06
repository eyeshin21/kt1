using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.Events;

public class UIManager : Singleton<UIManager>
{
    const string pathIngameMenu = "UI/IngameMenu";
    const string pathMainMenu = "UI/MainMenu";
    const string pathTabMenu = "UI/TabMenu";
    const string pathWinMenu = "UI/WinMenu";
    const string pathShopSkinMenu = "UI/ShopSkinMenu";
    const string pathDailyBonusMenu = "UI/DailyBonusMenu";
    const string pathRateMenu = "UI/RateMenu";
    const string pathNoticeMenu = "UI/NoticeMenu";
    const string pathAdsLoadingMenu = "UI/AdsLoadingMenu";
    const string pathSettingMenu = "UI/SettingMenu";
    const string pathBuyItemMenu = "UI/BuyItemMenu";
    const string pathItemUsingMenu = "UI/ItemUsingMenu";
    const string pathRemoveAdsMenu = "UI/RemoveAdsMenu";
    const string pathStarterPackMenu = "UI/StarterPackMenu";
    const string pathShopMenu = "UI/ShopMenu";
    const string pathRefillMenu = "UI/RefillMenu";
    const string pathLoseMenu = "UI/LoseMenu";
    const string pathResetLevelMenu = "UI/ResetLevelMenu";
    const string pathRewardMenu = "UI/RewardMenu";
    const string pathStartRaceMenu = "UI/StartRaceMenu";
    const string pathRaceMenu = "UI/RaceMenu";
    const string pathNoelDealMenu = "UI/NoelDealMenu";
    const string pathLuckySpinMenu = "UI/LuckySpinMenu";
    const string pathCheatMenu = "UI/CheatMenu";
    const string pathDay3PassMenu = "UI/Day3PassMenu";
    const string pathNoticeHardMenu = "UI/NoticeHardMenu";
    const string pathNewBoosterMenu = "UI/NewBoosterMenu";
    const string pathNewElementMenu = "UI/NewElementMenu";
    const string pathOutOfSpaceMenu = "UI/OutOfSpaceMenu";
    const string pathTipMenu = "UI/TipMenu";
    const string pathReviveMenu = "UI/ReviveMenu";

    [Header("UI Panel")]
    public IngameMenu ingameMenu;
    public CGTeam.MainMenu mainMenu;
    public TabMenu tabMenu;
    [HideInInspector]
    public WinMenu winMenu;
    [HideInInspector]
    public ShopMenu shopMenu;    
    [HideInInspector]
    public RemoveAdsMenu removeAdsMenu;
    [HideInInspector]
    public StarterPackMenu starterPackMenu;
    [HideInInspector]
    public RatePanel rateMenu;
    [HideInInspector]
    public NoticePanel noticeMenu;
    [HideInInspector]
    public AdsLoadingMenu adsLoadingMenu;
    [HideInInspector]
    public SettingMenu settingMenu;
    [HideInInspector]
    public BuyItemMenu buyItemMenu;
    [HideInInspector]
    public ItemUsingMenu itemUsingMenu;
    [HideInInspector]
    public LoseMenu loseMenu;
    [HideInInspector]
    public RewardMenu rewardMenu;
    [HideInInspector]
    public CheatMenu cheatMenu;
    [HideInInspector]
    public NoticeHardMenu noticeHardMenu;
    [HideInInspector]
    public NewBoosterMenu newBoosterMenu;
    [HideInInspector]
    public NewElementMenu newElementMenu;
    [HideInInspector]
    public OutOfSpaceMenu outOfSpaceMenu;

    [Header("Canvas")]
    [SerializeField] Transform parentUI1;
    [SerializeField] Transform parentUI2;
    public GameObject blockUI;

    [Header("VFX")]
    public ParticleSystem vfxWin;

    [Header("Item")]
    public SpriteRenderer bgItem;

    [HideInInspector]
    public int stateShowPack;

    private void Start()
    {
        EventManager.StartListening(EventVariables.Purchased, Purchased);
        EventManager.StartListening("ShowAdsBundle", ShowRemoveAdsMenu);
        EventManager.StartListening("ShowStarterPackBundle", ShowStarterPackMenu);

        UserConfig.Instance.isLoading = false;
        SoundManager.instance.PlayBGMusic("Music");

        if (UserConfig.Instance.CurLevel < 16)
        {
            GameManager.Instance.LoadLevel();
            ShowIngameMenu();
        }
        else
        {
            ShowMainMenu();
        }

        //if (UserConfig.Instance.isNewDay)
        //{
        //    if (UserConfig.Instance.DailyLogin < 7)
        //    {
        //        ShowDailyBonusMenu();
        //    }
        //    else
        //    {
        //        UserConfig.Instance.DailyLogin++;
        //        PlayerPrefs.SetString(GameplayVariables.lastTimeOpen, System.DateTime.Now.Date.ToString());
        //    }
        //}
        //else
        //{
        //    bool hasShowMenu = false;
        //    if (UserConfig.Instance.IsRacing)
        //    {
        //        System.DateTime currentTime = System.DateTime.Now;
        //        int myDistance = UserConfig.Instance.BestLevel - UserConfig.Instance.LevelStartRace;

        //        int duration = ((int)(currentTime - UserConfig.Instance.StartTimeRace).TotalSeconds);

        //        if (duration >= RemoteConfig.Instance.TimeRace || myDistance >= RemoteConfig.Instance.DistanceRace)
        //        {
        //            hasShowMenu = true;
        //            ShowRaceMenu();
        //        }
        //    }

        //    if (!hasShowMenu)
        //    {
        //        if (UserConfig.Instance.BestLevel >= 2)
        //        {
        //            if (!UserConfig.Instance.hasBuyStarterPack)
        //            {
        //                stateShowPack = 2;
        //                ShowStarterPackMenu();
        //            }
        //            else if (!UserConfig.Instance.hasBuy3DayPassPack)
        //            {
        //                stateShowPack = 3;
        //                ShowDay3PassMenu();
        //            }else if(!UserConfig.Instance.hasBuyDeal1 || !UserConfig.Instance.hasBuyDeal2)
        //            {
        //                stateShowPack = 4;
        //                ShowNoelDealMenu();
        //            }
        //        }
        //    }
        //}
    }

    void Purchased()
    {
        if (noticeMenu == null || (noticeMenu != null && !noticeMenu.gameObject.activeInHierarchy))
        {
            ShowNoticeMenu("Successfully Purchase");
        }
    }

    public void ShowBGItem(UnityEvent evt)
    {
        bgItem.gameObject.SetActive(true);

        bgItem.color = new Color(0, 0, 0, 0);
        bgItem.DOFade(0.85f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            evt?.Invoke();
        });
    }

    public void ShowNoticeHardMenu()
    {
        if (noticeHardMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathNoticeHardMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            noticeHardMenu = go.GetComponent<NoticeHardMenu>();
        }
        noticeHardMenu.Show();
    }

    public void ShowRewardMenu(RewardData rewardData)
    {
        if (rewardMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathRewardMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            rewardMenu = go.GetComponent<RewardMenu>();
        }
        rewardMenu.rewardData = rewardData;
        rewardMenu.Show();
    }

    public void ShowLoseMenu()
    {
        if (loseMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathLoseMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            loseMenu = go.GetComponent<LoseMenu>();
        }
        loseMenu.Show();
    }

    public void ShowOutOfSpaceMenu()
    {
        if (outOfSpaceMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathOutOfSpaceMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            outOfSpaceMenu = go.GetComponent<OutOfSpaceMenu>();
        }
        outOfSpaceMenu.Show();
    }

    public void ShowItemUsingMenu(eBooster eBooster, bool isTut = false)
    {
        if (itemUsingMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathItemUsingMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            itemUsingMenu = go.GetComponent<ItemUsingMenu>();
        }

        itemUsingMenu.eBooster = eBooster;
        itemUsingMenu.isTut = isTut;
        itemUsingMenu.Show();
    }

    public void ShowNewBoosterMenu(eTypeBooster eTypeBooster)
    {
        if (newBoosterMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathNewBoosterMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            newBoosterMenu = go.GetComponent<NewBoosterMenu>();
        }
        newBoosterMenu.eTypeBooster = eTypeBooster;
        newBoosterMenu.Show();
    }

    public void ShowNewElementMenu(eTypeElement eTypeElement)
    {
        if (newElementMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathNewElementMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            newElementMenu = go.GetComponent<NewElementMenu>();
        }
        newElementMenu.eTypeElement = eTypeElement;
        newElementMenu.Show();
    }

    public void ShowBuyItemMenu(eBooster eBooster)
    {
        if (buyItemMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathBuyItemMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            buyItemMenu = go.GetComponent<BuyItemMenu>();
        }
        buyItemMenu.eBooster = eBooster;
        buyItemMenu.Show();
    }

    public void ShowSettingMenu()
    {
        if (settingMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathSettingMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            settingMenu = go.GetComponent<SettingMenu>();
        }
        settingMenu.Show();
    }

    public void ShowIngameMenu()
    {
        //if (ingameMenu == null)
        //{
        //    GameObject obj = Resources.Load<GameObject>(pathIngameMenu);
        //    GameObject go = Instantiate(obj);
        //    go.transform.SetParent(parentUI1);
        //    go.transform.localPosition = Vector3.zero;
        //    go.transform.localScale = Vector3.one;

        //    ingameMenu = go.GetComponent<IngameMenu>();
        //}

        ingameMenu.Show();
    }

    public void ShowMainMenu()
    {
        //if (mainMenu == null)
        //{
        //    GameObject obj = Resources.Load<GameObject>(pathMainMenu);
        //    GameObject go = Instantiate(obj);
        //    go.transform.SetParent(parentUI1);
        //    go.transform.localPosition = Vector3.zero;
        //    go.transform.localScale = Vector3.one;

        //    mainMenu = go.GetComponent<MainMenu>();
        //}

        mainMenu.Show();
    }
    
    public void ShowTabMenu()
    {
        //if (tabMenu == null)
        //{
        //    GameObject obj = Resources.Load<GameObject>(pathTabMenu);
        //    GameObject go = Instantiate(obj);
        //    go.transform.SetParent(parentUI2);
        //    go.transform.localPosition = Vector3.zero;
        //    go.transform.localScale = Vector3.one;

        //    tabMenu = go.GetComponent<TabMenu>();
        //}

        tabMenu.Show();
    }

    public void ShowAdsLoadingMenu(string content = "Ads Loading...")
    {
        if (adsLoadingMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathAdsLoadingMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            adsLoadingMenu = go.GetComponent<AdsLoadingMenu>();
        }
        adsLoadingMenu.content = content;
        adsLoadingMenu.Show();
    }

    public void ShowWinMenu()
    {
        ingameMenu.Hide();
        if (winMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathWinMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            winMenu = go.GetComponent<WinMenu>();
        }
        winMenu.Show();
    }
    
    public void ShowShopMenu()
    {
        if (shopMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathShopMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI1);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            shopMenu = go.GetComponent<ShopMenu>();
        }
        shopMenu.Show();
    }  
    
    public void ShowRemoveAdsMenu()
    {
        if (removeAdsMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathRemoveAdsMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            removeAdsMenu = go.GetComponent<RemoveAdsMenu>();
        }
        removeAdsMenu.Show();
    }

    public void ShowStarterPackMenu()
    {
        if (starterPackMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathStarterPackMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            starterPackMenu = go.GetComponent<StarterPackMenu>();
        }
        starterPackMenu.Show();
    }

    public void ShowRateMenu()
    {
        if (rateMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathRateMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            rateMenu = go.GetComponent<RatePanel>();
        }
        rateMenu.Show();
    }

    public void ShowNoticeMenu(string notice, bool checkInternet = false)
    {
        if (noticeMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathNoticeMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            noticeMenu = go.GetComponent<NoticePanel>();
        }

        noticeMenu.content = notice;
        noticeMenu.checkInternet = checkInternet;
        noticeMenu.Show();
    }

    public void ShowCheatMenu()
    {
        if (cheatMenu == null)
        {
            GameObject obj = Resources.Load<GameObject>(pathCheatMenu);
            GameObject go = Instantiate(obj);
            go.transform.SetParent(parentUI2);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;

            cheatMenu = go.GetComponent<CheatMenu>();
        }
        cheatMenu.Show();
    }

    #region Touch and Mouse
    public bool OnPointerDown()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            return true;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                return true;
            }
        }
#endif
        return false;
    }

    public bool OnPointerUp()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonUp(0))
        {
            return true;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                return true;
            }
        }
#endif
        return false;
    }

    public bool IsPoiterUI()
    {
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            // Check if finger is over a UI element
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
        }
#endif
        return false;
    }

    public Vector3 PositionTouch()
    {
#if UNITY_EDITOR
        return Input.mousePosition;
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            return Input.GetTouch(0).position;
        }
#endif
        return Vector3.zero;
    }
    #endregion
}
