using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelAnalysis
{
    public int level;
    public int totalColor;
    public List<ColorRate> colorsRate = new List<ColorRate>();
    public List<Element> elements = new List<Element>();
}

[System.Serializable]
public class ColorRate
{
    public string colorName;
    public string rate;
}

[System.Serializable]
public class Element
{
    public string name;
    public int amount;
}
