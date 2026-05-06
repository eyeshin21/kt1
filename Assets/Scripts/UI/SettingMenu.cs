using DG.Tweening;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingMenu : UIMenu
{
    [SerializeField] GameObject onMusic;
    [SerializeField] GameObject switchMusic;
    [SerializeField] GameObject onSound;
    [SerializeField] GameObject switchSound;
    [SerializeField] GameObject onVibrate;
    [SerializeField] GameObject switchVibrate;

    [SerializeField] Transform board;
    [SerializeField] Image bg;

    [SerializeField] GameObject buttonHome;
    [SerializeField] GameObject buttonRestorePurchase;

    public override void Start()
    {
        base.Start();

        SetStateSound();
        SetStateMusic();
        SetStateVibrate();
    }

    public override void Show()
    {
        base.Show();

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.85f, 0.25f).SetEase(Ease.Linear);

        board.localScale = Vector3.zero;
        board.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        if (UIManager.Instance.mainMenu.gameObject.activeSelf)
        {
            buttonHome.SetActive(false);
        }
        else
        {
            buttonHome.SetActive(true);
        }

#if UNITY_IOS
        buttonRestorePurchase.SetActive(true);
#else
        buttonRestorePurchase.SetActive(false);
#endif

    }

    public override void Hide()
    {
        base.Hide();

        bg.DOFade(0, 0.1f).SetEase(Ease.Linear);
        board.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }

    void SetStateSound()
    {
        if (UserConfig.Instance.Sound)
        {
            onSound.SetActive(true);
            switchSound.transform.DOLocalMoveX(35f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            onSound.SetActive(false);
            switchSound.transform.DOLocalMoveX(-35f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    void SetStateMusic()
    {
        if (UserConfig.Instance.Music)
        {
            onMusic.SetActive(true);
            switchMusic.transform.DOLocalMoveX(35f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            onMusic.SetActive(false);
            switchMusic.transform.DOLocalMoveX(-35f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    void SetStateVibrate()
    {
        if (UserConfig.Instance.Vibrate)
        {
            onVibrate.SetActive(true);
            switchVibrate.transform.DOLocalMoveX(35f, 0.3f).SetEase(Ease.OutBack);
        }
        else
        {
            onVibrate.SetActive(false);
            switchVibrate.transform.DOLocalMoveX(-35f, 0.3f).SetEase(Ease.OutBack);
        }
    }

    public void PressedMusicBtn()
    {
        UserConfig.Instance.Music = !UserConfig.Instance.Music;
        EventManager.EmitEventData(EventVariables.ChangeMusic, UserConfig.Instance.Music);
        SetStateMusic();
    }

    public void PressedSoundBtn()
    {
        UserConfig.Instance.Sound = !UserConfig.Instance.Sound;
        SetStateSound();
    }

    public void PressedVibrateBtn()
    {
        UserConfig.Instance.Vibrate = !UserConfig.Instance.Vibrate;
        SetStateVibrate();
    }

    public void PressedResumeBtn()
    {
        Hide();
    }

    public void PressedRestorePurchaseBtn()
    {
        Bridge.Instance.RestorePurchase();
    }

    public void PressedHomeBtn()
    {
        if (UIManager.Instance.mainMenu.gameObject.activeSelf)
        {
            Hide();
        }
        else
        {
            UnityEvent e = new UnityEvent();
            e.AddListener(() =>
            {
                Hide();
                GameplayController.Instance.RecycleLevel();
                UIManager.Instance.ingameMenu.Hide();
                UIManager.Instance.mainMenu.Show();
            });
            FadeMenu.Instance.Fade(e);
            Bridge.Instance.OnGameFinished(false, ShooterManager.Instance.shooters.Count, UserConfig.Instance.CurLevel);
        }
    }
}
