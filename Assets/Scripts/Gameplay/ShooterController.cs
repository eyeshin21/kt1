using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;

public class ShooterController : MonoBehaviour
{
    [Header("Config")]
    public float moveSpeed;
    public bool isShooting = false;

    [Header("Shooter Data")]
    public bool isActive = false;
    public ColorEnum color;
    public int capacity;
    public int realCapacity;
    public Vector2Int pos;
    public Vector2Int outPos;
    public ParkingSlot slot;

    [Header("Model")]
    public TextMeshPro txtCapacity;
    public GameObject objModel;
    public GameObject objInActive;
    public Transform shake;
    public Transform scale;
    public Renderer[] inActiveRenderers;
    public Renderer[] activeRenderers;
    public Collider tapColl;
    public Animator anim;
    public Outline outline;

    [Header("Element Hidden")]
    public bool isHidden;
    public GameObject objHidden;

    [Header("Element Link")]
    public bool isLinked;
    public Vector2Int linkedPos;
    public ShooterController linkedShooter;
    public GameObject objLink;

    [Header("Element Ice")]
    public bool hasIce;
    public int iceCount;
    public GameObject objIce;
    public TextMeshPro txtIceCount;
    public ParticleSystem vfxIceBreak;
    public ParticleSystem vfxIceBreakEnd;

    [Header("Element Crate")]
    public bool hasCrate;
    public int crateCount;
    public GameObject objCrate;
    public TextMeshPro txtCrateCount;
    public ParticleSystem vfxCrateBreak;
    public ParticleSystem vfxCrateBreakEnd;

    [Header("Element Pin")]
    public bool hasPin = false;

    [Header("Element Cloth")]
    public bool hasCloth = false;

    [Header("Element LockChain")]
    public bool isLockChain = false;

    [Header("Element Shutter")]
    public bool hasShutter;
    public bool isShutterOpen;
    public GameObject objShutter;
    public Transform[] shutterDoors;

    [Header("Element Lock & Key")]
    public bool isLock;
    public bool isKey;
    public int lockCode;
    public GameObject objLock;
    public GameObject objKey;
    public Transform unlockPos;
    public MeshRenderer lockRenderer;
    public MeshRenderer keyRenderer;
    public MeshRenderer keyAnimRenderer;
    public Animator animLock;
    public Animator animKey;

    [Header("Hook")]
    public GameObject fakeHook;
    public Transform shootPoint;
    public GameObject hookPrefab;
    public List<HookController> hooks = new List<HookController>();
    public AnimationCurve[] curves;

    private void Start()
    {
        EventManager.StartListening(EventVariables.OnShooterMoveOut, OnShooterMoveOut);
        EventManager.StartListening(EventVariables.CheckShutter, CheckShutter);
        EventManager.StartListening(EventVariables.UnlockShooter, UnlockShooter);
    }

    public void Init(ShooterData data, Vector2Int pos)
    {
        this.pos = pos;
        isHidden = data.isHidden;
        color = data.color;
        capacity = data.capacity;
        realCapacity = data.capacity;
        isLinked = data.isLink;
        if (isLinked)
        {
            linkedPos = data.linkedPos.ToVector2Int();
        }

        hasIce = data.hasIce;
        iceCount = data.iceCount;
        objIce.SetActive(hasIce);
        txtIceCount.text = iceCount.ToString();

        hasCrate = data.hasCrate;
        crateCount = data.crateCount;
        objCrate.SetActive(hasCrate);
        txtCrateCount.text = crateCount.ToString();

        hasShutter = data.hasShutter;
        isShutterOpen = data.isShutterOpen;
        objShutter.SetActive(data.hasShutter);
        for (int i = 0; i < shutterDoors.Length; i++)
        {
            shutterDoors[i].localScale = isShutterOpen ? new Vector3(0, 1, 1) : Vector3.one;
        }

        if (hasShutter)
        {
            scale.localScale = isShutterOpen ? Vector3.one : Vector3.zero;
            scale.gameObject.SetActive(isShutterOpen);
        }
        else
        {
            scale.gameObject.SetActive(true);
            scale.localScale = Vector3.one;
        }

        isLock = data.isLock;
        isKey = data.isKey;
        lockCode = data.lockCode;
        objLock.SetActive(isLock);
        objKey.SetActive(isKey);

        objKey.transform.localPosition = Vector3.back * 0.7f;
        objKey.transform.localEulerAngles = Vector3.forward * 45f;

        if (isLock || isKey)
        {
            lockRenderer.sharedMaterial = MaterialCache.GetLockMat(lockCode);
            keyRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);
            keyAnimRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);
        }

        if (isKey)
        {
            animKey.SetTrigger("Action");
        }

        objHidden.SetActive(isHidden);
        objInActive.SetActive(!isHidden);
        objModel.SetActive(false);
        fakeHook.SetActive(false);

        SetColor();

        txtCapacity.text = capacity.ToString();
        txtCapacity.gameObject.SetActive(false);

        transform.localScale = Vector3.one;
    }

    public void SetColor()
    {
        var shooterInactiveMat = MaterialCache.GetShooterInactiveMat(color);
        var shooterActiveSelectableMat = MaterialCache.GetShooterActiveSelectableMat(color);

        for (int i = 0; i < inActiveRenderers.Length; i++)
        {
            inActiveRenderers[i].sharedMaterial = shooterInactiveMat;
        }

        for (int i = 0; i < activeRenderers.Length; i++)
        {
            activeRenderers[i].sharedMaterial = shooterActiveSelectableMat;
        }
    }

    public void SetColorActive()
    {
        var shooterActiveMat = MaterialCache.GetShooterActiveMat(color);

        for (int i = 0; i < activeRenderers.Length; i++)
        {
            activeRenderers[i].sharedMaterial = shooterActiveMat;
        }
    }

    public void Recycle()
    {
        tapColl.enabled = true;

        transform.DOKill();
        isRotateToDefault = false;

        isShooting = false;
        isActive = false;
        hasIce = false;
        iceCount = 0;
        isLock = false;
        isKey = false;
        isHidden = false;
        hasCrate = false;
        crateCount = 0;
        hasPin = false;
        hasCloth = false;
        hasShutter = false;
        isShutterOpen = false;
        isLockChain = false;

        StopAllCoroutines();
        coroutineShoot = null;
        coroutineCheckShoot = null;
        coroutineCheckPullBack = null;

        for (int i = 0; i < hooks.Count; i++)
        {
            if (hooks[i] != null)
            {
                hooks[i].Recycle();
            }
        }
        hooks.Clear();

        objHidden.SetActive(false);
        objInActive.SetActive(false);
        objModel.SetActive(false);
        objLink.SetActive(false);
        objIce.SetActive(false);
        objCrate.SetActive(false);
        objShutter.SetActive(false);

        scale.localScale = Vector3.one;

        fakeHook.SetActive(false);

        screwsQueue.Clear();

        slot = null;
        linkedShooter = null;

        outline.enabled = false;

        gameObject.Recycle();
    }

    public void OnShooterMoveOut()
    {
        if (hasIce)
        {
            if (isActive)
            {
                iceCount--;
                txtIceCount.text = iceCount.ToString();

                if (iceCount == 0)
                {
                    hasIce = false;
                    objIce.SetActive(false);

                    Active2();

                    vfxIceBreakEnd.Play();
                }
                else
                {
                    vfxIceBreak.Play();
                    PunchScale();
                }
            }
        }

        if (hasCrate)
        {
            if (isActive)
            {
                crateCount--;
                txtCrateCount.text = crateCount.ToString();

                if (crateCount == 0)
                {
                    hasCrate = false;
                    objCrate.SetActive(false);

                    Active2();

                    vfxCrateBreakEnd.Play();
                }
                else
                {
                    vfxCrateBreak.Play();
                    PunchScale();
                }
            }
        }
    }

    public void CheckShutter()
    {
        if (hasShutter)
        {
            isShutterOpen = !isShutterOpen;

            if (isShutterOpen)
            {
                for (int i = 0; i < shutterDoors.Length; i++)
                {
                    shutterDoors[i].DOKill(true);
                    shutterDoors[i].DOScaleX(0f, 0.25f).SetEase(Ease.Linear);
                }
                scale.transform.DOKill(true);
                scale.gameObject.SetActive(true);
                scale.DOScale(1f, 0.25f).SetDelay(0.1f).SetEase(Ease.OutBack);
            }
            else
            {
                for (int i = 0; i < shutterDoors.Length; i++)
                {
                    shutterDoors[i].DOKill(true);
                    shutterDoors[i].DOScaleX(1f, 0.25f).SetDelay(0.1f).SetEase(Ease.Linear);
                }

                scale.transform.DOKill(true);
                scale.DOScale(0f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    scale.gameObject.SetActive(false);
                });
            }
        }
    }

    public void UnlockShooter()
    {
        if (isLock)
        {
            ShooterController keyShooter = (ShooterController)EventManager.GetData(EventVariables.UnlockShooter);
            if (keyShooter.lockCode == lockCode)
            {
                objKey.transform.position = keyShooter.objKey.transform.position;
                objKey.transform.eulerAngles = keyShooter.objKey.transform.eulerAngles;
                objKey.SetActive(true);

                Vector3[] path = new Vector3[]
                {
                    ((objKey.transform.position + unlockPos.position) / 2) + Vector3.back * 2f,
                    unlockPos.position
                };

                objKey.transform.DOScale(2f, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    objKey.transform.DOScale(1f, 0.25f).SetEase(Ease.Linear);
                });
                objKey.transform.DORotateQuaternion(unlockPos.rotation, 0.5f).SetEase(Ease.Linear);
                objKey.transform.DOPath(path, 0.5f, PathType.CatmullRom, PathMode.Full3D).SetEase(Ease.Linear).OnComplete(() =>
                {
                    objKey.SetActive(false);
                    animLock.SetTrigger("Unlock");
                    DOVirtual.DelayedCall(0.5f, OnEndAnimUnlock, false);
                });
            }
        }
    }

    public void OnEndAnimUnlock()
    {
        isLock = false;
        objLock.SetActive(false);
        if (isActive)
        {
            Active2();
        }
    }

    public void CheckLink()
    {
        if (isLinked)
        {
            if (linkedShooter == null)
            {
                linkedShooter = ShooterManager.Instance.shooterGrid[linkedPos.x, linkedPos.y];
                linkedShooter.linkedShooter = this;

                objLink.SetActive(true);

                if (!isActive)
                {
                    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.273f);
                }
                else
                {
                    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                }

                if (pos.x == linkedPos.x)
                {
                    if (pos.y + 1 == linkedPos.y)
                    {
                        objLink.transform.localEulerAngles = Vector3.zero;
                    }
                    else if (pos.y - 1 == linkedPos.y)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 180f;
                    }
                }
                else if (pos.y == linkedPos.y)
                {
                    if (pos.x + 1 == linkedPos.x)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 270f;
                    }
                    else if (pos.x - 1 == linkedPos.x)
                    {
                        objLink.transform.localEulerAngles = Vector3.forward * 90f;
                    }
                }
            }
        }
    }

    public void OnTap()
    {
        if (isActive && !hasIce && !isLock && !hasCrate && !hasPin && !hasCloth && !isLockChain)
        {
            if (hasShutter && !isShutterOpen) return;

            if (!isLinked)
            {
                slot = ParkingManager.Instance.GetFreeSlot();
                if (slot != null)
                {
                    EventManager.EmitEvent(EventVariables.OnShooterMoveOut);
                    ShooterManager.Instance.CanMoveOut(pos, out List<Point> path);
                    tapColl.enabled = false;
                    ShooterManager.Instance.RemoveShooter(this);
                    EventManager.EmitEvent(EventVariables.CheckPin);

                    slot.parkedShooter = this;
                    Move(path);
                }
                else
                {
                    Shake();
                    //WarningMenu.Instance.Show(Vector2.zero, "The parking space is full!");
                }
            }
            else
            {
                if (ParkingManager.Instance.Is2SlotAvailable(out ParkingSlot slot1, out ParkingSlot slot2))
                {
                    if (ShooterManager.Instance.CanMoveOut(pos, out List<Point> path))
                    {
                        EventManager.EmitEvent(EventVariables.OnShooterMoveOut);
                        slot = slot1;
                        tapColl.enabled = false;
                        slot.parkedShooter = this;
                        Move(path);
                        ShooterManager.Instance.RemoveShooter(this, false, false);

                        EventManager.EmitEvent(EventVariables.OnShooterMoveOut);
                        linkedShooter.slot = slot2;
                        linkedShooter.tapColl.enabled = false;
                        linkedShooter.slot.parkedShooter = linkedShooter;
                        ShooterManager.Instance.CanMoveOut(linkedShooter.pos, out path);
                        linkedShooter.Move(path);
                        ShooterManager.Instance.RemoveShooter(linkedShooter, true, false);
                    }
                    else
                    {
                        EventManager.EmitEvent(EventVariables.OnShooterMoveOut);
                        linkedShooter.slot = slot1;
                        linkedShooter.tapColl.enabled = false;
                        linkedShooter.slot.parkedShooter = linkedShooter;
                        ShooterManager.Instance.CanMoveOut(linkedShooter.pos, out path);
                        linkedShooter.Move(path);
                        ShooterManager.Instance.RemoveShooter(linkedShooter, false, false);

                        EventManager.EmitEvent(EventVariables.OnShooterMoveOut);
                        slot = slot2;
                        tapColl.enabled = false;
                        slot.parkedShooter = this;
                        ShooterManager.Instance.CanMoveOut(pos, out path);
                        Move(path);
                        ShooterManager.Instance.RemoveShooter(this, true, false);
                    }

                    EventManager.EmitEvent(EventVariables.CheckPin);

                    objLink.SetActive(false);
                    linkedShooter.objLink.SetActive(false);

                    isLinked = false;
                    linkedShooter.isLinked = fakeHook;
                    linkedShooter.linkedShooter = null;
                    linkedShooter = null;

                    ShooterManager.Instance.CheckActiveShooter();
                }
                else
                {
                    Shake();
                    //WarningMenu.Instance.Show(Vector2.zero, "Not enough parking space!");
                }
            }
        }
        else
        {
            Shake();
        }

        /*
        if (!isLinked)
        {
            if (ShooterManager.Instance.CanMoveOut(pos, out List<Point> path))
            {
                slot = ParkingManager.Instance.GetFreeSlot();
                if (slot != null)
                {
                    tapColl.enabled = false;
                    ShooterManager.Instance.RemoveShooter(this);

                    slot.parkedShooter = this;
                    Move(path);
                }
                else
                {
                    Shake();
                    WarningMenu.Instance.Show(Vector2.zero, "The parking space is full!");
                }
            }
            else
            {
                Shake();
            }
        }
        else
        {
            if (CanMoveOut())
            {
                if (ParkingManager.Instance.Is2SlotAvailable(out ParkingSlot slot1, out ParkingSlot slot2))
                {
                    if (ShooterManager.Instance.CanMoveOut(pos, out List<Point> path))
                    {
                        slot = slot1;
                        tapColl.enabled = false;
                        ShooterManager.Instance.RemoveShooter(this);
                        slot.parkedShooter = this;
                        Move(path);

                        linkedShooter.slot = slot2;
                        linkedShooter.tapColl.enabled = false;
                        ShooterManager.Instance.RemoveShooter(linkedShooter);
                        linkedShooter.slot.parkedShooter = linkedShooter;
                        ShooterManager.Instance.CanMoveOut(linkedShooter.pos, out path);
                        linkedShooter.Move(path);
                    }
                    else
                    {
                        linkedShooter.slot = slot1;
                        linkedShooter.tapColl.enabled = false;
                        ShooterManager.Instance.RemoveShooter(linkedShooter);
                        linkedShooter.slot.parkedShooter = linkedShooter;
                        ShooterManager.Instance.CanMoveOut(linkedShooter.pos, out path);
                        linkedShooter.Move(path);

                        slot = slot2;
                        tapColl.enabled = false;
                        ShooterManager.Instance.RemoveShooter(this);
                        slot.parkedShooter = this;
                        ShooterManager.Instance.CanMoveOut(pos, out path);
                        Move(path);
                    }

                    objLink.SetActive(false);
                    linkedShooter.objLink.SetActive(false);
                }
                else
                {
                    Shake();
                    WarningMenu.Instance.Show(Vector2.zero, "Not enough parking space!");
                }
            }
            else
            {
                Shake();
            }
        }
        */
    }

    Tween tweenSelect;
    public void Select()
    {
        tapColl.enabled = false;
        outline.enabled = true;

        tweenSelect = DOVirtual.Float(0.5f, 2f, 1f, result =>
        {
            outline.OutlineWidth = result;
        }).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
    }

    public void Unselect()
    {
        outline.enabled = false;

        if (tweenSelect != null)
        {
            tweenSelect.Kill();
            tweenSelect = null;
        }
    }

    public void Swap(int newCapacity, ColorEnum newColor)
    {
        realCapacity = newCapacity;
        capacity = newCapacity;
        color = newColor;

        SetColor();
        txtCapacity.text = capacity.ToString();
    }

    public void ClearFromSlot()
    {
        StopCheckShoot();

        slot.parkedShooter = null;
        slot.extraParkedShooter = this;

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalk", true);

        float targetY = transform.localPosition.y + 1;
        transform.DOLocalRotate(Vector3.zero, 0.1f);
        transform.DOLocalMoveY(targetY, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
        {
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsWalk", false);
            CheckShoot();
        });
    }

    public void ClearToSlot(ParkingSlot parkingSlot)
    {
        StopCheckShoot();

        slot.parkedShooter = null;
        slot = parkingSlot;
        slot.extraParkedShooter = this;

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalk", true);

        Vector3 direction = slot.extraSlot.position - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction);

        transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.Linear);
        transform.DOMove(slot.extraSlot.position, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
        {
            anim.SetBool("IsIdle", true);
            anim.SetBool("IsWalk", false);
            CheckShoot();
        });
    }

    public bool CanUndo()
    {
        if (!isShooting && realCapacity > 0 && ShooterManager.Instance.shooterGrid[pos.x, pos.y] == null)
        {
            return true;
        }

        return false;
    }

    public void Undo()
    {
        StopCheckShoot();

        ShooterManager.Instance.GetPath(outPos, pos, out List<Point> path);

        fakeHook.SetActive(false);

        slot.RemoveShooter(this);
        ParkingManager.Instance.parkedShooters.Remove(this);
        slot = null;

        ShooterManager.Instance.shooterGrid[pos.x, pos.y] = this;
        ShooterManager.Instance.shooters.Add(this);
        ShooterManager.Instance.CheckActiveShooter();

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalk", true);

        if (path != null && path.Count > 0)
        {
            Vector3[] waypoints = new Vector3[path.Count];
            for (int i = 0; i < path.Count; i++)
            {
                waypoints[i] = ShooterManager.Instance.GetWorldPos(path[i].x, path[i].y);
            }

            transform.DOLocalPath(waypoints, moveSpeed, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.Linear).SetSpeedBased(true).SetLookAt(0, Vector3.forward, Vector3.right).OnComplete(() =>
            {
                anim.SetBool("IsIdle", true);
                anim.SetBool("IsWalk", false);
                SetColor();
                transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast).OnComplete(() =>
                {
                    tapColl.enabled = true;
                });
            });

        }
        else
        {
            Vector3 targetPos = ShooterManager.Instance.GetWorldPos(pos.x, pos.y);
            Vector3 direction = targetPos - transform.localPosition;

            transform.DOKill();
            transform.DORotateQuaternion(Quaternion.FromToRotation(Vector3.up, direction), 0.1f).SetEase(Ease.Linear);
            transform.DOLocalMove(targetPos, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
            {
                anim.SetBool("IsIdle", true);
                anim.SetBool("IsWalk", false);
                SetColor();
                transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast).OnComplete(() =>
                {
                    tapColl.enabled = true;
                });
            });
        }
    }

    public void MoveToSpawnPos(bool fromTunnel = false)
    {
        Vector3 spawnWorldPos = ShooterManager.Instance.GetWorldPos(pos.x, pos.y);
        transform.DOLocalMove(spawnWorldPos, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
        {
            Active2();
        });

        if (fromTunnel)
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void CheckKey()
    {
        if (isKey)
        {
            isKey = false;
            objKey.SetActive(false);
            EventManager.EmitEventData(EventVariables.UnlockShooter, this);
        }
    }

    public void DisableShutter()
    {
        if (hasShutter)
        {
            hasShutter = false;
            objShutter.SetActive(false);
        }
    }

    public void Move(List<Point> path)
    {
        GameplayController.Instance.StopCheckLose();

        CheckKey();
        DisableShutter();

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalk", true);

        scale.DOKill(true);
        scale.localScale = Vector3.one;
        scale.DOPunchScale(Vector3.one * -0.2f, 0.15f).SetEase(Ease.Linear).OnComplete(() =>
        {
            SetColorActive();

            if (path.Count > 0)
            {
                outPos = new Vector2Int(path[path.Count - 1].x, path[path.Count - 1].y);

                Vector3[] waypoints = new Vector3[path.Count + 1];
                for (int i = 0; i < path.Count; i++)
                {
                    waypoints[i] = ShooterManager.Instance.GetWorldPos(path[i].x, path[i].y);
                }
                waypoints[path.Count] = ShooterManager.Instance.GetWorldPos(path[path.Count - 1].x, path[path.Count - 1].y + 1);
                lastLocalPos = transform.localPosition;
                transform.DOLocalPath(waypoints, moveSpeed, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.Linear).SetSpeedBased(true).OnUpdate(OnFollowingPath).OnComplete(() =>
                {
                    MoveToParkingSlot();
                });
            }
            else
            {
                outPos = pos;

                Vector3 targetPos = ShooterManager.Instance.GetWorldPos(pos.x, pos.y + 1);
                transform.DOLocalMove(targetPos, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
                {
                    MoveToParkingSlot();
                });
            }
        });

        EventManager.EmitEvent(EventVariables.CheckShutter);
    }

    Vector3 lastLocalPos;
    float zOffsetDegrees = 0f;
    float smoothing = 1f;
    private void OnFollowingPath()
    {
        Vector3 currLocalPos = transform.localPosition;
        Vector3 delta = currLocalPos - lastLocalPos;

        if (delta.sqrMagnitude > 0.0001f)
        {
            float angleDeg = Mathf.Atan2(-delta.x, delta.y) * Mathf.Rad2Deg;
            angleDeg += zOffsetDegrees;

            Vector3 curLocalEuler = transform.localEulerAngles;
            float newZ = Mathf.LerpAngle(curLocalEuler.z, angleDeg, smoothing);
            transform.localEulerAngles = new Vector3(curLocalEuler.x, curLocalEuler.y, newZ);
        }

        lastLocalPos = currLocalPos;
    }

    public void MoveToParkingSlot()
    {
        //Debug.Log(GameplayController.Instance.board.up);
        GameplayController.Instance.StopCheckLose();

        Vector3 directionToParkingSlot = slot.transform.position - transform.position;
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, directionToParkingSlot);

        transform.DORotateQuaternion(targetRotation, 0.1f).SetEase(Ease.Linear);
        transform.DOMove(slot.transform.position, moveSpeed).SetEase(Ease.Linear).SetSpeedBased(true).OnComplete(() =>
        {
            GameplayController.Instance.StopCheckLose();

            anim.SetBool("IsIdle", true);
            anim.SetBool("IsWalk", false);

            fakeHook.SetActive(true);
            txtCapacity.gameObject.SetActive(true);

            ParkingManager.Instance.parkedShooters.Add(this);

            CheckShoot();

            ParkingManager.Instance.CheckWarning();
            GameplayController.Instance.CheckLose();
        });
    }

    public void MoveOut()
    {
        ParkingManager.Instance.parkedShooters.Remove(this);
        slot.RemoveShooter(this);

        //float targetX = transform.position.x >= 0 ? 12f : -12f;
        //float targetY = transform.position.y + 1f;

        //Vector3[] waypoints = new Vector3[]
        //{
        //    new Vector3(transform.position.x, targetY, 0),
        //    new Vector3(targetX, targetY, 0)
        //};

        float targetLocalX = transform.localPosition.x >= 0 ? 12f : -12f;
        float targetLocalY = transform.localPosition.y + 1f;

        Vector3 pos1 = transform.parent.TransformPoint(new Vector3(transform.localPosition.x, targetLocalY, 0));
        Vector3 pos2 = transform.parent.TransformPoint(new Vector3(targetLocalX, targetLocalY, 0));

        Vector3[] waypoints = new Vector3[]
        {
            pos1,
            pos2
        };

        anim.SetBool("IsIdle", false);
        anim.SetBool("IsWalk", true);

        lastLocalPos = transform.localPosition;
        transform.DOPath(waypoints, moveSpeed, PathType.CatmullRom, PathMode.TopDown2D).SetEase(Ease.Linear).SetSpeedBased(true).OnUpdate(OnFollowingPath).OnComplete(() =>
        {
            Recycle();
        });

        EventManager.EmitEvent(EventVariables.CheckCloth);

        GameplayController.Instance.CheckLose();
    }

    public void CheckShoot()
    {
        StopCheckShoot();
        coroutineCheckShoot = StartCoroutine(IE_CheckShoot());
    }

    public void StopCheckShoot()
    {
        if (coroutineCheckShoot != null)
        {
            StopCoroutine(coroutineCheckShoot);
            coroutineCheckShoot = null;
        }

        isRotateToDefault = false;
    }

    WaitUntil waitUntilPriority;
    WaitUntil waitUntilCanControl = new WaitUntil(() => GameManager.Instance.canControl);
    WaitForSeconds waitForCheckShoot = new WaitForSeconds(0.1f);
    bool isRotateToDefault = false;
    Queue<ScrewController> screwsQueue = new Queue<ScrewController>();

    Coroutine coroutineCheckShoot;
    IEnumerator IE_CheckShoot()
    {
        isShooting = false;

        if (transform.eulerAngles != Vector3.zero && !isRotateToDefault)
        {
            isRotateToDefault = true;
            transform.DOKill();
            transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast).OnComplete(() =>
            {
                isRotateToDefault = false;
            });
        }

        if (!GameManager.Instance.canControl)
        {
            yield return waitUntilCanControl;
        }

        if (!CheckPriority())
        {
            if (waitUntilPriority == null)
            {
                waitUntilPriority = new WaitUntil(() => CheckPriority());
            }
            yield return waitUntilPriority;
        }

        while (realCapacity > 0)
        {
            List<ScrewController> screwsToShoot = new List<ScrewController>();
            List<ScrewController> screwsActive = GameplayController.Instance.levelController.screwsActive;
            for (int i = 0; i < screwsActive.Count; i++)
            {
                if (screwsActive[i].color == color && screwsActive[i].CanPullUp())
                {
                    screwsToShoot.Add(screwsActive[i]);
                }
            }

            if (screwsToShoot.Count > 0)
            {
                if (realCapacity < screwsToShoot.Count)
                {
                    int diff = screwsToShoot.Count - realCapacity;
                    for (int i = 0; i < diff; i++)
                    {
                        screwsToShoot.RemoveAt(screwsToShoot.Count - 1);
                    }
                }

                for (int i = 0; i < screwsToShoot.Count; i++)
                {
                    Shoot(screwsToShoot[i]);
                }
            }

            if (!isShooting)
            {
                if (transform.eulerAngles != Vector3.zero && !isRotateToDefault)
                {
                    isRotateToDefault = true;
                    transform.DOKill();
                    transform.DOLocalRotate(Vector3.zero, 0.5f, RotateMode.Fast).OnComplete(() =>
                    {
                        isRotateToDefault = false;
                    });
                }
            }

            yield return waitForCheckShoot;
        }
    }

    bool CheckPriority()
    {
        List<ShooterController> parkedShooters = ParkingManager.Instance.parkedShooters;

        int index = parkedShooters.IndexOf(this);

        for (int i = index - 1; i >= 0; i--)
        {
            if (parkedShooters[i].color == color && (parkedShooters[i].realCapacity > 0))// || parkedShooters[i].isShooting || parkedShooters[i].CanShootAnyScrew()))
            {
                return false;
            }
        }

        return true;
    }

    public void Shoot(ScrewController screw)
    {
        screw.PreHook();
        screwsQueue.Enqueue(screw);
        realCapacity--;

        if (coroutineShoot == null)
        {
            coroutineShoot = StartCoroutine(IE_Shoot());
        }
    }

    WaitForSeconds waitShootWarmup = new WaitForSeconds(0.2f);
    WaitForSeconds waitForHook = new WaitForSeconds(0.1f);

    Coroutine coroutineShoot;
    IEnumerator IE_Shoot()
    {
        StopCheckPullBack();

        transform.DOKill();
        isRotateToDefault = false;

        isShooting = true;
        anim.SetTrigger("Shoot");

        if (hooks.Count == 0)
        {
            Vector3 midPoint = Vector3.zero;
            foreach (var screw in screwsQueue)
            {
                midPoint += screw.transform.position;
            }
            midPoint /= screwsQueue.Count;

            Vector3 direction = midPoint - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.up, direction);
            transform.localEulerAngles = Vector3.forward * transform.localEulerAngles.z;
        }

        yield return waitShootWarmup;
        fakeHook.SetActive(false);

        while (screwsQueue.Count > 0)
        {
            ScrewController screwToHook = screwsQueue.Dequeue();

            Vector3 endPos = screwToHook.transform.position + Vector3.back * 0.5f;

            GameObject obj = hookPrefab.Spawn(shootPoint);
            HookController hook = obj.GetComponent<HookController>();
            hook.Init(this, color, shootPoint.position, endPos, curves[Random.Range(0, curves.Length)], Random.Range(10, 15));
            hook.Shoot(screwToHook);
            hooks.Add(hook);

            yield return waitForHook;
        }

        if (screwsQueue.Count > 0)
        {
            coroutineShoot = StartCoroutine(IE_Shoot());
        }
        else
        {
            coroutineCheckPullBack = StartCoroutine(IE_CheckPullBack());
            coroutineShoot = null;
        }
    }

    public void StopCheckPullBack()
    {
        if (coroutineCheckPullBack != null)
        {
            StopCoroutine(coroutineCheckPullBack);
            coroutineCheckPullBack = null;
        }
    }

    WaitUntil waitForPullBack;
    Coroutine coroutineCheckPullBack;
    IEnumerator IE_CheckPullBack()
    {
        if (waitForPullBack == null)
        {
            waitForPullBack = new WaitUntil(() => hooks.Count == 0);
        }
        yield return waitForPullBack;

        isShooting = false;
        fakeHook.SetActive(true);

        if (realCapacity > 0)
        {
            GameplayController.Instance.CheckLose();
        }
        else
        {
            MoveOut();
            GameplayController.Instance.CheckWin();
        }

        coroutineCheckPullBack = null;
    }

    public void AddHookScrew()
    {
        capacity--;
        if (capacity > 0)
        {
            txtCapacity.text = capacity.ToString();
        }
        else
        {
            txtCapacity.gameObject.SetActive(false);
        }
    }

    public bool CanMoveOut()
    {
        if (ShooterManager.Instance.CanMoveOut(pos) || (isLinked && ShooterManager.Instance.CanMoveOut(linkedPos)))
        {
            return true;
        }

        return false;
    }

    public bool CanTap()
    {
        if (isActive && !hasIce && !isLock && !hasCrate && !hasPin && !hasCloth && !isLockChain)
        {
            if (hasShutter && !isShutterOpen) return false;

            if (!isLinked)
            {
                return true;
            }
            else
            {
                if (ParkingManager.Instance.Is2SlotAvailable(out ParkingSlot slot1, out ParkingSlot slot2))
                {
                    return true;
                }
            }

            //if (!isLinked)
            //{
            //    slot = ParkingManager.Instance.GetFreeSlot();
            //    if (slot != null)
            //    {
            //        return true;
            //    }
            //}
            //else
            //{
            //    if (ParkingManager.Instance.Is2SlotAvailable(out ParkingSlot slot1, out ParkingSlot slot2))
            //    {
            //        return true;
            //    }
            //}
        }

        return false;
    }

    public void Active()
    {
        isActive = true;

        objInActive.SetActive(false);
        objModel.SetActive(true);

        anim.SetBool("IsIdle", true);
        //txtCapacity.gameObject.SetActive(true);
    }

    public void Active2()
    {
        isActive = true;

        if (hasIce)
        {

        }
        else if (hasCrate)
        {

        }
        else if (hasPin)
        {

        }
        else if (hasCloth)
        {

        }
        else if (isLock)
        {

        }
        else if (isLockChain)
        {

        }
        else
        {
            PunchScale();

            objInActive.SetActive(false);
            objModel.SetActive(true);

            anim.SetBool("IsIdle", true);
            //txtCapacity.gameObject.SetActive(true);

            if (isHidden)
            {
                isHidden = false;
                objHidden.SetActive(false);

                GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_Show");
                obj.transform.parent = transform;
                obj.transform.localPosition = Vector3.back * 0.5f;
                obj.transform.localScale = Vector3.one;
            }

            if (isLinked)
            {
                ShooterController linkedShooter = ShooterManager.Instance.shooterGrid[linkedPos.x, linkedPos.y];
                if (!linkedShooter.isActive)
                {
                    linkedShooter.Active2();
                }

                if (objLink.activeSelf)
                {
                    objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                }
            }
        }
    }

    public void CheckActive()
    {
        if (CanMoveOut())
        {
            if (!isActive)
            {
                isActive = true;

                if (hasIce)
                {

                }
                else if (hasCrate)
                {

                }
                else if (hasPin)
                {

                }
                else if (hasCloth)
                {

                }
                else if (isLock)
                {

                }
                else if (isLockChain)
                {

                }
                else
                {
                    PunchScale();

                    objInActive.SetActive(false);
                    objModel.SetActive(true);

                    anim.SetBool("IsIdle", true);
                    //txtCapacity.gameObject.SetActive(true);

                    if (isHidden)
                    {
                        isHidden = false;
                        objHidden.SetActive(false);

                        GameObject obj = GameManager.Instance.InstantiatePrefab("VFX/VFX_Show");
                        obj.transform.parent = transform;
                        obj.transform.localPosition = Vector3.back * 0.5f;
                        obj.transform.localScale = Vector3.one;
                    }

                    if (isLinked && objLink.activeSelf)
                    {
                        objLink.transform.GetChild(0).localPosition = new Vector3(0, 0.5f, -0.387f);
                    }
                }
            }
        }
        else
        {
            if (!isActive)
            {
                txtCapacity.gameObject.SetActive(false);
            }
            else
            {
                isActive = false;
                objInActive.SetActive(true);
                objModel.SetActive(false);
                txtCapacity.gameObject.SetActive(false);
            }
        }
    }

    public void PunchScale()
    {
        scale.DOKill(true);
        scale.DOPunchScale(Vector3.one * 0.3f, 0.25f).SetEase(Ease.Linear);
    }

    public void Shake()
    {
        shake.DOKill(true);
        shake.DOPunchRotation(Vector3.forward * 20f, 0.3f).SetEase(Ease.Linear);
    }

    public bool CanShootAnyScrew()
    {
        if (isShooting || realCapacity == 0) return true;
        if (!CheckPriority()) return false;

        List<ScrewController> screwActives = GameplayController.Instance.levelController.screwsActive;
        for (int i = 0; i < screwActives.Count; i++)
        {
            if (screwActives[i].color == color && screwActives[i].CanPullUp())
            {
                return true;
            }
        }

        return false;
    }


#if UNITY_EDITOR
    [SerializeField] private bool debug = false;
    void OnDrawGizmosSelected()
    {
        if (debug)
        {
            Vector2 origin = transform.position;
            Vector2 size = new Vector2(20, 2);
            float angle = 0f;
            Vector2 direction = Vector2.up;
            float distance = 5f;

            // Tính toán ma trận quay để vẽ Gizmo
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 halfExtents = size * 0.5f;

            // Vị trí sau khi cast
            Vector3 castEnd = origin + direction.normalized * distance;

            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(castEnd, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);

            Gizmos.color = Color.green;
            Gizmos.matrix = Matrix4x4.TRS(origin, rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, size);
        }
    }
#endif

}
