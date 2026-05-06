using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class StarterPackMenu : UIMenu
{
    [SerializeField] eIAPKey keyIAP = eIAPKey.kStarterPack;

    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Show()
    {
        base.Show();

        //txtPrice.text = string.Format("{0} {1}", CGTeamBridge.Instance.GetProductCurrencyFromStore(keyIAP), CGTeamBridge.Instance.GetProductPriceFromStore(keyIAP));
        txtPrice.text = Bridge.Instance.GetProductPriceStringFromStore(keyIAP);

        EventManager.StartListening(EventVariables.BuyStarterPack, OnPurchase);
    }

    public override void Hide()
    {
        base.Hide();
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

        Hide();
    }

    public void OnClickButtonBuy()
    {
        Bridge.Instance.Purchase(keyIAP);
    }
}
