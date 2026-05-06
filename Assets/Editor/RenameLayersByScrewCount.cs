using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RenameLayersByScrewCount
{
    [MenuItem("Tools/Rename Layers By Screw Count")]
    static void RenamePrefabs()
    {
        string targetFolder = "Assets/Resources/Layers";

        // Lấy tất cả prefab trong folder
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

        // Map: screwCount -> list prefab path
        Dictionary<int, List<string>> screwMap = new Dictionary<int, List<string>>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            LevelLayer layer = prefab.GetComponent<LevelLayer>();
            if (layer == null) continue; // chỉ xử lý prefab có LevelLayer

            // Đếm screw
            int screwCount = 0;
            ShapeController[] shapes = prefab.GetComponentsInChildren<ShapeController>(true);

            foreach (var shape in shapes)
            {
                if (shape.screws != null)
                    screwCount += shape.screws.Count;
            }

            if (!screwMap.ContainsKey(screwCount))
                screwMap[screwCount] = new List<string>();

            screwMap[screwCount].Add(path);
        }

        // Rename
        foreach (var kvp in screwMap.OrderBy(k => k.Key))
        {
            int screwCount = kvp.Key;
            List<string> paths = kvp.Value;

            for (int i = 0; i < paths.Count; i++)
            {
                string newName = $"{screwCount}-{i}";
                AssetDatabase.RenameAsset(paths[i], newName);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Rename prefab theo số lượng screw hoàn tất");
    }
}
