using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GDLockChain : MonoBehaviour
{
    public int x;
    public int y;

    public Transform offset;
    public Transform rotate;
    public Transform blockLeft;
    public Transform blockRight;

    public GameObject chainPrefab;
    public Transform chainParent;
    public List<GameObject> objChains = new List<GameObject>();

    public MeshRenderer lockRenderer;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(Axis axis, int length, int lockCode)
    {
        for (int i = 0; i < objChains.Count;i++)
        {
            objChains[i].Recycle();
        }
        objChains.Clear();

        switch (axis)
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

        //float posBlock = length % 2 == 0 ? (length - 1 + 0.4f) : (length / 2f) + 0.4f;
        float posBlock = (length / 2f) + 0.4f;

        blockLeft.localPosition = new Vector3(-posBlock, blockLeft.localPosition.y, blockLeft.localPosition.z);
        blockRight.localPosition = new Vector3(posBlock, blockLeft.localPosition.y, blockLeft.localPosition.z);

        float spacing = 0.25f;
        int totalChain = Mathf.RoundToInt(length / spacing) + 2;

        float startX = -(((totalChain - 1) / 2f) * spacing);

        for (int i = 0; i < totalChain; i++)
        {
            GameObject objChain = chainPrefab.Spawn(chainParent);
            objChain.transform.localPosition = new Vector3(startX, 0, -0.8f);
            objChain.transform.localEulerAngles = i % 2 == 0 ? Vector3.zero : Vector3.right * 90f;
            objChain.transform.localScale = Vector3.one;
            objChains.Add(objChain);
            startX += spacing;
        }

        SetLockCode(lockCode);
    }

    public void SetLockCode(int lockCode)
    {
        lockRenderer.material = Resources.Load<Material>(Path.Combine("Materials", "Locks", lockCode.ToString()));
    }
}
