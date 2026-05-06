using UnityEngine;
using System.Collections.Generic;

public enum eTypeGenerateBox
{
    OneBox,
    ManyBoxs
}

public class MeshToBoxColliders : MonoBehaviour
{
    public int clusterSize = 200;
    public eTypeGenerateBox eTypeGenerateBox;

    private void OnValidate()
    {
        switch (eTypeGenerateBox)
        {
            case eTypeGenerateBox.OneBox:
                GenerateBoxCollider();
                break;
            case eTypeGenerateBox.ManyBoxs:
                GenerateBoxColliders();
                break;
        }
    }

    void GenerateBoxCollider()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.Recycle();
        }

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null) return;

        MeshCollider meshCollider = GetComponent<MeshCollider>();

        Mesh mesh = meshFilter.sharedMesh;
        Bounds bounds = mesh.bounds;

        //GameObject box = new GameObject("BoxCollider");
        //box.transform.parent = transform;
        //box.transform.localPosition = bounds.center;
        //box.transform.localRotation = Quaternion.identity;

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.center = bounds.center;
        boxCollider.size = bounds.size;
    }

    void GenerateBoxColliders()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null || meshCollider.sharedMesh == null)
        {
            return;
        }

        if (meshCollider.sharedMesh == null)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }

        Mesh mesh = meshCollider.sharedMesh;
        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        List<List<Vector3>> clusters = ClusterTriangles(vertices, triangles, clusterSize);

        foreach (var cluster in clusters)
        {
            CreateBoxForCluster(cluster);
        }
    }

    List<List<Vector3>> ClusterTriangles(Vector3[] vertices, int[] triangles, int maxClusterSize)
    {
        List<List<Vector3>> clusters = new List<List<Vector3>>();
        List<Vector3> currentCluster = new List<Vector3>();

        for (int i = 0; i < triangles.Length; i += 3)
        {
            currentCluster.Add(vertices[triangles[i]]);
            currentCluster.Add(vertices[triangles[i + 1]]);
            currentCluster.Add(vertices[triangles[i + 2]]);

            if (currentCluster.Count >= maxClusterSize * 3)
            {
                clusters.Add(new List<Vector3>(currentCluster));
                currentCluster.Clear();
            }
        }

        if (currentCluster.Count > 0)
            clusters.Add(currentCluster);

        return clusters;
    }

    void CreateBoxForCluster(List<Vector3> cluster)
    {
        if (cluster.Count == 0) return;

        Bounds bounds = new Bounds(cluster[0], Vector3.zero);
        foreach (var point in cluster)
        {
            bounds.Encapsulate(point);
        }

        GameObject box = new GameObject("BoxCollider");
        box.transform.parent = transform;
        box.transform.localPosition = bounds.center;
        box.transform.localRotation = Quaternion.identity;

        BoxCollider boxCollider = box.AddComponent<BoxCollider>();
        boxCollider.size = bounds.size;
    }
}
