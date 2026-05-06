using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TunnelController : MonoBehaviour
{
    public Vector2Int pos;
    public Vector2Int spawnPos;
    public Direction direction;
    public TextMeshPro txtCount;
    public List<ShooterController> shooters = new List<ShooterController>();

    public void Init(ShooterTileData data)
    {
        direction = data.tunnelDirection;
        pos = data.pos.ToVector2Int();

        switch (direction)
        {
            case Direction.Left:
                transform.localEulerAngles = Vector3.forward * 90f;
                txtCount.transform.localEulerAngles = Vector3.forward * -90f;
                spawnPos = new Vector2Int(pos.x - 1, pos.y);
                break;
            case Direction.Right:
                transform.localEulerAngles = Vector3.forward * 270f;
                txtCount.transform.localEulerAngles = Vector3.forward * -270f;
                spawnPos = new Vector2Int(pos.x + 1, pos.y);
                break;
            case Direction.Up:
                transform.localEulerAngles = Vector3.zero;
                txtCount.transform.localEulerAngles = Vector3.zero;
                spawnPos = new Vector2Int(pos.x, pos.y + 1);
                break;
            case Direction.Down:
                transform.localEulerAngles = Vector3.forward * 180f;
                txtCount.transform.localEulerAngles = Vector3.forward * -180f;
                spawnPos = new Vector2Int(pos.x, pos.y - 1);
                break;
        }

        txtCount.gameObject.SetActive(true);
        txtCount.text = data.shooters.Count.ToString();
    }

    public void Recycle()
    {
        if (checkActiveNextShooter != null)
        {
            checkActiveNextShooter.Kill();
            checkActiveNextShooter = null;
        }

        for (int i = 0; i < shooters.Count; i++)
        {
            shooters[i].Recycle();
        }
        shooters.Clear();

        gameObject.Recycle();
    }

    Tween checkActiveNextShooter;
    public void CheckActiveNextShooter()
    {
        if (shooters.Count > 0)
        {
            if (ShooterManager.Instance.shooterGrid[spawnPos.x, spawnPos.y] == null)
            {
                ShooterController nextShooter = shooters[0];
                nextShooter.isActive = true;
                ShooterManager.Instance.shooterGrid[spawnPos.x, spawnPos.y] = nextShooter;
                shooters.RemoveAt(0);

                checkActiveNextShooter = DOVirtual.DelayedCall(0.15f, () =>
                {
                    nextShooter.gameObject.SetActive(true);
                    nextShooter.pos = spawnPos;
                    nextShooter.MoveToSpawnPos(true);

                    txtCount.gameObject.SetActive(shooters.Count > 0);
                    txtCount.text = shooters.Count.ToString();

                    checkActiveNextShooter = null;
                }, false);
            }
        }
    }
}
