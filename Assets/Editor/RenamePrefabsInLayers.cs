using UnityEditor;
using UnityEngine;

public class RenamePrefabsInLayers
{
    [MenuItem("Tools/Rename Prefabs In Resources/Layers")]
    static void RenamePrefabs()
    {
        string targetFolder = "Assets/Resources/Layers";

        // Tìm tất cả prefab trong folder Layers
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { targetFolder });

        int index = 1;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            // Đảm bảo đúng prefab
            if (path.EndsWith(".prefab"))
            {
                AssetDatabase.RenameAsset(path, "a-" + index.ToString());
                index++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Renamed {index - 1} prefabs in {targetFolder}");
    }
}
