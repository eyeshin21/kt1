using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelDesign : Singleton<LevelDesign>
{
    public LevelData levelData = new LevelData();

    [Header("UI")]
    public UILevelDesign UILevelDesign;
    public UIGround UIGround;
    public UIShooter UIShooter;
    public UIColorEntry UIColorEntry;

    [Header("Colors")]
    public List<Color> colors;

    private void Start()
    {
        UIGround.Show();
        UIShooter.Hide();
        UIColorEntry.Hide();
    }

    public void LoadLevel(int level)
    {
        TextAsset data = Resources.Load<TextAsset>($"LevelData/{level}");
        if (data.text.Contains("shooterGridSize"))
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(data.text);
        }
        else
        {
            levelData = JsonConvert.DeserializeObject<LevelData>(SaveSystem.Decrypt(System.Convert.ToBase64String(data.bytes), "DeoDucDu0cD@u"));
        }

        UIGround.Show();
        UIShooter.Hide();
        UIColorEntry.Hide();
    }

    public void SaveLevel(int level)
    {
        string txt = JsonConvert.SerializeObject(levelData);

        string path = Path.Combine(Application.dataPath, "Resources", "LevelData");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, string.Concat(string.Format("{0}", level), ".json"));
        File.WriteAllText(path, txt);

#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log(string.Format("Save level {0} success!!!", level));
    }

    [Header("Export Data Analysis")]
    [SerializeField] private int fromLevel = 1;
    [SerializeField] private int toLevel = 100;

    [ContextMenu("Export Data")]
    public void ExportData()
    {
        List<LevelAnalysis> levelAnalyses = new List<LevelAnalysis>();

        for (int i = fromLevel; i <= toLevel; i++)
        {
            LevelAnalysis levelAnalysis = new LevelAnalysis();
            levelAnalysis.level = i;

            TextAsset textAsset = Resources.Load<TextAsset>(Path.Combine("LevelData", i.ToString()));
            LevelData levelData = JsonConvert.DeserializeObject<LevelData>(textAsset.text);

            Dictionary<string, int> elementDicts = new Dictionary<string, int>();

            int totalAmountColor = 0;
            Dictionary<ColorEnum, int> colorDicts = new Dictionary<ColorEnum, int>();
            foreach (var shooterTileData in levelData.shooterTileDatas)
            {
                foreach (var shooterData in shooterTileData.shooters)
                {
                    if (shooterData == null) continue;
                    if (shooterData.color == ColorEnum.None) continue;

                    if (colorDicts.ContainsKey(shooterData.color))
                    {
                        colorDicts[shooterData.color]++;
                    }
                    else
                    {
                        colorDicts.Add(shooterData.color, 1);
                    }

                    totalAmountColor++;

                    if (shooterData.isHidden)
                    {
                        if (!elementDicts.ContainsKey("Hidden Shooter"))
                        {
                            elementDicts.Add("Hidden Shooter", 0);
                        }

                        elementDicts["Hidden Shooter"]++;
                    }

                    if (shooterData.isLink)
                    {
                        if (!elementDicts.ContainsKey("Linked Shooter"))
                        {
                            elementDicts.Add("Linked Shooter", 0);
                        }

                        elementDicts["Linked Shooter"]++;
                    }

                    if (shooterData.isLock)
                    {
                        if (!elementDicts.ContainsKey("Lock & Key"))
                        {
                            elementDicts.Add("Lock & Key", 0);
                        }

                        elementDicts["Lock & Key"]++;
                    }

                    if (shooterData.hasIce)
                    {
                        if (!elementDicts.ContainsKey("Frozen Shooter"))
                        {
                            elementDicts.Add("Frozen Shooter", 0);
                        }

                        elementDicts["Frozen Shooter"]++;
                    }

                    if (shooterData.hasCrate)
                    {
                        if (!elementDicts.ContainsKey("Crate"))
                        {
                            elementDicts.Add("Crate", 0);
                        }

                        elementDicts["Crate"]++;
                    }
                }

                if (shooterTileData.tunnelDirection != Direction.None)
                {
                    if (!elementDicts.ContainsKey("Tunnel"))
                    {
                        elementDicts.Add("Tunnel", 0);
                    }

                    elementDicts["Tunnel"]++;
                }
            }

            levelAnalysis.totalColor = colorDicts.Keys.ToList().Count;

            foreach (var kvp in colorDicts)
            {
                ColorRate colorRate = new ColorRate();
                colorRate.colorName = kvp.Key.ToString();
                colorRate.rate = $"{Mathf.RoundToInt((kvp.Value / (float)totalAmountColor) * 100f)}%";
                levelAnalysis.colorsRate.Add(colorRate);
            }

            foreach (var kvp in elementDicts)
            {
                Element element = new Element();
                element.name = kvp.Key;
                element.amount = kvp.Value;

                if (kvp.Key.Equals("Linked Shooter"))
                {
                    element.amount = kvp.Value / 2;
                }

                levelAnalysis.elements.Add(element);
            }

            levelAnalyses.Add(levelAnalysis);
        }

        string txt = JsonConvert.SerializeObject(levelAnalyses);

        string path = Path.Combine(Application.dataPath, "Resources");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        path = Path.Combine(path, "LevelAnalyses.json");
        File.WriteAllText(path, txt);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        Debug.Log("Save Data Level Analyses success!!!");
    }
}

