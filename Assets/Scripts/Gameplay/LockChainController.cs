using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class LockChainController : MonoBehaviour
{
    public Transform offset;
    public Transform rotate;

    [Header("Block")]
    public Transform blockLeft;
    public Transform blockRight;

    [Header("Chain")]
    public GameObject chainPrefab;
    public Transform chainParent;
    public Transform chainParentLeft;
    public Transform chainParentRight;
    public List<GameObject> objChains = new List<GameObject>();

    [Header("Lock")]
    public GameObject objLock;
    public GameObject objKey;
    public Transform unlockPos;
    public MeshRenderer lockRenderer;
    public MeshRenderer keyRenderer;
    public MeshRenderer keyAnimRenderer;
    public Animator animLock;
    public Animator animKey;

    public Vector2Int pos;
    public List<ShooterController> shooters = new List<ShooterController>();

    bool isLock = false;
    Axis lockAxis;
    int lockCode;
    float length;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(EventVariables.UnlockShooter, Unlock);
    }

    public void Init(ShooterTileData data)
    {
        pos = data.pos.ToVector2Int();
        lockAxis = data.lockChainAxis;
        lockCode = data.lockChainCode;
        length = data.lockChainLength;

        isLock = true;

        lockRenderer.sharedMaterial = MaterialCache.GetLockMat(lockCode);
        keyRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);
        keyAnimRenderer.sharedMaterial = MaterialCache.GetKeyMat(lockCode);

        switch (lockAxis)
        {
            case Axis.Vertical:
                rotate.localEulerAngles = Vector3.forward * 90f;
                offset.localPosition = Vector3.up * ((length - 1) / 2f);
                break;
            case Axis.Horizontal:
                rotate.localEulerAngles = Vector3.zero;
                offset.localPosition = Vector3.right * ((length - 1) / 2f);
                break;
        }

        float posBlock = (length / 2f) + 0.4f;

        blockLeft.localPosition = new Vector3(-posBlock, blockLeft.localPosition.y, blockLeft.localPosition.z);
        blockRight.localPosition = new Vector3(posBlock, blockLeft.localPosition.y, blockLeft.localPosition.z);

        float spacing = 0.25f;
        int totalChain = Mathf.RoundToInt(length / spacing) + 2;

        float startX = -(((totalChain - 1) / 2f) * spacing);

        for (int i = 0; i < totalChain; i++)
        {
            GameObject objChain = chainPrefab.Spawn(chainParent);
            objChain.transform.localPosition = Vector3.right * startX;
            objChain.transform.localEulerAngles = i % 2 == 0 ? Vector3.zero : Vector3.right * 90f;
            objChain.transform.localScale = Vector3.one;
            objChains.Add(objChain);
            startX += spacing;
        }

        int totalChainLeft = totalChain / 2;
        for (int i = 0; i < totalChainLeft; i++)
        {
            objChains[i].transform.parent = chainParentLeft;
        }
        for (int i = totalChainLeft; i < totalChain; i++)
        {
            objChains[i].transform.parent = chainParentRight;
        }
    }

    public void CheckShooter()
    {
        switch (lockAxis)
        {
            case Axis.Vertical:
                for (int y = pos.y; y < pos.y + length; y++)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[pos.x, y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
            case Axis.Horizontal:
                for (int x = pos.x; x < pos.x + length; x++)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[x, pos.y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
        }

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].isLockChain = true;
        }
    }

    public void Unlock()
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

        objLock.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
        {
            objLock.SetActive(false);
        });

        chainParentLeft.transform.DOScaleX(0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            chainParentLeft.gameObject.SetActive(false);
        });

        chainParentRight.transform.DOScaleX(0f, 0.3f).SetEase(Ease.Linear).OnComplete(() =>
        {
            chainParentLeft.gameObject.SetActive(false);
        });

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].isLockChain = false;

            if (shooters[i].isActive)
            {
                shooters[i].Active2();
            }
        }
    }

    public void Recycle()
    {
        isLock = false;

        objKey.transform.DOKill();
        objKey.transform.localScale = Vector3.one;

        objLock.transform.DOKill();
        objLock.SetActive(true);
        objLock.transform.localScale = Vector3.one;

        chainParentLeft.DOKill();
        chainParentRight.DOKill();

        chainParentLeft.gameObject.SetActive(true);
        chainParentRight.gameObject.SetActive(true);

        chainParentLeft.localScale = Vector3.one;
        chainParentRight.localScale = Vector3.one;

        for (int i = 0; i < objChains.Count; i++)
        {
            objChains[i].Recycle();
        }
        objChains.Clear();

        shooters.Clear();

        gameObject.Recycle();
    }
}
