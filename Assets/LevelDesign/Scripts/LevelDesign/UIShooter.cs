using System;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIShooter : UI, IPointerDownHandler, IPointerUpHandler
{
    public bool canPickTile = false;

    [Header("UI")]
    public TMP_InputField inputShooterCapacityAll;
    public GameObject btnCreate;
    public GameObject btnRemove;
    public TextMeshProUGUI txtTotalColor;
    public TextMeshProUGUI txtColorCount;

    [Header("Element")]
    public TMP_Dropdown dropdownElement;

    [Header("Element Shooter")]
    public GameObject elementShooter;
    public TMP_Dropdown dropdownShooterColor;
    public TMP_InputField inputShooterCapacity;

    public Toggle toggleShooterHidden;

    public TMP_InputField inputLinkedPosX;
    public TMP_InputField inputLinkedPosY;
    public TextMeshProUGUI txtCurrentPos;

    public Toggle toggleHasIce;
    public TMP_InputField inputIceCount;

    public Toggle toggleHasCrate;
    public TMP_InputField inputCrateCount;

    public Toggle toggleHasLock;
    public Toggle toggleHasKey;
    public TMP_InputField inputLockCode;

    public Toggle toggleHasShutter;
    public Toggle toggleIsShutterOpen;

    [Header("Element Tunnel")]
    public GameObject elementTunnel;
    public TMP_Dropdown dropdownTunnelDirection;
    public ScrollRect scrollShooterData;
    public GameObject shooterDataUIPrefab;
    public List<ShooterDataUI> shooterDataUIs = new List<ShooterDataUI>();
    public GameObject btnAddShooterData;

    [Header("Element Pin")]
    public GameObject elementPin;
    public TMP_Dropdown dropdownPinDirection;

    [Header("Element Cloth")]
    public GameObject elementCloth;
    public TMP_Dropdown dropdownClothType;
    public TMP_InputField inputClothCount;

    [Header("Element Lock Chain")]
    public GameObject elementLockChain;
    public TMP_Dropdown dropdownLockChainAxis;
    public TMP_InputField inputLockChainLength;
    public TMP_InputField inputLockChainCode;

    [Header("Grid")]
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject shooterTilePrefab;
    public ShooterTile[,] shooterTileGrid;
    public ShooterTile selectedTile;

    public override void Start()
    {
        base.Start();

        ColorEnum[] colorEnums = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));

        dropdownShooterColor.options.Clear();
        for (int i = 0; i < colorEnums.Length; i++)
        {
            dropdownShooterColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
        }

        dropdownShooterColor.value = 0;
        dropdownShooterColor.RefreshShownValue();

        btnCreate.SetActive(false);
        btnRemove.SetActive(false);
    }

    public override void Show()
    {
        base.Show();

        foreach (var shooterDataUI in shooterDataUIs)
        {
            shooterDataUI.gameObject.Recycle();
        }
        shooterDataUIs.Clear();

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

        UpdateTotalColor();
    }

    public void UpdateTotalColor()
    {
        Dictionary<ColorEnum, int> shooterDict = new Dictionary<ColorEnum, int>();

        List<ColorEnum> allColors = new List<ColorEnum>();
        if (LevelDesign.Instance.levelData.shooterTileDatas.Count > 0)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                foreach (var shooter in data.shooters)
                {
                    if (shooter.color != ColorEnum.None)
                    {
                        if (!allColors.Contains(shooter.color))
                        {
                            allColors.Add(shooter.color);
                        }

                        if (!shooterDict.ContainsKey(shooter.color))
                        {
                            shooterDict.Add(shooter.color, 0);
                        }

                        shooterDict[shooter.color] += shooter.capacity;
                    }
                }
            }
        }

        txtTotalColor.text = $"Total Color: {allColors.Count}";
        txtColorCount.text = "";

        if (int.TryParse(LevelDesign.Instance.UILevelDesign.levelInput.text, out int level))
        {
            LevelController currentLevel = Resources.Load<LevelController>($"Levels/Level_{level}");
            if (currentLevel != null)
            {
                currentLevel.SetUp();
                Dictionary<ColorEnum, int> screwDict = new Dictionary<ColorEnum, int>();

                foreach (var screw in currentLevel.screws)
                {
                    if (screw.color != ColorEnum.None)
                    {
                        if (!allColors.Contains(screw.color))
                        {
                            allColors.Add(screw.color);
                        }
                        if (!screwDict.ContainsKey(screw.color))
                        {
                            screwDict.Add(screw.color, 0);
                        }
                        screwDict[screw.color]++;
                    }
                }

                string txt = "";
                foreach (var color in allColors)
                {
                    int amountShooter = 0;
                    int amountScrew = 0;

                    if (shooterDict.ContainsKey(color))
                    {
                        amountShooter = shooterDict[color];
                    }

                    if (screwDict.ContainsKey(color))
                    {
                        amountScrew = screwDict[color];
                    }

                    if (amountShooter == 0 && amountScrew == 0) continue;

                    string colorText = "white";
                    if (amountShooter == amountScrew)
                    {
                        colorText = "green";
                    }
                    else
                    {
                        colorText = "red";
                    }

                    txt += $"{color.ToString()}: <color={colorText}>{amountShooter}</color>/{amountScrew}<br>";
                }

                txtColorCount.text = txt;
            }
        }
        
    }

    public void ResetSelectedTile()
    {
        if (selectedTile != null)
        {
            EventManager.SetDataGroup("OnSelectTile", -1, -1);
            EventManager.EmitEvent("OnSelectTile");
            selectedTile = null;

            txtCurrentPos.text = $"Current Pos:";

            btnCreate.SetActive(false);
            btnRemove.SetActive(false);
        }
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

    public void OnSelectTile(int x, int y)
    {
        EventManager.SetDataGroup("OnSelectTile", x, y);
        EventManager.EmitEvent("OnSelectTile");

        selectedTile = shooterTileGrid[x, y];

        txtCurrentPos.text = $"Current Pos: {x}x{y}";

        ShooterTileData data = null;
        foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
        {
            if (tileData.pos.x == x && tileData.pos.y == y)
            {
                data = tileData;
            }
        }

        foreach (var dataUI in shooterDataUIs)
        {
            dataUI.gameObject.Recycle();
        }
        shooterDataUIs.Clear();

        if (data != null)
        {
            if (data.shooters.Count > 0)
            {
                if (data.tunnelDirection != Direction.None)
                {
                    dropdownElement.value = 1;

                    dropdownTunnelDirection.value = (int)data.tunnelDirection;
                    foreach (var shooterData in data.shooters)
                    {
                        int index = shooterDataUIs.Count;

                        GameObject obj = shooterDataUIPrefab.Spawn(scrollShooterData.content);
                        obj.transform.localScale = Vector3.one;

                        ShooterDataUI shooterDataUI = obj.GetComponent<ShooterDataUI>();
                        shooterDataUI.Init(index);
                        shooterDataUI.SetUp(shooterData);

                        shooterDataUIs.Add(shooterDataUI);

                        btnAddShooterData.transform.parent.SetAsLastSibling();
                    }
                }
                else
                {
                    if (dropdownElement.value == 1)
                    {
                        dropdownElement.value = 0;
                    }

                    TMP_Dropdown.OptionData optionData = null;
                    foreach (var option in dropdownShooterColor.options)
                    {
                        if (option.text.Equals(data.shooters[0].color.ToString()))
                        {
                            optionData = option;
                        }
                    }

                    dropdownShooterColor.value = dropdownShooterColor.options.IndexOf(optionData);
                    inputShooterCapacity.text = data.shooters[0].capacity.ToString();
                    toggleShooterHidden.isOn = data.shooters[0].isHidden;
                    toggleHasIce.isOn = data.shooters[0].hasIce;
                    inputIceCount.text = data.shooters[0].iceCount.ToString();
                    toggleHasCrate.isOn = data.shooters[0].hasCrate;
                    inputCrateCount.text = data.shooters[0].crateCount.ToString();
                    toggleHasKey.isOn = data.shooters[0].isKey;
                    toggleHasLock.isOn = data.shooters[0].isLock;
                    inputLockCode.text = data.shooters[0].lockCode.ToString();
                    toggleHasShutter.isOn = data.shooters[0].hasShutter;
                    toggleIsShutterOpen.isOn = data.shooters[0].isShutterOpen;

                    if (data.shooters[0].isLink)
                    {
                        inputLinkedPosX.text = data.shooters[0].linkedPos.x.ToString();
                        inputLinkedPosY.text = data.shooters[0].linkedPos.y.ToString();
                    }
                    else
                    {
                        inputLinkedPosX.text = string.Empty;
                        inputLinkedPosY.text = string.Empty;
                    }
                }

                CheckActiveCreateBtn(data);
            }
            else
            {
                CheckActiveCreateBtn(data);
            }
        }
    }

    public void CheckActiveCreateBtn(ShooterTileData data)
    {
        bool activeCreateBtn = true;
        switch (dropdownElement.value)
        {
            case 0:
                {
                    if (data.shooters.Count > 0)
                    {
                        activeCreateBtn = false;
                    }
                    break;
                }
            case 1:
                {
                    if (data.tunnelDirection != Direction.None)
                    {
                        activeCreateBtn = false;
                    }
                    break;
                }
            case 2: // Pin
                {
                    if (data.pinDirection != Direction.None)
                    {
                        dropdownPinDirection.value = (int)data.pinDirection;
                        activeCreateBtn = false;
                    }
                    break;
                }
            case 3: // Cloth
                {
                    if (data.clothType != ClothType.None)
                    {
                        dropdownClothType.value = (int)data.clothType;
                        inputClothCount.text = data.clothCount.ToString();
                        activeCreateBtn = false;
                    }
                }
                break;
            case 4: // Lock Chain
                {
                    if (data.lockChainAxis != Axis.None)
                    {
                        dropdownLockChainAxis.value = (int)data.lockChainAxis;
                        inputLockChainLength.text = data.lockChainLength.ToString();
                        inputLockChainCode.text = data.lockChainCode.ToString();

                        activeCreateBtn = false;
                    }
                    break;
                }
        }

        btnCreate.SetActive(activeCreateBtn);
        btnRemove.SetActive(!activeCreateBtn);
    }

    public void OnClickButtonSetAllShooterCapacity()
    {
        int capacity = int.Parse(inputShooterCapacityAll.text);
        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
        {
            foreach (var shooterData in data.shooters)
            {
                shooterData.capacity = capacity;
            }
        }

        Show();
    }

    public void OnClickButtonLink()
    {
        int linkedPosX = int.Parse(inputLinkedPosX.text);
        int linkedPosY = int.Parse(inputLinkedPosY.text);

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isLink = true;
                        data.shooters[0].linkedPos = new Vector2IntS(linkedPosX, linkedPosY);
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                }

                if (data.pos.x == linkedPosX && data.pos.y == linkedPosY)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isLink = true;
                        data.shooters[0].linkedPos = new Vector2IntS(selectedTile.x, selectedTile.y);
                        shooterTileGrid[linkedPosX, linkedPosY].shooter.SetUp(data.shooters[0]);
                    }
                }
            }
        }
    }

    public void OnClickButtonUnlink()
    {
        int linkedPosX = int.Parse(inputLinkedPosX.text);
        int linkedPosY = int.Parse(inputLinkedPosY.text);

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isLink = false;
                        data.shooters[0].linkedPos = new Vector2IntS();
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                }

                if (data.pos.x == linkedPosX && data.pos.y == linkedPosY)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isLink = false;
                        data.shooters[0].linkedPos = new Vector2IntS();
                        shooterTileGrid[linkedPosX, linkedPosY].shooter.SetUp(data.shooters[0]);
                    }
                }
            }
        }
    }

    public void OnDropdownElementValueChange(int option)
    {
        elementShooter.SetActive(option == 0);
        elementTunnel.SetActive(option == 1);
        elementPin.SetActive(option == 2);
        elementCloth.SetActive(option == 3);
        elementLockChain.SetActive(option == 4);

        if (selectedTile != null)
        {
            switch (option)
            {
                case 2: // Pin
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                if (data.pinDirection != Direction.None)
                                {
                                    dropdownPinDirection.value = (int)data.pinDirection;
                                    activeCreateBtn = false;
                                }
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
                case 3: // Cloth
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                if (data.clothType != ClothType.None)
                                {
                                    dropdownClothType.value = (int)data.clothType;
                                    inputClothCount.text = data.clothCount.ToString();
                                    activeCreateBtn = false;
                                }
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
                case 4: // Lock Chain
                    {
                        bool activeCreateBtn = true;
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                if (data.lockChainAxis != Axis.None)
                                {
                                    dropdownLockChainAxis.value = (int)data.lockChainAxis;
                                    inputLockChainLength.text = data.lockChainLength.ToString();
                                    inputLockChainCode.text = data.lockChainCode.ToString();

                                    activeCreateBtn = false;
                                }
                                break;
                            }
                        }

                        btnCreate.SetActive(activeCreateBtn);
                        btnRemove.SetActive(!activeCreateBtn);
                        break;
                    }
            }
        }
    }

    public void OnToggleHidden(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isHidden = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleHasIce(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].hasIce = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnInputIceCountValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        int iceCount = int.Parse(value);
                        data.shooters[0].iceCount = iceCount;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleHasCrate(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].hasCrate = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnInputCrateCountValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        int crateCount = int.Parse(value);
                        data.shooters[0].crateCount = crateCount;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleHasLock(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isLock = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleHasKey(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isKey = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnInputLockCodeValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        int lockCode = int.Parse(value);
                        data.shooters[0].lockCode = lockCode;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleHasShutter(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].hasShutter = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnToggleIsShutterOpen(bool isOn)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        data.shooters[0].isShutterOpen = isOn;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnDropdownShooterColorValueChange(int option)
    {
        if (option == 0) return;

        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        ColorEnum color = Enum.Parse<ColorEnum>(dropdownShooterColor.options[option].text);
                        data.shooters[0].color = color;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }

            UpdateTotalColor();
        }
    }

    public void OnInputCapacityValueChange(string value)
    {
        if (selectedTile != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    if (data.shooters.Count > 0 && data.tunnelDirection == Direction.None)
                    {
                        int capacity = int.Parse(value);
                        data.shooters[0].capacity = capacity;
                        selectedTile.shooter.SetUp(data.shooters[0]);
                    }
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangeTunnelDirection(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.tunnelDirection = (Direction)option;
                    selectedTile.tunnel.SetDirection((Direction)option);
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangePinDirection(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.pin != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.pinDirection = (Direction)option;
                    selectedTile.pin.SetDirection((Direction)option);
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangeClothType(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.cloth != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.clothType = (ClothType)option;
                    selectedTile.cloth.SetClothType((ClothType)option);
                    break;
                }
            }
        }
    }

    public void OnInputClothCountValueChange(string value)
    {
        if (selectedTile != null && selectedTile.cloth != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.clothCount = int.Parse(value);
                    selectedTile.cloth.SetClothCount(int.Parse(value));
                    break;
                }
            }
        }
    }

    public void OnDropdownValueChangeLockChainAxis(int option)
    {
        if (option == 0) return;

        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.lockChainAxis = (Axis)option;
                    selectedTile.lockChain.SetUp(data.lockChainAxis, data.lockChainLength, data.lockChainCode);
                    break;
                }
            }
        }
    }

    public void OnInputLockChainLengthValueChange(string value)
    {
        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.lockChainLength = int.Parse(value);
                    selectedTile.lockChain.SetUp(data.lockChainAxis, data.lockChainLength, data.lockChainCode);
                    break;
                }
            }
        }
    }

    public void OnInputLockChainCodeValueChange(string value)
    {
        if (selectedTile != null && selectedTile.lockChain != null)
        {
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.lockChainCode = int.Parse(value);
                    selectedTile.lockChain.SetLockCode(int.Parse(value));
                    break;
                }
            }
        }
    }

    public void OnClickButtonCreate()
    {
        if (selectedTile != null)
        {
            switch (dropdownElement.value)
            {
                case 0: // Shooter
                    {
                        selectedTile.SpawnShooter();

                        ShooterData shooterData = new ShooterData();
                        shooterData.capacity = int.Parse(inputShooterCapacity.text);
                        shooterData.color = Enum.Parse<ColorEnum>(dropdownShooterColor.options[dropdownShooterColor.value].text);
                        shooterData.isHidden = toggleShooterHidden.isOn;
                        shooterData.hasIce = toggleHasIce.isOn;
                        shooterData.iceCount = int.Parse(inputIceCount.text);
                        shooterData.hasCrate = toggleHasCrate.isOn;
                        shooterData.crateCount = int.Parse(inputCrateCount.text);
                        shooterData.isKey = toggleHasKey.isOn;
                        shooterData.isLock = toggleHasLock.isOn;
                        shooterData.lockCode = int.Parse(inputLockCode.text);
                        shooterData.hasShutter = toggleHasShutter.isOn;
                        shooterData.isShutterOpen = toggleIsShutterOpen.isOn;

                        selectedTile.shooter.SetUp(shooterData);

                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.shooters.Add(shooterData);
                                break;
                            }
                        }
                        break;
                    }
                case 1: // Tunnel
                    {
                        selectedTile.SpawnTunnel();

                        Direction tunnelDirection = (Direction)dropdownTunnelDirection.value;

                        selectedTile.tunnel.SetUp(shooterDataUIs.Count, tunnelDirection);

                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.tunnelDirection = tunnelDirection;

                                foreach (var dataUI in shooterDataUIs)
                                {
                                    ShooterData shooterData = new ShooterData();
                                    shooterData.color = dataUI.GetColor();
                                    shooterData.capacity = dataUI.GetAmount();
                                    data.shooters.Add(shooterData);
                                }
                                break;
                            }
                        }
                        break;
                    }
                case 2: // Pin
                    {
                        selectedTile.SpawnPin();

                        Direction pinDirection = (Direction)dropdownPinDirection.value;

                        selectedTile.pin.SetUp(pinDirection);

                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.pinDirection = pinDirection;
                                break;
                            }
                        }
                        break;
                    }
                case 3: // Cloth
                    {
                        selectedTile.SpawnCloth();

                        ClothType clothType = (ClothType)dropdownClothType.value;
                        int clothCount = int.Parse(inputClothCount.text);

                        selectedTile.cloth.SetUp(clothType, clothCount);

                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.clothType = clothType;
                                data.clothCount = clothCount;
                                break;
                            }
                        }
                        break;
                    }
                case 4: // Lock Chain
                    {
                        selectedTile.SpawnLockChain();

                        Axis lockChainAxis = (Axis)dropdownLockChainAxis.value;
                        int lockChainLength = int.Parse(inputLockChainLength.text);
                        int lockChainCode = int.Parse(inputLockChainCode.text);

                        selectedTile.lockChain.SetUp(lockChainAxis, lockChainLength, lockChainCode);

                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.lockChainAxis = lockChainAxis;
                                data.lockChainLength = lockChainLength;
                                data.lockChainCode = lockChainCode;
                                break;
                            }
                        }
                        break;
                    }
            }

            btnCreate.SetActive(false);
            btnRemove.SetActive(true);

            UpdateTotalColor();
        }
    }

    public void OnClickButtonRemove()
    {
        if (selectedTile != null)
        {
            switch (dropdownElement.value)
            {
                case 0: // Shooter
                case 1: // Tunnel
                    {
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.shooters.Clear();
                                data.tunnelDirection = Direction.None;

                                if (selectedTile.shooter != null)
                                {
                                    selectedTile.shooter.gameObject.Recycle();
                                }

                                if (selectedTile.tunnel)
                                {
                                    selectedTile.tunnel.gameObject.Recycle();
                                }

                                btnRemove.SetActive(false);
                                btnCreate.SetActive(true);
                                break;
                            }
                        }
                        break;
                    }
                case 2: // Pin
                    {
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.pinDirection = Direction.None;

                                if (selectedTile.pin)
                                {
                                    selectedTile.pin.gameObject.Recycle();
                                }

                                btnRemove.SetActive(false);
                                btnCreate.SetActive(true);
                                break;
                            }
                        }
                        break;
                    }
                case 3: // Cloth
                    {
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.clothType = ClothType.None;

                                if (selectedTile.cloth)
                                {
                                    selectedTile.cloth.gameObject.Recycle();
                                }

                                btnRemove.SetActive(false);
                                btnCreate.SetActive(true);
                                break;
                            }
                        }
                        break;
                    }
                case 4: // Lock Chain
                    {
                        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
                        {
                            if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                            {
                                data.lockChainAxis = Axis.None;

                                if (selectedTile.lockChain)
                                {
                                    selectedTile.lockChain.gameObject.Recycle();
                                }

                                btnRemove.SetActive(false);
                                btnCreate.SetActive(true);
                                break;
                            }
                        }
                        break;
                    }
            }

            UpdateTotalColor();
        }
    }

    public void OnClickButtonAddShooterData()
    {
        int index = shooterDataUIs.Count;

        GameObject obj = shooterDataUIPrefab.Spawn(scrollShooterData.content);
        obj.transform.localScale = Vector3.one;

        ShooterData shooterData = new ShooterData();

        ShooterDataUI shooterDataUI = obj.GetComponent<ShooterDataUI>();


        shooterDataUIs.Add(shooterDataUI);

        btnAddShooterData.transform.parent.SetAsLastSibling();

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            selectedTile.tunnel.SetTotalShooter(shooterDataUIs.Count);
            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (data.pos.x == selectedTile.x && data.pos.y == selectedTile.y)
                {
                    data.shooters.Add(shooterData);
                    break;
                }
            }
        }

        shooterDataUI.Init(index);
        shooterDataUI.SetUp(shooterData);

        UpdateTotalColor();
    }

    public void RemoveShooterData(int index)
    {
        shooterDataUIs.RemoveAt(index);

        if (selectedTile != null && selectedTile.tunnel != null)
        {
            selectedTile.tunnel.SetTotalShooter(shooterDataUIs.Count);

            foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                if (tileData.pos.x == LevelDesign.Instance.UIShooter.selectedTile.x && tileData.pos.y == LevelDesign.Instance.UIShooter.selectedTile.y)
                {
                    tileData.shooters.RemoveAt(index);
                    break;
                }
            }
        }

        for (int i = 0; i < shooterDataUIs.Count; i++)
        {
            shooterDataUIs[i].Init(i);
        }

        UpdateTotalColor();
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

    public override void Hide()
    {
        base.Hide();
    }
}

public enum ElementType
{
    Shooter = 0,
    Tunnel = 1,
    Link = 2
}