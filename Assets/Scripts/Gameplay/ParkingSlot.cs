using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class ParkingSlot : MonoBehaviour
{
    public int levelUnlock = 0;
    public bool isLocked = false;
    public GameObject objBase;
    public MeshRenderer meshRenderer;
    public SpriteRenderer bg;
    public Sprite[] backSprs;

    public GameObject objLock;
    public TextMeshPro txtUnlockLevel;
    public SkeletonAnimation animLock;

    public Collider tapColl;
    public Transform extraSlot;

    public ShooterController parkedShooter;
    public ShooterController extraParkedShooter;

    private void Start()
    {
        EventManager.StartListening(EventVariables.AddSlot, AddSlot);
    }

    public void Init()
    {
        int level = UserConfig.Instance.CurLevel;

        isLocked = level < levelUnlock;

        txtUnlockLevel.text = $"Level {levelUnlock}";

        if (level == levelUnlock && !PlayerPrefs.HasKey($"UnlockParkingSlotLevel{levelUnlock}"))
        {
            PlayerPrefs.SetInt($"UnlockParkingSlotLevel{levelUnlock}", 1);

            objLock.SetActive(true);
            animLock.AnimationState.SetAnimation(0, "close", true);

            Unlock(1.5f);
        }
        else
        {
            if (isLocked)
            {
                objLock.SetActive(true);
                animLock.AnimationState.SetAnimation(0, "close", true);
            }
            else
            {
                objLock.SetActive(false);
            }
        }

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        bg.sprite = backSprs[(int)eTypeLevel];
    }

    Tween tweenDelayUnlock;
    public void Unlock(float delay = 0f)
    {
        if (tweenDelayUnlock != null)
        {
            tweenDelayUnlock.Kill();
            tweenDelayUnlock = null;
        }

        tweenDelayUnlock = DOVirtual.DelayedCall(delay, () =>
        {
            animLock.AnimationState.SetAnimation(0, "open", false).Complete += (trackEntry) =>
            {
                objLock.SetActive(false);

                GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_AddScrewToTruck");
                obj.transform.position = objLock.transform.position + Vector3.back;
                obj.transform.localScale = Vector3.one;
                obj.transform.localEulerAngles = new Vector3(72, 0, 0);
            };

            tweenDelayUnlock = null;
        }, false);
    }

    Tween tweenWarning;
    int idBaseColor = Shader.PropertyToID("_BaseColor");
    Color warningColor = new Color(1f, 0.5f, 0.5f, 1f);
    public void Warning()
    {
        if (tweenWarning == null)
        {
            //tweenWarning = DOVirtual.Color(Color.white, warningColor, 0.25f, result =>
            //{
            //    meshRenderer.material.SetColor(idBaseColor, result);
            //}).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            //{
            //    tweenWarning = null;
            //});
            tweenWarning = bg.DOColor(Color.red, 0.25f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                tweenWarning = null;
            });
        }
    }

    public void AddSlot()
    {
        ParkingSlot slot = (ParkingSlot)EventManager.GetData(EventVariables.AddSlot);

        if (slot == this)
        {
            slot.isLocked = false;
            Unlock(0);
        }
    }

    public void RemoveShooter(ShooterController shooterToRemove)
    {
        if (parkedShooter == shooterToRemove)
        {
            parkedShooter = null;
        }

        if (extraParkedShooter == shooterToRemove)
        {
            extraParkedShooter = null;
        }

        ParkingManager.Instance.CheckWarning();
    }

    public void Recycle()
    {
        parkedShooter = null;
        extraParkedShooter = null;

        if (tweenDelayUnlock != null)
        {
            tweenDelayUnlock.Kill();
            tweenDelayUnlock = null;
        }
    }
}
