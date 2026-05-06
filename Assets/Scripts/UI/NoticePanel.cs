using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Events;

public class NoticePanel : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] GameObject noInternetObj;
    [SerializeField] GameObject vfxNoInternet;

    [HideInInspector]
    public string content;

    [HideInInspector]
    public bool checkInternet;

    public override void Show()
    {
        base.Show();

        if (noInternetObj != null)
        {
            if (checkInternet)
            {
                noInternetObj.SetActive(true);
                vfxNoInternet.SetActive(false);
            }
            else
            {
                noInternetObj.SetActive(false);
            }
        }

        contentTxt.text = content;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            //CGTeamBridge.instance.ShowMRECs();
        });

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public override void Hide()
    {
        base.Hide();

        if (checkInternet)
        {
            popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            {
                vfxNoInternet.SetActive(true);
            });
            StartCoroutine(StartCheck());
        }
        else
        {
            bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
            popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }

    IEnumerator StartCheck()
    {
        WaitForSeconds waitForSeconds = new WaitForSeconds(1.0f);
        while (UserConfig.Instance.HasTurnOffInternet())
        {
            yield return waitForSeconds;
        }
        //CGTeamBridge.instance.HideMRECs();
        gameObject.SetActive(false);
    }
}
