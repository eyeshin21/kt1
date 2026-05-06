using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GDTunnel : MonoBehaviour
{
    public int x;
    public int y;

    public TextMeshPro txtTotalShooter;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetUp(int totalShooter, Direction direction)
    {
        SetTotalShooter(totalShooter);
        SetDirection(direction);
    }

    public void SetTotalShooter(int totalShooter)
    {
        txtTotalShooter.text = totalShooter.ToString();
    }

    public void SetDirection(Direction direction)
    {
        switch (direction)
        {
            case Direction.Left:
                transform.localEulerAngles = Vector3.forward * 90f;
                txtTotalShooter.transform.localEulerAngles = Vector3.forward * -90f;
                break;
            case Direction.Right:
                transform.localEulerAngles = Vector3.forward * 270f;
                txtTotalShooter.transform.localEulerAngles = Vector3.forward * -270f;
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                txtTotalShooter.transform.localEulerAngles = Vector3.zero;
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.forward * 180f;
                txtTotalShooter.transform.localEulerAngles = Vector3.forward * -180f;
                break;
        }
    }
}
