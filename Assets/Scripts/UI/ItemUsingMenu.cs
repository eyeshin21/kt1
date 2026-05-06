using DG.Tweening;
using Lean.Touch;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUsingMenu : UIMenu
{
    [SerializeField] Transform popup;
    [SerializeField] Image iconImg;
    [SerializeField] TextMeshProUGUI contentTxt;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] GameObject closeBtn;

    [HideInInspector] public eBooster eBooster;
    [HideInInspector] public bool isTut = false;

    ShooterController parkedShooter;
    ShooterController shooterToSwap;

    public override void Show()
    {
        base.Show();

        popup.localScale = Vector3.zero;
        popup.DOScale(Vector3.one * 0.8f, 0.5f).SetEase(Ease.OutBack);

        GameManager.Instance.canControl = false;

        UIManager.Instance.ingameMenu.ActiveUI(false);

        parkedShooter = null;
        shooterToSwap = null;

        switch (eBooster)
        {
            case eBooster.Swap:
                nameTxt.text = "Swap";
                contentTxt.text = "Choose a Hook on the waiting slot!";
                break;
            case eBooster.Magnet:
                nameTxt.text = "Magnet";
                contentTxt.text = "Choose a Hook on the waiting slot!";
                break;
        }

        iconImg.sprite = GameManager.Instance.LoadSprite("Icons/Boosters/" + eBooster);
        //iconImg.SetNativeSize();

        foreach (var parkedShooter in ParkingManager.Instance.parkedShooters)
        {
            parkedShooter.tapColl.enabled = true;
        }

        LeanTouch.OnFingerTap += HandleFingerTap;

        closeBtn.SetActive(!isTut);

        if (isTut && ParkingManager.Instance.parkedShooters.Count > 0)
        {
            UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("", Vector2.zero, Vector2.zero, Vector2.zero, GameplayController.Instance.canvas.WorldToCanvasPosition(ParkingManager.Instance.parkedShooters[0].transform.position, GameplayController.Instance.cam), 0);
        }
        //UIManager.Instance.ShowBGItem(null);
    }

    void HandleFingerTap(LeanFinger finger)
    {
        Ray ray = finger.GetRay();

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.TryGetComponent(out ShooterController shooter))
            {
                if (eBooster == eBooster.Swap)
                {
                    if (parkedShooter == null && shooter.slot != null && !shooter.isShooting && shooter.capacity > 0)
                    {
                        parkedShooter = shooter;
                        parkedShooter.Select();

                        contentTxt.text = "Choose a Shooter to swap!";

                        SoundManager.instance.PlaySound("Select");
                        HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

                        if (isTut)
                        {
                            UIManager.Instance.ingameMenu.tutorialUI.ShowHandTutorial("", Vector2.zero, Vector2.zero, Vector2.zero, GameplayController.Instance.canvas.WorldToCanvasPosition(ShooterManager.Instance.shooters[3].transform.position, GameplayController.Instance.cam), 0);
                        }
                    }
                    else if (shooterToSwap == null && shooter.slot == null)
                    {
                        if (shooter.isLinked || shooter.isHidden || shooter.hasIce || shooter.hasCrate || shooter.isLock || shooter.isLockChain || shooter.hasPin || shooter.hasCloth || (shooter.hasShutter && !shooter.isShutterOpen))
                        {
                            shooter.Shake();
                            return;
                        }

                        if (isTut)
                        {
                            if (ShooterManager.Instance.shooters.IndexOf(shooter) == 3)
                            {
                                shooterToSwap = shooter;
                                isTut = false;
                                UIManager.Instance.ingameMenu.tutorialUI.Hide();

                                SoundManager.instance.PlaySound("Select");
                                HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                            }
                        }
                        else
                        {
                            shooterToSwap = shooter;

                            SoundManager.instance.PlaySound("Select");
                            HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
                        }
                    }

                    if (parkedShooter != null && shooterToSwap != null)
                    {
                        UserConfig.Instance.AmountSwap--;
                        UIManager.Instance.ingameMenu.SetStateSwapItem();

                        ShooterManager.Instance.SwapShooter(parkedShooter, shooterToSwap);

                        Bridge.Instance.TrackUseBooster("swap", UserConfig.Instance.CurLevel);
                        Hide();
                    }
                }
                else if (eBooster == eBooster.Magnet)
                {
                    if (shooter.slot != null && shooter.realCapacity > 0)
                    {
                        if (isTut)
                        {
                            isTut = false;
                            UIManager.Instance.ingameMenu.tutorialUI.Hide();
                        }

                        List<ScrewController> screws = GameplayController.Instance.levelController.screws;
                        List<ScrewController> screwsToFill = new List<ScrewController>();
                        for (int j = 0; j < screws.Count; j++)
                        {
                            if (screws[j].color == shooter.color)
                            {
                                screwsToFill.Add(screws[j]);

                                if (screwsToFill.Count == shooter.realCapacity)
                                {
                                    break;
                                }
                            }
                        }

                        for (int j = 0; j < screwsToFill.Count; j++)
                        {
                            shooter.Shoot(screwsToFill[j]);
                        }

                        UserConfig.Instance.AmountMagnet--;
                        UIManager.Instance.ingameMenu.SetStateMagnetItem();

                        SoundManager.instance.PlaySound("Select");
                        HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

                        Bridge.Instance.TrackUseBooster("magnet", UserConfig.Instance.CurLevel);
                        Hide();
                    }
                }
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
        LeanTouch.OnFingerTap -= HandleFingerTap;

        if (parkedShooter != null)
        {
            parkedShooter.Unselect();
            parkedShooter = null;
        }
        shooterToSwap = null;

        foreach (var parkedShooter in ParkingManager.Instance.parkedShooters)
        {
            parkedShooter.tapColl.enabled = false;
        }

        popup.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            //UIManager.Instance.bgItem.gameObject.SetActive(false);
            gameObject.SetActive(false);
            UIManager.Instance.ingameMenu.ActiveUI(true);
            GameManager.Instance.canControl = true;
        });
    }
}
