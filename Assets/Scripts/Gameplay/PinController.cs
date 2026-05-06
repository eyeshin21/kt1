using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TigerForge;
using UnityEngine;

public class PinController : MonoBehaviour
{
    public GameObject objPin;

    public Vector2Int pos;
    public Vector2Int checkPos;
    public Direction direction;
    public List<ShooterController> shooters = new List<ShooterController>();

    bool isUnpin = false;

    private void Start()
    {
        EventManager.StartListening(EventVariables.CheckPin, CheckPin);
    }

    public void Init(ShooterTileData data)
    {
        pos = data.pos.ToVector2Int();
        direction = data.pinDirection;

        isUnpin = false;

        switch (direction)
        {
            case Direction.Left:
                checkPos = new Vector2Int(pos.x + 1, pos.y);
                transform.localEulerAngles = Vector3.forward * 90f;
                break;
            case Direction.Right:
                checkPos = new Vector2Int(pos.x - 1, pos.y);
                transform.localEulerAngles = Vector3.forward * 270f;
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                checkPos = new Vector2Int(pos.x, pos.y - 1);
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.forward * 180f;
                checkPos = new Vector2Int(pos.x, pos.y + 1);
                break;
        }
    }

    public void CheckShooter()
    {
        switch (direction)
        {
            case Direction.Left:
                for (int x = pos.x; x >= pos.x - 2; x--)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[x, pos.y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
            case Direction.Right:
                for (int x = pos.x; x <= pos.x + 2; x++)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[x, pos.y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
            case Direction.Up:
                for (int y = pos.y; y <= pos.y + 2; y++)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[pos.x, y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
            case Direction.Down:
                for (int y = pos.y; y >= pos.y - 2; y--)
                {
                    ShooterController shooter = ShooterManager.Instance.shooterGrid[pos.x, y];
                    if (shooter != null)
                    {
                        shooters.Add(shooter);
                    }
                }
                break;
        }

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].hasPin = true;
        }
    }

    public void CheckPin()
    {
        if (!isUnpin)
        {
            if (ShooterManager.Instance.shooterGrid[checkPos.x, checkPos.y] == null)
            {
                Unpin();
            }
        }
    }

    public void Unpin()
    {
        isUnpin = true;

        objPin.transform.DOScale(0f, 0.25f).SetEase(Ease.Linear);
        transform.DOLocalMove(ShooterManager.Instance.GetWorldPos(checkPos.x, checkPos.y), 0.25f).SetEase(Ease.Linear).OnComplete(()=>
        {
            for (int i = 0; i < shooters.Count; i++)
            {
                shooters[i].hasPin = false;

                if (shooters[i].isActive)
                {
                    shooters[i].Active2();
                }
            }

            Recycle();
        });
    }

    public void Recycle()
    {
        transform.DOKill();
        objPin.transform.localScale = Vector3.one;
        shooters.Clear();
        isUnpin = true;

        gameObject.Recycle();
    }
}
