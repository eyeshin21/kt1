using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RatePanel : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] GameObject[] stars;
    [SerializeField] Image bg;

    [SerializeField] RectTransform rateRect;

    int scoreRate = 0;

    bool isFirst = false;

    public override void Show()
    {
        base.Show();

        isFirst = false;
        rateRect.gameObject.SetActive(false);

        UserConfig.Instance.LastTimeShowRate = UserConfig.Instance.CurLevel;

        scoreRate = 0;
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < scoreRate)
            {
                stars[i].SetActive(true);
            }
            else
            {
                stars[i].SetActive(false);
            }
        }

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public override void Hide()
    {
        //CGTeamBridge.instance.HideMRECs();
        bg.DOFade(0, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            //CGTeamBridge.instance.SetResumeAds(false);
            gameObject.SetActive(false);
        });

        base.Hide();
    }

    public void PressedStarBtn(int index)
    {
        if (!isFirst)
        {
            isFirst = true;
            rateRect.gameObject.SetActive(true);

            rateRect.localScale = Vector3.zero;
            rateRect.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        }

        SoundManager.instance.PlaySound("RateStar");
        scoreRate = index;
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < index)
            {
                stars[i].SetActive(true);
            }
            else
            {
                stars[i].SetActive(false);
            }
        }
    }

    public void PressedRateBtn()
    {
        UserConfig.Instance.Rate = true;
        if (scoreRate >= 4)
        {
            //CGTeamBridge.instance.SetResumeAds(true);
            //InAppReviewManager.Instance.RequestReview();
        }
        Hide();
    }
}
