using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GDShooter : MonoBehaviour
{
    public int x;
    public int y;

    public MeshRenderer[] meshRenderers;
    public TextMeshPro txtCapacity;

    public GameObject objLink;

    public GameObject objIce;
    public TextMeshPro txtIceCount;

    public GameObject objCrate;
    public TextMeshPro txtCrateCount;

    public MeshRenderer lockRenderer;
    public MeshRenderer keyRenderer;

    public GameObject objShutter;
    public GameObject objShutterGate;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(ShooterData data)
    {
        for (int i = 0; i < meshRenderers.Length; i++)
        {
            if (data.isHidden)
            {
                meshRenderers[i].material = Resources.Load<Material>(Path.Combine("Materials", "Shooters", "Inactive", "Hidden"));
                //txtCapacity.text = "?";
            }
            else
            {
                meshRenderers[i].material = Resources.Load<Material>(Path.Combine("Materials", "Shooters", "Active", data.color.ToString()));
                //txtCapacity.text = data.capacity.ToString();
            }

            meshRenderers[i].gameObject.SetActive(false);
        }

        txtCapacity.text = data.capacity.ToString();

        switch (data.capacity)
        {
            case 4:
                meshRenderers[0].gameObject.SetActive(true);
                break;
            case 6:
                meshRenderers[1].gameObject.SetActive(true);
                break;
            case 8:
                meshRenderers[2].gameObject.SetActive(true);
                break;
            default:
                meshRenderers[0].gameObject.SetActive(true);
                break;
        }

        objIce.SetActive(data.hasIce);
        txtIceCount.text = data.iceCount.ToString();

        objCrate.SetActive(data.hasCrate);
        txtCrateCount.text = data.crateCount.ToString();

        lockRenderer.gameObject.SetActive(data.isLock);
        keyRenderer.gameObject.SetActive(data.isKey);

        lockRenderer.material = Resources.Load<Material>(Path.Combine("Materials", "Locks", data.lockCode.ToString()));
        keyRenderer.material = Resources.Load<Material>(Path.Combine("Materials", "Keys", data.lockCode.ToString()));

        objShutter.SetActive(data.hasShutter);
        objShutterGate.SetActive(!data.isShutterOpen);

        objLink.SetActive(data.isLink);
        if (data.isLink)
        {
            if (x == data.linkedPos.x)
            {
                if (y + 1 == data.linkedPos.y)
                {
                    objLink.transform.localEulerAngles = Vector3.zero;
                }
                else if (y - 1 == data.linkedPos.y)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 180f;
                }
            }
            else if (y == data.linkedPos.y)
            {
                if (x + 1 == data.linkedPos.x)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 270f;
                }
                else if (x - 1 == data.linkedPos.x)
                {
                    objLink.transform.localEulerAngles = Vector3.forward * 90f;
                }
            }
        }
    }
}
