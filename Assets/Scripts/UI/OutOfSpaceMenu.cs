using DG.Tweening;
using Facebook.Unity;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OutOfSpaceMenu : UIMenu
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] RectTransform board;
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] Image icon;
    [SerializeField] TextMeshProUGUI txtDescription;
    [SerializeField] Sprite[] sprIcons;

    [Header("Starter Pack")]
    [SerializeField] GameObject objStarterPack;
    [SerializeField] eIAPKey keyIAP = eIAPKey.kStarterPack;
    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Start()
    {
        base.Start();
        EventManager.StartListening(EventVariables.BuyStarterPack, OnPurchase);
    }

    public override void Show()
    {
        base.Show();

        EconomyMenu.instance.Show();

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        if (ParkingManager.Instance.CanAddSlot())
        {
            icon.sprite = sprIcons[0];
            txtDescription.text = "Resurrect and add a slot!";
        }
        else
        {
            icon.sprite = sprIcons[1];
            txtDescription.text = "Resurrect and clear 3 slots!";
        }

        icon.SetNativeSize();

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        canvasGroup.DOKill();
        canvasGroup.alpha = 1f;

        if (Bridge.Instance.IsNonConsumablePurchased(eIAPKey.kStarterPack))
        {
            board.anchoredPosition = Vector2.up * 100f;
            objStarterPack.SetActive(false);
        }
        else
        {
            board.anchoredPosition = Vector2.up * 340f;
            objStarterPack.SetActive(true);

            objStarterPack.transform.localScale = Vector3.zero;
            objStarterPack.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

            txtPrice.text = Bridge.Instance.GetProductPriceStringFromStore(keyIAP);
        }
    }

    public override void Hide()
    {
        base.Hide();

        EconomyMenu.instance.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });

        objStarterPack.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
    }

    public void PressedBackBtn()
    {
        base.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
            UIManager.Instance.ShowLoseMenu();
            //GameManager.Instance.OnLose();
        });
    }

    public void PressedKeepPlayingBtn()
    {
        if (UserConfig.Instance.Coin >= 900)
        {
            EconomyMenu.instance.AddGold(-900, null, null);
            GameplayController.Instance.Revive();

            Bridge.Instance.TrackBuyBooster("revive", UserConfig.Instance.CurLevel);
            Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", 900, "buy_revive", UserConfig.Instance.Coin);
            Hide();
        }
        else
        {
            //WarningMenu.Instance.Show(Vector2.zero, "Not Enough Coin");
            UIManager.Instance.ShowShopMenu();
        }
    }

    public void PressedWatchAdsBtn()
    {
        SoundManager.instance.PlaySound("GetReward");
        GameplayController.Instance.Revive();

        Bridge.Instance.TrackBuyBooster("revive", UserConfig.Instance.CurLevel);
        Hide();
    }

    void OnPurchase()
    {
        EconomyMenu.instance.AddGold(2400, iconCoin, null);
        UserConfig.Instance.AmountUndo += 1;
        UserConfig.Instance.AmountExtraSlot += 1;
        UserConfig.Instance.AmountSwap += 1;
        UserConfig.Instance.AmountMagnet += 1;

        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Undo);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.ExtraSlot);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Swap);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Magnet);

        if (UIManager.Instance.mainMenu != null && UIManager.Instance.mainMenu.gameObject.activeInHierarchy)
        {
            UIManager.Instance.mainMenu.objBtnStarterPack.SetActive(false);
        }

        GameplayController.Instance.Revive();

        Hide();
    }

    public void OnClickButtonBuy()
    {
        Bridge.Instance.Purchase(keyIAP);
    }

    public void OnPointerDown()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.Linear);
    }

    public void OnPointerUp()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.2f).SetEase(Ease.Linear);
    }
}
