using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ShopCoinUI : MonoBehaviour
{
    [SerializeField] eIAPKey keyIAP;
    [SerializeField] TextMeshProUGUI txtPrice;
    [SerializeField] Transform iconCoin;

    // Start is called before the first frame update
    void Start()
    {
        //txtPrice.text = string.Format("{0} {1}", Bridge.Instance.GetProductCurrencyFromStore(keyIAP), Bridge.Instance.GetProductPriceFromStore(keyIAP));
        txtPrice.text = Bridge.Instance.GetProductPriceStringFromStore(keyIAP);

        EventManager.StartListening(EventVariables.PurchaseCoin, OnPurchase);
    }

    void OnPurchase()
    {
        eIAPKey eIAPKey = (eIAPKey)EventManager.GetData(EventVariables.PurchaseCoin);
        if (eIAPKey == keyIAP)
        {
            int coin = 0;
            switch (keyIAP)
            {
                case eIAPKey.kCoin1:
                    coin = 1000;
                    break;
                case eIAPKey.kCoin2:
                    coin = 5000;
                    break;
                case eIAPKey.kCoin3:
                    coin = 10000;
                    break;
                case eIAPKey.kCoin4:
                    coin = 25000;
                    break;
                case eIAPKey.kCoin5:
                    coin = 50000;
                    break;
                case eIAPKey.kCoin6:
                    coin = 100000;
                    break;
            }

            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                Bridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel, "currency", "coin", coin, "purchase", UserConfig.Instance.Coin);
            });
            EconomyMenu.instance.AddGold(coin, iconCoin, e);
        }
    }

    public void OnClick()
    {
        Bridge.Instance.Purchase(keyIAP);
    }
}
