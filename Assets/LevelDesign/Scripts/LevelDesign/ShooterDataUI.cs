using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShooterDataUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI txtIndex;
    [SerializeField] TMP_Dropdown dropdownColor;
    [SerializeField] TMP_InputField amountInput;

    int index;

    public void Init(int index)
    {
        this.index = index;
        txtIndex.text = (index + 1).ToString();

        ColorEnum[] colorEnums = (ColorEnum[])Enum.GetValues(typeof(ColorEnum));

        dropdownColor.options.Clear();
        for (int i = 0; i < colorEnums.Length; i++)
        {
            dropdownColor.options.Add(new TMP_Dropdown.OptionData(colorEnums[i].ToString()));
        }

        //dropdownColor.value = 0;
        //dropdownColor.RefreshShownValue();
    }

    public void SetUp(ShooterData data)
    {
        TMP_Dropdown.OptionData optionData = null;
        foreach (var option in dropdownColor.options)
        {
            if (option.text.Equals(data.color.ToString()))
            {
                optionData = option;
                break;
            }
        }
        amountInput.text = data.capacity.ToString();
        dropdownColor.value = dropdownColor.options.IndexOf(optionData);
    }

    public ColorEnum GetColor()
    {
        return Enum.Parse<ColorEnum>(dropdownColor.options[dropdownColor.value].text);
    }

    public int GetAmount()
    {
        return int.Parse(amountInput.text);
    }

    public void OnValueChangeAmountInput(string value)
    {
        if (LevelDesign.Instance.UIShooter.selectedTile != null)
        {
            if (LevelDesign.Instance.UIShooter.selectedTile.tunnel != null)
            {
                foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
                {
                    if (tileData.pos.x == LevelDesign.Instance.UIShooter.selectedTile.x && tileData.pos.y == LevelDesign.Instance.UIShooter.selectedTile.y)
                    {
                        if (int.TryParse(value, out int amount))
                        {
                            tileData.shooters[index].capacity = amount;
                        }
                        break;
                    }
                }
            }
        }
    }

    public void OnValueChangeDropdownColor(int value)
    {
        if (LevelDesign.Instance.UIShooter.selectedTile != null)
        {
            if (LevelDesign.Instance.UIShooter.selectedTile.tunnel != null)
            {
                foreach (var tileData in LevelDesign.Instance.levelData.shooterTileDatas)
                {
                    if (tileData.pos.x == LevelDesign.Instance.UIShooter.selectedTile.x && tileData.pos.y == LevelDesign.Instance.UIShooter.selectedTile.y)
                    {
                        if (Enum.TryParse(dropdownColor.options[value].text, out ColorEnum color))
                        {
                            tileData.shooters[index].color = color;
                        }
                        break;
                    }
                }
            }
        }

        LevelDesign.Instance.UIShooter.UpdateTotalColor();
    }

    public void OnClickButtonDelete()
    {
        LevelDesign.Instance.UIShooter.RemoveShooterData(index);
        gameObject.Recycle();
    }
}
