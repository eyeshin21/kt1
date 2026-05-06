using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eTypeConfigPolygonCollider
{
    Add,
    Ratio,
    FlipX
}

public class ConfigColidder : MonoBehaviour
{
    [SerializeField] PolygonCollider2D polygonCollider;
    [SerializeField] eTypeConfigPolygonCollider eTypeConfigPolygonCollider;
    [SerializeField] Vector2 diff;
    [SerializeField] float ratio;
    [SerializeField] int index;

    private void OnValidate()
    {
        if (polygonCollider != null)
        {
            Vector2[] vector2s = polygonCollider.GetPath(index);
            switch (eTypeConfigPolygonCollider)
            {
                case eTypeConfigPolygonCollider.Add:
                    for (int i = 0; i < vector2s.Length; i++)
                    {
                        vector2s[i] += diff;
                    }
                    break;
                case eTypeConfigPolygonCollider.Ratio:
                    for (int i = 0; i < vector2s.Length; i++)
                    {
                        vector2s[i] *= ratio;
                    }
                    break;
                case eTypeConfigPolygonCollider.FlipX:
                    for (int i = 0; i < vector2s.Length; i++)
                    {
                        vector2s[i] = new Vector2(-vector2s[i].x, vector2s[i].y);
                    }
                    break;
            }

            polygonCollider.SetPath(index, vector2s);
        }
    }
}
