using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GDPin : MonoBehaviour
{
    public int x;
    public int y;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(Direction direction)
    {
        SetDirection(direction);
    }

    public void SetDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                transform.localEulerAngles = Vector3.forward * 90f;
                break;
            case Direction.Right:
                transform.localEulerAngles = Vector3.forward * 270f;
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.forward * 180f;
                break;
        }
    }
}
