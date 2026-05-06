using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;


#if UNITY_EDITOR
public class LevelDataShiftTool : EditorWindow
{
    private int startN = 10;
    private int endN = 150;
    private int shiftX = 1; // cho phép âm/dương
    private string folder = "Assets/Resources/LevelData";

    [MenuItem("Tools/LevelData/Shift Levels (+/-X)")]
    public static void Open()
    {
        GetWindow<LevelDataShiftTool>("Shift LevelData +/-X");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Shift level file names by X (X can be negative)", EditorStyles.boldLabel);
        folder = EditorGUILayout.TextField("Folder", folder);
        startN = EditorGUILayout.IntField("Start N", startN);
        endN = EditorGUILayout.IntField("End", endN);
        shiftX = EditorGUILayout.IntField("Shift X", shiftX);

        EditorGUILayout.Space(8);

        if (GUILayout.Button("SHIFT"))
        {
            Shift();
        }
    }

    private void Shift()
    {
        if (!AssetDatabase.IsValidFolder(folder))
        {
            Debug.LogError($"Folder not found: {folder}");
            return;
        }

        if (startN < 1 || endN < startN)
        {
            Debug.LogError("Invalid range.");
            return;
        }

        if (shiftX == 0)
        {
            Debug.LogWarning("Shift X = 0, nothing to do.");
            return;
        }

        // Load all numeric files in folder (ignore .meta)
        var absFolder = Path.GetFullPath(folder);
        var allFilesAbs = Directory.GetFiles(absFolder)
            .Where(p => !p.EndsWith(".meta"))
            .ToList();

        // Map: levelNumber -> unity asset path
        Dictionary<int, string> map = new Dictionary<int, string>();

        foreach (var abs in allFilesAbs)
        {
            var nameNoExt = Path.GetFileNameWithoutExtension(abs);
            if (!int.TryParse(nameNoExt, out var n)) continue;

            var projAssetsPath = Application.dataPath.Replace('\\', '/'); // .../Project/Assets
            var absNorm = abs.Replace('\\', '/');
            if (!absNorm.StartsWith(projAssetsPath)) continue;

            var unityPath = "Assets" + absNorm.Substring(projAssetsPath.Length);
            map[n] = unityPath;
        }

        // Validate: new indices must be >= 1
        for (int i = startN; i <= endN; i++)
        {
            if (!map.ContainsKey(i)) continue;
            int newIndex = i + shiftX;
            if (newIndex < 1)
            {
                Debug.LogError($"Shift would create invalid level number: {i} -> {newIndex}. Abort.");
                return;
            }
        }

        // Collision check: avoid overwriting files outside movable set
        // If target exists and it won't be moved away by this operation => abort.
        for (int i = startN; i <= endN; i++)
        {
            if (!map.ContainsKey(i)) continue;

            var fromPath = map[i];
            var ext = Path.GetExtension(fromPath);
            int targetIndex = i + shiftX;
            var toPath = $"{folder}/{targetIndex}{ext}";

            bool targetExists = File.Exists(Path.GetFullPath(toPath));

            bool targetWillBeMovedAway =
                (targetIndex >= startN && targetIndex <= endN && map.ContainsKey(targetIndex));

            if (targetExists && !targetWillBeMovedAway)
            {
                Debug.LogError($"Target exists outside movable range, abort to avoid overwrite: {toPath}");
                return;
            }
        }

        // Decide rename order:
        // shiftX > 0 => End -> Start
        // shiftX < 0 => Start -> End
        int step = shiftX > 0 ? -1 : 1;
        int iStart = shiftX > 0 ? endN : startN;
        int iEndExclusive = shiftX > 0 ? (startN - 1) : (endN + 1);

        AssetDatabase.StartAssetEditing();
        try
        {
            for (int i = iStart; i != iEndExclusive; i += step)
            {
                if (!map.ContainsKey(i)) continue;

                var fromPath = map[i];
                var ext = Path.GetExtension(fromPath);
                var toPath = $"{folder}/{i + shiftX}{ext}";

                var err = AssetDatabase.MoveAsset(fromPath, toPath);
                if (!string.IsNullOrEmpty(err))
                {
                    Debug.LogError($"Move failed: {fromPath} -> {toPath}\n{err}");
                    return;
                }
            }
        }
        finally
        {
            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }

        Debug.Log($"Done! Shifted levels {startN}..{endN} by {shiftX} in: {folder}");
    }
}
#endif
