using CW.Common;
using DG.Tweening;
using Spine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class ShapeController : MonoBehaviour
{
    public ColorEnum color;

    public Rigidbody2D rb;
    public HingeJoint2D hinge;
    [HideInInspector] public bool isActiveRb = false;

    public LevelLayer layer;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer outline;
    public SortingGroup sortingGroup;

    public Transform trsfHole;
    public Transform trsfScrew;
    public List<ScrewController> screws;

    Material woodMat;

    public void Init()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (hinge == null) hinge = GetComponent<HingeJoint2D>();
        if (sortingGroup == null) sortingGroup = GetComponent<SortingGroup>();
        if (spriteRenderer == null) spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (outline == null) outline = transform.GetChild(1).GetComponent<SpriteRenderer>();

        screws = new List<ScrewController>();
        foreach (Transform child in trsfScrew)
        {
            if (child.TryGetComponent(out ScrewController screw))
            {
                screw.Init();
                screw.shape = null;
                screws.Add(screw);
                screw.gameObject.SetActive(false);
            }
        }
    }

    public void SetMaterial(Material woodMat)
    {
        spriteRenderer.color = Color.white;
        this.woodMat = woodMat;
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
        switch (color)
        {
            case ColorEnum.None:
                spriteRenderer.color = new Color32(255, 255, 255, 200);
                break;
            case ColorEnum.Blue:
                spriteRenderer.color = new Color32(135, 164, 231, 200);
                break;
            case ColorEnum.Red:
                spriteRenderer.color = new Color32(234, 93, 113, 200);
                break;
            case ColorEnum.Green:
                spriteRenderer.color = new Color32(142, 228, 124, 200);
                break;
            case ColorEnum.Yellow:
                spriteRenderer.color = new Color32(255, 233, 123, 200);
                break;
            case ColorEnum.Pink:
                spriteRenderer.color = new Color32(238, 165, 230, 200);
                break;
            case ColorEnum.Violet:
                spriteRenderer.color = new Color32(203, 153, 249, 200);
                break;
            case ColorEnum.Cyan:
                spriteRenderer.color = new Color32(55, 196, 237, 200);
                break;
            case ColorEnum.Orange:
                spriteRenderer.color = new Color32(255, 179, 146, 200);
                break;
            case ColorEnum.Lime:
                spriteRenderer.color = new Color32(149, 241, 35, 200);
                break;
            case ColorEnum.Brown:
                spriteRenderer.color = new Color32(152, 112, 71, 200);
                break;
            case ColorEnum.Peach:
                spriteRenderer.color = new Color32(255, 216, 177, 200);
                break;
        }

        if (outline != null)
        {
            outline.color = Color.white;
        }

        screws = new List<ScrewController>();
        for (int i = 0; i < trsfScrew.childCount; i++)
        {
            if (trsfScrew.GetChild(i).TryGetComponent(out ScrewController screw))
            {
                screws.Add(screw);
            }
        }
    }
#endif
    #endregion

    public void SetSilverSprite(bool fade)
    {
        //spriteRenderer.color = new Color32(128, 128, 128, 200);
        spriteRenderer.color = Color.white;
        outline.color = new Color32(130, 130, 130, 255);

        spriteRenderer.sharedMaterial = MaterialCache.GetWoodMat(3);

        if (fade)
        {
            //spriteRenderer.color = new Color32(128, 128, 128, 0);
            spriteRenderer.color = new Color32(255, 255, 255, 0);
            outline.color = new Color32(130, 130, 130, 0);
            //spriteRenderer.DOFade(0.8f, 0.5f);
            spriteRenderer.DOFade(1f, 0.5f);
            outline.DOFade(1f, 0.5f);
        }
    }

    public void ShowUp()
    {
        spriteRenderer.DOKill(true);
        outline.DOKill(true);

        Color32 targetColor = Color.white;

        /*
        switch (color)
        {
            case ColorEnum.None:
                targetColor = new Color32(255, 255, 255, 200);
                break;
            case ColorEnum.Blue:
                targetColor = new Color32(135, 164, 231, 200);
                break;
            case ColorEnum.Red:
                targetColor = new Color32(234, 93, 113, 200);
                break;
            case ColorEnum.Green:
                targetColor = new Color32(142, 228, 124, 200);
                break;
            case ColorEnum.Yellow:
                targetColor = new Color32(255, 233, 123, 200);
                break;
            case ColorEnum.Pink:
                targetColor = new Color32(238, 165, 230, 200);
                break;
            case ColorEnum.Violet:
                targetColor = new Color32(203, 153, 249, 200);
                break;
            case ColorEnum.Cyan:
                targetColor = new Color32(55, 196, 237, 200);
                break;
            case ColorEnum.Orange:
                targetColor = new Color32(255, 179, 146, 200);
                break;
            case ColorEnum.Lime:
                targetColor = new Color32(149, 241, 35, 200);
                break;
            case ColorEnum.Brown:
                targetColor = new Color32(152, 112, 71, 200);
                break;
            case ColorEnum.Peach:
                targetColor = new Color32(255, 216, 177, 200);
                break;
        }
        */

        screws.Clear();
        screws = trsfScrew.GetComponentsInChildren<ScrewController>(true).ToList();

        if (screws != null)
        {
            foreach (var screw in screws)
            {
                screw.gameObject.SetActive(true);
                screw.shape = this;

                GameObject hole = GameplayController.Instance.holePrefab.Spawn(trsfHole);
                hole.transform.position = screw.transform.position;
                hole.transform.rotation = Quaternion.identity;
                hole.SetActive(false);
                screw.hole = hole;

                GameplayController.Instance.levelController.screwsActive.Add(screw);
                screw.CheckCanPullUp();
            }

            if (screws.Count == 1)
            {
                if (GameplayController.Instance.levelController.currentActiveLayer > 1)
                {
                    hinge.anchor = screws[0].transform.localPosition;
                    hinge.connectedBody = screws[0].rb;
                    rb.bodyType = RigidbodyType2D.Dynamic;
                }
            }
            else if (screws.Count == 0)
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                outline.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

                hinge.enabled = false;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }

        spriteRenderer.sharedMaterial = woodMat;
        //outline.color = Color.white;

        spriteRenderer.color = new Color32(128, 128, 128, 255);
        spriteRenderer.DOColor(targetColor, 0.5f).SetEase(Ease.Linear);
        outline.DOColor(Color.white, 0.5f).SetEase(Ease.Linear);

        for (int i = 0; i < screws.Count; i++)
        {
            if (screws[i] != null)
            {
                Skeleton skeleton = screws[i].anim.skeleton;
                skeleton.A = 0;
                DOVirtual.Float(0f, 1f, 0.5f, result =>
                {
                    skeleton.A = result;
                });
            }
        }
    }

    public void DeactiveRb()
    {
        isActiveRb = false;
    }

    public void Appear()
    {
        outline.DOFade(1f, 0.5f).SetEase(Ease.Linear);
        spriteRenderer.DOFade(1f, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            for (int i = 0; i < screws.Count; i++)
            {
                if (screws[i] != null)
                    screws[i].anim.skeleton.A = 1;
            }
        });
    }

    public void RemoveScrew(ScrewController screw)
    {
        screws.Remove(screw);

        if (screws.Count == 1)
        {
            hinge.anchor = screws[0].transform.localPosition;
            hinge.connectedBody = screws[0].rb;
            rb.bodyType = RigidbodyType2D.Dynamic;

            isActiveRb = true;
            Invoke(nameof(DeactiveRb), 1f);
        }
        else if (screws.Count == 0)
        {
            hinge.enabled = false;
            rb.bodyType = RigidbodyType2D.Dynamic;
            layer.CheckDoneLayer();

            spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            outline.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

            foreach (Transform child in trsfHole)
            {
                if (child.TryGetComponent(out SpriteRenderer sprHole))
                {
                    sprHole.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
            }

            isActiveRb = true;
            Invoke(nameof(DeactiveRb), 1f);
        }
    }

    public void OnLose()
    {
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
        }
    }

    public void OnRevive()
    {
        if (hinge.connectedBody != null)
        {
            if (screws.Count <= 1)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
        else
        {
            if (screws.Count == 0)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    private void OnDestroy()
    {
        spriteRenderer.DOKill();
        outline.DOKill();

        for (int i = 0; i < trsfHole.childCount; i++)
        {
            if (trsfHole.GetChild(i).TryGetComponent(out SpriteRenderer sprHole))
            {
                sprHole.maskInteraction = SpriteMaskInteraction.None;
            }
            trsfHole.GetChild(i).gameObject.Recycle();
        }
    }
}
