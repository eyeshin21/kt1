using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelDesign : MonoBehaviour
{
    public TMP_InputField levelInput;

    [Header("Tab")]
    public Tabs currentTab;
    public Toggle toggleTabGround;
    
    public void OnToggleTabGround(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.Ground)
            {
                currentTab = Tabs.Ground;
                LevelDesign.Instance.UIGround.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIGround.Hide();
        }
    }
    
    public void OnToggleTabShooter(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.Shooter)
            {
                currentTab = Tabs.Shooter;
                LevelDesign.Instance.UIShooter.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIShooter.Hide();
        }
    }

    public void OnToggleTabColorEntry(bool isOn)
    {
        if (isOn)
        {
            if (currentTab != Tabs.ColorEntry)
            {
                currentTab = Tabs.ColorEntry;
                LevelDesign.Instance.UIColorEntry.Show();
            }
        }
        else
        {
            LevelDesign.Instance.UIColorEntry.Hide();
        }
    }

    public void OnClickButtonLoad()
    {
        if (int.TryParse(levelInput.text, out int level))
        {
            LevelDesign.Instance.LoadLevel(level);
            toggleTabGround.isOn = true;
        }
    }

    public void OnClickButtonSave()
    {
        if (int.TryParse(levelInput.text, out int level))
        {
            LevelDesign.Instance.SaveLevel(level);
        }
    }
}

public enum Tabs
{
    Ground = 0,
    Shooter = 1,
    ColorEntry = 2
}