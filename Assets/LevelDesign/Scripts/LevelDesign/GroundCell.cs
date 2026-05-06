using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GroundCell : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerUpHandler
{
    public int x;
    public int y;
    public GroundType type;
    public Image img;
    public List<Color> colors;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void SetType(GroundType type)
    {
        this.type = type;
        img.color = colors[(int)type];

        LevelDesign.Instance.UIGround.OnSetGroundType(x, y, type);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        LevelDesign.Instance.UIGround.canPickGround = true;
        SetType(LevelDesign.Instance.UIGround.selectedGroundType);
        LevelDesign.Instance.UIGround.ActiveScroll(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (LevelDesign.Instance.UIGround.canPickGround)
        {
            SetType(LevelDesign.Instance.UIGround.selectedGroundType);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        LevelDesign.Instance.UIGround.canPickGround = false;
        LevelDesign.Instance.UIGround.ActiveScroll(true);
    }
}

public enum GroundType
{
    Grass = 0,
    Road = 1
}
