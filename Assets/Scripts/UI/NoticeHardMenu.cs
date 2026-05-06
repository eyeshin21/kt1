using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeHardMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image bg;
    [SerializeField] Image icon;
    [SerializeField] Image imgName;
    [SerializeField] Image back;
    [SerializeField] Sprite[] iconSprs;
    [SerializeField] Sprite[] nameSprs;
    [SerializeField] Sprite[] backSprs;
    [SerializeField] ParticleSystem[] particleSystems;
    [SerializeField] Color[] colors;

    public override void Show()
    {
        base.Show();

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        icon.sprite = iconSprs[(int)eTypeLevel - 2];
        icon.SetNativeSize();

        imgName.sprite = nameSprs[(int)eTypeLevel - 2];
        imgName.SetNativeSize();

        back.sprite = backSprs[(int)eTypeLevel - 2];
        back.SetNativeSize();

        switch (eTypeLevel)
        {
            case eTypeLevel.Hard:
                for(int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].startColor = colors[0];
                }
                break;
            case eTypeLevel.SuperHard:
                for (int i = 0; i < particleSystems.Length; i++)
                {
                    particleSystems[i].startColor = colors[1];
                }
                break;
        }

        SoundManager.instance.PlaySound("Warning");
        UserConfig.Instance.LevelShowRemoveAds = UserConfig.Instance.CurLevel;

        bg.color = new Color(0, 0, 0, 0);
        bg.DOFade(0.9f, 0.25f).SetEase(Ease.Linear);

        popup.localScale = new Vector3(0, 0, 1);
        popup.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);

        Invoke(nameof(Hide), 1.25f);
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
