using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGround : UI, IPointerDownHandler, IPointerUpHandler
{
    public bool canPickGround = false;

    [Header("Grid")]
    [SerializeField] TMP_InputField gridSizeXInput;
    [SerializeField] TMP_InputField gridSizeYInput;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] GridLayoutGroup gridLayoutGroup;

    [Header("Cell")]
    public GameObject groundCellPrefab;
    public GroundCell[,] groundCellGrid;
    public GroundType selectedGroundType = GroundType.Grass;

    public override void Show()
    {
        base.Show();

        if (LevelDesign.Instance.levelData.shooterTileDatas.Count > 0)
        {
            if (groundCellGrid != null)
            {
                foreach (var cell in groundCellGrid)
                {
                    cell.gameObject.Recycle();
                }

                groundCellGrid = null;
            }

            int gridSizeX = LevelDesign.Instance.levelData.shooterGridSize.x;
            int gridSizeY = LevelDesign.Instance.levelData.shooterGridSize.y;

            gridSizeXInput.text = gridSizeX.ToString();
            gridSizeYInput.text = gridSizeY.ToString();

            GenerateGrid();

            foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
            {
                groundCellGrid[data.pos.x, data.pos.y].SetType(GroundType.Road);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        canPickGround = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        canPickGround = false;
    }

    public void OnToggleGrass(bool isOn)
    {
        selectedGroundType = GroundType.Grass;
    } 
    
    public void OnToggleRoad(bool isOn)
    {
        selectedGroundType = GroundType.Road;
    }

    public void ActiveScroll(bool active)
    {
        scrollRect.enabled = active;
    }

    public void OnSetGroundType(int x, int y, GroundType type)
    {
        int index = -1;
        foreach (var data in LevelDesign.Instance.levelData.shooterTileDatas)
        {
            if (data.pos.x == x && data.pos.y == y)
            {
                index = LevelDesign.Instance.levelData.shooterTileDatas.IndexOf(data);
                break;
            }
        }

        if (type == GroundType.Grass)
        {
            if (index != -1)
            {
                LevelDesign.Instance.levelData.shooterTileDatas.RemoveAt(index);
            }
        }
        else
        {
            if (index == -1)
            {
                ShooterTileData newData = new ShooterTileData();
                newData.pos = new Vector2IntS(x, y);

                LevelDesign.Instance.levelData.shooterTileDatas.Add(newData);
            }
        }
    }

    public void OnClickButtonGenGrid()
    {
        int xInput = int.Parse(gridSizeXInput.text);
        int yInput = int.Parse(gridSizeYInput.text);

        LevelDesign.Instance.levelData.shooterGridSize = new Vector2IntS(xInput, yInput);
        LevelDesign.Instance.levelData.shooterTileDatas.Clear();

        if (groundCellGrid != null)
        {
            foreach (var cell in groundCellGrid)
            {
                cell.gameObject.Recycle();
            }

            groundCellGrid = null;
        }

        GenerateGrid();
    }

    public void GenerateGrid()
    {
        int xInput = int.Parse(gridSizeXInput.text);
        int yInput = int.Parse(gridSizeYInput.text);
        gridLayoutGroup.constraintCount = xInput;

        groundCellGrid = new GroundCell[xInput, yInput];

        for (int y = yInput - 1; y >= 0; y--)
        {
            for (int x = 0; x < xInput; x++)
            {
                GameObject obj = groundCellPrefab.Spawn(scrollRect.content);
                obj.transform.localScale = Vector3.one;

                GroundCell groundCell = obj.GetComponent<GroundCell>();
                groundCell.Init(x, y);

                groundCellGrid[x, y] = groundCell;
            }
        }
    }

    public override void Hide()
    {
        base.Hide();
    }
}
