using DG.Tweening;
using UnityEngine;

public class TabMenu : UIMenu
{
    [SerializeField] Transform iconLockShop;
    [SerializeField] Transform iconLockSkin;

    public override void Show()
    {
        base.Show();
    }

    public void OnClickButtonHome()
    {

    }

    public void OnClickButtonShop()
    {
        iconLockShop.DOKill(true);
        iconLockShop.DOPunchRotation(Vector3.forward * 10, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true);

        WarningMenu.Instance.Show(Vector2.zero, "Coming soon!");
    }
    
    public void OnClickButtonSkin()
    {
        iconLockSkin.DOKill(true);
        iconLockSkin.DOPunchRotation(Vector3.forward * 10, 0.5f).SetEase(Ease.OutBounce).SetUpdate(true);

        WarningMenu.Instance.Show(Vector2.zero, "Coming soon!");
    }

    public override void Hide()
    {
        base.Hide();
        gameObject.SetActive(false);
    }

}
