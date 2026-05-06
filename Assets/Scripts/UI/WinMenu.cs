using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class WinMenu : UIMenu
{
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI levelTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;
    [SerializeField] TextMeshProUGUI coinTxt;
    [SerializeField] GameObject objIcon;

    [Header("New Element")]
    [SerializeField] Transform elementParent;
    [SerializeField] GameObject elementObj;
    [SerializeField] GameObject newFeatureObj;
    [SerializeField] TextMeshProUGUI progressElementTxt;
    [SerializeField] TextMeshProUGUI elementNameTxt;
    [SerializeField] Image elementImg;
    [SerializeField] Image progressElementImg;

    [Header("Spin")]
    [SerializeField] RectTransform arrow;
    [SerializeField] RectTransform posLeft;
    [SerializeField] RectTransform posRight;
    [SerializeField] RectTransform[] rtsfBonus;
    [SerializeField] TextMeshProUGUI[] txtsBonus;
    [SerializeField] TextMeshProUGUI txtCoinClaimAds;

    [Header("Animation")]
    [SerializeField] Transform levelTitle;
    [SerializeField] Transform spin;
    [SerializeField] Transform nextBtn;
    [SerializeField] Transform nextBtnAds;

    bool canContinue;

    int coinBonus = 50;
    int coinSpin = 100;

    public override void Show()
    {
        base.Show();
        canContinue = true;

        EconomyMenu.instance.Show();

        int timePlay = (int)(Time.time - GameManager.Instance.startTime);
        Bridge.Instance.OnGameFinished(true, timePlay, UserConfig.Instance.CurLevel);

        int minute = timePlay / 60;
        int second = timePlay % 60;

        string minuteStr = "";
        if (minute > 1)
        {
            minuteStr = minute + " minutes";
        }
        else
        {
            minuteStr = minute + " minute";
        }

        string secondStr = "";
        if (second > 1)
        {
            secondStr = second + " seconds";
        }
        else
        {
            secondStr = second + " second";
        }

        descriptionTxt.text = "It took " + minuteStr + " and " + secondStr;

        ProgressElement progressElement = GameManager.Instance.GetProgressElement();
        if (progressElement == null)
        {
            elementObj.SetActive(false);
            objIcon.SetActive(true);
        }
        else
        {
            elementObj.SetActive(true);
            objIcon.SetActive(false);
            newFeatureObj.transform.localScale = Vector3.zero;

            //progressElementTxt.text = string.Format("{0}/{1}", UserConfig.Instance.CurLevel - progressElement.prevLevel, progressElement.nextLevel - progressElement.prevLevel);
            float progress = ((float)(UserConfig.Instance.CurLevel - progressElement.prevLevel)) / (progressElement.nextLevel - progressElement.prevLevel);
            progressElementTxt.text = $"{Mathf.RoundToInt(progress * 100f)}%";

            Sprite sprElement = Resources.Load<Sprite>($"Icons/Elements/{progressElement.eTypeElement.ToString()}");
            elementImg.sprite = sprElement;
            progressElementImg.sprite = sprElement;
            elementImg.SetNativeSize();
            progressElementImg.SetNativeSize();

            switch (progressElement.eTypeElement)
            {
                case eTypeElement.Hidden:
                    elementNameTxt.text = "Hidden";
                    break;
                case eTypeElement.Tunnel:
                    elementNameTxt.text = "Tunnel";
                    break;
                case eTypeElement.LinkedHook:
                    elementNameTxt.text = "Linked Hook";
                    break;
                case eTypeElement.Ice:
                    elementNameTxt.text = "Ice";
                    break;
                case eTypeElement.LockAndKey:
                    elementNameTxt.text = "Lock And Key";
                    break;
                case eTypeElement.CircleHook:
                    elementNameTxt.text = "Circle Hook";
                    break;
                case eTypeElement.TriangleHook:
                    elementNameTxt.text = "Triangle Hook";
                    break;
                case eTypeElement.Crate:
                    elementNameTxt.text = "Crate";
                    break;
                case eTypeElement.Pin:
                    elementNameTxt.text = "Pin";
                    break;
                case eTypeElement.Shutter:
                    elementNameTxt.text = "Pin";
                    break;
                case eTypeElement.Cloth:
                    elementNameTxt.text = "Cloth";
                    break;
            }

            //if (progress == 0 || elementParent.childCount == 0)
            //{
            //    if (elementParent.childCount > 0)
            //    {
            //        elementParent.GetChild(0).gameObject.Recycle();
            //    }
            //    GameObject tempElementObj = GameManager.Instance.InstantiatePrefab("Icons/Elements/" + progressElement.eTypeElement);
            //    tempElementObj.transform.parent = elementParent;
            //    tempElementObj.transform.localPosition = Vector3.zero;
            //    tempElementObj.transform.localScale = Vector3.one;
            //}

            progressElementImg.fillAmount = progress;
            int tempNewProgress = UserConfig.Instance.CurLevel + 1 - progressElement.prevLevel;
            float newProgress = (float)tempNewProgress / (progressElement.nextLevel - progressElement.prevLevel);

            progressElementImg.DOKill();
            progressElementImg.DOFillAmount(newProgress, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                //progressElementTxt.text = string.Format("{0}/{1}", tempNewProgress, progressElement.nextLevel - progressElement.prevLevel);
                if (newProgress == 1)
                {
                    newFeatureObj.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
                }
            });
            DOVirtual.Float(progress, newProgress, 0.5f, result =>
            {
                progressElementTxt.text = $"{Mathf.RoundToInt(result * 100f)}%";
            }).SetEase(Ease.Linear);
        }

        coinBonus = GameManager.Instance.GetCoinRewardLevel(UserConfig.Instance.CurLevel);
        coinSpin = coinBonus * 2;

        levelTxt.text = "Level " + UserConfig.Instance.CurLevel;
        UserConfig.Instance.WinLevel++;
        UserConfig.Instance.CurLevel++;
        UserConfig.Instance.AmountWinStreak++;

        coinTxt.text = $"+{coinBonus}";
        txtCoinClaimAds.text = $"+{coinSpin}";

        // Anim UI
        bg.color = new Color(0, 0f, 0f, 0);
        bg.DOFade(0.95f, 0.25f).SetEase(Ease.Linear);
        ShowAnimWinGame();

        if (UserConfig.Instance.CurLevel >= GameManager.MAX_LEVEL)
        {
            UserConfig.Instance.NextLevel = -1;
        }
    }

    void ShowAnimWinGame()
    {
        levelTitle.localScale = Vector3.zero;
        spin.localScale = Vector3.zero;
        nextBtn.localScale = Vector3.zero;
        nextBtnAds.localScale = Vector3.zero;

        //arrow.DOKill();
        //arrow.anchoredPosition = new Vector2(0, arrow.anchoredPosition.y);
        //spin.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.25f).OnComplete(() =>
        //{
        //    Spin();
        //});

        levelTitle.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        nextBtnAds.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.5f);
        nextBtn.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.75f);
    }

    private void Spin()
    {
        for (int i = 0; i < txtsBonus.Length; i++)
        {
            if (i == 2)
            {
                txtsBonus[i].fontSize = 50;
            }
            else
            {
                txtsBonus[i].fontSize = 35;
            }
        }

        coinSpin = Mathf.RoundToInt(coinBonus * 3f);
        txtCoinClaimAds.text = string.Format("{0}", coinSpin);

        arrow.DOAnchorPosX(posRight.anchoredPosition.x, 0.5f).SetEase(Ease.InQuad).SetUpdate(true)
            .OnUpdate(() => ArrowCallBack())
            .OnComplete(() =>
            {
                arrow.DOAnchorPosX(posLeft.anchoredPosition.x, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad).SetUpdate(true).OnUpdate(() => ArrowCallBack());
            });
    }

    private void ArrowCallBack()
    {
        for (int i = 0; i < txtsBonus.Length; i++)
        {
            txtsBonus[i].fontSize = 35;
        }

        if (arrow.anchoredPosition.x <= rtsfBonus[0].anchoredPosition.x)
        {
            coinSpin = Mathf.RoundToInt(coinBonus * 1.5f);
            txtsBonus[0].fontSize = 50;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[0].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[1].anchoredPosition.x)
        {
            txtsBonus[1].fontSize = 50;
            coinSpin = coinBonus * 2;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[1].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[2].anchoredPosition.x)
        {
            txtsBonus[2].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 2.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[2].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[4].anchoredPosition.x)
        {
            txtsBonus[3].fontSize = 50;
            coinSpin = coinBonus * 3;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[4].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[5].anchoredPosition.x)
        {
            txtsBonus[4].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 2.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[5].anchoredPosition.x && arrow.anchoredPosition.x <= rtsfBonus[6].anchoredPosition.x)
        {
            txtsBonus[5].fontSize = 50;
            coinSpin = coinBonus * 2;
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
        else if (arrow.anchoredPosition.x >= rtsfBonus[6].anchoredPosition.x)
        {
            txtsBonus[6].fontSize = 50;
            coinSpin = Mathf.RoundToInt(coinBonus * 1.5f);
            txtCoinClaimAds.text = string.Format("{0}", coinSpin);
        }
    }

    public void PressedNextBtn()
    {
        if (canContinue)
        {
            canContinue = false;
            arrow.DOKill();

            UnityEvent onDoneAddGold = new UnityEvent();
            onDoneAddGold.AddListener(() =>
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(() =>
                {
                    Hide();
                    GameManager.Instance.LoadLevel();
                    UIManager.Instance.ShowIngameMenu();
                });
                FadeMenu.Instance.Fade(e, true);

                Bridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel - 1, "currency", "coin", coinBonus, "reward", UserConfig.Instance.Coin);
            });
            EconomyMenu.instance.AddGold(coinBonus, coinTxt.transform, onDoneAddGold);
        }
    }

    public void PressedCoinAdsBtn()
    {
        SoundManager.instance.PlaySound("GetReward");

        if (canContinue)
        {
            canContinue = false;

            arrow.DOKill();

            UnityEvent onDoneAddGold = new UnityEvent();
            onDoneAddGold.AddListener(() =>
            {
                UnityEvent e = new UnityEvent();
                e.AddListener(() =>
                {
                    Hide();
                    GameManager.Instance.LoadLevel();
                    UIManager.Instance.ShowIngameMenu();
                });
                FadeMenu.Instance.Fade(e, true);

                Bridge.Instance.LogEarnCurrency(UserConfig.Instance.CurLevel - 1, "currency", "coin", coinSpin, "watch_ads", UserConfig.Instance.Coin);
            });
            EconomyMenu.instance.AddGold(coinSpin, txtCoinClaimAds.transform, onDoneAddGold);
        }

        //if (UserConfig.Instance.Ticket > 0)
        //{
        //    EconomyMenu.instance.AddTicket(-1);
        //    EconomyMenu.instance.TicketFly(coinAdsTF.anchoredPosition, e);
        //}
        //else
        //{
        //    CGTeamBridge.instance.ShowRewarded("reward_coin", null, e, null);
        //}
    }

    public void PressedRaceBtn()
    {
        //tutorialRect.gameObject.SetActive(false);
        //if (UserConfig.Instance.IsRacing)
        //{
        //    UIManager.Instance.ShowRaceMenu();
        //}
        //else
        //{
        //    UIManager.Instance.ShowStartRaceMenu();
        //}
    }

    public void PressedHomeBtn()
    {
        //tutorialRect.gameObject.SetActive(false);
        //UnityEvent e = new UnityEvent();
        //e.AddListener(() =>
        //{
        //    Hide();
        //    GameManager.Instance.RecycleLevel();
        //    UIManager.Instance.ShowMainMenu();
        //});
        //FadeMenu.Instance.Fade(e);
    }

    public override void Hide()
    {
        base.Hide();
        arrow.DOKill();
        //PicturePreview.Instance.Hide();
        gameObject.SetActive(false);
    }
}
