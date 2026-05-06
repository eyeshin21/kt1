using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class ShooterManager : Singleton<ShooterManager>
{
    [Header("Grid")]
    public Vector2Int gridSize;
    public bool[,] walkableGrid;
    public bool[,] groundGrid;
    public ShooterController[,] shooterGrid;
    public GameObject gridPiecePrefab;
    public Transform gridPieceParent;
    public List<GameObject> gridPieces = new List<GameObject>();

    [Header("Mask")]
    public Transform trsfMask;
    public SpriteRenderer bg;
    public SpriteRenderer bg2;
    public Sprite[] backSprs;

    [Header("Corners")]
    public GameObject inCornerPrefab;
    public GameObject outCornerPrefab;
    public GameObject crossCornerPrefab;
    public GameObject straightCornerPrefab;
    public Transform cornerParent;
    public List<GameObject> corners = new List<GameObject>();

    [Header("Shooter")]
    public GameObject shooter4Prefab;
    public GameObject shooter6Prefab;
    public GameObject shooter8Prefab;
    public Transform shooterParent;
    public List<ShooterController> shooters = new List<ShooterController>();

    [Header("Tunnel")]
    public GameObject tunnelPrefab;
    public List<TunnelController> tunnels = new List<TunnelController>();
    public TunnelController[,] tunnelGrid;

    [Header("Pin")]
    public GameObject pinPrefab;
    public List<PinController> pins = new List<PinController>();

    [Header("Cloth")]
    public GameObject cloth2x2Prefab;
    public GameObject cloth2x3Prefab;
    public GameObject cloth3x2Prefab;
    public List<ClothController> clothes = new List<ClothController>();

    [Header("Lock Chain")]
    public GameObject lockChainPrefab;
    public List<LockChainController> lockChains = new List<LockChainController>();

    [Header("Cell")]
    public GameObject cellPrefab;
    public List<GameObject> cells;
    public Transform cellParent;
    public float cellSize = 1f;
    public Sprite[] cellSprs;

    private float startX;
    private float startY;

    public void Init(Vector2Int gridSize, List<ShooterTileData> tileDatas)
    {
        this.gridSize = gridSize;

        shooterGrid = new ShooterController[gridSize.x, gridSize.y];
        tunnelGrid = new TunnelController[gridSize.x, gridSize.y];
        groundGrid = new bool[gridSize.x, gridSize.y];
        walkableGrid = new bool[gridSize.x, gridSize.y];

        startX = -(gridSize.x - 1) / 2f * cellSize;
        //startY = -(gridSize.y - 1) / 2f * cellSize;
        startY = -(cellSize / 2f);

        for (int i = 0; i < tileDatas.Count; i++)
        {
            ShooterTileData data = tileDatas[i];

            SpawnCell(data);

            groundGrid[data.pos.x, data.pos.y] = true;

            if (data.shooters.Count > 0)
            {
                if (data.shooters.Count == 1 && data.tunnelDirection == Direction.None)
                {
                    shooterGrid[data.pos.x, data.pos.y] = SpawnShooter(data, data.shooters[0]);
                }

                if (data.tunnelDirection != Direction.None)
                {
                    GameObject newTunnel = tunnelPrefab.Spawn(shooterParent);
                    newTunnel.name = $"Tunnel_{data.pos.x}_{data.pos.y}";
                    newTunnel.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);

                    TunnelController tunnel = newTunnel.GetComponent<TunnelController>();
                    tunnel.Init(data);
                    tunnels.Add(tunnel);

                    for (int y = 0; y < data.shooters.Count; y++)
                    {
                        ShooterData shooterData = data.shooters[y];
                        ShooterController shooter = SpawnShooter(data, shooterData);
                        shooter.gameObject.name = $"Shooter_{y} ({newTunnel.name})";
                        shooter.gameObject.SetActive(false);
                        tunnel.shooters.Add(shooter);
                    }

                    tunnelGrid[data.pos.x, data.pos.y] = tunnel;
                }

                if (data.pinDirection != Direction.None)
                {
                    GameObject newPin = pinPrefab.Spawn(shooterParent);
                    newPin.name = $"Pin_{data.pos.x}_{data.pos.y}";
                    newPin.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);

                    PinController pin = newPin.GetComponent<PinController>();
                    pin.Init(data);
                    pins.Add(pin);
                }

                if (data.clothType != ClothType.None)
                {
                    GameObject newCloth;

                    switch (data.clothType)
                    {
                        case ClothType.TwoxTwo:
                            newCloth = cloth2x2Prefab.Spawn(shooterParent);
                            break;
                        case ClothType.TwoxThree:
                            newCloth = cloth2x3Prefab.Spawn(shooterParent);
                            break;
                        case ClothType.ThreexTwo:
                            newCloth = cloth3x2Prefab.Spawn(shooterParent);
                            break;
                        default:
                            newCloth = cloth2x2Prefab.Spawn(shooterParent);
                            break;
                    }

                    newCloth.name = $"Cloth_{data.pos.x}_{data.pos.y}";
                    newCloth.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);

                    ClothController cloth = newCloth.GetComponent<ClothController>();
                    cloth.Init(data);
                    clothes.Add(cloth);
                }

                if (data.lockChainAxis != Axis.None)
                {
                    GameObject newLockChain = lockChainPrefab.Spawn(shooterParent);
                    newLockChain.name = $"LockChain_{data.pos.x}_{data.pos.y}";
                    newLockChain.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);

                    LockChainController lockChain = newLockChain.GetComponent<LockChainController>();
                    lockChain.Init(data);
                    lockChains.Add(lockChain);
                }
            }
        }

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        bg.sprite = backSprs[(int)eTypeLevel];
        bg2.sprite = backSprs[(int)eTypeLevel];

        int middleY = gridSize.y % 2 == 0 ? gridSize.y / 2 : (gridSize.y + 1) / 2;
        Vector2 middlePos = GetWorldPos(0, middleY - 1);
        trsfMask.localPosition = new Vector3(0, gridSize.y % 2 != 0 ? middlePos.y : middlePos.y + (cellSize / 2f), -0.61f);
        trsfMask.localPosition += Vector3.down * (0.25f / 2f);

        float maskSize = 32f;
        float realSize = (cellSize / maskSize) * 100f;
        trsfMask.localScale = new Vector3(realSize * gridSize.x, realSize * gridSize.y + (0.25f * 3), 1);

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].CheckLink();
        }

        SpawnCorner();

        CheckTunnel();
        CheckPin();
        CheckCloth();
        CheckLockChain();
        //CheckActiveShooter();
    }

    public ShooterController SpawnShooter(ShooterTileData data, ShooterData shooterData)
    {
        GameObject newShooter;
        switch (shooterData.capacity)
        {
            case 6:
                newShooter = shooter6Prefab.Spawn(shooterParent);
                break;
            case 8:
                newShooter = shooter8Prefab.Spawn(shooterParent);
                break;
            default:
                newShooter = shooter4Prefab.Spawn(shooterParent);
                break;
        }
        newShooter.name = $"Shooter_{data.pos.x}_{data.pos.y}";
        newShooter.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);

        ShooterController shooter = newShooter.GetComponent<ShooterController>();
        shooter.Init(shooterData, data.pos.ToVector2Int());

        shooters.Add(shooter);

        return shooter;
    }

    public void SpawnCell(ShooterTileData data)
    {
        GameObject newCell = cellPrefab.Spawn(cellParent);
        newCell.name = $"Cell_{data.pos.x}_{data.pos.y}";
        newCell.transform.localPosition = GetWorldPos(data.pos.x, data.pos.y);
        newCell.transform.localScale = Vector3.one * cellSize;

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        newCell.GetComponentInChildren<SpriteRenderer>().sprite = cellSprs[(int)eTypeLevel];

        cells.Add(newCell);
    }

    public void SpawnGridPiece(int x, int y)
    {
        GameObject newGridPiece = gridPiecePrefab.Spawn(gridPieceParent);
        newGridPiece.name = $"Piece_{x}_{y}";
        newGridPiece.transform.localPosition = GetWorldPos(x, y);
        newGridPiece.transform.localScale = Vector3.one * cellSize;

        gridPieces.Add(newGridPiece);
    }

    public void SpawnCorner()
    {
        Vector2Int gridSize = new Vector2Int(shooterGrid.GetLength(0), shooterGrid.GetLength(1));

        int total = 12;

        for (int y = -total; y < gridSize.y; y++)
        {
            for (int x = -(total / 2); x <= gridSize.x + (total / 2); x++)
            {
                bool center = IsValidCell(x, y);

                bool left = IsValidCell(x - 1, y);
                bool right = IsValidCell(x + 1, y);
                bool up = IsValidCell(x, y + 1);
                bool down = IsValidCell(x, y - 1);

                bool upLeft = IsValidCell(x - 1, y + 1);
                bool downLeft = IsValidCell(x - 1, y - 1);

                bool upRight = IsValidCell(x + 1, y + 1);
                bool downRight = IsValidCell(x + 1, y - 1);

                if (!center)
                {
                    if (x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y)
                    {
                        SpawnGridPiece(x, y);

                        if (y == 0)
                        {
                            SpawnGridPiece(x, y - 1);
                        }
                    }

                    if (left)
                    {
                        if (upLeft)
                        {
                            if (downLeft)
                            {
                                if (!up)
                                {
                                    if (!down)
                                    {
                                        SpawnStraightCorner(x, y, Direction.Left);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition += Vector3.up * (cellSize / 4f);

                                        SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                        if (downRight)
                                        {
                                            straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    if (y != gridSize.y - 1)
                                    {
                                        SpawnInCorner(x, y, Direction.Up, Direction.Left);
                                    }

                                    if (!down)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition -= Vector3.up * (cellSize / 4f);
                                    }
                                    else
                                    {
                                        SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                        if (downRight)
                                        {
                                            if (!right)
                                            {
                                                GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                                straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                                straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (down)
                                {
                                    if (!right)
                                    {
                                        if (downRight)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner2.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner2.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                    }
                                }

                                if (up)
                                {
                                    SpawnInCorner(x, y, Direction.Up, Direction.Left);
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition += Vector3.up * (cellSize / 4f);

                                    GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                        straightCorner2.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner2.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (y != gridSize.y - 1)
                            {
                                SpawnOutCorner(x, y, Direction.Up, Direction.Left);
                            }
                            else
                            {
                                SpawnInCorner(x, y, Direction.Up, Direction.Left);

                                if (!right)
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                }
                            }

                            if (downLeft)
                            {
                                if (down)
                                {
                                    SpawnInCorner(x, y, Direction.Down, Direction.Left);

                                    if (downRight)
                                    {
                                        if (!right)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                            straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition -= Vector3.up * (cellSize / 4f);
                                }
                            }
                            else
                            {
                                GameObject outCorner = SpawnOutCorner(x, y, Direction.Down, Direction.Left);

                                if (y == 0)
                                {
                                    outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                    GameObject straightCorner2 = SpawnStraightCorner(x, y, Direction.Left);
                                    straightCorner2.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner2.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                }
                            }

                            if (up)
                            {
                                SpawnCrossCorner(x, y, Direction.Up, Direction.Left);
                            }
                        }

                    }
                    else
                    {
                        if (downLeft)
                        {
                            if (down)
                            {
                                if (downRight)
                                {
                                    if (!right)
                                    {
                                        SpawnStraightCorner(x, y, Direction.Down);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                                }
                            }
                        }
                        else
                        {
                            if (down)
                            {
                                if (downRight)
                                {
                                    if (!right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Down);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);
                                    }
                                }
                            }
                        }

                        if (y == gridSize.y - 1)
                        {
                            GameObject straightCorner = SpawnStraightCorner(x, y, Direction.Up);

                            if (right)
                            {
                                straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);
                            }
                        }
                    }
                }
                else
                {
                    if (!left)
                    {
                        if (!upLeft)
                        {
                            if (!downLeft)
                            {
                                if (up)
                                {
                                    if (down)
                                    {
                                        SpawnStraightCorner(x - 1, y, Direction.Right);
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition += Vector3.up * (cellSize / 4f);

                                        GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                        if (y == 0)
                                        {
                                            outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                            GameObject straightCorner2 = SpawnStraightCorner(x - 1, y, Direction.Right);
                                            straightCorner2.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner2.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }

                                        if (!downRight)
                                        {
                                            if (right)
                                            {
                                                straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                                straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                                straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                                if (y == 0)
                                                {
                                                    straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (y != gridSize.y - 1)
                                    {
                                        SpawnOutCorner(x - 1, y, Direction.Up, Direction.Right);
                                    }
                                    else
                                    {
                                        SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);
                                    }

                                    if (down)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition -= Vector3.up * (cellSize / 4f);
                                    }
                                    else
                                    {
                                        GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                        if (y == 0)
                                        {
                                            outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                            GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                            straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }

                                        if (!downRight)
                                        {
                                            GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                            straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                            straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                            if (y == 0)
                                            {
                                                straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                SpawnInCorner(x - 1, y, Direction.Down, Direction.Right);

                                if (y == gridSize.y - 1)
                                {
                                    SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);
                                    continue;
                                }

                                if (!up)
                                {
                                    SpawnOutCorner(x - 1, y, Direction.Up, Direction.Right);
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition += Vector3.up * (cellSize / 4f);
                                }
                            }
                        }
                        else
                        {
                            SpawnInCorner(x - 1, y, Direction.Up, Direction.Right);

                            if (!downLeft)
                            {
                                if (!down)
                                {
                                    GameObject outCorner = SpawnOutCorner(x - 1, y, Direction.Down, Direction.Right);

                                    if (y == 0)
                                    {
                                        outCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);

                                        GameObject straightCorner2 = SpawnStraightCorner(x - 1, y, Direction.Right);
                                        straightCorner2.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner2.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                    }

                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x - 1, y, Direction.Right);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition -= Vector3.up * (cellSize / 4f);
                                }
                            }
                            else
                            {
                                if (!down)
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                    if (y == 0)
                                    {
                                        straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                    }
                                }
                                else
                                {
                                    SpawnInCorner(x - 1, y, Direction.Down, Direction.Right);
                                }
                            }

                            if (!up)
                            {
                                SpawnCrossCorner(x, y + 1, Direction.Down, Direction.Left);
                            }
                        }
                    }
                    else
                    {
                        if (!downLeft)
                        {
                            if (!down)
                            {
                                if (!downRight)
                                {
                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }
                                    }
                                    else
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }
                                    }
                                }
                                else
                                {
                                    GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                    straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                    straightCorner.transform.localPosition -= Vector3.right * (cellSize / 4f);

                                    if (y == 0)
                                    {
                                        straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!down)
                            {
                                if (!downRight)
                                {
                                    if (right)
                                    {
                                        GameObject straightCorner = SpawnStraightCorner(x, y - 1, Direction.Up);
                                        straightCorner.transform.localScale = new Vector3(1, cellSize / 2f, 1);
                                        straightCorner.transform.localPosition += Vector3.right * (cellSize / 4f);

                                        if (y == 0)
                                        {
                                            straightCorner.transform.localPosition += Vector3.down * (cellSize * 0.25f);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    GameObject SpawnStraightCorner(int x, int y, Direction direction)
    {
        GameObject newCorner = straightCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"StraightCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        newCorner.GetComponentInChildren<MeshRenderer>().sharedMaterial = MaterialCache.GetCornerMat(eTypeLevel);

        switch (direction)
        {
            case Direction.Left:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
                break;
            case Direction.Right:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 180f;
                break;
            case Direction.Up:
                newCorner.transform.localEulerAngles = Vector3.forward * 270f;
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.up * (cellSize / 2f);
                break;
            case Direction.Down:
                newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.down * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 90f;
                break;
        }

        corners.Add(newCorner);

        return newCorner;
    }

    GameObject SpawnInCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = inCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"InCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        newCorner.GetComponentInChildren<MeshRenderer>().sharedMaterial = MaterialCache.GetCornerMat(eTypeLevel);

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.up * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 270f;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.down * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 90f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 180f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    GameObject SpawnOutCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = outCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"OutCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        newCorner.GetComponentInChildren<MeshRenderer>().sharedMaterial = MaterialCache.GetCornerMat(eTypeLevel);

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.up * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 90f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 180f;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.down * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 270f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    GameObject SpawnCrossCorner(int x, int y, Direction vertical, Direction horizontal)
    {
        GameObject newCorner = crossCornerPrefab.Spawn(cornerParent);
        newCorner.name = $"CrossCorner_{x}_{y}";
        newCorner.transform.localScale = Vector3.one * cellSize;

        eTypeLevel eTypeLevel = GameManager.Instance.GetTypeLevel(UserConfig.Instance.CurLevel);
        if (eTypeLevel == eTypeLevel.Tutorial)
        {
            eTypeLevel = eTypeLevel.Normal;
        }
        newCorner.GetComponentInChildren<MeshRenderer>().sharedMaterial = MaterialCache.GetCornerMat(eTypeLevel);

        if (vertical == Direction.Up)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.up * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 270f;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
        }
        else if (vertical == Direction.Down)
        {
            newCorner.transform.localPosition = GetWorldPos(x, y) + Vector3.down * (cellSize / 2f);
            if (horizontal == Direction.Left)
            {
                newCorner.transform.localPosition += Vector3.left * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.zero;
            }
            else if (horizontal == Direction.Right)
            {
                newCorner.transform.localPosition += Vector3.right * (cellSize / 2f);
                newCorner.transform.localEulerAngles = Vector3.forward * 270f;

            }
        }

        corners.Add(newCorner);
        return newCorner;
    }

    public void RemoveShooter(ShooterController shooterToRemove, bool checkTunnel = true, bool checkActive = true)
    {
        shooters.Remove(shooterToRemove);
        shooterGrid[shooterToRemove.pos.x, shooterToRemove.pos.y] = null;

        if (checkTunnel)
        {
            CheckTunnel();
        }

        if (shooterGrid[shooterToRemove.pos.x, shooterToRemove.pos.y] == null)
        {
            for (int x = 0; x < walkableGrid.GetLength(0); x++)
            {
                for (int y = 0; y < walkableGrid.GetLength(1); y++)
                {
                    if (shooterGrid[x, y] || !groundGrid[x, y] || tunnelGrid[x, y])
                    {
                        walkableGrid[x, y] = false;
                    }
                    else
                    {
                        walkableGrid[x, y] = groundGrid[x, y];
                    }
                }
            }

            if (checkActive)
            {
                Point source = new Point(shooterToRemove.pos.x, shooterToRemove.pos.y);
                List<Point> points = BFS.SearchShooter(walkableGrid, source);

                if (points.Count > 0)
                {
                    for (int i = 0; i < points.Count; i++)
                    {
                        Point point = points[i];
                        ShooterController shooter = shooterGrid[point.x, point.y];
                        if (shooter != null && !shooter.isActive)
                        {
                            shooter.Active2();
                        }
                    }
                }
            }
        }
    }

    public void CheckActiveShooter()
    {
        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].CheckActive();
        }
    }

    public void CheckTunnel()
    {
        for (int i = 0; i < tunnels.Count; i++)
        {
            tunnels[i].CheckActiveNextShooter();
        }
    }

    public void CheckPin()
    {
        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].CheckShooter();
        }
    }

    public void CheckCloth()
    {
        for (int i = 0; i < clothes.Count; i++)
        {
            clothes[i].CheckShooter();
        }
    }

    public void CheckLockChain()
    {
        for (int i = 0; i < lockChains.Count; i++)
        {
            lockChains[i].CheckShooter();
        }
    }

    public void SwapShooter(ShooterController parkedShooter, ShooterController shooterToSwap)
    {
        //int cacheCapacity = parkedShooter.realCapacity;
        //ColorEnum cacheColor = parkedShooter.color;

        //parkedShooter.Swap(shooterToSwap.realCapacity, shooterToSwap.color);
        //shooterToSwap.Swap(cacheCapacity, cacheColor);

        parkedShooter.StopCheckShoot();

        ParkingSlot curSlot = parkedShooter.slot;
        Vector2Int gridPos = shooterToSwap.pos;
        shooterToSwap.pos = parkedShooter.pos;

        curSlot.RemoveShooter(parkedShooter);
        ParkingManager.Instance.parkedShooters.Remove(parkedShooter);
        parkedShooter.slot = null;

        parkedShooter.pos = gridPos;
        shooterGrid[gridPos.x, gridPos.y] = parkedShooter;
        shooters.Add(parkedShooter);

        shooters.Remove(shooterToSwap);
        shooterToSwap.slot = curSlot;
        curSlot.parkedShooter = shooterToSwap;
        ParkingManager.Instance.parkedShooters.Add(shooterToSwap);

        Vector3 gridWorldPos = GetWorldPos(gridPos.x, gridPos.y);
        parkedShooter.transform.DOLocalMove(gridWorldPos, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            parkedShooter.SetColor();
            parkedShooter.fakeHook.SetActive(false);
            parkedShooter.tapColl.enabled = true;
            parkedShooter.CheckActive();
        });

        shooterToSwap.SetColorActive();
        shooterToSwap.tapColl.enabled = false;
        shooterToSwap.Active();
        shooterToSwap.fakeHook.SetActive(true);
        shooterToSwap.CheckKey();
        shooterToSwap.DisableShutter();
        shooterToSwap.transform.DOMove(curSlot.transform.position, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            shooterToSwap.CheckShoot();
        });
    }

    public bool CanMoveOut(Vector2Int startPos)
    {
        int maxY = shooterGrid.GetLength(1) - 1;

        if (startPos.y == maxY)
        {
            return true;
        }

        for (int x = 0; x < walkableGrid.GetLength(0); x++)
        {
            for (int y = 0; y < walkableGrid.GetLength(1); y++)
            {
                if (shooterGrid[x, y] || !groundGrid[x, y] || tunnelGrid[x, y])
                {
                    walkableGrid[x, y] = false;
                }
                else
                {
                    walkableGrid[x, y] = groundGrid[x, y];
                }
            }
        }

        Point source = new Point(startPos.x, startPos.y);
        List<Point> points = BFS.Search(walkableGrid, source);

        return points != null;
    }

    public bool CanMoveOut(Vector2Int startPos, out List<Point> points)
    {
        int maxY = shooterGrid.GetLength(1) - 1;

        if (startPos.y == maxY)
        {
            points = new List<Point>();
            return true;
        }

        for (int x = 0; x < walkableGrid.GetLength(0); x++)
        {
            for (int y = 0; y < walkableGrid.GetLength(1); y++)
            {
                if (shooterGrid[x, y] || !groundGrid[x, y] || tunnelGrid[x, y])
                {
                    walkableGrid[x, y] = false;
                }
                else
                {
                    walkableGrid[x, y] = groundGrid[x, y];
                }
            }
        }

        Point source = new Point(startPos.x, startPos.y);
        points = BFS.Search(walkableGrid, source);

        return points != null;
    }

    public bool GetPath(Vector2Int startPos, Vector2Int endPos, out List<Point> points)
    {
        if (startPos == endPos)
        {
            points = new List<Point>();
            return true;
        }

        for (int x = 0; x < walkableGrid.GetLength(0); x++)
        {
            for (int y = 0; y < walkableGrid.GetLength(1); y++)
            {
                if (shooterGrid[x, y] || !groundGrid[x, y] || tunnelGrid[x, y])
                {
                    walkableGrid[x, y] = false;
                }
                else
                {
                    walkableGrid[x, y] = groundGrid[x, y];
                }
            }
        }

        Point source = new Point(startPos.x, startPos.y);
        Point end = new Point(endPos.x, endPos.y);
        points = BFS.Search(walkableGrid, source, end);

        return points != null;
    }

    public bool IsValidCell(int x, int y)
    {
        Vector2Int gridSize = new Vector2Int(shooterGrid.GetLength(0), shooterGrid.GetLength(1));
        return (x >= 0) && (x < gridSize.x) && (y >= 0) && (y < gridSize.y) && groundGrid[x, y];
    }

    public Vector3 GetWorldPos(int x, int y, Vector2Int gridSize)
    {
        return new Vector3(startX + x * cellSize, (startY + y - (gridSize.y - 1)) * cellSize, 0);
    }

    public Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(startX + x * cellSize, (startY + y - (shooterGrid.GetLength(1) - 1)) * cellSize, 0);
    }

    public void Recycle()
    {
        for (int i = 0; i < corners.Count; i++)
        {
            corners[i].Recycle();
        }
        corners.Clear();

        for (int i = 0; i < gridPieces.Count; i++)
        {
            gridPieces[i].Recycle();
        }
        gridPieces.Clear();

        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].Recycle();
        }
        cells.Clear();

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].Recycle();
        }
        shooters.Clear();

        for (int i = 0; i < tunnels.Count; i++)
        {
            tunnels[i].Recycle();
        }
        tunnels.Clear();

        for (int i = 0; i < pins.Count; i++)
        {
            pins[i].Recycle();
        }
        pins.Clear();

        for (int i = 0; i < clothes.Count; i++)
        {
            clothes[i].Recycle();
        }
        clothes.Clear();

        for (int i = 0; i < lockChains.Count; i++)
        {
            lockChains[i].Recycle();
        }
        lockChains.Clear();
    }
}
