using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] RectTransform unmaskRect;
    [SerializeField] RectTransform unmaskPanel;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] RectTransform contentRect;
    [SerializeField] RectTransform handRect;
    [SerializeField] RectTransform arrowRect;
    [SerializeField] GameObject blockUI;
    [SerializeField] Animator animHand;

    public void ShowHandTutorial(string content, Vector2 sizeUnmask, Vector2 pointUnmask, Vector2 contentPos, Vector2 handAnchorPos, int tutType = 0) // tutType = 0: tap, 1: drag, 2: zoom
    {
        gameObject.SetActive(true);
        handRect.gameObject.SetActive(false);
        arrowRect.gameObject.SetActive(false);
        contentRect.gameObject.SetActive(false);

        this.contentTxt.text = content;
        contentRect.anchoredPosition = contentPos;

        handRect.anchoredPosition = handAnchorPos;

        this.contentTxt.gameObject.SetActive(false);
        this.contentTxt.transform.DOKill();
        this.contentTxt.transform.localScale = Vector3.one;

        //Debug.Log(sizeUnmask);

        if (sizeUnmask != Vector2.zero)
        {
            blockUI.SetActive(true);
            unmaskPanel.gameObject.SetActive(true);

            unmaskRect.gameObject.SetActive(true);
            unmaskRect.sizeDelta = new Vector2(5000, 5000);
            unmaskRect.DOSizeDelta(sizeUnmask, 0.35f).SetEase(Ease.Linear).OnComplete(() =>
            {
                this.contentTxt.gameObject.SetActive(true);
                //this.contentTxt.transform.DOScale(1.1f, 0.5f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                handRect.gameObject.SetActive(true);
                if (content != "")
                {
                    contentRect.gameObject.SetActive(true);
                }
                blockUI.SetActive(false);
            });

            unmaskRect.anchoredPosition = pointUnmask;
        }
        else
        {
            this.contentTxt.gameObject.SetActive(true);
            if (content != "")
            {
                contentRect.gameObject.SetActive(true);
            }

            blockUI.SetActive(false);
            unmaskPanel.gameObject.SetActive(false);
            unmaskRect.gameObject.SetActive(false);
            handRect.gameObject.SetActive(true);
        }

        if (tutType == 1)
        {
            animHand.Play("Base Layer.TutorialDrag", 0, 0);
        }
        else if (tutType == 2)
        {
            animHand.Play("Base Layer.TutorialZoom", 0, 0);
        }
        else
        {
            animHand.Play("Base Layer.TutorialTap", 0, 0);
        }
    }

    public void ShowArrowTutorial(string content, Vector2 size, Vector2 pointUnmask, Vector2 contentPos, Vector2 arrowAnchorPos, float angleArrow)
    {
        unmaskPanel.gameObject.SetActive(true);
        gameObject.SetActive(true);
        handRect.gameObject.SetActive(false);
        arrowRect.gameObject.SetActive(false);

        this.contentTxt.text = content;
        contentRect.anchoredPosition = contentPos;
        unmaskRect.anchoredPosition = pointUnmask;
        arrowRect.anchoredPosition = arrowAnchorPos;
        arrowRect.localEulerAngles = new Vector3(0, 0, angleArrow);

        this.contentTxt.gameObject.SetActive(false);
        this.contentTxt.transform.DOKill();
        this.contentTxt.transform.localScale = Vector3.one;

        blockUI.SetActive(true);
        unmaskRect.sizeDelta = new Vector2(5000, 5000);
        unmaskRect.DOSizeDelta(size, 0.35f).SetEase(Ease.Linear).OnComplete(() =>
        {
            this.contentTxt.gameObject.SetActive(true);
            arrowRect.gameObject.SetActive(true);
            contentRect.gameObject.SetActive(true);
            blockUI.SetActive(false);
        });
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
