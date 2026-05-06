using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelLayer : MonoBehaviour
{
    public List<ShapeController> shapes = new List<ShapeController>();
    public Vector3 offsetMin = Vector3.zero;
    public Vector3 offsetMax = Vector3.zero;

    private SortingGroup _sortingGroup;
    public SortingGroup sortingGroup
    {
        get
        {
            if (_sortingGroup == null)
            {
                _sortingGroup = GetComponent<SortingGroup>();
            }

            return _sortingGroup;
        }
    }

    [HideInInspector] public bool isDone = false;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (Application.isPlaying) return;

        shapes.Clear();
        shapes = GetComponentsInChildren<ShapeController>(true).ToList();
    }
#endif

    public void Init(int idWoodTexture)
    {
        shapes = GetComponentsInChildren<ShapeController>(true).ToList();

        Material woodMat = MaterialCache.GetWoodMat(idWoodTexture);
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].Init();

            shapes[i].layer = this;
            shapes[i].gameObject.layer = gameObject.layer;
            shapes[i].spriteRenderer.gameObject.layer = gameObject.layer;
            shapes[i].outline.gameObject.layer = gameObject.layer;

            shapes[i].SetMaterial(woodMat);
        }
    }

    public void SetSiverShape(bool fade)
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].SetSilverSprite(fade);
        }
    }

    Tween checkDoneLayer = null;
    public void ShowLayer()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].ShowUp();
        }

        //StartCoroutine(IE_Appear());

        if (checkDoneLayer != null)
        {
            checkDoneLayer.Kill();
        }

        checkDoneLayer = DOVirtual.DelayedCall(0.1f, () =>
        {
            CheckDoneLayer();
        }, false);
    }

    //WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    //IEnumerator IE_Appear()
    //{
    //    yield return waitForEndOfFrame;

    //    for (int i = 0; i < shapes.Count; i++)
    //    {
    //        shapes[i].Appear();
    //        yield return waitForEndOfFrame;
    //    }
    //}

    public void CheckDoneLayer()
    {
        if (isDone) return;

        for (int i = 0; i < shapes.Count; i++)
        {
            if (shapes[i].screws.Count > 0)
            {
                return;
            }
        }

        isDone = true;

        GameplayController.Instance.levelController.ShowNextLayer();
    }

    public void OnLose()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            if (shapes[i].gameObject.activeSelf)
            {
                shapes[i].OnLose();
            }
        }
    }

    public void OnRevive()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            if (shapes[i].gameObject.activeSelf)
            {
                shapes[i].OnRevive();
            }
        }
    }

    private void OnDestroy()
    {
        if (checkDoneLayer != null)
        {
            checkDoneLayer.Kill();
        }
    }

    /*
#if UNITY_EDITOR
    [ContextMenu("Update Colors")]
    public void UpdateColors()
    {
        List<ShapeController> shapes = GetComponentsInChildren<ShapeController>(true).ToList();
        foreach (var shape in shapes)
        {
            ColorEnum colorShape = (ColorEnum)((int)shape.color + 1);
            shape.color = colorShape;

            List<ScrewController> screws = shape.GetComponentsInChildren<ScrewController>().ToList();

            foreach (var screw in screws)
            {
                ColorEnum colorScrew = (ColorEnum)((int)screw.color + 1);
                screw.color = colorScrew;
            }
        }
    }
#endif
    */
}
