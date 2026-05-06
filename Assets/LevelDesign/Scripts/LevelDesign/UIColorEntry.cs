using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIColorEntry : UI, IPointerDownHandler, IPointerUpHandler
{
    public bool canPickTile = false;
    public List<SelectedShooter> selectedShooters = new List<SelectedShooter>();
    public List<ColorEntry> colorEntries = new List<ColorEntry>();
    public TextMeshProUGUI txtAdded;

    [Header("Grid")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;
    [SerializeField] Button btnAdd;

    [Header("Cell")]
    public GameObject shooterTilePrefab;
    public ShooterTile[,] shooterTileGrid;
    public ShooterTile selectedTile;
    public TextMeshProUGUI txtShooterData;

    [Header("Element Tunnel")]
    public ScrollRect scrollDataTunnel;
    public GameObject shooterTunnelDataUIPrefab;
    public List<ShooterTunnelDataUI> shooterTunnelDataUIs = new List<ShooterTunnelDataUI>();

    public override void Show()
    {
        base.Show();

        selectedShooters.Clear();
        colorEntries.Clear();

        btnAdd.interactable = false;
        txtShooterData.text = "";

        if (LevelDesign.Instance.levelData.shooterTileDatas.Count > 0)
        {
            if (shooterTileGrid != null)
            {
                foreach (var tile in shooterTileGrid)
                {
                    tile.gameObject.Recycle();
                }

                shooterTileGrid = null;
            }

            int gridSizeX = LevelDesign.Instance.levelData.shooterGridSize.x;
            int gridSizeY = LevelDesign.Instance.levelData.shooterGridSize.y;

            GenerateGrid(gridSizeX, gridSizeY);

            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                shooterTileGrid[data.pos.x, data.pos.y].ActiveTile(true);

                if (data.shooters.Count > 0)
                {
                    if (data.tunnelDirection != Direction.None)
                    {
                        shooterTileGrid[data.pos.x, data.pos.y].SpawnTunnel();
                        shooterTileGrid[data.pos.x, data.pos.y].tunnel.SetUp(data.shooters.Count, data.tunnelDirection);
                    }

                    if (data.pinDirection != Direction.None)
                    {
                        shooterTileGrid[data.pos.x, data.pos.y].SpawnPin();
                        shooterTileGrid[data.pos.x, data.pos.y].pin.SetUp(data.pinDirection);
                    }

                    if (data.clothType != ClothType.None)
                    {
                        shooterTileGrid[data.pos.x, data.pos.y].SpawnCloth();
                        shooterTileGrid[data.pos.x, data.pos.y].cloth.SetUp(data.clothType, data.clothCount);
                    }

                    if (data.lockChainAxis != Axis.None)
                    {
                        shooterTileGrid[data.pos.x, data.pos.y].SpawnLockChain();
                        shooterTileGrid[data.pos.x, data.pos.y].lockChain.SetUp(data.lockChainAxis, data.lockChainLength, data.lockChainCode);
                    }

                    if (data.shooters.Count == 1 && data.tunnelDirection == Direction.None)
                    {
                        shooterTileGrid[data.pos.x, data.pos.y].SpawnShooter();
                        shooterTileGrid[data.pos.x, data.pos.y].shooter.SetUp(data.shooters[0]);
                    }
                }
            }
        }

        UpdateTotalAdded();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canPickTile = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canPickTile = false;
    }

    public void ActiveScroll(bool active)
    {
        scrollRect.enabled = active;
    }

    public void ResetSelectedTile()
    {
        if (selectedTile != null)
        {
            EventManager.SetDataGroup("OnSelectTile", -1, -1);
            EventManager.EmitEvent("OnSelectTile");
            selectedTile = null;

            foreach (var dataUI in shooterTunnelDataUIs)
            {
                dataUI.gameObject.Recycle();
            }
            shooterTunnelDataUIs.Clear();
        }
    }

    public void OnSelectTile(int x, int y)
    {
        EventManager.SetDataGroup("OnSelectTile", x, y);
        EventManager.EmitEvent("OnSelectTile");

        txtShooterData.text = "";

        foreach (var dataUI in shooterTunnelDataUIs)
        {
            dataUI.gameObject.Recycle();
        }
        shooterTunnelDataUIs.Clear();

        selectedTile = shooterTileGrid[x, y];

        ShooterTileData data = null;
        foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
        {
            if (tileData.pos.x == x && tileData.pos.y == y)
            {
                data = tileData;
            }
        }

        if (data != null)
        {
            if (data.shooters.Count > 0)
            {
                if (data.tunnelDirection != Direction.None)
                {
                    foreach (var shooterData in data.shooters)
                    {
                        int index = shooterTunnelDataUIs.Count;

                        GameObject obj = shooterTunnelDataUIPrefab.Spawn(scrollDataTunnel.content);
                        obj.transform.localScale = Vector3.one;

                        ShooterTunnelDataUI shooterDataUI = obj.GetComponent<ShooterTunnelDataUI>();
                        shooterDataUI.Init(index);
                        shooterDataUI.SetUp(shooterData);

                        if (IsAdded(data.pos.x, data.pos.y, true, index))
                        {
                            shooterDataUI.Added();
                        }

                        shooterTunnelDataUIs.Add(shooterDataUI);
                    }

                    btnAdd.interactable = false;
                }
                else
                {
                    if (data.shooters[0].isHidden)
                    {
                        txtShooterData.text = data.shooters[0].color.ToString();
                        txtShooterData.color = LevelDesign.Instance.colors[(int)data.shooters[0].color];
                    }

                    if (!IsAdded(data.pos.x, data.pos.y))
                    {
                        btnAdd.interactable = true;
                    }
                    else
                    {
                        btnAdd.interactable = false;
                    }
                }
            }
            else
            {
                btnAdd.interactable = false;
            }
        }
    }

    public void OnClickButtonAdd()
    {
        if (selectedTile != null)
        {
            ShooterTileData data = null;
            foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (tileData.pos.x == selectedTile.x && tileData.pos.y == selectedTile.y)
                {
                    data = tileData;
                }
            }

            if (data != null)
            {
                ShooterData shooterData = data.shooters[0];

                SelectedShooter selectedShooter = new SelectedShooter();
                selectedShooter.x = data.pos.x;
                selectedShooter.y = data.pos.y;
                selectedShooters.Add(selectedShooter);

                ColorEntry colorEntry = new ColorEntry(shooterData.capacity, shooterData.color);
                colorEntries.Add(colorEntry);

                btnAdd.interactable = false;

                selectedTile.Added();
                selectedTile = null;
            }
        }

        UpdateTotalAdded();
    }

    public bool IsAdded(int x, int y, bool isInsideTunnel = false, int tunnelIndex = 0)
    {
        foreach (var data in selectedShooters)
        {
            if (data.x == x && data.y == y && isInsideTunnel == data.isInsideTunnel && tunnelIndex == data.indexInTunnel)
            {
                return true;
            }
        }

        return false;
    }

    public void GenerateGrid(int gridSizeX, int gridSizeY)
    {
        gridLayoutGroup.constraintCount = gridSizeX;

        shooterTileGrid = new ShooterTile[gridSizeX, gridSizeY];

        for (int y = gridSizeY - 1; y >= 0; y--)
        {
            for (int x = 0; x < gridSizeX; x++)
            {
                GameObject obj = shooterTilePrefab.Spawn(scrollRect.content);
                obj.transform.localScale = Vector3.one;

                ShooterTile shooterTile = obj.GetComponent<ShooterTile>();
                shooterTile.Init(x, y);
                shooterTile.ActiveTile(false);

                shooterTileGrid[x, y] = shooterTile;
            }
        }
    }

    public void UpdateTotalAdded()
    {
        txtAdded.text = "";

        int index = 1;
        foreach (var colorEntry in colorEntries)
        {
            txtAdded.text += $"{index}. {colorEntry.color.ToString()}: {colorEntry.quantity}<br>";
            index++;
        }
    }

    public void OnClickButtonLoadColorEntry()
    {
        if (int.TryParse(LevelDesign.Instance.UILevelDesign.levelInput.text, out int level))
        {
            TextAsset data = Resources.Load<TextAsset>($"ColorEntry/{level}");
            colorEntries = new List<ColorEntry>(JsonConvert.DeserializeObject<List<ColorEntry>>(data.text));

            UpdateTotalAdded();
        }
    }

    public void OnClickButtonGenerateColorEntry()
    {
        if (int.TryParse(LevelDesign.Instance.UILevelDesign.levelInput.text, out int level))
        {
            int totalShooter = 0;
            foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                foreach (var shooterData in tileData.shooters)
                {
                    totalShooter++;
                }
            }

            if (totalShooter != colorEntries.Count)
            {
                if (colorEntries.Count > totalShooter)
                {
                    Debug.LogError("Add thừa color entry!!!");
                }
                else
                {
                    Debug.LogError("Chưa add đủ color entry!!!");
                }
                return;
            }

            string txt = JsonConvert.SerializeObject(colorEntries);

            string path = Path.Combine(Application.dataPath, "Resources", "ColorEntry");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            path = Path.Combine(path, string.Concat(string.Format("{0}", level), ".json"));
            File.WriteAllText(path, txt);

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif

            Debug.Log(string.Format("Save color entry for level {0} success!!!", level));
        }
    }

    public override void Hide()
    {
        base.Hide();
    }
}

[System.Serializable]
public class SelectedShooter
{
    public int x;
    public int y;
    public bool isInsideTunnel = false;
    public int indexInTunnel = 0;
}
