using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public enum eTypeElement
{
    None = 0,
    Hidden = 1,
    Tunnel = 2,
    LinkedHook = 3,
    Ice = 4,
    LockAndKey = 5,
    CircleHook = 6,
    TriangleHook = 7,
    Crate = 8,
    Pin = 9,
    Shutter = 10,
    Cloth = 11
}

public class NewElementMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] Transform parentNewElement;

    [HideInInspector] public eTypeElement eTypeElement;

    public override void Show()
    {
        base.Show();

        switch (eTypeElement)
        {
            case eTypeElement.Hidden:
                nameTxt.text = "Hidden";
                contentTxt.text = "The hook is colorless until it becomes movable.";
                break;
            case eTypeElement.Tunnel:
                nameTxt.text = "Tunnel";
                contentTxt.text = "Collect the hook in front of the tunnel to reveal the hooks inside.";
                break;
            case eTypeElement.LinkedHook:
                nameTxt.text = "Linked Hook";
                contentTxt.text = "Two hooks are attached together, and when selected, both will move up together.";
                break;
            case eTypeElement.Ice:
                nameTxt.text = "Ice";
                contentTxt.text = "Frozen hook can be retrieved after 3 turns.";
                break;
            case eTypeElement.LockAndKey:
                nameTxt.text = "Lock And Key";
                contentTxt.text = "Can be picked when unlocked.";
                break;
            case eTypeElement.CircleHook:
                nameTxt.text = "Circle Hook";
                contentTxt.text = "It can hook 6 screws.";
                break;
            case eTypeElement.TriangleHook:
                nameTxt.text = "Triangle Hook";
                contentTxt.text = "It can hook 8 screws.";
                break;
            case eTypeElement.Crate:
                nameTxt.text = "Crate";
                contentTxt.text = "Break the crate by moving a certain number of hooks.";
                break;
            case eTypeElement.Pin:
                nameTxt.text = "Pin";
                contentTxt.text = "Pick up the hook behind the pin to pull the pin.";
                break;
            case eTypeElement.Shutter:
                nameTxt.text = "Pin";
                contentTxt.text = "Shutter opens or closes with each move.";
                break;
            case eTypeElement.Cloth:
                nameTxt.text = "Cloth";
                contentTxt.text = "Collect and fill the required number of hooks to unveil the cloth";
                break;
        }

        if (parentNewElement.childCount > 0)
        {
            parentNewElement.GetChild(0).gameObject.Recycle();
        }

        GameObject obj = GameManager.Instance.InstantiatePrefab("Icons/Elements/" + eTypeElement);
        obj.transform.parent = parentNewElement;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public override void Hide()
    {
        base.Hide();

        bg.DOFade(0f, 0.1f).SetEase(Ease.Linear);
        popup.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
