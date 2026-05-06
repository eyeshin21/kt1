using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;

public class RewardMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] GameObject rewardUIPrefab;
    [SerializeField] RectTransform rewardParent;
    [SerializeField] TextMeshProUGUI titleTxt;

    [HideInInspector]
    public RewardData rewardData;

    List<AwardItemUi> awardItemUis = new List<AwardItemUi>();
    bool hasCoin = false;
    bool hasTicket = false;

    public override void Show()
    {
        base.Show();

        hasCoin = false;
        hasTicket = false;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        titleTxt.text = rewardData.title;

        for (int i = 0; i < rewardData.systemAwardItems.Length; i++)
        {
            GameObject obj = rewardUIPrefab.Spawn(rewardParent);
            obj.transform.localScale = Vector3.one;
            AwardItemUi awardItemUi = obj.GetComponent<AwardItemUi>();
            awardItemUi.Init(rewardData.systemAwardItems[i].type, rewardData.systemAwardItems[i].num);

            if (rewardData.systemAwardItems[i].type == eSystemItemType.Coin)
            {
                hasCoin = true;
            }
            if (rewardData.systemAwardItems[i].type == eSystemItemType.Ticket)
            {
                hasTicket = true;
            }

            awardItemUis.Add(awardItemUi);
        }
    }

    public override void Hide()
    {
        base.Hide();

        if (hasCoin)
        {
            EconomyMenu.instance.SetCoinTxt(UserConfig.Instance.Coin);
            EconomyMenu.instance.PlayVFXCoin();
            SoundManager.instance.PlaySound("Coin");
        }

        //if (hasTicket)
        //{
        //    EconomyMenu.instance.SetTicketTxt(UserConfig.Instance.Ticket);
        //    EconomyMenu.instance.PlayVFXTicket();
        //    SoundManager.instance.PlaySound("Coin");
        //}

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            for (int i = 0; i < awardItemUis.Count; i++)
            {
                awardItemUis[i].gameObject.Recycle();
            }
            awardItemUis.Clear();

            gameObject.SetActive(false);
        });
    }
}

[Serializable]
public class RewardData
{
    public string title;
    public SystemAwardItem[] systemAwardItems;

    public RewardData(string title, SystemAwardItem[] systemAwardItems)
    {
        this.title = title;
        this.systemAwardItems = systemAwardItems;
    }
}

[Serializable]
public class SystemAwardItem
{
    public eSystemItemType type;
    public int num;

    public SystemAwardItem(eSystemItemType type, int num)
    {
        this.type = type;
        this.num = num;
    }
}

public enum eSystemItemType
{
    Coin,
    Ticket,
    ScrewDrive,
    Bomb,
    Hint,
    Undo
}
