using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using TigerForge;

public class CheatMenu : UIMenu
{
    [SerializeField] TMP_InputField inputLevel;
    [SerializeField] TMP_InputField inputPassword;

    public override void Show()
    {
        base.Show();

        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        base.Hide();

        gameObject.SetActive(false);
    }

    public void PressedGoToLevelBtn()
    {
        if (inputPassword.text == "toiga")
        {
            if (inputLevel.text != "")
            {
                UserConfig.Instance.CurLevel = int.Parse(inputLevel.text);
            }

            UIManager.Instance.ingameMenu.PressedReplayBtn();

            Hide();
        }
    }

    public void PressedRemoveAds()
    {
        if (inputPassword.text == "toiga")
        {
            UserConfig.Instance.HasAds = false;
            EventManager.EmitEvent(EventVariables.Purchased);
            EventManager.EmitEvent(EventVariables.BuyRemoveAds);
        }
    }

    public void PressedShowHideUI()
    {
        if (inputPassword.text == "toiga")
        {
            UIManager.Instance.ingameMenu.PressedActiveUI();
        }
    }

    public void PressedCheatCoin()
    {
        if (inputPassword.text == "toiga")
        {
            EconomyMenu.instance.AddGold(100000, null, null);
        }
    }

    public void PressedShowMediabuggerBtn()
    {
        if (inputPassword.text == "toiga")
        {
            //MaxSdk.ShowMediationDebugger();

            Hide();
        }
    }
}
