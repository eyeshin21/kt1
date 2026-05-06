using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EncryptData
{
    [MenuItem("Tools/Encrypt Level")]
    public static void EncryptLevel()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        string dataStr = AssetDatabase.LoadAssetAtPath<TextAsset>(path).text;
        if (dataStr.Contains("shooterGridSize"))
        {
            File.WriteAllBytes(path, Convert.FromBase64String(SaveSystem.Encrypt(dataStr, "DeoDucDu0cD@u")));
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Decrypt level")]
    public static void DecryptLevel()
    {
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        TextAsset dataAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);

        if (!dataAsset.text.Contains("shooterGridSize"))
        {
            File.WriteAllText(path, SaveSystem.Decrypt(Convert.ToBase64String(dataAsset.bytes), "DeoDucDu0cD@u"));
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Encrypt All Level")]
    public static void EncryptAllLevel()
    {
        var dataLevels = Resources.LoadAll<TextAsset>("LevelData");
        Debug.Log($"Total Levels: {dataLevels.Length}");

        foreach (var level in dataLevels)
        {
            if (level.text.Contains("shooterGridSize"))
            {
                File.WriteAllBytes($"Assets/Resources/LevelData/{level.name}.json", Convert.FromBase64String(SaveSystem.Encrypt(level.text, "DeoDucDu0cD@u")));
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Tools/Decrypt All Level")]
    public static void DecryptAllLevel()
    {
        var dataLevels = Resources.LoadAll<TextAsset>("LevelData");
        Debug.Log($"Total Levels: {dataLevels.Length}");

        foreach (var level in dataLevels)
        {
            if (!level.text.Contains("shooterGridSize"))
            {
                File.WriteAllText($"Assets/Resources/LevelData/{level.name}.json", SaveSystem.Decrypt(Convert.ToBase64String(level.bytes), "DeoDucDu0cD@u"));
            }
        }
        
        AssetDatabase.Refresh();
    }
}
