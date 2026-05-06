using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScrewController : MonoBehaviour
{
    public ColorEnum color;
    public SkeletonAnimation anim;
    public SortingGroup sortingGroup;
    public SpriteRenderer sprScrew;

    public ShapeController shape;
    public GameObject hole;

    public Rigidbody2D rb;
    public CircleCollider2D coll;

    [SerializeField] float speed;

    [SerializeField] private Color[] screwColors;

    public void Init()
    {
        if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (coll == null) coll = GetComponent<CircleCollider2D>();

        anim.Skeleton.SetSkin(color.ToString().ToLower());
        transform.eulerAngles = Vector3.zero;
    }

    #region Editor
#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        Rebuild();
    }

    public void Rebuild()
    {
        sprScrew.color = screwColors[(int)color];

        if (color == ColorEnum.None) return;

        string skinName = color.ToString().ToLower();
        if (anim.Skeleton.Data.FindSkin(skinName) == null) return;
        if (!anim.enabled) anim.enabled = true;

        anim.initialSkinName = skinName;
        anim.Initialize(true);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere((Vector2)transform.position + coll.offset, coll.radius);
    }
#endif
    #endregion

    public bool CanPullUp()
    {
        if (!coll.enabled) return false;

        Collider2D[] shapeColls = Physics2D.OverlapCircleAll((Vector2)transform.position + coll.offset, coll.radius * 0.9f, GameplayController.Instance.shapeLayer);

        if (shapeColls.Length > 0)
        {
            foreach (var shapeColl in shapeColls)
            {
                if (shapeColl.attachedRigidbody.TryGetComponent(out ShapeController otherShape)
                    && shape != null
                    && shape.layer != null
                    && shape.layer.sortingGroup != null
                    && otherShape != shape
                    && otherShape.layer != null
                    && otherShape.sortingGroup != null
                    && otherShape.layer.sortingGroup.sortingOrder > shape.layer.sortingGroup.sortingOrder)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public void PreHook()
    {
        GameplayController.Instance.levelController.screwsActive.Remove(this);
        GameplayController.Instance.levelController.screws.Remove(this);
        coll.enabled = false;
    }

    public void Hook(Action onPulledUp = null)
    {
        sortingGroup.sortingOrder = 3;
        sortingGroup.sortAtRoot = true;

        if (hole != null)
        {
            hole.SetActive(true);
        }

        anim.AnimationState.SetAnimation(0, "up", false).Complete += delegate
        {
            anim.AnimationState.SetAnimation(0, "up idle", true);

            if (shape != null)
            {
                shape.RemoveScrew(this);
            }

            shape = null;

            onPulledUp?.Invoke();
        };

        SoundManager.instance.PlaySound("Place");
        HapticFeedbackController.TriggerHaptics(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    //public void MoveToScrewBox(ScrewBoxController screwBox, Action onPulledUp = null)
    //{
    //    GameplayController.instance.levelController.screwsActive.Remove(this);
    //    GameplayController.instance.levelController.screws.Remove(this);

    //    coll.enabled = false;

    //    Transform slot = screwBox.GetFreeSlot();

    //    sortingGroup.sortingOrder = 3;
    //    sortingGroup.sortAtRoot = true;
    //    transform.parent = slot;

    //    transform.eulerAngles = Vector3.zero;

    //    if (hole != null)
    //    {
    //        hole.SetActive(true);
    //    }

    //    anim.AnimationState.SetAnimation(0, "up", false).Complete += delegate
    //    {
    //        if (shape != null)
    //        {
    //            shape.RemoveScrew(this);
    //        }

    //        shape = null;

    //        onPulledUp?.Invoke();

    //        anim.AnimationState.SetAnimation(0, "up idle", true);

    //        transform.DOScale(Vector3.one, speed);
    //        transform.DOLocalMove(Vector3.zero, speed).SetEase(Ease.Linear).OnComplete(() =>
    //        {
    //            sortingGroup.sortAtRoot = false;

    //            anim.AnimationState.SetAnimation(0, "down", false).Complete += delegate
    //            {
    //                anim.AnimationState.SetAnimation(0, "down idle", true);
    //            };

    //            GameplayController.instance.levelController.UpdateTotalScrew();

    //            AudioController.instance.PlaySoundPlace();
    //        });
    //    };
    //}

    public void CheckCanPullUp()
    {
        if (!coll.enabled) return;

        if (CanPullUp())
        {
            ParkingManager.Instance.CheckShoot();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckCanPullUp();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
