using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShooterTile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int x;
    public int y;
    public Image img;

    [Header("Shooter")]
    public GameObject shooterPrefab;
    public GDShooter shooter;

    [Header("Tunnel")]
    public GameObject tunnelPrefab;
    public GDTunnel tunnel;

    [Header("Pin")]
    public GameObject pinPrefab;
    public GDPin pin;

    [Header("Cloth")]
    public GameObject clothPrefab;
    public GDCloth cloth;

    [Header("Lock Chain")]
    public GameObject lockChainPrefab;
    public GDLockChain lockChain;

    private void Start()
    {
        EventManager.StartListening("OnSelectTile", Select);
    }

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void ActiveTile(bool active)
    {
        img.enabled = active;
    }

    public void Select()
    {
        EventManager.DataGroup[] data = EventManager.GetDataGroup("OnSelectTile");

        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Shooter:
                if (x == data[0].ToInt() && y == data[1].ToInt())
                {
                    img.color = Color.green;
                }
                else
                {
                    img.color = Color.white;
                }
                break;
            case Tabs.ColorEntry:
                if (x == data[0].ToInt() && y == data[1].ToInt())
                {
                    img.color = Color.yellow;
                }
                else
                {
                    if (LevelDesign.Instance.UIColorEntry.IsAdded(x, y))
                    {
                        img.color = Color.green;
                    }
                    else
                    {
                        img.color = Color.white;
                    }
                }
                break;
        }
    }

    public void Added()
    {
        img.color = Color.green;
    }

    public void SpawnShooter()
    {
        GameObject obj = shooterPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        shooter = obj.GetComponent<GDShooter>();
        shooter.Init(x, y);
    }

    public void SpawnTunnel()
    {
        GameObject obj = tunnelPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 80f;

        tunnel = obj.GetComponent<GDTunnel>();
        tunnel.Init(x, y);
    }

    public void SpawnPin()
    {
        GameObject obj = pinPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        pin = obj.GetComponent<GDPin>();
        pin.Init(x, y);
    }

    public void SpawnCloth()
    {
        GameObject obj = clothPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 100f;

        cloth = obj.GetComponent<GDCloth>();
        cloth.Init(x, y);
    }

    public void SpawnLockChain()
    {
        GameObject obj = lockChainPrefab.Spawn(transform);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one * 85f;

        lockChain = obj.GetComponent<GDLockChain>();
        lockChain.Init(x, y);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Shooter:
                LevelDesign.Instance.UIShooter.canPickTile = true;
                LevelDesign.Instance.UIShooter.OnSelectTile(x, y);
                LevelDesign.Instance.UIShooter.ActiveScroll(false);
                break;
            case Tabs.ColorEntry:
                LevelDesign.Instance.UIColorEntry.canPickTile = true;
                LevelDesign.Instance.UIColorEntry.OnSelectTile(x, y);
                LevelDesign.Instance.UIColorEntry.ActiveScroll(false);
                break;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Shooter:
                if (LevelDesign.Instance.UIShooter.canPickTile)
                {
                    LevelDesign.Instance.UIShooter.OnSelectTile(x, y);
                }
                break;
            case Tabs.ColorEntry:
                if (LevelDesign.Instance.UIColorEntry.canPickTile)
                {
                    LevelDesign.Instance.UIColorEntry.OnSelectTile(x, y);
                }
                break;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        switch (LevelDesign.Instance.UILevelDesign.currentTab)
        {
            case Tabs.Shooter:
                LevelDesign.Instance.UIShooter.canPickTile = false;
                LevelDesign.Instance.UIShooter.ActiveScroll(true);
                break;
            case Tabs.ColorEntry:
                LevelDesign.Instance.UIColorEntry.canPickTile = false;
                LevelDesign.Instance.UIColorEntry.ActiveScroll(true);
                break;
        }
    }
}
