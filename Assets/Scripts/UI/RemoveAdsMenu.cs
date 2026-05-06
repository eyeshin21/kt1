using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class RemoveAdsMenu : UIMenu
{
    [SerializeField] eIAPKey keyIAP = eIAPKey.kRemoveAdsBundle;

    [SerializeField] Transform iconCoin;
    [SerializeField] TextMeshProUGUI txtPrice;

    public override void Show()
    {
        base.Show();

        //txtPrice.text = string.Format("{0} {1}", CGTeamBridge.Instance.GetProductCurrencyFromStore(keyIAP), CGTeamBridge.Instance.GetProductPriceFromStore(keyIAP));
        txtPrice.text = Bridge.Instance.GetProductPriceStringFromStore(keyIAP);

        EventManager.StartListening(EventVariables.BuyRemoveAds, OnPurchase);
    }

    public override void Hide()
    {
        base.Hide();
    }

    void OnPurchase()
    {
        EconomyMenu.instance.AddGold(2000, iconCoin, null);
        UserConfig.Instance.AmountUndo += 5;
        UserConfig.Instance.AmountExtraSlot += 5;
        UserConfig.Instance.AmountSwap += 5;
        
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Undo);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.ExtraSlot);
        EventManager.EmitEventData(EventVariables.AddItem, (int)eBooster.Swap);

        if (UIManager.Instance.mainMenu != null && UIManager.Instance.mainMenu.gameObject.activeInHierarchy)
        {
            UIManager.Instance.mainMenu.objBtnRemoveAds.SetActive(false);
        }

        Hide();
    }

    public void OnClickButtonBuy()
    {
        CGTeamBridge.Instance.Purchase(keyIAP);
    }
}
