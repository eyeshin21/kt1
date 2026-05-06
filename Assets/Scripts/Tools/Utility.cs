using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CGTeam
{
    public static class Utility
    {
        public static bool IsInObject(Vector2 center, float radius, Vector2 pos)
        {
            return Vector2.Distance(center, pos) <= radius;
        }

        public static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float degrees)
        {
            return Quaternion.AngleAxis(degrees, axis) * vector;
        }

        static Vector3 RotatePointAroundLine(Vector3 pointToRotate, Vector3 pointOnLine0, Vector3 pointOnLine1, float degrees)
        {
            Vector3 localVector = pointToRotate - pointOnLine0;
            Vector3 axis = pointOnLine1 - pointOnLine0;
            Vector3 rotatedVector = RotateVectorAroundAxis(localVector, axis, degrees);
            return rotatedVector + pointOnLine0;
        }

        public static float CalculateLengthOfLine(Vector3[] path)
        {
            float length = 0;
            for (int i = 0; i < path.Length - 1; i++)
            {
                length += Vector3.Distance(path[i], path[i + 1]);
            }
            return length;
        }

        public static float CalculateLengthOfLine(Vector2[] path)
        {
            float length = 0;
            for (int i = 0; i < path.Length - 1; i++)
            {
                length += Vector2.Distance(path[i], path[i + 1]);
            }
            return length;
        }

        public static Vector2[] GetNearestLine(Transform lineTf, Vector2 startPoint, bool start)
        {
            float minDist = float.MaxValue;
            int indexNearestPoint = 0;
            int sign = 1;
            float offset = 0.3f;
            for (int i = 0; i < lineTf.childCount; i++)
            {
                float tempDist = Vector2.Distance(startPoint, lineTf.GetChild(i).position);
                if (tempDist < minDist)
                {
                    minDist = tempDist;
                    indexNearestPoint = i;
                }
            }

            if (Vector2.Distance(startPoint, lineTf.GetChild(indexNearestPoint).TransformPoint(new Vector3(0, offset, 0))) > Vector2.Distance(startPoint, lineTf.GetChild(indexNearestPoint).TransformPoint(new Vector3(0, -offset, 0))))
            {
                sign = -1;
            }
            else
            {
                sign = 1;
            }

            Vector2[] newPath;
            int tempLength;
            if (start)
            {
                tempLength = indexNearestPoint + 2;
                newPath = new Vector2[indexNearestPoint + 2];
                for (int i = indexNearestPoint; i >= 0; i--)
                {
                    newPath[indexNearestPoint - i] = lineTf.GetChild(i).TransformPoint(new Vector3(0, offset * sign, 0));
                }
            }
            else
            {
                tempLength = lineTf.childCount - indexNearestPoint + 1;
                newPath = new Vector2[tempLength];
                for (int i = indexNearestPoint; i < lineTf.childCount; i++)
                {
                    newPath[i - indexNearestPoint] = lineTf.GetChild(i).TransformPoint(new Vector3(0, offset * sign, 0));
                }
            }

            newPath[tempLength - 1] = newPath[tempLength - 2] + (newPath[tempLength - 2] - newPath[tempLength - 3]);
            return newPath;
        }

        public static GameObject[] GetArrayGameObjectNearestLine(Transform lineTf, Vector2 startPoint, bool start)
        {
            float minDist = float.MaxValue;
            int indexNearestPoint = 0;
            int sign = 1;
            float offset = 0.3f;
            for (int i = 0; i < lineTf.childCount; i++)
            {
                float tempDist = Vector2.Distance(startPoint, lineTf.GetChild(i).position);
                if (tempDist < minDist)
                {
                    minDist = tempDist;
                    indexNearestPoint = i;
                }
            }

            if (Vector2.Distance(startPoint, lineTf.GetChild(indexNearestPoint).TransformPoint(new Vector3(0, offset, 0))) > Vector2.Distance(startPoint, lineTf.GetChild(indexNearestPoint).TransformPoint(new Vector3(0, -offset, 0))))
            {
                sign = -1;
            }
            else
            {
                sign = 1;
            }

            GameObject[] newPath;
            int tempLength;
            if (start)
            {
                tempLength = indexNearestPoint + 2;
                newPath = new GameObject[tempLength];
                for (int i = indexNearestPoint; i >= 0; i--)
                {
                    newPath[indexNearestPoint - i] = new GameObject();
                    //newPath[indexNearestPoint - i] = GameManager.Instance.InstantiatePrefab("Test");
                    newPath[indexNearestPoint - i].transform.parent = lineTf.GetChild(i);
                    newPath[indexNearestPoint - i].transform.localPosition = new Vector3(0, offset * sign, 0);
                }
                newPath[tempLength - 1] = new GameObject();
                newPath[tempLength - 1].transform.parent = lineTf.GetChild(0);
                newPath[tempLength - 1].transform.localPosition = new Vector3(-offset, offset * sign, 0);
            }
            else
            {
                tempLength = lineTf.childCount - indexNearestPoint + 1;
                newPath = new GameObject[tempLength];
                for (int i = indexNearestPoint; i < lineTf.childCount; i++)
                {
                    newPath[i - indexNearestPoint] = new GameObject();
                    //newPath[i - indexNearestPoint] = GameManager.Instance.InstantiatePrefab("Test");
                    newPath[i - indexNearestPoint].transform.parent = lineTf.GetChild(i);
                    newPath[i - indexNearestPoint].transform.localPosition = new Vector3(0, offset * sign, 0);
                }
                newPath[tempLength - 1] = new GameObject();
                newPath[tempLength - 1].transform.parent = lineTf.GetChild(lineTf.childCount - 1);
                newPath[tempLength - 1].transform.localPosition = new Vector3(offset, offset * sign, 0);
            }

            return newPath;
        }
    }
}
