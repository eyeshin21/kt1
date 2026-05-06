using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using TigerForge;

public class BuyItemMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;

    [SerializeField] TextMeshProUGUI titleTxt;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] TextMeshProUGUI amountTxt;
    [SerializeField] Image[] iconImgs;
    [SerializeField] Sprite[] iconItem;
    [SerializeField] TextMeshProUGUI priceTxt;
    [SerializeField] int[] prices;

    [HideInInspector]
    public eBooster eBooster;

    public override void Show()
    {
        base.Show();

        EconomyMenu.instance.Show();

        //Bridge.Instance.TrackCustomEvent("show_buy_booster", UserConfig.Instance.CurLevel);
        switch (eBooster)
        {
            case eBooster.Undo:
                titleTxt.text = "Undo";
                contentTxt.text = "Move the placed block back to its original position.";
                //amountTxt.text = "x2";
                break;
            case eBooster.ExtraSlot:
                titleTxt.text = "Extra Slot";
                contentTxt.text = "Move the three queued hooks up one row.";
                //amountTxt.text = "x2";
                break;
            case eBooster.Swap:
                titleTxt.text = "Swap";
                contentTxt.text = "Swap one hook in the queue with any random hook.";
                //amountTxt.text = "x2";
                break;
            case eBooster.AddSlot:
                titleTxt.text = "Add Slot";
                contentTxt.text = "Add a slot!";
                amountTxt.text = "";
                break;
            case eBooster.Magnet:
                titleTxt.text = "Magnet";
                contentTxt.text = "Instantly fill a selected waiting hook.";
                //amountTxt.text = "x2";
                break;
        }

        priceTxt.text = prices[(int)eBooster].ToString();
        for (int i = 0; i < iconImgs.Length; i++)
        {
            iconImgs[i].sprite = iconItem[(int)eBooster];
            iconImgs[i].SetNativeSize();
        }

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void PressedBuyRewardBtn()
    {
        SoundManager.instance.PlaySound("GetReward");
        switch (eBooster)
        {
            case eBooster.Undo:
                UserConfig.Instance.AmountUndo += 2;
                Bridge.Instance.TrackBuyBooster("undo", UserConfig.Instance.CurLevel);
                break;
            case eBooster.ExtraSlot:
                UserConfig.Instance.AmountExtraSlot += 2;
                Bridge.Instance.TrackBuyBooster("extra_slot", UserConfig.Instance.CurLevel);
                break;
            case eBooster.Swap:
                UserConfig.Instance.AmountSwap += 2;
                Bridge.Instance.TrackBuyBooster("swap", UserConfig.Instance.CurLevel);
                break;
            case eBooster.AddSlot:
                EventManager.EmitEvent(EventVariables.AddSlot);
                Bridge.Instance.TrackBuyBooster("add_slot", UserConfig.Instance.CurLevel);
                break;
            case eBooster.Magnet:
                UserConfig.Instance.AmountMagnet += 2;
                Bridge.Instance.TrackBuyBooster("magnet", UserConfig.Instance.CurLevel);
                break;
        }
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);
        Hide();
    }

    public void PressedBuyCoinBtn()
    {
        int price = prices[(int)eBooster];
        if (UserConfig.Instance.Coin >= price)
        {
            EconomyMenu.instance.AddGold(-price, null, null);
            switch (eBooster)
            {
                case eBooster.Undo:
                    UserConfig.Instance.AmountUndo += 1;
                    Bridge.Instance.TrackBuyBooster("undo", UserConfig.Instance.CurLevel);
                    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    break;
                case eBooster.ExtraSlot:
                    UserConfig.Instance.AmountExtraSlot += 1;
                    Bridge.Instance.TrackBuyBooster("extra_slot", UserConfig.Instance.CurLevel);
                    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    break;
                case eBooster.Swap:
                    UserConfig.Instance.AmountSwap += 1;
                    Bridge.Instance.TrackBuyBooster("swap", UserConfig.Instance.CurLevel);
                    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    break;
                case eBooster.AddSlot:
                    EventManager.EmitEvent(EventVariables.AddSlot);
                    Bridge.Instance.TrackBuyBooster("add_slot", UserConfig.Instance.CurLevel);
                    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    break;
                case eBooster.Magnet:
                    UserConfig.Instance.AmountMagnet += 1;
                    Bridge.Instance.TrackBuyBooster("magnet", UserConfig.Instance.CurLevel);
                    Bridge.Instance.LogSpendResource(UserConfig.Instance.CurLevel, "currency", "coin", price, "buy_booster", UserConfig.Instance.Coin);
                    break;
            }
            EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster);
            Hide();
        }
        else
        {
            //WarningMenu.Instance.Show(Vector2.zero, "Not enough coin!");
            UIManager.Instance.ShowShopMenu();
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
    }
}
