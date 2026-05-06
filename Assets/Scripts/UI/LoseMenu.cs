using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using TMPro;

public class LoseMenu : UIMenu
{
    [SerializeField] TextMeshProUGUI levelTxt;

    [Header("Animation")]
    [SerializeField] Image bg;
    [SerializeField] RectTransform boardRect;

    public override void Show()
    {
        base.Show();
        levelTxt.text = "Level " + UserConfig.Instance.CurLevel;

        Bridge.Instance.OnGameFinished(false, ShooterManager.Instance.shooters.Count, UserConfig.Instance.CurLevel);

        if (UIManager.Instance.settingMenu != null && UIManager.Instance.settingMenu.gameObject.activeInHierarchy)
        {
            UIManager.Instance.settingMenu.Hide();
        }

        bg.color = new Color(0, 0f, 0f, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        boardRect.localScale = Vector3.zero;
        boardRect.DOScale(1f, 0.25f).SetEase(Ease.OutBack);

        //boardRect.anchoredPosition = new Vector2(0, 1150);
        //boardRect.DOAnchorPosY(0, 0.25f).SetEase(Ease.OutBack).SetDelay(0.25f);
    }

    public override void Hide()
    {
        base.Hide();

        gameObject.SetActive(false);
    }

    public void PressedReplayBtn()
    {
        //UnityEvent evt = new UnityEvent();
        //evt.AddListener(() =>
        //{
        UserConfig.Instance.isRewardLive = true;
        UnityEvent e = new UnityEvent();
        e.AddListener(() =>
        {
            Hide();
            GameManager.Instance.LoadLevel();
            UIManager.Instance.ShowIngameMenu();
        });
        FadeMenu.Instance.Fade(e, true);
        //});
        //CGTeamBridge.instance.ShowInterstitial("replay", evt);
    }

    public void PressedBackBtn()
    {
        Hide();
    }
}
