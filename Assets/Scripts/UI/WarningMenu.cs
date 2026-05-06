using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class WarningMenu : Singleton<WarningMenu>
{
    [SerializeField] RectTransform warningRect;
    [SerializeField] TextMeshProUGUI warningTxt;
    [SerializeField] CanvasGroup canvasGroup;

    bool isShowing;

    private void Start()
    {
        isShowing = false;
        canvasGroup.alpha = 0;
    }

    public void Show(Vector2 anchor, string description)
    {
        if (isShowing) return;

        isShowing = true;

        warningRect.DOKill();
        warningTxt.DOKill();


        if (description != warningTxt.text)
        {
            warningTxt.text = description;
        }
        warningRect.anchoredPosition = anchor;
        canvasGroup.alpha = 1;

        warningRect.localScale = Vector3.zero;
        warningRect.DOScale(1, 0.2f).SetEase(Ease.OutBack);

        warningRect.DOLocalMoveY(anchor.y + 200, 1).SetEase(Ease.InOutSine).SetDelay(0.38f);
        canvasGroup.DOFade(0, 0.5f).SetEase(Ease.InOutSine).SetDelay(1.2f).OnComplete(() =>
        {
            isShowing = false;
        });
    }
}
