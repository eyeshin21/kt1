using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum eBooster
{
    Undo = 0,
    ExtraSlot = 1,
    Swap = 2,
    AddSlot = 3,
    Magnet = 4
}

public class IngameMenu : UIMenu
{
    public TutorialUI tutorialUI;

    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] GameObject booster;
    [SerializeField] Transform trsfBoard;
    [SerializeField] Transform trsfBottom;
    [SerializeField] int[] prices;
    [SerializeField] GameObject objHard;
    [SerializeField] GameObject objSuperHard;

    [Header("Undo")]
    [SerializeField] TextMeshProUGUI amountUndoTxt;
    [SerializeField] GameObject objAddUndo;
    [SerializeField] GameObject objPriceUndo;
    [SerializeField] TextMeshProUGUI txtPriceUndo;
    [SerializeField] GameObject lockUndo;
    [SerializeField] GameObject unlockUndo;
    [SerializeField] TextMeshProUGUI levelUnlockUndoTxt;
    [SerializeField] CanvasGroup undoGroup;
    [SerializeField] AnimatedButton undoBtn;
    [SerializeField] GameObject blockUndo;

    [Header("ExtraSlot")]
    [SerializeField] TextMeshProUGUI amountExtraSlotTxt;
    [SerializeField] GameObject objPriceExtraSlot;
    [SerializeField] GameObject objAddExtraSlot;
    [SerializeField] TextMeshProUGUI txtPriceExtraSlot;
    [SerializeField] GameObject lockExtraSlot;
    [SerializeField] GameObject unlockExtraSlot;
    [SerializeField] TextMeshProUGUI levelUnlockExtraSlotTxt;

    [Header("Swap")]
    [SerializeField] TextMeshProUGUI amountSwapTxt;
    [SerializeField] GameObject objAddSwap;
    [SerializeField] GameObject objPriceSwap;
    [SerializeField] TextMeshProUGUI txtPriceSwap;
    [SerializeField] GameObject lockSwap;
    [SerializeField] GameObject unlockSwap;
    [SerializeField] TextMeshProUGUI levelUnlockSwapTxt;

    [Header("Magnet")]
    [SerializeField] TextMeshProUGUI amountMagnetTxt;
    [SerializeField] GameObject objAddMagnet;
    [SerializeField] GameObject objPriceMagnet;
    [SerializeField] TextMeshProUGUI txtPriceMagnet;
    [SerializeField] GameObject lockMagnet;
    [SerializeField] GameObject unlockMagnet;
    [SerializeField] TextMeshProUGUI levelUnlockMagnetTxt;

    bool canControlUI = true;

    public override void Start()
    {
        base.Start();

        EventManager.StartListening(EventVariables.EndGame, EndGame);
        EventManager.StartListening(EventVariables.LoadLevel, LoadLevel);
        EventManager.StartListening(EventVariables.AddItem, AddItem);
        EventManager.StartListening(EventVariables.Revive, Revive);

        levelUnlockUndoTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO);
        levelUnlockExtraSlotTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT);
        levelUnlockSwapTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP);
        levelUnlockMagnetTxt.text = string.Format("Lv {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET);
    }

    void Revive()
    {
        GameManager.Instance.canControl = true;
        canControlUI = true;
    }

    void LoadLevel()
    {
        canControlUI = true;
    }

    void EndGame()
    {
        canControlUI = false;
    }

    void AddItem()
    {
        int idItem = EventManager.GetInt(EventVariables.AddItem);
        switch ((eBooster)idItem)
        {
            case eBooster.Undo:
                SetStateUndoItem();
                break;
            case eBooster.ExtraSlot:
                SetStateExtraSlotItem();
                break;
            case eBooster.Swap:
                SetStateSwapItem();
                break;
            case eBooster.Magnet:
                SetStateMagnetItem();
                break;
        }
    }

    void SetStateUndoItem()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO)
        {
            unlockUndo.SetActive(true);
            lockUndo.SetActive(false);

            if (UserConfig.Instance.AmountUndo > 0)
            {
                amountUndoTxt.transform.parent.gameObject.SetActive(true);
                amountUndoTxt.gameObject.SetActive(true);
                amountUndoTxt.text = UserConfig.Instance.AmountUndo.ToString();
                objAddUndo.SetActive(false);
                //objPriceUndo.SetActive(false);
            }
            else
            {
                objAddUndo.SetActive(true);
                amountUndoTxt.transform.parent.gameObject.SetActive(false);
                amountUndoTxt.gameObject.SetActive(false);
                //objPriceUndo.SetActive(true);
                //txtPriceUndo.text = prices[(int)eBooster.Undo].ToString();
            }
        }
        else
        {
            unlockUndo.SetActive(false);
            lockUndo.SetActive(true);
        }
    }

    void SetStateExtraSlotItem()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT)
        {
            unlockExtraSlot.SetActive(true);
            lockExtraSlot.SetActive(false);

            if (UserConfig.Instance.AmountExtraSlot > 0)
            {
                amountExtraSlotTxt.transform.parent.gameObject.SetActive(true);
                amountExtraSlotTxt.gameObject.SetActive(true);
                amountExtraSlotTxt.text = UserConfig.Instance.AmountExtraSlot.ToString();
                objAddExtraSlot.SetActive(false);
                //objPriceExtraSlot.SetActive(false);
            }
            else
            {
                amountExtraSlotTxt.transform.parent.gameObject.SetActive(false);
                amountExtraSlotTxt.gameObject.SetActive(false);
                objAddExtraSlot.SetActive(true);
                //objPriceExtraSlot.SetActive(true);
                //txtPriceExtraSlot.text = prices[(int)eBooster.ExtraSlot].ToString();
            }
        }
        else
        {
            unlockExtraSlot.SetActive(false);
            lockExtraSlot.SetActive(true);
        }
    }

    public void SetStateSwapItem()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP)
        {
            unlockSwap.SetActive(true);
            lockSwap.SetActive(false);

            if (UserConfig.Instance.AmountSwap > 0)
            {
                amountSwapTxt.transform.parent.gameObject.SetActive(true);
                amountSwapTxt.gameObject.SetActive(true);
                amountSwapTxt.text = UserConfig.Instance.AmountSwap.ToString();
                objAddSwap.SetActive(false);
                //objPriceSwap.SetActive(false);
            }
            else
            {
                amountSwapTxt.transform.parent.gameObject.SetActive(false);
                amountSwapTxt.gameObject.SetActive(false);
                objAddSwap.SetActive(true);
                //objPriceSwap.SetActive(true);
                //txtPriceSwap.text = prices[(int)eBooster.Swap].ToString();
            }
        }
        else
        {
            unlockSwap.SetActive(false);
            lockSwap.SetActive(true);
        }
    }

    public void SetStateMagnetItem()
    {
        if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
        {
            unlockMagnet.SetActive(true);
            lockMagnet.SetActive(false);

            if (UserConfig.Instance.AmountMagnet > 0)
            {
                amountMagnetTxt.transform.parent.gameObject.SetActive(true);
                amountMagnetTxt.gameObject.SetActive(true);
                amountMagnetTxt.text = UserConfig.Instance.AmountMagnet.ToString();
                objAddMagnet.SetActive(false);
                //objPriceMagnet.SetActive(false);
            }
            else
            {
                amountMagnetTxt.transform.parent.gameObject.SetActive(false);
                amountMagnetTxt.gameObject.SetActive(false);
                objAddMagnet.SetActive(true);
                //objPriceMagnet.SetActive(true);
                //txtPriceMagnet.text = prices[(int)eBooster.Magnet].ToString();
            }
        }
        else
        {
            unlockMagnet.SetActive(false);
            lockMagnet.SetActive(true);
        }
    }
   
    public void ActiveAddSlotBooster(bool active)
    {
        if (active)
        {
            undoBtn.interactable = true;
            undoGroup.alpha = 1;
            blockUndo.SetActive(false);
        }
        else
        {
            undoBtn.interactable = false;
            undoGroup.alpha = 0.85f;
            blockUndo.SetActive(true);
        }
    }

    public void PressedSettingBtn()
    {
        if (canControlUI && ShooterManager.Instance.shooters.Count > 0)
        {
            UIManager.Instance.ShowSettingMenu();
        }
    }

    public void PressedReplayBtn()
    {
        Bridge.Instance.OnGameFinished(false, ShooterManager.Instance.shooters.Count, UserConfig.Instance.CurLevel);
        ReplayLevel();
    }

    public void ReplayLevel()
    {
        if (canControlUI)
        {
            SoundManager.instance.StopSoundMedium();
            canControlUI = false;
            EventManager.EmitEvent(EventVariables.RecycleLevel);

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                GameManager.Instance.LoadLevel();
                Show();
            });
            FadeMenu.Instance.Fade(e, true);
        }
    }

    public void PressedItemUndoBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO)
            {
                if (ShooterManager.Instance.shooters.Count == 0) return;

                if (UserConfig.Instance.AmountUndo > 0)
                {
                    if (ParkingManager.Instance.Undo())
                    {
                        UserConfig.Instance.AmountUndo--;
                        SetStateUndoItem();

                        Bridge.Instance.TrackUseBooster("undo", UserConfig.Instance.CurLevel);
                    }
                    else
                    {
                        WarningMenu.Instance.Show(Vector2.zero, "Can't undo any hooks!");
                    }

                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO)
                    {
                        tutorialUI.Hide();
                    }
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.Undo);

                    //int price = prices[(int)eBooster.Undo];
                    //if (UserConfig.Instance.Coin >= price)
                    //{
                    //    EconomyMenu.instance.AddGold(-price, null, null);
                    //    UserConfig.Instance.AmountUndo += 1;
                    //    Bridge.Instance.TrackBuyBooster("undo", UserConfig.Instance.CurLevel);
                    //    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    //    EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Undo);

                    //    PressedItemUndoBtn();
                    //}
                    //else
                    //{
                    //    if (ShooterManager.Instance.shooters.Count > 0)
                    //    {
                    //        UIManager.Instance.ShowShopMenu();
                    //    }
                    //}
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_UNDO));
            }
        }
    }

    public void PressedItemExtraSlotBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT)
            {
                if (ShooterManager.Instance.shooters.Count == 0) return;

                if (UserConfig.Instance.AmountExtraSlot > 0)
                {
                    if (ParkingManager.Instance.Clear())
                    {
                        UserConfig.Instance.AmountExtraSlot--;
                        SetStateExtraSlotItem();

                        Bridge.Instance.TrackUseBooster("extra_slot", UserConfig.Instance.CurLevel);
                    }
                    else
                    {
                        if (!ParkingManager.Instance.IsAnyExtraSlotAvailable())
                        {
                            WarningMenu.Instance.Show(Vector2.zero, "Extra slots is full!");
                        }
                        else
                        {
                            WarningMenu.Instance.Show(Vector2.zero, "Can't clear any slots!");
                        }
                    }

                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT)
                    {
                        tutorialUI.Hide();
                    }
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.ExtraSlot);

                    //int price = prices[(int)eBooster.ExtraSlot];
                    //if (UserConfig.Instance.Coin >= price)
                    //{
                    //    EconomyMenu.instance.AddGold(-price, null, null);
                    //    UserConfig.Instance.AmountExtraSlot += 1;
                    //    Bridge.Instance.TrackBuyBooster("extra_slot", UserConfig.Instance.CurLevel);
                    //    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    //    EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.ExtraSlot);

                    //    PressedItemExtraSlotBtn();
                    //}
                    //else
                    //{
                    //    if (ShooterManager.Instance.shooters.Count > 0)
                    //    {
                    //        UIManager.Instance.ShowShopMenu();
                    //    }
                    //}
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_EXTRA_SLOT));
            }
        }
    }

    public void PressedItemSwapBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP)
            {
                if (ShooterManager.Instance.shooters.Count == 0) return;

                if (UserConfig.Instance.AmountSwap > 0)
                {
                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP)
                    {
                        tutorialUI.Hide();
                    }

                    if (ParkingManager.Instance.parkedShooters.Count > 0)
                    {
                        UIManager.Instance.ShowItemUsingMenu(eBooster.Swap);
                    }
                    else
                    {
                        WarningMenu.Instance.Show(Vector2.zero, "Waiting slots are empty!");
                    }
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.Swap);

                    //int price = prices[(int)eBooster.Swap];
                    //if (UserConfig.Instance.Coin >= price)
                    //{
                    //    EconomyMenu.instance.AddGold(-price, null, null);
                    //    UserConfig.Instance.AmountSwap += 1;
                    //    Bridge.Instance.TrackBuyBooster("swap", UserConfig.Instance.CurLevel);
                    //    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    //    EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Swap);

                    //    PressedItemSwapBtn();
                    //}
                    //else
                    //{
                    //    if (ShooterManager.Instance.shooters.Count > 0)
                    //    {
                    //        UIManager.Instance.ShowShopMenu();
                    //    }
                    //}
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_SWAP));
            }
        }
    }

    public void PressedItemMagnetBtn()
    {
        if (canControlUI)
        {
            if (UserConfig.Instance.CurLevel >= GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
            {
                if (ShooterManager.Instance.shooters.Count == 0) return;

                if (UserConfig.Instance.AmountMagnet > 0)
                {
                    if (UserConfig.Instance.CurLevel == GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET)
                    {
                        tutorialUI.Hide();
                    }

                    if (ParkingManager.Instance.parkedShooters.Count > 0)
                    {
                        UIManager.Instance.ShowItemUsingMenu(eBooster.Magnet);
                    }
                    else
                    {
                        WarningMenu.Instance.Show(Vector2.zero, "Waiting slots are empty!");
                    }
                }
                else
                {
                    UIManager.Instance.ShowBuyItemMenu(eBooster.Magnet);

                    //int price = prices[(int)eBooster.Magnet];
                    //if (UserConfig.Instance.Coin >= price)
                    //{
                    //    EconomyMenu.instance.AddGold(-price, null, null);
                    //    UserConfig.Instance.AmountMagnet += 1;
                    //    Bridge.Instance.TrackBuyBooster("magnet", UserConfig.Instance.CurLevel);
                    //    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    //    EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Magnet);

                    //    PressedItemMagnetBtn();
                    //}
                    //else
                    //{
                    //    if (ShooterManager.Instance.shooters.Count > 0)
                    //    {
                    //        UIManager.Instance.ShowShopMenu();
                    //    }
                    //}
                }
            }
            else
            {
                WarningMenu.Instance.Show(Vector2.zero, string.Format("Unlock item at level {0}", GameConfig.LEVEL_UNLOCK_BOOSTER_MAGNET));
            }
        }
    }
    
    public void PressedRemoveAdsBtn()
    {
        //CGTeamBridge.instance.LogEvent("purchase_removeads");
        //CGTeamBridge.instance.SetResumeAds(true);
        //IAPManager.Instance.Purchase(IAPManager.kRemoveAds, () =>
        //{
        //    CGTeamBridge.instance.SetResumeAds(false);
        //});
    }

    public override void Show()
    {
        base.Show();

        ActiveAddSlotBooster(true);
        EconomyMenu.instance.Hide();

        if (!content.activeSelf)
        {
            ActiveUI(true);
        }

        booster.SetActive(UserConfig.Instance.CurLevel != 1);

        //if (UserConfig.Instance.CurLevel == 1)
        //{
        //    tutorialUI.ShowHandTutorial("Tap to Select", Vector2.zero, Vector2.zero, new Vector2(0, -800f), new Vector2(-40f, -320f), 0);
        //}
        //else
        //{
        //    tutorialUI.Hide();
        //}

        SetStateUndoItem();
        SetStateExtraSlotItem();
        SetStateSwapItem();
        SetStateMagnetItem();

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        SetStateLevel(eTypeLevel);

        //if (eTypeLevel != eTypeLevel.Normal && eTypeLevel != eTypeLevel.Medium)
        //{
        //    Invoke(nameof(ShowNoticeHardLevel), 0.5f);
        //}

        levelTxt.text = $"Lv {UserConfig.Instance.CurLevel.ToString()}";

        //if (UserConfig.Instance.HasTurnOffInternet())
        //{
        //    UIManager.Instance.ShowNoticeMenu("Internet Connection Error", true);
        //}

        UserConfig.Instance.amountPlay++;
    }

    public void ShowBottom()
    {
        booster.SetActive(true);
    }

    void SetStateLevel(eTypeLevel eTypeLevel)
    {
        objHard.SetActive(eTypeLevel == eTypeLevel.Hard);
        objSuperHard.SetActive(eTypeLevel == eTypeLevel.SuperHard);
    }

    void ShowNoticeHardLevel()
    {
        UIManager.Instance.ShowNoticeHardMenu();
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

    #region Video
    [Header("Video")]
    [SerializeField] GameObject content;
    public void PressedActiveUI()
    {
        bool active = content.activeSelf;
        content.SetActive(!active);

        if (active)
        {
            EconomyMenu.instance.Hide();
        }
        else
        {
            EconomyMenu.instance.Show();
        }
    }

    public void ActiveUI(bool active)
    {
        content.SetActive(active);
    }
    #endregion

    #region Cheat
    public void PressedCheatBtn()
    {
        float diffTime = Time.time - GameManager.Instance.startTimeCheat;
        if (diffTime > 1)
        {
            GameManager.Instance.tapCheat = 1;
            GameManager.Instance.startTimeCheat = Time.time;
        }
        else
        {
            GameManager.Instance.tapCheat++;
            if (GameManager.Instance.tapCheat == 5)
            {
                // Show Ui Cheat
                UIManager.Instance.ShowCheatMenu();
            }
        }

        GameManager.Instance.startTimeCheat = Time.time;
    }
    #endregion
}
