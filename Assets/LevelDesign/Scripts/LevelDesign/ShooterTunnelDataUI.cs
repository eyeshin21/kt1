using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShooterTunnelDataUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIndex;
    [SerializeField] TextMeshProUGUI txtColor;
    [SerializeField] TextMeshProUGUI txtAmount;

    [SerializeField] Button btnAdd;
    [SerializeField] TextMeshProUGUI txtButtonAdd;

    int index;
    ColorEnum color;
    int capacity;

    public void Init(int index)
    {
        this.index = index;
        txtIndex.text = (index + 1).ToString();

        txtButtonAdd.text = "Add";
        btnAdd.interactable = true;
    }

    public void SetUp(ShooterData data)
    {
        color = data.color;
        capacity = data.capacity;

        txtAmount.text = data.capacity.ToString();
        txtColor.text = data.color.ToString();
        txtColor.color = LevelDesign.Instance.colors[(int)data.color];
    }

    public void Added()
    {
        txtButtonAdd.text = "Added";
        btnAdd.interactable = false;
    }

    public void OnClickButtonAdd()
    {
        SelectedShooter selectedShooter = new SelectedShooter();
        selectedShooter.x = LevelDesign.Instance.UIColorEntry.selectedTile.x;
        selectedShooter.y = LevelDesign.Instance.UIColorEntry.selectedTile.y;
        selectedShooter.isInsideTunnel = true;
        selectedShooter.indexInTunnel = index;
        LevelDesign.Instance.UIColorEntry.selectedShooters.Add(selectedShooter);

        ColorEntry colorEntry = new ColorEntry(capacity, color);
        LevelDesign.Instance.UIColorEntry.colorEntries.Add(colorEntry);

        Added();

        LevelDesign.Instance.UIColorEntry.UpdateTotalAdded();
    }
}
