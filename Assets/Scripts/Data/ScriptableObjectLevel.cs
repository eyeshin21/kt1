using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public class ScriptableObjectLevel : ScriptableObject
{
    public LevelInfo[] levelInfos;
}

[Serializable]
public class LevelInfo
{
    public eTypeLevel eTypeLevel;
    public int coin;
    public eTypeElement eTypeElement;
}

public enum eTypeLevel
{
    Normal = 0,
    Tutorial = 1,
    Hard = 2,
    SuperHard = 3
}

[Serializable]
public class ProgressElement
{
    public int prevLevel;
    public int nextLevel;
    public eTypeElement eTypeElement;

    public ProgressElement(int prevLevel, int nextLevel, eTypeElement eTypeElement)
    {
        this.prevLevel = prevLevel;
        this.nextLevel = nextLevel;
        this.eTypeElement = eTypeElement;
    }
}

#if UNITY_EDITOR
public class LevelScriptableEditor : MonoBehaviour
{
    [MenuItem("Assets/Create/My Level Data")]
    public static void CreateMyAsset()
    {
        ScriptableObjectLevel asset = new ScriptableObjectLevel();

        EditorUtility.SetDirty(asset);
        AssetDatabase.CreateAsset(asset, "Assets/ScriptableObjects/Level.asset");
        AssetDatabase.SaveAssets();

        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif

