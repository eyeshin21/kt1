using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIMenu : MonoBehaviour
{
    [SerializeField] bool canCache = true;
    RectTransform rect;

    public virtual void Start()
    {
        rect = GetComponent<RectTransform>();
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    public virtual void Show()
    {
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public virtual void Hide()
    {
        if (!canCache)
        {
            Destroy(gameObject);
        }
    }
}
